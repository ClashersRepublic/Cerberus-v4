namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports.Pgm
{
    using System;
    using System.Net.Sockets;
    using AsyncIO;
    using JetBrains.Annotations;

    internal class PgmListener : Own, IProactorEvents
    {
        [NotNull] private readonly SocketBase m_socket;
        [NotNull] private readonly IOObject m_ioObject;
        private AsyncSocket m_handle;
        private PgmSocket m_pgmSocket;
        private PgmSocket m_acceptedSocket;
        private PgmAddress m_address;

        public PgmListener([NotNull] IOThread ioThread, [NotNull] SocketBase socket, [NotNull] Options options)
            : base(ioThread, options)
        {
            this.m_socket = socket;

            this.m_ioObject = new IOObject(ioThread);
        }

        /// <exception cref="InvalidException">Unable to parse the address's port number, or the IP address could not be parsed.</exception>
        /// <exception cref="NetMQException">Error establishing underlying socket.</exception>
        public void Init([NotNull] string network)
        {
            this.m_address = new PgmAddress(network);

            this.m_pgmSocket = new PgmSocket(this.m_options, PgmSocketType.Listener, this.m_address);
            this.m_pgmSocket.Init();

            this.m_handle = this.m_pgmSocket.Handle;

            try
            {
                this.m_handle.Bind(this.m_address.Address);
                this.m_pgmSocket.InitOptions();
                this.m_handle.Listen(this.m_options.Backlog);
            }
            catch (SocketException ex)
            {
                this.Close();

                throw NetMQException.Create(ex);
            }

            this.m_socket.EventListening(this.m_address.ToString(), this.m_handle);
        }

        public override void Destroy()
        {
        }

        protected override void ProcessPlug()
        {
            // Start polling for incoming connections.
            this.m_ioObject.SetHandler(this);
            this.m_ioObject.AddSocket(this.m_handle);

            this.Accept();
        }

        /// <summary>
        ///     Process a termination request.
        /// </summary>
        /// <param name="linger">a time (in milliseconds) for this to linger before actually going away. -1 means infinite.</param>
        protected override void ProcessTerm(int linger)
        {
            this.m_ioObject.SetHandler(this);
            this.m_ioObject.RemoveSocket(this.m_handle);
            this.Close();
            base.ProcessTerm(linger);
        }

        private void Close()
        {
            if (this.m_handle == null)
            {
                return;
            }

            try
            {
                this.m_handle.Dispose();
                this.m_socket.EventClosed(this.m_address.ToString(), this.m_handle);
            }
            catch (SocketException ex)
            {
                this.m_socket.EventCloseFailed(this.m_address.ToString(), ex.SocketErrorCode.ToErrorCode());
            }
            catch (NetMQException ex)
            {
                this.m_socket.EventCloseFailed(this.m_address.ToString(), ex.ErrorCode);
            }

            this.m_handle = null;
        }

        /// <summary>
        ///     This method is called when a message receive operation has been completed.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        public void InCompleted(SocketError socketError, int bytesTransferred)
        {
            if (socketError != SocketError.Success)
            {
                this.m_socket.EventAcceptFailed(this.m_address.ToString(), socketError.ToErrorCode());

                // dispose old object                
                this.m_acceptedSocket.Handle.Dispose();

                this.Accept();
            }
            else
            {
                this.m_acceptedSocket.InitOptions();

                PgmSession pgmSession = new PgmSession(this.m_acceptedSocket, this.m_options);

                IOThread ioThread = this.ChooseIOThread(this.m_options.Affinity);

                SessionBase session = SessionBase.Create(ioThread, false, this.m_socket, this.m_options, new Address(this.m_handle.LocalEndPoint));

                session.IncSeqnum();
                this.LaunchChild(session);
                this.SendAttach(session, pgmSession, false);
                this.m_socket.EventAccepted(this.m_address.ToString(), this.m_acceptedSocket.Handle);

                this.Accept();
            }
        }

        private void Accept()
        {
            this.m_acceptedSocket = new PgmSocket(this.m_options, PgmSocketType.Receiver, this.m_address);
            this.m_acceptedSocket.Init();

            this.m_handle.Accept(this.m_acceptedSocket.Handle);
        }

        /// <summary>
        ///     This method would be called when a message Send operation has been completed, although here it only throws a
        ///     NotSupportedException.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NotSupportedException">This operation is not supported on the PgmListener class.</exception>
        public void OutCompleted(SocketError socketError, int bytesTransferred)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     This would be called when the a expires, although here it only throws a NotSupportedException.
        /// </summary>
        /// <param name="id">an integer used to identify the timer (not used here)</param>
        /// <exception cref="NotSupportedException">This operation is not supported on the PgmListener class.</exception>
        public void TimerEvent(int id)
        {
            throw new NotSupportedException();
        }
    }
}