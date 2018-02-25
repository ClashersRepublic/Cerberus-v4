/*      
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2009 iMatix Corporation
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

namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Sockets;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Patterns;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports.Ipc;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports.Pgm;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports.Tcp;
    using JetBrains.Annotations;

    internal class SessionBase : Own,
        Pipe.IPipeEvents, IProactorEvents,
        IMsgSink, IMsgSource
    {
        /// <summary>
        ///     If true, this session (re)connects to the peer. Otherwise, it's
        ///     a transient session created by the listener.
        /// </summary>
        private readonly bool m_connect;

        /// <summary>
        ///     Pipe connecting the session to its socket.
        /// </summary>
        private Pipe m_pipe;

        /// <summary>
        ///     This set is added to with pipes we are disconnecting, but haven't yet completed
        /// </summary>
        private readonly HashSet<Pipe> m_terminatingPipes;

        /// <summary>
        ///     This flag is true if the remainder of the message being processed
        ///     is still in the pipe.
        /// </summary>
        private bool m_incompleteIn;

        /// <summary>
        ///     True if termination have been suspended to push the pending
        ///     messages to the network.
        /// </summary>
        private bool m_pending;

        /// <summary>
        ///     The protocol I/O engine connected to the session.
        /// </summary>
        private IEngine m_engine;

        /// <summary>
        ///     The socket the session belongs to.
        /// </summary>
        private readonly SocketBase m_socket;

        /// <summary>
        ///     I/O thread the session is living in. It will be used to plug in
        ///     the engines into the same thread.
        /// </summary>
        private readonly IOThread m_ioThread;

        /// <summary>
        ///     ID of the linger timer (0x20)
        /// </summary>
        private const int LingerTimerId = 0x20;

        /// <summary>
        ///     True is linger timer is running.
        /// </summary>
        private bool m_hasLingerTimer;

        /// <summary>
        ///     If true, identity has been sent to the network.
        /// </summary>
        private bool m_identitySent;

        /// <summary>
        ///     If true, identity has been received from the network.
        /// </summary>
        private bool m_identityReceived;

        /// <summary>
        ///     Protocol and address to use when connecting.
        /// </summary>
        private readonly Address m_addr;

        [NotNull] private readonly IOObject m_ioObject;

        /// <summary>
        ///     Create a return a new session.
        ///     The specific subclass of SessionBase that is created is dictated by the SocketType specified by the options
        ///     argument.
        /// </summary>
        /// <param name="ioThread">the <c>IOThread</c> for this session to run in</param>
        /// <param name="connect">whether to immediately connect</param>
        /// <param name="socket">the socket to connect</param>
        /// <param name="options">an <c>Options</c> that provides the SocketType that dictates which type of session to create</param>
        /// <param name="addr">an <c>Address</c> object that specifies the protocol and address to connect to</param>
        /// <returns>the newly-created instance of whichever subclass of SessionBase is specified by the options</returns>
        /// <exception cref="InvalidException">The socket must be of the correct type.</exception>
        [NotNull]
        public static SessionBase Create([NotNull] IOThread ioThread, bool connect, [NotNull] SocketBase socket, [NotNull] Options options, [NotNull] Address addr)
        {
            switch (options.SocketType)
            {
                case ZmqSocketType.Req:
                    return new Req.ReqSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Dealer:
                    return new Dealer.DealerSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Rep:
                    return new Rep.RepSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Router:
                    return new Router.RouterSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Pub:
                    return new Pub.PubSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Xpub:
                    return new XPub.XPubSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Sub:
                    return new Sub.SubSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Xsub:
                    return new XSub.XSubSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Push:
                    return new Push.PushSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Pull:
                    return new Pull.PullSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Pair:
                    return new Pair.PairSession(ioThread, connect, socket, options, addr);
                case ZmqSocketType.Stream:
                    return new Stream.StreamSession(ioThread, connect, socket, options, addr);
                default:
                    throw new InvalidException("SessionBase.Create called with invalid SocketType of " + options.SocketType);
            }
        }

        /// <summary>
        ///     Create a new SessionBase object from the given IOThread, socket, and Address.
        /// </summary>
        /// <param name="ioThread">the IOThread for this session to run on</param>
        /// <param name="connect">this flag dictates whether to connect</param>
        /// <param name="socket">the socket to contain</param>
        /// <param name="options">Options that dictate the settings of this session</param>
        /// <param name="addr">an Address that dictates the protocol and IP-address to use when connecting</param>
        public SessionBase([NotNull] IOThread ioThread, bool connect, [NotNull] SocketBase socket, [NotNull] Options options, [NotNull] Address addr)
            : base(ioThread, options)
        {
            this.m_ioObject = new IOObject(ioThread);

            this.m_connect = connect;
            this.m_socket = socket;
            this.m_ioThread = ioThread;
            this.m_addr = addr;

            if (options.RawSocket)
            {
                this.m_identitySent = true;
                this.m_identityReceived = true;
            }

            this.m_terminatingPipes = new HashSet<Pipe>();
        }

        /// <summary>
        ///     Terminate and release any contained resources.
        ///     This cancels the linger-timer if that exists, and terminates the protocol-engine if that exists.
        /// </summary>
        public override void Destroy()
        {
            Debug.Assert(this.m_pipe == null);

            // If there's still a pending linger timer, remove it.
            if (this.m_hasLingerTimer)
            {
                this.m_ioObject.CancelTimer(SessionBase.LingerTimerId);
                this.m_hasLingerTimer = false;
            }

            // Close the engine.
            this.m_engine?.Terminate();
        }

        /// <summary>
        ///     Attach the given pipe to this session.
        /// </summary>
        /// <remarks>
        ///     This is to be used once only, when creating the session.
        /// </remarks>
        public void AttachPipe([NotNull] Pipe pipe)
        {
            Debug.Assert(!this.IsTerminating);
            Debug.Assert(this.m_pipe == null);
            Debug.Assert(pipe != null);
            this.m_pipe = pipe;
            this.m_pipe.SetEventSink(this);
        }

        /// <summary>
        ///     Read a message from the pipe.
        /// </summary>
        /// <param name="msg">a reference to a Msg to put the message into</param>
        /// <returns>true if the Msg is successfully sent</returns>
        public virtual bool PullMsg(ref Msg msg)
        {
            // First message to send is identity
            if (!this.m_identitySent)
            {
                msg.InitPool(this.m_options.IdentitySize);
                msg.Put(this.m_options.Identity, 0, this.m_options.IdentitySize);
                this.m_identitySent = true;
                this.m_incompleteIn = false;

                return true;
            }

            if (this.m_pipe == null || !this.m_pipe.Read(ref msg))
            {
                return false;
            }

            this.m_incompleteIn = msg.HasMore;

            return true;
        }

        /// <summary>
        ///     Write the given Msg to the pipe.
        /// </summary>
        /// <param name="msg">the Msg to push to the pipe</param>
        /// <returns>true if the Msg was successfully sent</returns>
        public virtual bool PushMsg(ref Msg msg)
        {
            // First message to receive is identity (if required).
            if (!this.m_identityReceived)
            {
                msg.SetFlags(MsgFlags.Identity);
                this.m_identityReceived = true;

                if (!this.m_options.RecvIdentity)
                {
                    msg.Close();
                    msg.InitEmpty();
                    return true;
                }
            }

            if (this.m_pipe != null && this.m_pipe.Write(ref msg))
            {
                msg.InitEmpty();
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Set the identity-sent and identity-received flags to false.
        /// </summary>
        protected virtual void Reset()
        {
            // Restore identity flags.
            if (this.m_options.RawSocket)
            {
                this.m_identitySent = true;
                this.m_identityReceived = true;
            }
            else
            {
                this.m_identitySent = false;
                this.m_identityReceived = false;
            }
        }

        /// <summary>
        ///     Flush any messages that are in the pipe downstream.
        /// </summary>
        public void Flush()
        {
            this.m_pipe?.Flush();
        }

        /// <summary>
        ///     Remove any half processed messages. Flush unflushed messages.
        ///     Call this function when engine disconnect to get rid of leftovers.
        /// </summary>
        private void CleanPipes()
        {
            if (this.m_pipe != null)
            {
                // Get rid of half-processed messages in the out pipe. Flush any
                // unflushed messages upstream.
                this.m_pipe.Rollback();
                this.m_pipe.Flush();

                // Remove any half-read message from the in pipe.
                while (this.m_incompleteIn)
                {
                    Msg msg = new Msg();
                    msg.InitEmpty();

                    if (!this.PullMsg(ref msg))
                    {
                        Debug.Assert(!this.m_incompleteIn);
                        break;
                    }

                    msg.Close();
                }
            }
        }

        /// <summary>
        ///     This gets called by ProcessPipeTermAck or XTerminated to respond to the termination of the given pipe.
        /// </summary>
        /// <param name="pipe">the pipe that was terminated</param>
        public void Terminated(Pipe pipe)
        {
            // Drop the reference to the deallocated pipe.
            Debug.Assert(this.m_pipe == pipe || this.m_terminatingPipes.Contains(pipe));

            if (this.m_pipe == pipe)
                // If this is our current pipe, remove it
            {
                this.m_pipe = null;
            }
            else
                // Remove the pipe from the detached pipes set
            {
                this.m_terminatingPipes.Remove(pipe);
            }

            if (!this.IsTerminating && this.m_options.RawSocket)
            {
                if (this.m_engine != null)
                {
                    this.m_engine.Terminate();
                    this.m_engine = null;
                }

                this.Terminate();
            }

            // If we are waiting for pending messages to be sent, at this point
            // we are sure that there will be no more messages and we can proceed
            // with termination safely.
            if (this.m_pending && this.m_pipe == null && this.m_terminatingPipes.Count == 0)
            {
                this.ProceedWithTerm();
            }
        }

        /// <summary>
        ///     Indicate that the given pipe is now ready for reading.
        ///     Pipe calls this on it's sink in response to ProcessActivateRead.
        /// </summary>
        /// <param name="pipe">the pipe to indicate is ready for reading</param>
        public void ReadActivated(Pipe pipe)
        {
            // Skip activating if we're detaching this pipe
            if (this.m_pipe != pipe)
            {
                Debug.Assert(this.m_terminatingPipes.Contains(pipe));
                return;
            }

            if (this.m_engine != null)
            {
                this.m_engine.ActivateOut();
            }
            else
            {
                this.m_pipe.CheckRead();
            }
        }

        public void WriteActivated(Pipe pipe)
        {
            // Skip activating if we're detaching this pipe
            if (this.m_pipe != pipe)
            {
                Debug.Assert(this.m_terminatingPipes.Contains(pipe));
                return;
            }

            this.m_engine?.ActivateIn();
        }

        public void Hiccuped(Pipe pipe)
        {
            // Hiccups are always sent from session to socket, not the other
            // way round.
            throw new NotSupportedException("Must Override");
        }

        /// <summary>
        ///     Get the contained socket.
        /// </summary>
        [NotNull]
        public SocketBase Socket
        {
            get
            {
                return this.m_socket;
            }
        }

        /// <summary>
        ///     Process the Plug-request by setting this SessionBase as the handler for the io-object
        ///     and starting connecting (without waiting).
        /// </summary>
        protected override void ProcessPlug()
        {
            this.m_ioObject.SetHandler(this);
            if (this.m_connect)
            {
                this.StartConnecting(false);
            }
        }

        /// <summary>
        ///     Process the Attach-request by hooking up the pipes
        ///     and plugging in the given engine.
        /// </summary>
        /// <param name="engine">the IEngine to plug in</param>
        protected override void ProcessAttach(IEngine engine)
        {
            Debug.Assert(engine != null);

            // Create the pipe if it does not exist yet.
            if (this.m_pipe == null && !this.IsTerminating)
            {
                ZObject[] parents = {this, this.m_socket};
                int[] highWaterMarks = {this.m_options.ReceiveHighWatermark, this.m_options.SendHighWatermark};
                int[] lowWaterMarks = {this.m_options.ReceiveLowWatermark, this.m_options.SendLowWatermark};
                bool[] delays = {this.m_options.DelayOnClose, this.m_options.DelayOnDisconnect};
                Pipe[] pipes = Pipe.PipePair(parents, highWaterMarks, lowWaterMarks, delays);

                // Plug the local end of the pipe.
                pipes[0].SetEventSink(this);

                // Remember the local end of the pipe.
                Debug.Assert(this.m_pipe == null);
                this.m_pipe = pipes[0];

                // Ask socket to plug into the remote end of the pipe.
                this.SendBind(this.m_socket, pipes[1]);
            }

            // Plug in the engine.
            Debug.Assert(this.m_engine == null);
            this.m_engine = engine;
            this.m_engine.Plug(this.m_ioThread, this);
        }

        /// <summary>
        ///     Flush out any leftover messages and call Detached.
        /// </summary>
        public void Detach()
        {
            // Engine is dead. Let's forget about it.
            this.m_engine = null;

            // Remove any half-done messages from the pipes.
            this.CleanPipes();

            // Send the event to the derived class.
            this.Detached();

            // Just in case there's only a delimiter in the pipe.
            this.m_pipe?.CheckRead();
        }

        /// <summary>
        ///     Process a termination request.
        /// </summary>
        /// <param name="linger">a time (in milliseconds) for this to linger before actually going away. -1 means infinite.</param>
        protected override void ProcessTerm(int linger)
        {
            Debug.Assert(!this.m_pending);

            // If the termination of the pipe happens before the term command is
            // delivered there's nothing much to do. We can proceed with the
            // standard termination immediately.
            if (this.m_pipe == null)
            {
                this.ProceedWithTerm();
                return;
            }

            this.m_pending = true;

            // If there's finite linger value, delay the termination.
            // If linger is infinite (negative) we don't even have to set
            // the timer.
            if (linger > 0)
            {
                Debug.Assert(!this.m_hasLingerTimer);
                this.m_ioObject.AddTimer(linger, SessionBase.LingerTimerId);
                this.m_hasLingerTimer = true;
            }

            // Start pipe termination process. Delay the termination till all messages
            // are processed in case the linger time is non-zero.
            this.m_pipe.Terminate(linger != 0);

            // TODO: Should this go into pipe_t::terminate ?
            // In case there's no engine and there's only delimiter in the
            // pipe it wouldn't be ever read. Thus we check for it explicitly.
            this.m_pipe.CheckRead();
        }

        /// <summary>
        ///     Call this function to move on with the delayed process-termination request.
        /// </summary>
        private void ProceedWithTerm()
        {
            // The pending phase have just ended.
            this.m_pending = false;

            // Continue with standard termination.
            base.ProcessTerm(0);
        }

        /// <summary>
        ///     This is called when the timer expires.
        /// </summary>
        /// <param name="id">an integer used to identify the timer</param>
        public void TimerEvent(int id)
        {
            // Linger period expired. We can proceed with termination even though
            // there are still pending messages to be sent.
            Debug.Assert(id == SessionBase.LingerTimerId);
            this.m_hasLingerTimer = false;

            // Ask pipe to terminate even though there may be pending messages in it.
            Debug.Assert(this.m_pipe != null);
            this.m_pipe.Terminate(false);
        }

        /// <summary>
        ///     The parent SessionBase class calls this when the Detach method finishes detaching.
        /// </summary>
        private void Detached()
        {
            // Transient session self-destructs after peer disconnects.
            if (!this.m_connect)
            {
                this.Terminate();
                return;
            }

            // For delayed connect situations, terminate the pipe
            // and reestablish later on
            if (this.m_pipe != null && this.m_options.DelayAttachOnConnect
                                    && this.m_addr.Protocol != Address.PgmProtocol && this.m_addr.Protocol != Address.EpgmProtocol)
            {
                this.m_pipe.Hiccup();
                this.m_pipe.Terminate(false);
                this.m_terminatingPipes.Add(this.m_pipe);
                this.m_pipe = null;
            }

            this.Reset();

            // Reconnect.
            if (this.m_options.ReconnectIvl != -1)
            {
                this.StartConnecting(true);
            }

            // For subscriber sockets we hiccup the inbound pipe, which will cause
            // the socket object to resend all the subscriptions.
            if (this.m_pipe != null && (this.m_options.SocketType == ZmqSocketType.Sub || this.m_options.SocketType == ZmqSocketType.Xsub))
            {
                this.m_pipe.Hiccup();
            }
        }

        /// <summary>
        ///     Begin connecting.
        /// </summary>
        /// <param name="wait">Whether to wait a bit before actually attempting to connect</param>
        private void StartConnecting(bool wait)
        {
            Debug.Assert(this.m_connect);

            // Choose I/O thread to run connector in. Given that we are already
            // running in an I/O thread, there must be at least one available.
            IOThread ioThread = this.ChooseIOThread(this.m_options.Affinity);
            Debug.Assert(ioThread != null);

            // Create the connector object.

            switch (this.m_addr.Protocol)
            {
                case Address.TcpProtocol:
                {
                    this.LaunchChild(new TcpConnector(ioThread, this, this.m_options, this.m_addr, wait));
                    return;
                }
                case Address.IpcProtocol:
                {
                    this.LaunchChild(new IpcConnector(ioThread, this, this.m_options, this.m_addr, wait));
                    return;
                }
                case Address.PgmProtocol:
                case Address.EpgmProtocol:
                {
                    PgmSender pgmSender = new PgmSender(this.m_ioThread, this.m_options, this.m_addr, wait);
                    pgmSender.Init((PgmAddress) this.m_addr.Resolved);
                    this.SendAttach(this, pgmSender);
                    return;
                }
            }

            Debug.Assert(false);
        }

        /// <summary>
        ///     Override the ToString method to also show the socket-id.
        /// </summary>
        /// <returns>the type of this object and [ socket-id ]</returns>
        public override string ToString()
        {
            return base.ToString() + "[" + this.m_options.SocketId + "]";
        }

        /// <summary>
        ///     This method would be called when a message receive operation has been completed, although here it only throws a
        ///     NotSupportedException.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NotSupportedException">This operation is not supported on the SessionBase class.</exception>
        public virtual void InCompleted(SocketError socketError, int bytesTransferred)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     This method would be called when a message Send operation has been completed, although here it only throws a
        ///     NotSupportedException.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NotSupportedException">This operation is not supported on the SessionBase class.</exception>
        public virtual void OutCompleted(SocketError socketError, int bytesTransferred)
        {
            throw new NotSupportedException();
        }
    }
}