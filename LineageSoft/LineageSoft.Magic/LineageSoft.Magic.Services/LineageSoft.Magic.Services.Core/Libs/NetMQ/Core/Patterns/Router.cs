/*
    Copyright (c) 2012 iMatix Corporation
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2011 VMware, Inc.
    Copyright (c) 2007-2015 Other contributors as noted in the AUTHORS file

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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Patterns.Utils;
    using LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Utils;
    using JetBrains.Annotations;

    /// <summary>
    ///     A Router is a subclass of SocketBase
    /// </summary>
    internal class Router : SocketBase
    {
        private static readonly Random s_random = new Random();

        public class RouterSession : SessionBase
        {
            public RouterSession([NotNull] IOThread ioThread, bool connect, [NotNull] SocketBase socket, [NotNull] Options options, [NotNull] Address addr)
                : base(ioThread, connect, socket, options, addr)
            {
            }
        }

        /// <summary>
        ///     An instance of class Outpipe contains a Pipe and a boolean property Active.
        /// </summary>
        private class Outpipe
        {
            public Outpipe([NotNull] Pipe pipe, bool active)
            {
                this.Pipe = pipe;
                this.Active = active;
            }

            [NotNull]
            public Pipe Pipe { get; }

            public bool Active;
        }

        /// <summary>
        ///     Fair queueing object for inbound pipes.
        /// </summary>
        private readonly FairQueueing m_fairQueueing;

        /// <summary>
        ///     True if there is a message held in the pre-fetch buffer.
        /// </summary>
        private bool m_prefetched;

        /// <summary>
        ///     If true, the receiver got the message part with
        ///     the peer's identity.
        /// </summary>
        private bool m_identitySent;

        /// <summary>
        ///     Holds the prefetched identity.
        /// </summary>
        private Msg m_prefetchedId;

        /// <summary>
        ///     Holds the prefetched message.
        /// </summary>
        private Msg m_prefetchedMsg;

        /// <summary>
        ///     If true, more incoming message parts are expected.
        /// </summary>
        private bool m_moreIn;

        /// <summary>
        ///     We keep a set of pipes that have not been identified yet.
        /// </summary>
        private readonly HashSet<Pipe> m_anonymousPipes;

        /// <summary>
        ///     Outbound pipes indexed by the peer IDs.
        /// </summary>
        private readonly Dictionary<byte[], Outpipe> m_outpipes;

        /// <summary>
        ///     The pipe we are currently writing to.
        /// </summary>
        private Pipe m_currentOut;

        /// <summary>
        ///     The pipe we are currently reading from.
        /// </summary>
        private Pipe m_currentIn;


        private bool m_closingCurrentIn;

        /// <summary>
        ///     If true, more outgoing message parts are expected.
        /// </summary>
        private bool m_moreOut;

        /// <summary>
        ///     Peer ID are generated. It's a simple increment and wrap-over
        ///     algorithm. This value is the next ID to use (if not used already).
        /// </summary>
        private int m_nextPeerId;

        /// <summary>
        ///     If true, report EHOSTUNREACH to the caller instead of silently dropping
        ///     the message targeting an unknown peer.
        /// </summary>
        private bool m_mandatory;

        /// <summary>
        ///     If true, router socket accepts non-zmq tcp connections
        /// </summary>
        private bool m_rawSocket;

        /// <summary>
        ///     When enabled new router connections with same identity take over old ones
        /// </summary>
        private bool m_handover;

        /// <summary>
        ///     Create a new Router instance with the given parent-Ctx, thread-id, and socket-id.
        /// </summary>
        /// <param name="parent">the Ctx that will contain this Router</param>
        /// <param name="threadId">the integer thread-id value</param>
        /// <param name="socketId">the integer socket-id value</param>
        public Router([NotNull] Ctx parent, int threadId, int socketId)
            : base(parent, threadId, socketId)
        {
            this.m_nextPeerId = Router.s_random.Next();
            this.m_options.SocketType = ZmqSocketType.Router;
            this.m_fairQueueing = new FairQueueing();
            this.m_prefetchedId = new Msg();
            this.m_prefetchedId.InitEmpty();
            this.m_prefetchedMsg = new Msg();
            this.m_prefetchedMsg.InitEmpty();
            this.m_anonymousPipes = new HashSet<Pipe>();
            this.m_outpipes = new Dictionary<byte[], Outpipe>(new ByteArrayEqualityComparer());
            this.m_options.RecvIdentity = true;
        }

        public override void Destroy()
        {
            base.Destroy();
            this.m_prefetchedId.Close();
            this.m_prefetchedMsg.Close();
        }

        /// <summary>
        ///     Register the pipe with this socket.
        /// </summary>
        /// <param name="pipe">the Pipe to attach</param>
        /// <param name="icanhasall">not used</param>
        protected override void XAttachPipe(Pipe pipe, bool icanhasall)
        {
            Debug.Assert(pipe != null);

            bool identityOk = this.IdentifyPeer(pipe);
            if (identityOk)
            {
                this.m_fairQueueing.Attach(pipe);
            }
            else
            {
                this.m_anonymousPipes.Add(pipe);
            }
        }


        protected override bool XSetSocketOption(ZmqSocketOption option, object optval)
        {
            switch (option)
            {
                case ZmqSocketOption.RouterRawSocket:
                    this.m_rawSocket = (bool) optval;
                    if (this.m_rawSocket)
                    {
                        this.m_options.RecvIdentity = false;
                        this.m_options.RawSocket = true;
                    }

                    return true;
                case ZmqSocketOption.RouterMandatory:
                    this.m_mandatory = (bool) optval;
                    return true;
                case ZmqSocketOption.RouterHandover:
                    this.m_handover = (bool) optval;
                    return true;
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
            if (!this.m_anonymousPipes.Remove(pipe))
            {
                Outpipe old;

                this.m_outpipes.TryGetValue(pipe.Identity, out old);
                this.m_outpipes.Remove(pipe.Identity);

                Debug.Assert(old != null);

                this.m_fairQueueing.Terminated(pipe);
                if (pipe == this.m_currentOut)
                {
                    this.m_currentOut = null;
                }
            }
        }

        /// <summary>
        ///     Indicate the given pipe as being ready for reading by this socket.
        /// </summary>
        /// <param name="pipe">the <c>Pipe</c> that is now becoming available for reading</param>
        protected override void XReadActivated(Pipe pipe)
        {
            if (!this.m_anonymousPipes.Contains(pipe))
            {
                this.m_fairQueueing.Activated(pipe);
            }
            else
            {
                bool identityOk = this.IdentifyPeer(pipe);
                if (identityOk)
                {
                    this.m_anonymousPipes.Remove(pipe);
                    this.m_fairQueueing.Attach(pipe);
                }
            }
        }

        /// <summary>
        ///     Indicate the given pipe as being ready for writing to by this socket.
        ///     This gets called by the WriteActivated method.
        /// </summary>
        /// <param name="pipe">the <c>Pipe</c> that is now becoming available for writing</param>
        protected override void XWriteActivated(Pipe pipe)
        {
            Outpipe outpipe = null;

            foreach (KeyValuePair<byte[], Outpipe> it in this.m_outpipes)
            {
                if (it.Value.Pipe == pipe)
                {
                    Debug.Assert(!it.Value.Active);
                    it.Value.Active = true;
                    outpipe = it.Value;
                    break;
                }
            }

            Debug.Assert(outpipe != null);
        }

        /// <summary>
        ///     Transmit the given message. The <c>Send</c> method calls this to do the actual sending.
        /// </summary>
        /// <param name="msg">the message to transmit</param>
        /// <returns><c>true</c> if the message was sent successfully</returns>
        /// <exception cref="HostUnreachableException">The receiving host must be identifiable.</exception>
        protected override bool XSend(ref Msg msg)
        {
            // If this is the first part of the message it's the ID of the
            // peer to send the message to.
            if (!this.m_moreOut)
            {
                Debug.Assert(this.m_currentOut == null);

                // If we have malformed message (prefix with no subsequent message)
                // then just silently ignore it.
                // TODO: The connections should be killed instead.
                if (msg.HasMore)
                {
                    this.m_moreOut = true;

                    // Find the pipe associated with the identity stored in the prefix.
                    // If there's no such pipe just silently ignore the message, unless
                    // mandatory is set.

                    byte[] identity = msg.Size == msg.Data.Length
                        ? msg.Data
                        : msg.CloneData();

                    Outpipe op;

                    if (this.m_outpipes.TryGetValue(identity, out op))
                    {
                        this.m_currentOut = op.Pipe;
                        if (!this.m_currentOut.CheckWrite())
                        {
                            op.Active = false;
                            this.m_currentOut = null;
                            if (this.m_mandatory)
                            {
                                this.m_moreOut = false;

                                if (op.Pipe.Active)
                                {
                                    return false;
                                }

                                throw new HostUnreachableException("In Router.XSend");
                            }
                        }
                    }
                    else if (this.m_mandatory)
                    {
                        this.m_moreOut = false;
                        throw new HostUnreachableException("In Router.XSend");
                    }
                }

                // Detach the message from the data buffer.
                msg.Close();
                msg.InitEmpty();

                return true;
            }

            if (this.m_options.RawSocket)
            {
                msg.ResetFlags(MsgFlags.More);
            }

            // Check whether this is the last part of the message.
            this.m_moreOut = msg.HasMore;

            // Push the message into the pipe. If there's no out pipe, just drop it.
            if (this.m_currentOut != null)
            {
                // Close the remote connection if user has asked to do so
                // by sending zero length message.
                // Pending messages in the pipe will be dropped (on receiving term-ack)
                if (this.m_rawSocket && msg.Size == 0)
                {
                    this.m_currentOut.Terminate(false);
                    msg.Close();
                    msg.InitEmpty();
                    this.m_currentOut = null;
                    return true;
                }

                bool ok = this.m_currentOut.Write(ref msg);
                if (!ok)
                {
                    this.m_currentOut = null;
                }
                else if (!this.m_moreOut)
                {
                    this.m_currentOut.Flush();
                    this.m_currentOut = null;
                }
            }
            else
            {
                msg.Close();
            }

            // Detach the message from the data buffer.            
            msg.InitEmpty();

            return true;
        }

        /// <summary>
        ///     Receive a message. The <c>Recv</c> method calls this lower-level method to do the actual receiving.
        /// </summary>
        /// <param name="msg">the <c>Msg</c> to receive the message into</param>
        /// <returns><c>true</c> if the message was received successfully, <c>false</c> if there were no messages to receive</returns>
        protected override bool XRecv(ref Msg msg)
        {
            if (this.m_prefetched)
            {
                if (!this.m_identitySent)
                {
                    msg.Move(ref this.m_prefetchedId);
                    this.m_identitySent = true;
                }
                else
                {
                    msg.Move(ref this.m_prefetchedMsg);
                    this.m_prefetched = false;
                }

                this.m_moreIn = msg.HasMore;

                if (!this.m_moreIn)
                {
                    if (this.m_closingCurrentIn)
                    {
                        this.m_currentIn.Terminate(true);
                        this.m_closingCurrentIn = false;
                    }

                    this.m_currentIn = null;
                }

                return true;
            }

            Pipe[] pipe = new Pipe[1];

            bool isMessageAvailable = this.m_fairQueueing.RecvPipe(pipe, ref msg);

            // It's possible that we receive peer's identity. That happens
            // after reconnection. The current implementation assumes that
            // the peer always uses the same identity.            
            while (isMessageAvailable && msg.IsIdentity)
            {
                isMessageAvailable = this.m_fairQueueing.RecvPipe(pipe, ref msg);
            }

            if (!isMessageAvailable)
            {
                return false;
            }

            Debug.Assert(pipe[0] != null);

            // If we are in the middle of reading a message, just return the next part.
            if (this.m_moreIn)
            {
                this.m_moreIn = msg.HasMore;

                if (!this.m_moreIn)
                {
                    if (this.m_closingCurrentIn)
                    {
                        this.m_currentIn.Terminate(true);
                        this.m_closingCurrentIn = false;
                    }

                    this.m_currentIn = null;
                }
            }
            else
            {
                // We are at the beginning of a message.
                // Keep the message part we have in the prefetch buffer
                // and return the ID of the peer instead.
                this.m_prefetchedMsg.Move(ref msg);

                this.m_prefetched = true;
                this.m_currentIn = pipe[0];

                byte[] identity = pipe[0].Identity;
                msg.InitPool(identity.Length);
                msg.Put(identity, 0, identity.Length);
                msg.SetFlags(MsgFlags.More);

                this.m_identitySent = true;
            }

            return true;
        }

        // Rollback any message parts that were sent but not yet flushed.
        protected void Rollback()
        {
            if (this.m_currentOut != null)
            {
                this.m_currentOut.Rollback();
                this.m_currentOut = null;
                this.m_moreOut = false;
            }
        }

        protected override bool XHasIn()
        {
            // If we are in the middle of reading the messages, there are
            // definitely more parts available.
            if (this.m_moreIn)
            {
                return true;
            }

            // We may already have a message pre-fetched.
            if (this.m_prefetched)
            {
                return true;
            }

            // Try to read the next message.
            // The message, if read, is kept in the pre-fetch buffer.
            Pipe[] pipe = new Pipe[1];

            bool isMessageAvailable = this.m_fairQueueing.RecvPipe(pipe, ref this.m_prefetchedMsg);

            // It's possible that we receive peer's identity. That happens
            // after reconnection. The current implementation assumes that
            // the peer always uses the same identity.
            // TODO: handle the situation when the peer changes its identity.
            while (isMessageAvailable && this.m_prefetchedMsg.IsIdentity)
            {
                isMessageAvailable = this.m_fairQueueing.RecvPipe(pipe, ref this.m_prefetchedMsg);
            }

            if (!isMessageAvailable)
            {
                return false;
            }

            Debug.Assert(pipe[0] != null);

            byte[] identity = pipe[0].Identity;
            this.m_prefetchedId = new Msg();
            this.m_prefetchedId.InitPool(identity.Length);
            this.m_prefetchedId.Put(identity, 0, identity.Length);
            this.m_prefetchedId.SetFlags(MsgFlags.More);

            this.m_prefetched = true;
            this.m_identitySent = false;
            this.m_currentIn = pipe[0];

            return true;
        }

        protected override bool XHasOut()
        {
            // In theory, ROUTER socket is always ready for writing. Whether actual
            // attempt to write succeeds depends on which pipe the message is going
            // to be routed to.
            return true;
        }

        private bool IdentifyPeer([NotNull] Pipe pipe)
        {
            byte[] identity;

            if (this.m_options.RawSocket)
            {
                // Always assign identity for raw-socket
                identity = new byte[5];
                byte[] result = BitConverter.GetBytes(this.m_nextPeerId++);
                Buffer.BlockCopy(result, 0, identity, 1, 4);
            }
            else
            {
                // Pick up handshake cases and also case where next identity is set

                Msg msg = new Msg();
                msg.InitEmpty();

                bool ok = pipe.Read(ref msg);

                if (!ok)
                {
                    return false;
                }

                if (msg.Size == 0)
                {
                    // Fall back on the auto-generation
                    identity = new byte[5];

                    byte[] result = BitConverter.GetBytes(this.m_nextPeerId++);

                    Buffer.BlockCopy(result, 0, identity, 1, 4);

                    msg.Close();
                }
                else
                {
                    identity = msg.CloneData();
                    msg.Close();

                    Outpipe existPipe;

                    if (this.m_outpipes.TryGetValue(identity, out existPipe))
                    {
                        if (!this.m_handover)
                        {
                            // Ignore peers with duplicate ID.
                            return false;
                        }

                        //  We will allow the new connection to take over this
                        //  identity. Temporarily assign a new identity to the
                        //  existing pipe so we can terminate it asynchronously.
                        byte[] newIdentity = new byte[5];
                        byte[] result = BitConverter.GetBytes(this.m_nextPeerId++);
                        Buffer.BlockCopy(result, 0, newIdentity, 1, 4);
                        existPipe.Pipe.Identity = newIdentity;
                        this.m_outpipes.Add(newIdentity, existPipe);

                        //  Remove the existing identity entry to allow the new
                        //  connection to take the identity.
                        this.m_outpipes.Remove(identity);

                        if (existPipe.Pipe == this.m_currentIn)
                        {
                            this.m_closingCurrentIn = true;
                        }
                        else
                        {
                            existPipe.Pipe.Terminate(true);
                        }
                    }
                }
            }

            pipe.Identity = identity;
            // Add the record into output pipes lookup table
            Outpipe outpipe = new Outpipe(pipe, true);
            this.m_outpipes.Add(identity, outpipe);

            return true;
        }
    }
}