/*
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2010 iMatix Corporation
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

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports.Tcp
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using AsyncIO;
    using JetBrains.Annotations;

    internal class TcpListener : Own, IProactorEvents
    {
        private const SocketOptionName IPv6Only = (SocketOptionName) 27;

        [NotNull] private readonly IOObject m_ioObject;

        /// <summary>
        ///     Address to listen on.
        /// </summary>
        [NotNull] private readonly TcpAddress m_address;

        /// <summary>
        ///     Underlying socket.
        /// </summary>
        [CanBeNull] private AsyncSocket m_handle;

        /// <summary>
        ///     socket being accepted
        /// </summary>
        /// <summary>
        ///     Socket the listener belongs to.
        /// </summary>
        [NotNull] private readonly SocketBase m_socket;

        /// <summary>
        ///     String representation of endpoint to bind to
        /// </summary>
        private string m_endpoint;

        /// <summary>
        ///     The port that was bound on
        /// </summary>
        private int m_port;

        /// <summary>
        ///     Create a new TcpListener on the given IOThread and socket.
        /// </summary>
        /// <param name="ioThread">the IOThread for this to live within</param>
        /// <param name="socket">a SocketBase to listen on</param>
        /// <param name="options">socket-related Options</param>
        public TcpListener([NotNull] IOThread ioThread, [NotNull] SocketBase socket, [NotNull] Options options)
            : base(ioThread, options)
        {
            this.m_ioObject = new IOObject(ioThread);
            this.m_address = new TcpAddress();
            this.m_handle = null;
            this.m_socket = socket;
        }

        /// <summary>
        ///     Release any contained resources (here - does nothing).
        /// </summary>
        public override void Destroy()
        {
            Debug.Assert(this.m_handle == null);
        }

        protected override void ProcessPlug()
        {
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

        /// <summary>
        ///     Set address to listen on.
        /// </summary>
        /// <param name="addr">a string denoting the address to set this to</param>
        public virtual void SetAddress([NotNull] string addr)
        {
            this.m_address.Resolve(addr, this.m_options.IPv4Only);

            try
            {
                this.m_handle = AsyncSocket.Create(this.m_address.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                Debug.Assert(this.m_handle != null);

                if (!this.m_options.IPv4Only && this.m_address.Address.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    try
                    {
                        // This is not supported on old windows operation system and might throw exception
                        this.m_handle.SetSocketOption(SocketOptionLevel.IPv6, TcpListener.IPv6Only, 0);
                    }
                    catch
                    {
                    }
                }

                #if NETSTANDARD1_3
                // This command is failing on linux
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    m_handle.ExclusiveAddressUse = false;
                                #else
                this.m_handle.ExclusiveAddressUse = false;
                #endif
                this.m_handle.Bind(this.m_address.Address);
                this.m_handle.Listen(this.m_options.Backlog);

                // Copy the port number after binding in case we requested a system-allocated port number (TCP port zero)
                this.m_address.Address.Port = this.m_handle.LocalEndPoint.Port;
                this.m_endpoint = this.m_address.ToString();

                this.m_socket.EventListening(this.m_endpoint, this.m_handle);

                this.m_port = this.m_handle.LocalEndPoint.Port;
            }
            catch (SocketException ex)
            {
                this.Close();
                throw NetMQException.Create(ex);
            }
        }

        private void Accept()
        {
            //m_acceptedSocket = AsyncSocket.Create(m_address.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            // start accepting socket async
            this.m_handle.Accept();

            // Disable TIME_WAIT tcp state
            if (this.m_options.DisableTimeWait)
            {
                this.m_handle.LingerState = new LingerOption(true, 0);
            }
        }

        /// <summary>
        ///     This is called when socket input has been completed.
        /// </summary>
        /// <param name="socketError">This indicates the status of the input operation - whether Success or some error.</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NetMQException">A non-recoverable socket-error occurred.</exception>
        public void InCompleted(SocketError socketError, int bytesTransferred)
        {
            switch (socketError)
            {
                case SocketError.Success:
                {
                    // TODO: check TcpFilters
                    AsyncSocket acceptedSocket = this.m_handle.GetAcceptedSocket();

                    acceptedSocket.NoDelay = true;

                    if (this.m_options.TcpKeepalive != -1)
                    {
                        acceptedSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, this.m_options.TcpKeepalive);

                        if (this.m_options.TcpKeepaliveIdle != -1 && this.m_options.TcpKeepaliveIntvl != -1)
                        {
                            ByteArraySegment bytes = new ByteArraySegment(new byte[12]);

                            Endianness endian = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

                            bytes.PutInteger(endian, this.m_options.TcpKeepalive, 0);
                            bytes.PutInteger(endian, this.m_options.TcpKeepaliveIdle, 4);
                            bytes.PutInteger(endian, this.m_options.TcpKeepaliveIntvl, 8);

                            acceptedSocket.IOControl(IOControlCode.KeepAliveValues, (byte[]) bytes, null);
                        }
                    }

                    // Create the engine object for this connection.
                    StreamEngine engine = new StreamEngine(acceptedSocket, this.m_options, this.m_endpoint);

                    // Choose I/O thread to run connector in. Given that we are already
                    // running in an I/O thread, there must be at least one available.
                    IOThread ioThread = this.ChooseIOThread(this.m_options.Affinity);

                    // Create and launch a session object. 
                    // TODO: send null in address parameter, is unneeded in this case
                    SessionBase session = SessionBase.Create(ioThread, false, this.m_socket, this.m_options, new Address(this.m_handle.LocalEndPoint));
                    session.IncSeqnum();
                    this.LaunchChild(session);

                    this.SendAttach(session, engine, false);

                    this.m_socket.EventAccepted(this.m_endpoint, acceptedSocket);

                    this.Accept();
                    break;
                }
                case SocketError.ConnectionReset:
                case SocketError.NoBufferSpaceAvailable:
                case SocketError.TooManyOpenSockets:
                {
                    this.m_socket.EventAcceptFailed(this.m_endpoint, socketError.ToErrorCode());

                    this.Accept();
                    break;
                }
                default:
                {
                    NetMQException exception = NetMQException.Create(socketError);

                    this.m_socket.EventAcceptFailed(this.m_endpoint, exception.ErrorCode);
                    throw exception;
                }
            }
        }

        /// <summary>
        ///     Close the listening socket.
        /// </summary>
        private void Close()
        {
            if (this.m_handle == null)
            {
                return;
            }

            try
            {
                this.m_handle.Dispose();
                this.m_socket.EventClosed(this.m_endpoint, this.m_handle);
            }
            catch (SocketException ex)
            {
                this.m_socket.EventCloseFailed(this.m_endpoint, ex.SocketErrorCode.ToErrorCode());
            }

            this.m_handle = null;
        }

        /// <summary>
        ///     Get the bound address for use with wildcards
        /// </summary>
        [NotNull]
        public virtual string Address
        {
            get
            {
                return this.m_address.ToString();
            }
        }

        /// <summary>
        ///     Get the port-number to listen on.
        /// </summary>
        public virtual int Port
        {
            get
            {
                return this.m_port;
            }
        }

        /// <summary>
        ///     This method would be called when a message Send operation has been completed - except in this case it simply throws
        ///     a NotImplementedException.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NotImplementedException">OutCompleted is not implemented on TcpListener.</exception>
        void IProactorEvents.OutCompleted(SocketError socketError, int bytesTransferred)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     This would be called when a timer expires, although here it only throws a NotSupportedException.
        /// </summary>
        /// <param name="id">an integer used to identify the timer (not used here)</param>
        /// <exception cref="NotSupportedException">TimerEvent is not supported on TcpListener.</exception>
        public void TimerEvent(int id)
        {
            throw new NotSupportedException();
        }
    }
}