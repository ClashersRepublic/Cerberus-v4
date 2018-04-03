namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports.Pgm
{
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Sockets;
    using AsyncIO;
    using JetBrains.Annotations;

    internal sealed class PgmSender : IOObject, IEngine, IProactorEvents
    {
        /// <summary>
        ///     ID of the timer used to delay the reconnection. Value is 1.
        /// </summary>
        private const int ReconnectTimerId = 1;

        private readonly Options m_options;
        private readonly Address m_addr;
        private readonly bool m_delayedStart;
        private readonly V1Encoder m_encoder;

        private AsyncSocket m_socket;
        private PgmSocket m_pgmSocket;

        private ByteArraySegment m_outBuffer;
        private int m_outBufferSize;

        private int m_writeSize;

        private enum State
        {
            Idle,
            Delaying,
            Connecting,
            Active,
            ActiveSendingIdle,
            Error
        }

        private State m_state;
        private PgmAddress m_pgmAddress;
        private SessionBase m_session;
        private int m_currentReconnectIvl;

        public PgmSender([NotNull] IOThread ioThread, [NotNull] Options options, [NotNull] Address addr, bool delayedStart)
            : base(ioThread)
        {
            this.m_options = options;
            this.m_addr = addr;
            this.m_delayedStart = delayedStart;
            this.m_encoder = null;
            this.m_outBuffer = null;
            this.m_outBufferSize = 0;
            this.m_writeSize = 0;
            this.m_encoder = new V1Encoder(0, this.m_options.Endian);
            this.m_currentReconnectIvl = this.m_options.ReconnectIvl;

            this.m_state = State.Idle;
        }

        public void Init([NotNull] PgmAddress pgmAddress)
        {
            this.m_pgmAddress = pgmAddress;

            this.m_pgmSocket = new PgmSocket(this.m_options, PgmSocketType.Publisher, (PgmAddress) this.m_addr.Resolved);
            this.m_pgmSocket.Init();

            this.m_socket = this.m_pgmSocket.Handle;

            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, 0);

            this.m_socket.Bind(localEndpoint);

            this.m_pgmSocket.InitOptions();

            this.m_outBufferSize = this.m_options.PgmMaxTransportServiceDataUnitLength;
            this.m_outBuffer = new ByteArraySegment(new byte[this.m_outBufferSize]);
        }

        public void Plug(IOThread ioThread, SessionBase session)
        {
            this.m_session = session;
            this.m_encoder.SetMsgSource(session);

            // get the first message from the session because we don't want to send identities
            Msg msg = new Msg();
            msg.InitEmpty();

            bool ok = session.PullMsg(ref msg);

            if (ok)
            {
                msg.Close();
            }

            this.AddSocket(this.m_socket);

            if (!this.m_delayedStart)
            {
                this.StartConnecting();
            }
            else
            {
                this.m_state = State.Delaying;
                this.AddTimer(this.GetNewReconnectIvl(), PgmSender.ReconnectTimerId);
            }
        }

        private void StartConnecting()
        {
            this.m_state = State.Connecting;

            try
            {
                this.m_socket.Connect(this.m_pgmAddress.Address);
            }
            catch (SocketException ex) when (ex.SocketErrorCode == SocketError.InvalidArgument)
            {
                this.Error();
            }
        }

        public void Terminate()
        {
            this.Destroy();
        }

        public void ActivateOut()
        {
            if (this.m_state == State.ActiveSendingIdle)
            {
                this.m_state = State.Active;
                this.m_writeSize = 0;
                this.BeginSending();
            }
        }

        public void ActivateIn()
        {
            Debug.Assert(false);
        }

        /// <summary>
        ///     This would be called when a timer expires, although here it only throws a NotSupportedException.
        /// </summary>
        /// <param name="id">an integer used to identify the timer (not used here)</param>
        /// <exception cref="NotImplementedException">This method must not be called on instances of PgmSender.</exception>
        public override void TimerEvent(int id)
        {
            if (this.m_state == State.Delaying)
            {
                this.StartConnecting();
            }
            else
            {
                Debug.Assert(false);
            }
        }

        /// <summary>
        ///     This method is called when a message Send operation has been completed.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NetMQException">A non-recoverable socket error occurred.</exception>
        public override void OutCompleted(SocketError socketError, int bytesTransferred)
        {
            if (this.m_state == State.Connecting)
            {
                if (socketError == SocketError.Success)
                {
                    this.m_state = State.Active;
                    this.m_writeSize = 0;

                    this.BeginSending();
                }
                else
                {
                    this.m_state = State.Error;
                    NetMQException.Create(socketError);
                }
            }
            else if (this.m_state == State.Active)
            {
                // We can write either all data or 0 which means rate limit reached.
                if (socketError == SocketError.Success && bytesTransferred == this.m_writeSize)
                {
                    this.m_writeSize = 0;

                    this.BeginSending();
                }
                else
                {
                    if (socketError == SocketError.ConnectionReset)
                    {
                        this.Error();
                    }
                    else
                    {
                        throw NetMQException.Create(socketError.ToErrorCode());
                    }
                }
            }
            else
            {
                Debug.Assert(false);
            }
        }

        private void BeginSending()
        {
            // If write buffer is empty,  try to read new data from the encoder.
            if (this.m_writeSize == 0)
            {
                // First two bytes (sizeof uint16_t) are used to store message 
                // offset in following steps. Note that by passing our buffer to
                // the get data function we prevent it from returning its own buffer.
                ByteArraySegment bf = new ByteArraySegment(this.m_outBuffer, sizeof(ushort));
                int bfsz = this.m_outBufferSize - sizeof(ushort);
                int offset = -1;
                this.m_encoder.GetData(ref bf, ref bfsz, ref offset);

                // If there are no data to write stop polling for output.
                if (bfsz == 0)
                {
                    this.m_state = State.ActiveSendingIdle;
                    return;
                }

                // Put offset information in the buffer.
                this.m_writeSize = bfsz + sizeof(ushort);

                this.m_outBuffer.PutUnsignedShort(this.m_options.Endian, offset == -1 ? (ushort) 0xffff : (ushort) offset, 0);
            }

            try
            {
                this.m_socket.Send((byte[]) this.m_outBuffer, this.m_outBuffer.Offset, this.m_writeSize, SocketFlags.None);
            }
            catch (SocketException ex)
            {
                if (ex.SocketErrorCode == SocketError.ConnectionReset)
                {
                    this.Error();
                }
                else
                {
                    throw NetMQException.Create(ex.SocketErrorCode, ex);
                }
            }
        }

        private void Error()
        {
            Debug.Assert(this.m_session != null);
            this.m_session.Detach();
            this.Destroy();
        }

        private void Destroy()
        {
            if (this.m_state == State.Delaying)
            {
                this.CancelTimer(PgmSender.ReconnectTimerId);
            }

            this.m_pgmSocket.Dispose();
            this.RemoveSocket(this.m_socket);
            this.m_encoder.SetMsgSource(null);
        }

        /// <summary>
        ///     Internal function to return a reconnect back-off delay.
        ///     Will modify the current_reconnect_ivl used for next call
        ///     Returns the currently used interval
        /// </summary>
        private int GetNewReconnectIvl()
        {
            // The new interval is the current interval + random value.
            int thisInterval = this.m_currentReconnectIvl + new Random().Next(0, this.m_options.ReconnectIvl);

            // Only change the current reconnect interval  if the maximum reconnect
            // interval was set and if it's larger than the reconnect interval.
            if (this.m_options.ReconnectIvlMax > 0 && this.m_options.ReconnectIvlMax > this.m_options.ReconnectIvl)
            {
                // Calculate the next interval
                this.m_currentReconnectIvl = this.m_currentReconnectIvl * 2;
                if (this.m_currentReconnectIvl >= this.m_options.ReconnectIvlMax)
                {
                    this.m_currentReconnectIvl = this.m_options.ReconnectIvlMax;
                }
            }

            return thisInterval;
        }

        /// <summary>
        ///     This method would be called when a message receive operation has been completed, although here it only throws a
        ///     NotSupportedException.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NotImplementedException">This method must not be called on instances of PgmSender.</exception>
        public override void InCompleted(SocketError socketError, int bytesTransferred)
        {
            throw new NotImplementedException();
        }
    }
}