/*      
    Copyright (c) 2010-2011 250bpm s.r.o.
    Copyright (c) 2011 VMware, Inc.
    Copyright (c) 2010-2015 Other contributors as noted in the AUTHORS file
        
    This file is part of 0MQ.

    0MQ is free software; you can redistribute it and/or modify it under
    the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.
        
    0MQ is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.
        
    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Patterns
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Text;
    using LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Patterns.Utils;
    using JetBrains.Annotations;

    internal class XPub : SocketBase
    {
        public class XPubSession : SessionBase
        {
            public XPubSession([NotNull] IOThread ioThread, bool connect, [NotNull] SocketBase socket, [NotNull] Options options, [NotNull] Address addr)
                : base(ioThread, connect, socket, options, addr)
            {
            }
        }

        /// <summary>
        ///     List of all subscriptions mapped to corresponding pipes.
        /// </summary>
        private readonly MultiTrie m_subscriptions;

        /// <summary>
        ///     Distributor of messages holding the list of outbound pipes.
        /// </summary>
        private readonly Distribution m_distribution;

        /// <summary>
        ///     If true, send all subscription messages upstream, not just
        ///     unique ones. The default is false.
        /// </summary>
        private bool m_verbose;

        /// <summary>
        ///     The default value is false.
        /// </summary>
        private bool m_manual;

        private bool m_broadcastEnabled;

        private Pipe m_lastPipe;
        private bool m_lastPipeIsBroadcast;

        private Msg m_welcomeMessage;

        /// <summary>
        ///     True if we are in the middle of sending a multipart message.
        /// </summary>
        private bool m_moreOut;

        /// <summary>
        ///     True if we are in the middle of receiving a multipart message.
        /// </summary>
        private bool m_moreIn;

        /// <summary>
        ///     List of pending (un)subscriptions, ie. those that were already
        ///     applied to the trie, but not yet received by the user.
        /// </summary>
        private readonly Queue<KeyValuePair<Msg, Pipe>> m_pendingMessages;

        private static readonly MultiTrie.MultiTrieDelegate s_markAsMatching;
        private static readonly MultiTrie.MultiTrieDelegate s_sendUnsubscription;

        static XPub()
        {
            XPub.s_markAsMatching = (pipe, data, size, arg) =>
            {
                XPub self = (XPub) arg;
                // skip the sender of a broadcast message
                if (!(self.m_broadcastEnabled && self.m_lastPipeIsBroadcast && self.m_lastPipe == pipe))
                {
                    self.m_distribution.Match(pipe);
                }
            };

            XPub.s_sendUnsubscription = (pipe, data, size, arg) =>
            {
                XPub self = (XPub) arg;

                if (self.m_options.SocketType != ZmqSocketType.Pub)
                {
                    // Place the unsubscription to the queue of pending (un)subscriptions
                    // to be retrieved by the user later on.

                    Msg unsubMsg = new Msg();
                    unsubMsg.InitPool(size + 1);
                    unsubMsg[0] = 0;
                    unsubMsg.Put(data, 1, size);

                    self.m_pendingMessages.Enqueue(new KeyValuePair<Msg, Pipe>(unsubMsg, pipe));
                }
            };
        }

        public XPub([NotNull] Ctx parent, int threadId, int socketId)
            : base(parent, threadId, socketId)
        {
            this.m_options.SocketType = ZmqSocketType.Xpub;

            this.m_welcomeMessage = new Msg();
            this.m_welcomeMessage.InitEmpty();

            this.m_subscriptions = new MultiTrie();
            this.m_distribution = new Distribution();
            this.m_pendingMessages = new Queue<KeyValuePair<Msg, Pipe>>();
        }

        /// <summary>
        ///     Register the pipe with this socket.
        /// </summary>
        /// <param name="pipe">the Pipe to attach</param>
        /// <param name="icanhasall">if true - subscribe to all data on the pipe</param>
        protected override void XAttachPipe(Pipe pipe, bool icanhasall)
        {
            Debug.Assert(pipe != null);
            this.m_distribution.Attach(pipe);

            // If icanhasall is specified, the caller would like to subscribe
            // to all data on this pipe, implicitly.
            if (icanhasall)
            {
                this.m_subscriptions.Add(null, 0, 0, pipe);
            }

            // if welcome message was set, write one to the pipe.
            if (this.m_welcomeMessage.Size > 0)
            {
                Msg copy = new Msg();
                copy.InitEmpty();
                copy.Copy(ref this.m_welcomeMessage);

                pipe.Write(ref copy);
                pipe.Flush();
            }

            // The pipe is active when attached. Let's read the subscriptions from
            // it, if any.
            this.XReadActivated(pipe);
        }

        /// <summary>
        ///     Indicate the given pipe as being ready for reading by this socket.
        /// </summary>
        /// <param name="pipe">the <c>Pipe</c> that is now becoming available for reading</param>
        protected override void XReadActivated(Pipe pipe)
        {
            // There are some subscriptions waiting. Let's process them.
            Msg sub = new Msg();
            bool isBroadcast = false;
            bool msgMore = false;
            while (pipe.Read(ref sub))
            {
                // Apply the subscription to the trie.
                int size = sub.Size;
                bool msgMoreTmp = sub.HasMore;
                if (!msgMore && !isBroadcast && size > 0 && (sub[0] == 0 || sub[0] == 1))
                {
                    if (this.m_manual)
                    {
                        this.m_pendingMessages.Enqueue(new KeyValuePair<Msg, Pipe>(sub, pipe));
                    }
                    else
                    {
                        bool unique = sub[0] == 0
                            ? this.m_subscriptions.Remove(sub.Data, sub.Offset + 1, size - 1, pipe)
                            : this.m_subscriptions.Add(sub.Data, sub.Offset + 1, size - 1, pipe);

                        // If the subscription is not a duplicate, store it so that it can be
                        // passed to used on next recv call.
                        if (this.m_options.SocketType == ZmqSocketType.Xpub && (unique || this.m_verbose))
                        {
                            this.m_pendingMessages.Enqueue(new KeyValuePair<Msg, Pipe>(sub, pipe));
                        }
                        else
                        {
                            sub.Close();
                        }
                    }
                }
                else if (!msgMore && this.m_broadcastEnabled && size > 0 && sub[0] == 2)
                {
                    this.m_pendingMessages.Enqueue(new KeyValuePair<Msg, Pipe>(sub, pipe));
                    isBroadcast = true;
                }
                else // process message unrelated to sub/unsub/broadcast
                {
                    this.m_pendingMessages.Enqueue(new KeyValuePair<Msg, Pipe>(sub, pipe));
                }

                msgMore = msgMoreTmp;
            }
        }

        /// <summary>
        ///     Indicate the given pipe as being ready for writing to by this socket.
        ///     This gets called by the WriteActivated method.
        /// </summary>
        /// <param name="pipe">the <c>Pipe</c> that is now becoming available for writing</param>
        protected override void XWriteActivated(Pipe pipe)
        {
            this.m_distribution.Activated(pipe);
        }

        /// <summary>
        ///     Set the specified option on this socket.
        /// </summary>
        /// <param name="option">which option to set</param>
        /// <param name="optionValue">the value to set the option to</param>
        /// <returns><c>true</c> if successful</returns>
        /// <exception cref="InvalidException">optionValue must be a byte-array.</exception>
        protected override bool XSetSocketOption(ZmqSocketOption option, object optionValue)
        {
            switch (option)
            {
                case ZmqSocketOption.XpubVerbose:
                {
                    this.m_verbose = (bool) optionValue;
                    return true;
                }
                case ZmqSocketOption.XPublisherManual:
                {
                    this.m_manual = (bool) optionValue;
                    return true;
                }
                case ZmqSocketOption.XPublisherBroadcast:
                {
                    this.m_broadcastEnabled = (bool) optionValue;
                    return true;
                }
                case ZmqSocketOption.Identity:
                {
                    if (this.m_manual && this.m_lastPipe != null)
                    {
                        byte[] val;

                        if (optionValue is string)
                        {
                            val = Encoding.ASCII.GetBytes((string) optionValue);
                        }
                        else if (optionValue is byte[])
                        {
                            val = (byte[]) optionValue;
                        }
                        else
                        {
                            throw new InvalidException($"In XPub.XSetSocketOption(Identity, {optionValue?.ToString() ?? "null"}) optionValue must be a string or byte-array.");
                        }

                        if (val.Length == 0 || val.Length > 255)
                        {
                            throw new InvalidException($"In XPub.XSetSocketOption(Identity,) optionValue yielded a byte-array of length {val.Length}, should be 1..255.");
                        }

                        this.m_lastPipe.Identity = val;
                        this.m_options.Identity = val;
                    }

                    return true;
                }

                case ZmqSocketOption.Subscribe:
                {
                    if (this.m_manual && this.m_lastPipe != null)
                    {
                        byte[] subscription = optionValue as byte[] ?? Encoding.ASCII.GetBytes((string) optionValue);
                        this.m_subscriptions.Add(subscription, 0, subscription.Length, this.m_lastPipe);
                        this.m_lastPipe = null;
                        return true;
                    }

                    break;
                }
                case ZmqSocketOption.Unsubscribe:
                {
                    if (this.m_manual && this.m_lastPipe != null)
                    {
                        byte[] subscription = optionValue as byte[] ?? Encoding.ASCII.GetBytes((string) optionValue);
                        this.m_subscriptions.Remove(subscription, 0, subscription.Length, this.m_lastPipe);
                        this.m_lastPipe = null;
                        return true;
                    }

                    break;
                }
                case ZmqSocketOption.XPublisherWelcomeMessage:
                {
                    this.m_welcomeMessage.Close();

                    if (optionValue != null)
                    {
                        byte[] bytes = optionValue as byte[];
                        if (bytes == null)
                        {
                            throw new InvalidException($"In XPub.XSetSocketOption({option},{optionValue}), optionValue must be a byte-array.");
                        }

                        byte[] welcomeBytes = new byte[bytes.Length];
                        bytes.CopyTo(welcomeBytes, 0);
                        this.m_welcomeMessage.InitGC(welcomeBytes, welcomeBytes.Length);
                    }
                    else
                    {
                        this.m_welcomeMessage.InitEmpty();
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     This is an override of the abstract method that gets called to signal that the given pipe is to be removed from
        ///     this socket.
        /// </summary>
        /// <param name="pipe">the Pipe that is being removed</param>
        protected override void XTerminated(Pipe pipe)
        {
            // Remove the pipe from the trie. If there are topics that nobody
            // is interested in anymore, send corresponding un-subscriptions
            // upstream.

            this.m_subscriptions.RemoveHelper(pipe, XPub.s_sendUnsubscription, this);

            this.m_distribution.Terminated(pipe);

            // remove a reference to a dead pipe
            if (this.m_lastPipe == pipe)
            {
                this.m_lastPipe = null;
            }
        }

        /// <summary>
        ///     Transmit the given message. The <c>Send</c> method calls this to do the actual sending.
        /// </summary>
        /// <param name="msg">the message to transmit</param>
        /// <returns><c>true</c> if the message was sent successfully</returns>
        protected override bool XSend(ref Msg msg)
        {
            bool msgMore = msg.HasMore;

            // For the first part of multipart message, find the matching pipes.
            if (!this.m_moreOut)
            {
                this.m_subscriptions.Match(msg.Data, msg.Offset, msg.Size, XPub.s_markAsMatching, this);
            }

            // Send the message to all the pipes that were marked as matching
            // in the previous step.
            this.m_distribution.SendToMatching(ref msg);

            // If we are at the end of multipart message we can mark all the pipes
            // as non-matching.
            if (!msgMore)
            {
                this.m_distribution.Unmatch();
                if (this.m_broadcastEnabled)
                {
                    this.m_lastPipeIsBroadcast = false;
                }

                this.m_lastPipe = null;
            }

            this.m_moreOut = msgMore;

            return true;
        }

        protected override bool XHasOut()
        {
            return this.m_distribution.HasOut();
        }

        /// <summary>
        ///     Receive a message. The <c>Recv</c> method calls this lower-level method to do the actual receiving.
        /// </summary>
        /// <param name="msg">the <c>Msg</c> to receive the message into</param>
        /// <returns><c>true</c> if the message was received successfully, <c>false</c> if there were no messages to receive</returns>
        protected override bool XRecv(ref Msg msg)
        {
            // If there is at least one 
            if (this.m_pendingMessages.Count == 0)
            {
                return false;
            }

            msg.Close();
            KeyValuePair<Msg, Pipe> msgPipePair = this.m_pendingMessages.Dequeue();
            msg = msgPipePair.Key;
            bool msgMore = msg.HasMore;

            // must check if m_lastPipe == null to avoid dequeue at the second frame of a broadcast message
            if (msgPipePair.Value != null && !this.m_moreIn)
            {
                if (!this.m_moreIn && this.m_broadcastEnabled && msg[0] == 2)
                {
                    this.m_lastPipeIsBroadcast = true;
                }

                if (!this.m_moreIn && this.m_manual && (msg[0] == 0 || msg[0] == 1))
                {
                    this.m_lastPipeIsBroadcast = false;
                }

                this.m_lastPipe = msgPipePair.Value;
            }

            this.m_options.Identity = this.m_lastPipe?.Identity;
            this.m_moreIn = msgMore;
            return true;
        }

        protected override bool XHasIn()
        {
            return this.m_pendingMessages.Count != 0;
        }
    }
}