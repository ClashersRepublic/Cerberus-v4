namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports.Pgm
{
    using System.Diagnostics;
    using System.Net.Sockets;
    using AsyncIO;
    using JetBrains.Annotations;

    internal sealed class PgmSession : IEngine, IProactorEvents
    {
        private AsyncSocket m_handle;
        private readonly Options m_options;
        private IOObject m_ioObject;
        private SessionBase m_session;
        private V1Decoder m_decoder;
        private bool m_joined;

        private int m_pendingBytes;
        private ByteArraySegment m_pendingData;

        private readonly ByteArraySegment m_data;

        /// <summary>
        ///     This enum-type is Idle, Receiving, Stuck, or Error.
        /// </summary>
        private enum State
        {
            Idle,
            Receiving,
            Stuck,
            Error
        }

        private State m_state;

        public PgmSession([NotNull] PgmSocket pgmSocket, [NotNull] Options options)
        {
            this.m_handle = pgmSocket.Handle;
            this.m_options = options;
            this.m_data = new byte[Config.PgmMaxTPDU];
            this.m_joined = false;

            this.m_state = State.Idle;
        }

        void IEngine.Plug(IOThread ioThread, SessionBase session)
        {
            this.m_session = session;
            this.m_ioObject = new IOObject(null);
            this.m_ioObject.SetHandler(this);
            this.m_ioObject.Plug(ioThread);
            this.m_ioObject.AddSocket(this.m_handle);

            this.DropSubscriptions();

            Msg msg = new Msg();
            msg.InitEmpty();

            // push message to the session because there is no identity message with pgm
            session.PushMsg(ref msg);

            this.m_state = State.Receiving;
            this.BeginReceive();
        }

        public void Terminate()
        {
        }

        public void BeginReceive()
        {
            this.m_data.Reset();
            this.m_handle.Receive((byte[]) this.m_data);
        }

        public void ActivateIn()
        {
            if (this.m_state == State.Stuck)
            {
                Debug.Assert(this.m_decoder != null);
                Debug.Assert(this.m_pendingData != null);

                // Ask the decoder to process remaining data.
                int n = this.m_decoder.ProcessBuffer(this.m_pendingData, this.m_pendingBytes);
                this.m_pendingBytes -= n;
                this.m_session.Flush();

                if (this.m_pendingBytes == 0)
                {
                    this.m_state = State.Receiving;
                    this.BeginReceive();
                }
            }
        }

        /// <summary>
        ///     This method is be called when a message receive operation has been completed.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        public void InCompleted(SocketError socketError, int bytesTransferred)
        {
            if (socketError != SocketError.Success || bytesTransferred == 0)
            {
                this.m_joined = false;
                this.Error();
            }
            else
            {
                // Read the offset of the fist message in the current packet.
                Debug.Assert(bytesTransferred >= sizeof(ushort));

                ushort offset = this.m_data.GetUnsignedShort(this.m_options.Endian, 0);
                this.m_data.AdvanceOffset(sizeof(ushort));
                bytesTransferred -= sizeof(ushort);

                // Join the stream if needed.
                if (!this.m_joined)
                {
                    // There is no beginning of the message in current packet.
                    // Ignore the data.
                    if (offset == 0xffff)
                    {
                        this.BeginReceive();
                        return;
                    }

                    Debug.Assert(offset <= bytesTransferred);
                    Debug.Assert(this.m_decoder == null);

                    // We have to move data to the beginning of the first message.
                    this.m_data.AdvanceOffset(offset);
                    bytesTransferred -= offset;

                    // Mark the stream as joined.
                    this.m_joined = true;

                    // Create and connect decoder for the peer.
                    this.m_decoder = new V1Decoder(0, this.m_options.MaxMessageSize, this.m_options.Endian);
                    this.m_decoder.SetMsgSink(this.m_session);
                }

                // Push all the data to the decoder.
                int processed = this.m_decoder.ProcessBuffer(this.m_data, bytesTransferred);
                if (processed < bytesTransferred)
                {
                    // Save some state so we can resume the decoding process later.
                    this.m_pendingBytes = bytesTransferred - processed;
                    this.m_pendingData = new ByteArraySegment(this.m_data, processed);

                    this.m_state = State.Stuck;
                }
                else
                {
                    this.m_session.Flush();

                    this.BeginReceive();
                }
            }
        }

        private void Error()
        {
            Debug.Assert(this.m_session != null);

            this.m_session.Detach();

            this.m_ioObject.RemoveSocket(this.m_handle);

            // Disconnect from I/O threads poller object.
            this.m_ioObject.Unplug();

            // Disconnect from session object.
            this.m_decoder?.SetMsgSink(null);

            this.m_session = null;

            this.m_state = State.Error;

            this.Destroy();
        }

        public void Destroy()
        {
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

        /// <summary>
        ///     This method would be called when a message Send operation has been completed, except in this case this method does
        ///     nothing.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        public void OutCompleted(SocketError socketError, int bytesTransferred)
        {
        }

        /// <summary>
        ///     This would be called when a timer expires, although here it does nothing.
        /// </summary>
        /// <param name="id">an integer used to identify the timer (not used here)</param>
        public void TimerEvent(int id)
        {
        }

        private void DropSubscriptions()
        {
            Msg msg = new Msg();
            msg.InitEmpty();

            while (this.m_session.PullMsg(ref msg))
            {
                msg.Close();
            }
        }

        public void ActivateOut()
        {
            this.DropSubscriptions();
        }
    }
}