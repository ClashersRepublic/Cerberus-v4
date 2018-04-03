/*
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2009 iMatix Corporation
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

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Sockets;
    using AsyncIO;
    using JetBrains.Annotations;

    internal sealed class StreamEngine : IEngine, IProactorEvents, IMsgSink
    {
        private class StateMachineAction
        {
            public StateMachineAction(Action action, SocketError socketError, int bytesTransferred)
            {
                this.Action = action;
                this.SocketError = socketError;
                this.BytesTransferred = bytesTransferred;
            }

            public Action Action { get; }

            public SocketError SocketError { get; }

            public int BytesTransferred { get; }
        }

        /// <summary>
        ///     This enum-type denotes the operational state of this StreamEngine - whether Closed, doing Handshaking, Active, or
        ///     Stalled.
        /// </summary>
        private enum State
        {
            Closed,
            Handshaking,
            Active,
            Stalled
        }

        private enum HandshakeState
        {
            Closed,
            SendingGreeting,
            ReceivingGreeting,
            SendingRestOfGreeting,
            ReceivingRestOfGreeting
        }

        private enum ReceiveState
        {
            Idle,
            Active,
            Stuck
        }

        private enum SendState
        {
            Idle,
            Active,
            Error
        }

        private enum Action
        {
            Start,
            InCompleted,
            OutCompleted,
            ActivateOut,
            ActivateIn
        }

        // Size of the greeting message:
        // Preamble (10 bytes) + version (1 byte) + socket type (1 byte).
        private const int GreetingSize = 12;
        private const int PreambleSize = 10;

        // Position of the version field in the greeting.
        private const int VersionPos = 10;

        //private IOObject io_object;
        private AsyncSocket m_handle;

        private ByteArraySegment m_inpos;
        private int m_insize;
        private DecoderBase m_decoder;
        private bool m_ioEnabled;

        private ByteArraySegment m_outpos;
        private int m_outsize;
        private EncoderBase m_encoder;

        // The receive buffer holding the greeting message
        // that we are receiving from the peer.
        private readonly byte[] m_greeting = new byte[12];

        // The number of bytes of the greeting message that
        // we have already received.

        private int m_greetingBytesRead;

        // The send buffer holding the greeting message
        // that we are sending to the peer.
        private readonly ByteArraySegment m_greetingOutputBuffer = new byte[12];

        // The session this engine is attached to.
        private SessionBase m_session;

        // Detached transient session.
        //private SessionBase leftover_session;

        private readonly Options m_options;

        // string representation of endpoint
        private readonly string m_endpoint;

        private bool m_plugged;

        // Socket
        private SocketBase m_socket;

        private IOObject m_ioObject;

        private SendState m_sendingState;
        private ReceiveState m_receivingState;

        private State m_state;
        private HandshakeState m_handshakeState;

        // queue for actions that happen during the state machine
        private readonly Queue<StateMachineAction> m_actionsQueue;

        public StreamEngine(AsyncSocket handle, Options options, string endpoint)
        {
            this.m_handle = handle;
            this.m_insize = 0;
            this.m_ioEnabled = false;
            this.m_sendingState = SendState.Idle;
            this.m_receivingState = ReceiveState.Idle;
            this.m_outsize = 0;
            this.m_session = null;
            this.m_options = options;
            this.m_plugged = false;
            this.m_endpoint = endpoint;
            this.m_socket = null;
            this.m_encoder = null;
            this.m_decoder = null;
            this.m_actionsQueue = new Queue<StateMachineAction>();

            // Set the socket buffer limits for the underlying socket.
            if (this.m_options.SendBuffer != 0)
            {
                this.m_handle.SendBufferSize = this.m_options.SendBuffer;
            }

            if (this.m_options.ReceiveBuffer != 0)
            {
                this.m_handle.ReceiveBufferSize = this.m_options.ReceiveBuffer;
            }
        }

        public void Destroy()
        {
            Debug.Assert(!this.m_plugged);

            if (this.m_handle != null)
            {
                try
                {
                    this.m_handle.Dispose();
                }
                catch (SocketException)
                {
                }

                this.m_handle = null;
            }
        }

        public void Plug(IOThread ioThread, SessionBase session)
        {
            Debug.Assert(!this.m_plugged);
            this.m_plugged = true;

            // Connect to session object.
            Debug.Assert(this.m_session == null);
            Debug.Assert(session != null);
            this.m_session = session;
            this.m_socket = this.m_session.Socket;

            this.m_ioObject = new IOObject(null);
            this.m_ioObject.SetHandler(this);

            // Connect to I/O threads poller object.
            this.m_ioObject.Plug(ioThread);
            this.m_ioObject.AddSocket(this.m_handle);
            this.m_ioEnabled = true;

            this.FeedAction(Action.Start, SocketError.Success, 0);
        }

        public void Terminate()
        {
            this.Unplug();
            this.Destroy();
        }

        private void Unplug()
        {
            Debug.Assert(this.m_plugged);
            this.m_plugged = false;

            // remove handle from proactor.
            if (this.m_ioEnabled)
            {
                this.m_ioObject.RemoveSocket(this.m_handle);
                this.m_ioEnabled = false;
            }

            // Disconnect from I/O threads poller object.
            this.m_ioObject.Unplug();

            this.m_state = State.Closed;

            // Disconnect from session object.
            this.m_encoder?.SetMsgSource(null);
            this.m_decoder?.SetMsgSink(null);
            this.m_session = null;
        }

        private void Error()
        {
            Debug.Assert(this.m_session != null);
            this.m_socket.EventDisconnected(this.m_endpoint, this.m_handle);
            this.m_session.Detach();
            this.Unplug();
            this.Destroy();
        }

        private void FeedAction(Action action, SocketError socketError, int bytesTransferred)
        {
            this.Handle(action, socketError, bytesTransferred);

            while (this.m_actionsQueue.Count > 0)
            {
                StateMachineAction stateMachineAction = this.m_actionsQueue.Dequeue();
                this.Handle(stateMachineAction.Action, stateMachineAction.SocketError, stateMachineAction.BytesTransferred);
            }
        }

        private void EnqueueAction(Action action, SocketError socketError, int bytesTransferred)
        {
            this.m_actionsQueue.Enqueue(new StateMachineAction(action, socketError, bytesTransferred));
        }

        private void Handle(Action action, SocketError socketError, int bytesTransferred)
        {
            switch (this.m_state)
            {
                case State.Closed:
                    switch (action)
                    {
                        case Action.Start:
                            if (this.m_options.RawSocket)
                            {
                                this.m_encoder = new RawEncoder(Config.OutBatchSize, this.m_session, this.m_options.Endian);
                                this.m_decoder = new RawDecoder(Config.InBatchSize, this.m_options.MaxMessageSize, this.m_session, this.m_options.Endian);

                                this.Activate();
                            }
                            else
                            {
                                this.m_state = State.Handshaking;
                                this.m_handshakeState = HandshakeState.Closed;
                                this.HandleHandshake(action, socketError, bytesTransferred);
                            }

                            break;
                    }

                    break;
                case State.Handshaking:
                    this.HandleHandshake(action, socketError, bytesTransferred);
                    break;
                case State.Active:
                    switch (action)
                    {
                        case Action.InCompleted:
                            this.m_insize = StreamEngine.EndRead(socketError, bytesTransferred);

                            this.ProcessInput();
                            break;
                        case Action.ActivateIn:

                            // if we stuck let's continue, other than that nothing to do
                            if (this.m_receivingState == ReceiveState.Stuck)
                            {
                                this.m_receivingState = ReceiveState.Active;
                                this.ProcessInput();
                            }

                            break;
                        case Action.OutCompleted:
                            int bytesSent = StreamEngine.EndWrite(socketError, bytesTransferred);

                            // IO error has occurred. We stop waiting for output events.
                            // The engine is not terminated until we detect input error;
                            // this is necessary to prevent losing incoming messages.
                            if (bytesSent == -1)
                            {
                                this.m_sendingState = SendState.Error;
                            }
                            else
                            {
                                this.m_outpos.AdvanceOffset(bytesSent);
                                this.m_outsize -= bytesSent;

                                this.BeginSending();
                            }

                            break;
                        case Action.ActivateOut:
                            // if we idle we start sending, other than do nothing
                            if (this.m_sendingState == SendState.Idle)
                            {
                                this.m_sendingState = SendState.Active;
                                this.BeginSending();
                            }

                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    break;
                case State.Stalled:
                    switch (action)
                    {
                        case Action.ActivateIn:
                            // There was an input error but the engine could not
                            // be terminated (due to the stalled decoder).
                            // Flush the pending message and terminate the engine now.
                            this.m_decoder.ProcessBuffer(this.m_inpos, 0);
                            Debug.Assert(!this.m_decoder.Stalled());
                            this.m_session.Flush();
                            this.Error();
                            break;
                        case Action.ActivateOut:
                            break;
                    }

                    break;
            }
        }

        private void BeginSending()
        {
            if (this.m_outsize == 0)
            {
                this.m_outpos = null;
                this.m_encoder.GetData(ref this.m_outpos, ref this.m_outsize);

                if (this.m_outsize == 0)
                {
                    this.m_sendingState = SendState.Idle;
                }
                else
                {
                    this.BeginWrite(this.m_outpos, this.m_outsize);
                }
            }
            else
            {
                this.BeginWrite(this.m_outpos, this.m_outsize);
            }
        }

        private void HandleHandshake(Action action, SocketError socketError, int bytesTransferred)
        {
            int bytesSent;
            int bytesReceived;

            switch (this.m_handshakeState)
            {
                case HandshakeState.Closed:
                    switch (action)
                    {
                        case Action.Start:
                            // Send the 'length' and 'flags' fields of the identity message.
                            // The 'length' field is encoded in the long format.

                            this.m_greetingOutputBuffer[this.m_outsize++] = 0xff;
                            this.m_greetingOutputBuffer.PutLong(this.m_options.Endian, (long) this.m_options.IdentitySize + 1, 1);
                            this.m_outsize += 8;
                            this.m_greetingOutputBuffer[this.m_outsize++] = 0x7f;

                            this.m_outpos = new ByteArraySegment(this.m_greetingOutputBuffer);

                            this.m_handshakeState = HandshakeState.SendingGreeting;

                            this.BeginWrite(this.m_outpos, this.m_outsize);
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    break;
                case HandshakeState.SendingGreeting:
                    switch (action)
                    {
                        case Action.OutCompleted:
                            bytesSent = StreamEngine.EndWrite(socketError, bytesTransferred);

                            if (bytesSent == -1)
                            {
                                this.Error();
                            }
                            else
                            {
                                this.m_outpos.AdvanceOffset(bytesSent);
                                this.m_outsize -= bytesSent;

                                if (this.m_outsize > 0)
                                {
                                    this.BeginWrite(this.m_outpos, this.m_outsize);
                                }
                                else
                                {
                                    this.m_greetingBytesRead = 0;

                                    ByteArraySegment greetingSegment = new ByteArraySegment(this.m_greeting, this.m_greetingBytesRead);

                                    this.m_handshakeState = HandshakeState.ReceivingGreeting;

                                    this.BeginRead(greetingSegment, StreamEngine.PreambleSize);
                                }
                            }

                            break;
                        case Action.ActivateIn:
                        case Action.ActivateOut:
                            // nothing to do
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    break;
                case HandshakeState.ReceivingGreeting:
                    switch (action)
                    {
                        case Action.InCompleted:
                            bytesReceived = StreamEngine.EndRead(socketError, bytesTransferred);

                            if (bytesReceived == -1)
                            {
                                this.Error();
                            }
                            else
                            {
                                this.m_greetingBytesRead += bytesReceived;

                                // check if it is an unversioned protocol
                                if (this.m_greeting[0] != 0xff || this.m_greetingBytesRead == 10 && (this.m_greeting[9] & 0x01) == 0)
                                {
                                    this.m_encoder = new V1Encoder(Config.OutBatchSize, this.m_options.Endian);
                                    this.m_encoder.SetMsgSource(this.m_session);

                                    this.m_decoder = new V1Decoder(Config.InBatchSize, this.m_options.MaxMessageSize, this.m_options.Endian);
                                    this.m_decoder.SetMsgSink(this.m_session);

                                    // We have already sent the message header.
                                    // Since there is no way to tell the encoder to
                                    // skip the message header, we simply throw that
                                    // header data away.
                                    int headerSize = this.m_options.IdentitySize + 1 >= 255 ? 10 : 2;
                                    byte[] tmp = new byte[10];
                                    ByteArraySegment bufferp = new ByteArraySegment(tmp);

                                    int bufferSize = headerSize;

                                    this.m_encoder.GetData(ref bufferp, ref bufferSize);

                                    Debug.Assert(bufferSize == headerSize);

                                    // Make sure the decoder sees the data we have already received.
                                    this.m_inpos = new ByteArraySegment(this.m_greeting);
                                    this.m_insize = this.m_greetingBytesRead;

                                    // To allow for interoperability with peers that do not forward
                                    // their subscriptions, we inject a phony subscription
                                    // message into the incoming message stream. To put this
                                    // message right after the identity message, we temporarily
                                    // divert the message stream from session to ourselves.
                                    if (this.m_options.SocketType == ZmqSocketType.Pub || this.m_options.SocketType == ZmqSocketType.Xpub)
                                    {
                                        this.m_decoder.SetMsgSink(this);
                                    }

                                    // handshake is done
                                    this.Activate();
                                }
                                else if (this.m_greetingBytesRead < 10)
                                {
                                    ByteArraySegment greetingSegment = new ByteArraySegment(this.m_greeting, this.m_greetingBytesRead);
                                    this.BeginRead(greetingSegment, StreamEngine.PreambleSize - this.m_greetingBytesRead);
                                }
                                else
                                {
                                    // The peer is using versioned protocol.
                                    // Send the rest of the greeting.
                                    this.m_outpos[this.m_outsize++] = 1; // Protocol version
                                    this.m_outpos[this.m_outsize++] = (byte) this.m_options.SocketType;

                                    this.m_handshakeState = HandshakeState.SendingRestOfGreeting;

                                    this.BeginWrite(this.m_outpos, this.m_outsize);
                                }
                            }

                            break;
                        case Action.ActivateIn:
                        case Action.ActivateOut:
                            // nothing to do
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    break;
                case HandshakeState.SendingRestOfGreeting:
                    switch (action)
                    {
                        case Action.OutCompleted:
                            bytesSent = StreamEngine.EndWrite(socketError, bytesTransferred);

                            if (bytesSent == -1)
                            {
                                this.Error();
                            }
                            else
                            {
                                this.m_outpos.AdvanceOffset(bytesSent);
                                this.m_outsize -= bytesSent;

                                if (this.m_outsize > 0)
                                {
                                    this.BeginWrite(this.m_outpos, this.m_outsize);
                                }
                                else
                                {
                                    ByteArraySegment greetingSegment = new ByteArraySegment(this.m_greeting, this.m_greetingBytesRead);

                                    this.m_handshakeState = HandshakeState.ReceivingRestOfGreeting;
                                    this.BeginRead(greetingSegment, StreamEngine.GreetingSize - this.m_greetingBytesRead);
                                }
                            }

                            break;
                        case Action.ActivateIn:
                        case Action.ActivateOut:
                            // nothing to do
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    break;
                case HandshakeState.ReceivingRestOfGreeting:
                    switch (action)
                    {
                        case Action.InCompleted:
                            bytesReceived = StreamEngine.EndRead(socketError, bytesTransferred);

                            if (bytesReceived == -1)
                            {
                                this.Error();
                            }
                            else
                            {
                                this.m_greetingBytesRead += bytesReceived;

                                if (this.m_greetingBytesRead < StreamEngine.GreetingSize)
                                {
                                    ByteArraySegment greetingSegment = new ByteArraySegment(this.m_greeting, this.m_greetingBytesRead);
                                    this.BeginRead(greetingSegment, StreamEngine.GreetingSize - this.m_greetingBytesRead);
                                }
                                else
                                {
                                    if (this.m_greeting[StreamEngine.VersionPos] == 0)
                                    {
                                        // ZMTP/1.0 framing.
                                        this.m_encoder = new V1Encoder(Config.OutBatchSize, this.m_options.Endian);
                                        this.m_encoder.SetMsgSource(this.m_session);

                                        this.m_decoder = new V1Decoder(Config.InBatchSize, this.m_options.MaxMessageSize, this.m_options.Endian);
                                        this.m_decoder.SetMsgSink(this.m_session);
                                    }
                                    else
                                    {
                                        // v1 framing protocol.
                                        this.m_encoder = new V2Encoder(Config.OutBatchSize, this.m_session, this.m_options.Endian);
                                        this.m_decoder = new V2Decoder(Config.InBatchSize, this.m_options.MaxMessageSize, this.m_session, this.m_options.Endian);
                                    }

                                    // handshake is done
                                    this.Activate();
                                }
                            }

                            break;
                        case Action.ActivateIn:
                        case Action.ActivateOut:
                            // nothing to do
                            break;
                        default:
                            Debug.Assert(false);
                            break;
                    }

                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private void Activate()
        {
            // Handshaking was successful.
            // Switch into the normal message flow.            
            this.m_state = State.Active;

            this.m_outsize = 0;

            this.m_sendingState = SendState.Active;
            this.BeginSending();

            this.m_receivingState = ReceiveState.Active;

            if (this.m_insize == 0)
            {
                this.m_decoder.GetBuffer(out this.m_inpos, out this.m_insize);
                this.BeginRead(this.m_inpos, this.m_insize);
            }
            else
            {
                this.ProcessInput();
            }
        }

        private void ProcessInput()
        {
            bool disconnection = false;
            int processed;

            if (this.m_insize == -1)
            {
                this.m_insize = 0;
                disconnection = true;
            }

            if (this.m_options.RawSocket)
            {
                if (this.m_insize == 0 || !this.m_decoder.MessageReadySize(this.m_insize))
                {
                    processed = 0;
                }
                else
                {
                    processed = this.m_decoder.ProcessBuffer(this.m_inpos, this.m_insize);
                }
            }
            else
            {
                // Push the data to the decoder.
                processed = this.m_decoder.ProcessBuffer(this.m_inpos, this.m_insize);
            }

            if (processed == -1)
            {
                disconnection = true;
            }
            else
            {
                // Stop polling for input if we got stuck.
                if (processed < this.m_insize)
                {
                    this.m_receivingState = ReceiveState.Stuck;

                    this.m_inpos.AdvanceOffset(processed);
                    this.m_insize -= processed;
                }
                else
                {
                    this.m_inpos = null;
                    this.m_insize = 0;
                }
            }

            // Flush all messages the decoder may have produced.
            this.m_session.Flush();

            // An input error has occurred. If the last decoded message
            // has already been accepted, we terminate the engine immediately.
            // Otherwise, we stop waiting for socket events and postpone
            // the termination until after the message is accepted.
            if (disconnection)
            {
                if (this.m_decoder.Stalled())
                {
                    this.m_ioObject.RemoveSocket(this.m_handle);
                    this.m_ioEnabled = false;
                    this.m_state = State.Stalled;
                }
                else
                {
                    this.Error();
                }
            }
            else if (this.m_receivingState != ReceiveState.Stuck)
            {
                this.m_decoder.GetBuffer(out this.m_inpos, out this.m_insize);
                this.BeginRead(this.m_inpos, this.m_insize);
            }
        }

        /// <summary>
        ///     This method is be called when a message receive operation has been completed.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        public void InCompleted(SocketError socketError, int bytesTransferred)
        {
            this.FeedAction(Action.InCompleted, socketError, bytesTransferred);
        }

        public void ActivateIn()
        {
            this.FeedAction(Action.ActivateIn, SocketError.Success, 0);
        }

        /// <summary>
        ///     This method is called when a message Send operation has been completed.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        public void OutCompleted(SocketError socketError, int bytesTransferred)
        {
            this.FeedAction(Action.OutCompleted, socketError, bytesTransferred);
        }

        public void ActivateOut()
        {
            this.FeedAction(Action.ActivateOut, SocketError.Success, 0);
        }

        public bool PushMsg(ref Msg msg)
        {
            Debug.Assert(this.m_options.SocketType == ZmqSocketType.Pub || this.m_options.SocketType == ZmqSocketType.Xpub);

            // The first message is identity.
            // Let the session process it.

            this.m_session.PushMsg(ref msg);

            // Inject the subscription message so that the ZMQ 2.x peer
            // receives our messages.
            msg.InitPool(1);
            msg.Put(1);

            bool isMessagePushed = this.m_session.PushMsg(ref msg);

            this.m_session.Flush();

            // Once we have injected the subscription message, we can
            // Divert the message flow back to the session.
            Debug.Assert(this.m_decoder != null);
            this.m_decoder.SetMsgSink(this.m_session);

            return isMessagePushed;
        }

        /// <param name="socketError">the SocketError that resulted from the write - which could be Success (no error at all)</param>
        /// <param name="bytesTransferred">this indicates the number of bytes that were transferred in the write</param>
        /// <returns>the number of bytes transferred if successful, -1 otherwise</returns>
        /// <exception cref="NetMQException">
        ///     If the socketError is not Success then it must be a valid recoverable error or the
        ///     number of bytes transferred must be zero.
        /// </exception>
        /// <remarks>
        ///     If socketError is SocketError.Success and bytesTransferred is > 0, then this returns bytesTransferred.
        ///     If bytes is zero, or the socketError is one of NetworkDown, NetworkReset, HostUn, Connection Aborted, TimedOut, or
        ///     ConnectionReset, - then -1 is returned.
        ///     Otherwise, a NetMQException is thrown.
        /// </remarks>
        private static int EndWrite(SocketError socketError, int bytesTransferred)
        {
            if (socketError == SocketError.Success && bytesTransferred > 0)
            {
                return bytesTransferred;
            }

            if (bytesTransferred == 0 ||
                socketError == SocketError.NetworkDown ||
                socketError == SocketError.NetworkReset ||
                socketError == SocketError.HostUnreachable ||
                socketError == SocketError.ConnectionAborted ||
                socketError == SocketError.TimedOut ||
                socketError == SocketError.ConnectionReset ||
                socketError == SocketError.AccessDenied)
            {
                return -1;
            }

            throw NetMQException.Create(socketError);
        }

        private void BeginWrite([NotNull] ByteArraySegment data, int size)
        {
            try
            {
                this.m_handle.Send((byte[]) data, data.Offset, size, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                this.EnqueueAction(Action.OutCompleted, ex.SocketErrorCode, 0);
            }
        }

        /// <param name="socketError">the SocketError that resulted from the read - which could be Success (no error at all)</param>
        /// <param name="bytesTransferred">this indicates the number of bytes that were transferred in the read</param>
        /// <returns>the number of bytes transferred if successful, -1 otherwise</returns>
        /// <exception cref="NetMQException">
        ///     If the socketError is not Success then it must be a valid recoverable error or the
        ///     number of bytes transferred must be zero.
        /// </exception>
        /// <remarks>
        ///     If socketError is SocketError.Success and bytesTransferred is > 0, then this returns bytesTransferred.
        ///     If bytes is zero, or the socketError is one of NetworkDown, NetworkReset, HostUn, Connection Aborted, TimedOut, or
        ///     ConnectionReset, - then -1 is returned.
        ///     Otherwise, a NetMQException is thrown.
        /// </remarks>
        private static int EndRead(SocketError socketError, int bytesTransferred)
        {
            if (socketError == SocketError.Success && bytesTransferred > 0)
            {
                return bytesTransferred;
            }

            if (bytesTransferred == 0 ||
                socketError == SocketError.NetworkDown ||
                socketError == SocketError.NetworkReset ||
                socketError == SocketError.HostUnreachable ||
                socketError == SocketError.ConnectionAborted ||
                socketError == SocketError.TimedOut ||
                socketError == SocketError.ConnectionReset ||
                socketError == SocketError.AccessDenied)
            {
                return -1;
            }

            throw NetMQException.Create(socketError);
        }

        private void BeginRead([NotNull] ByteArraySegment data, int size)
        {
            try
            {
                this.m_handle.Receive((byte[]) data, data.Offset, size, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                this.EnqueueAction(Action.InCompleted, ex.SocketErrorCode, 0);
            }
        }

        /// <summary>
        ///     This would be called when a timer expires, although here it only throws NotSupportedException.
        /// </summary>
        /// <param name="id">an integer used to identify the timer (not used here)</param>
        /// <exception cref="NotSupportedException">TimerEvent is not supported on StreamEngine.</exception>
        public void TimerEvent(int id)
        {
            throw new NotSupportedException();
        }
    }
}