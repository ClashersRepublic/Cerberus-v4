﻿/*
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

    internal sealed class Stream : SocketBase
    {
        private static readonly Random s_random = new Random();

        public class StreamSession : SessionBase
        {
            public StreamSession([NotNull] IOThread ioThread, bool connect, [NotNull] SocketBase socket, [NotNull] Options options, [NotNull] Address addr)
                : base(ioThread, connect, socket, options, addr)
            {
            }
        }

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
        ///     Outbound pipes indexed by the peer IDs.
        /// </summary>
        private readonly Dictionary<byte[], Outpipe> m_outpipes;

        /// <summary>
        ///     The pipe we are currently writing to.
        /// </summary>
        private Pipe m_currentOut;

        /// <summary>
        ///     If true, more outgoing message parts are expected.
        /// </summary>
        private bool m_moreOut;

        /// <summary>
        ///     Peer ID are generated. It's a simple increment and wrap-over
        ///     algorithm. This value is the next ID to use (if not used already).
        /// </summary>
        private int m_nextPeerId;

        public Stream([NotNull] Ctx parent, int threadId, int socketId)
            : base(parent, threadId, socketId)
        {
            this.m_prefetched = false;
            this.m_identitySent = false;
            this.m_currentOut = null;
            this.m_moreOut = false;
            this.m_nextPeerId = Stream.s_random.Next();

            this.m_options.SocketType = ZmqSocketType.Stream;

            this.m_fairQueueing = new FairQueueing();
            this.m_prefetchedId = new Msg();
            this.m_prefetchedId.InitEmpty();
            this.m_prefetchedMsg = new Msg();
            this.m_prefetchedMsg.InitEmpty();

            this.m_outpipes = new Dictionary<byte[], Outpipe>(new ByteArrayEqualityComparer());

            this.m_options.RawSocket = true;
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

            this.IdentifyPeer(pipe);
            this.m_fairQueueing.Attach(pipe);
        }

        /// <summary>
        ///     This is an override of the abstract method that gets called to signal that the given pipe is to be removed from
        ///     this socket.
        /// </summary>
        /// <param name="pipe">the Pipe that is being removed</param>
        protected override void XTerminated(Pipe pipe)
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

        /// <summary>
        ///     Indicate the given pipe as being ready for reading by this socket.
        /// </summary>
        /// <param name="pipe">the <c>Pipe</c> that is now becoming available for reading</param>
        protected override void XReadActivated(Pipe pipe)
        {
            this.m_fairQueueing.Activated(pipe);
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
        /// <exception cref="HostUnreachableException">In Stream.XSend</exception>
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
                            return false;
                        }
                    }
                    else
                    {
                        throw new HostUnreachableException("In Stream.XSend");
                    }
                }

                this.m_moreOut = true;

                msg.Close();
                msg.InitEmpty();

                return true;
            }

            // Ignore the MORE flag
            msg.ResetFlags(MsgFlags.More);

            // This is the last part of the message. 
            this.m_moreOut = false;

            // Push the message into the pipe. If there's no out pipe, just drop it.
            if (this.m_currentOut != null)
            {
                if (msg.Size == 0)
                {
                    this.m_currentOut.Terminate(false);
                    this.m_currentOut = null;
                    return true;
                }

                bool ok = this.m_currentOut.Write(ref msg);
                if (ok)
                {
                    this.m_currentOut.Flush();
                }

                this.m_currentOut = null;
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

                return true;
            }

            Pipe[] pipe = new Pipe[1];

            bool isMessageAvailable = this.m_fairQueueing.RecvPipe(pipe, ref this.m_prefetchedMsg);

            if (!isMessageAvailable)
            {
                return false;
            }

            Debug.Assert(pipe[0] != null);
            Debug.Assert(!this.m_prefetchedMsg.HasMore);

            // We have received a frame with TCP data.
            // Rather than sending this frame, we keep it in prefetched
            // buffer and send a frame with peer's ID.
            byte[] identity = pipe[0].Identity;
            msg.InitPool(identity.Length);
            msg.Put(identity, 0, identity.Length);
            msg.SetFlags(MsgFlags.More);

            this.m_prefetched = true;
            this.m_identitySent = true;

            return true;
        }

        protected override bool XHasIn()
        {
            // We may already have a message pre-fetched.
            if (this.m_prefetched)
            {
                return true;
            }

            // Try to read the next message.
            // The message, if read, is kept in the pre-fetch buffer.
            Pipe[] pipe = new Pipe[1];

            bool isMessageAvailable = this.m_fairQueueing.RecvPipe(pipe, ref this.m_prefetchedMsg);

            if (!isMessageAvailable)
            {
                return false;
            }

            Debug.Assert(pipe[0] != null);
            Debug.Assert(!this.m_prefetchedMsg.HasMore);

            byte[] identity = pipe[0].Identity;
            this.m_prefetchedId = new Msg();
            this.m_prefetchedId.InitPool(identity.Length);
            this.m_prefetchedId.Put(identity, 0, identity.Length);
            this.m_prefetchedId.SetFlags(MsgFlags.More);

            this.m_prefetched = true;
            this.m_identitySent = false;

            return true;
        }

        protected override bool XHasOut()
        {
            // In theory, STREAM socket is always ready for writing. Whether actual
            // attempt to write succeeds depends on which pipe the message is going
            // to be routed to.
            return true;
        }

        private void IdentifyPeer([NotNull] Pipe pipe)
        {
            // Always assign identity for raw-socket
            byte[] identity = new byte[5];

            byte[] result = BitConverter.GetBytes(this.m_nextPeerId++);

            Buffer.BlockCopy(result, 0, identity, 1, 4);

            this.m_options.Identity = identity;
            this.m_options.IdentitySize = (byte) identity.Length;

            pipe.Identity = identity;

            // Add the record into output pipes lookup table
            Outpipe outpipe = new Outpipe(pipe, true);
            this.m_outpipes.Add(identity, outpipe);
        }
    }
}