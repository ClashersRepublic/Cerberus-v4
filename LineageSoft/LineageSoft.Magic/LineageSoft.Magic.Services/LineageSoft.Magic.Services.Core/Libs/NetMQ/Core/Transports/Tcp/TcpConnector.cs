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

namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Transports.Tcp
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using AsyncIO;
    using JetBrains.Annotations;

    /// <summary>
    ///     If 'delay' is true connector first waits for a while, then starts connection process.
    /// </summary>
    internal class TcpConnector : Own, IProactorEvents
    {
        /// <summary>
        ///     ID of the timer used to delay the reconnection. Value is 1.
        /// </summary>
        private const int ReconnectTimerId = 1;

        private readonly IOObject m_ioObject;

        /// <summary>
        ///     Address to connect to. Owned by session_base_t.
        /// </summary>
        private readonly Address m_addr;

        /// <summary>
        ///     The underlying AsyncSocket.
        /// </summary>
        [CanBeNull] private AsyncSocket m_s;

        /// <summary>
        ///     If true file descriptor is registered with the poller and 'handle'
        ///     contains valid value.
        /// </summary>
        private bool m_handleValid;

        /// <summary>
        ///     If true, connector is waiting a while before trying to connect.
        /// </summary>
        private readonly bool m_delayedStart;

        /// <summary>
        ///     True if a timer has been started.
        /// </summary>
        private bool m_timerStarted;

        /// <summary>
        ///     Reference to the session we belong to.
        /// </summary>
        private readonly SessionBase m_session;

        /// <summary>
        ///     Current reconnect-interval. This gets updated for back-off strategy.
        /// </summary>
        private int m_currentReconnectIvl;

        /// <summary>
        ///     String representation of endpoint to connect to
        /// </summary>
        private readonly string m_endpoint;

        /// <summary>
        ///     Socket
        /// </summary>
        private readonly SocketBase m_socket;

        /// <summary>
        ///     Create a new TcpConnector object.
        /// </summary>
        /// <param name="ioThread">the I/O-thread for this TcpConnector to live on.</param>
        /// <param name="session">the session that will contain this</param>
        /// <param name="options">Options that define this new TcpC</param>
        /// <param name="addr">the Address for this Tcp to connect to</param>
        /// <param name="delayedStart">this boolean flag dictates whether to wait before trying to connect</param>
        public TcpConnector([NotNull] IOThread ioThread, [NotNull] SessionBase session, [NotNull] Options options, [NotNull] Address addr, bool delayedStart)
            : base(ioThread, options)
        {
            this.m_ioObject = new IOObject(ioThread);
            this.m_addr = addr;
            this.m_s = null;
            this.m_handleValid = false;
            this.m_delayedStart = delayedStart;
            this.m_timerStarted = false;
            this.m_session = session;
            this.m_currentReconnectIvl = this.m_options.ReconnectIvl;

            Debug.Assert(this.m_addr != null);
            this.m_endpoint = this.m_addr.ToString();
            this.m_socket = session.Socket;
        }

        /// <summary>
        ///     This does nothing.
        /// </summary>
        public override void Destroy()
        {
            Debug.Assert(!this.m_timerStarted);
            Debug.Assert(!this.m_handleValid);
            Debug.Assert(this.m_s == null);
        }

        /// <summary>
        ///     Begin connecting.  If a delayed-start was specified - then the reconnect-timer is set, otherwise this starts
        ///     immediately.
        /// </summary>
        protected override void ProcessPlug()
        {
            this.m_ioObject.SetHandler(this);
            if (this.m_delayedStart)
            {
                this.AddReconnectTimer();
            }
            else
            {
                this.StartConnecting();
            }
        }

        /// <summary>
        ///     Process a termination request.
        ///     This cancels the reconnect-timer, closes the AsyncSocket, and marks the socket-handle as invalid.
        /// </summary>
        /// <param name="linger">a time (in milliseconds) for this to linger before actually going away. -1 means infinite.</param>
        protected override void ProcessTerm(int linger)
        {
            if (this.m_timerStarted)
            {
                this.m_ioObject.CancelTimer(TcpConnector.ReconnectTimerId);
                this.m_timerStarted = false;
            }

            if (this.m_handleValid)
            {
                this.m_ioObject.RemoveSocket(this.m_s);
                this.m_handleValid = false;
            }

            if (this.m_s != null)
            {
                this.Close();
            }

            base.ProcessTerm(linger);
        }

        /// <summary>
        ///     This method would be called when a message receive operation has been completed, although here it only throws a
        ///     NotImplementedException.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NotImplementedException">InCompleted must not be called on a TcpConnector.</exception>
        public void InCompleted(SocketError socketError, int bytesTransferred)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Internal function to start the actual connection establishment.
        /// </summary>
        private void StartConnecting()
        {
            Debug.Assert(this.m_s == null);

            // Create the socket.
            try
            {
                this.m_s = AsyncSocket.Create(this.m_addr.Resolved.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SocketException)
            {
                this.AddReconnectTimer();
                return;
            }

            this.m_ioObject.AddSocket(this.m_s);
            this.m_handleValid = true;

            // Connect to the remote peer.
            try
            {
                this.m_s.Connect(this.m_addr.Resolved.Address.Address, this.m_addr.Resolved.Address.Port);
                this.m_socket.EventConnectDelayed(this.m_endpoint, ErrorCode.InProgress);
            }
            catch (SocketException ex)
            {
                this.OutCompleted(ex.SocketErrorCode, 0);
            }
            // TerminatingException can occur in above call to EventConnectDelayed via 
            // MonitorEvent.Write if corresponding PairSocket has been sent Term command
            catch (TerminatingException)
            {
            }
        }

        /// <summary>
        ///     This method is called when a message Send operation has been completed.
        /// </summary>
        /// <param name="socketError">a SocketError value that indicates whether Success or an error occurred</param>
        /// <param name="bytesTransferred">the number of bytes that were transferred</param>
        /// <exception cref="NetMQException">A non-recoverable socket error occurred.</exception>
        /// <exception cref="NetMQException">If the socketError is not Success then it must be a valid recoverable error.</exception>
        public void OutCompleted(SocketError socketError, int bytesTransferred)
        {
            if (socketError != SocketError.Success)
            {
                this.m_ioObject.RemoveSocket(this.m_s);
                this.m_handleValid = false;

                this.Close();

                // Try again to connect after a time,
                // as long as the error is one of these..
                if (socketError == SocketError.ConnectionRefused || socketError == SocketError.TimedOut ||
                    socketError == SocketError.ConnectionAborted ||
                    socketError == SocketError.HostUnreachable || socketError == SocketError.NetworkUnreachable ||
                    socketError == SocketError.NetworkDown || socketError == SocketError.AccessDenied ||
                    socketError == SocketError.OperationAborted)
                {
                    if (this.m_options.ReconnectIvl >= 0)
                    {
                        this.AddReconnectTimer();
                    }
                }
                else
                {
                    throw NetMQException.Create(socketError);
                }
            }
            else
            {
                this.m_ioObject.RemoveSocket(this.m_s);
                this.m_handleValid = false;

                this.m_s.NoDelay = true;

                // As long as the TCP keep-alive option is not -1 (indicating no change),
                if (this.m_options.TcpKeepalive != -1)
                {
                    // Set the TCP keep-alive option values to the underlying socket.
                    this.m_s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, this.m_options.TcpKeepalive);

                    if (this.m_options.TcpKeepaliveIdle != -1 && this.m_options.TcpKeepaliveIntvl != -1)
                    {
                        // Write the TCP keep-alive options to a byte-array, to feed to the IOControl method..
                        ByteArraySegment bytes = new ByteArraySegment(new byte[12]);

                        Endianness endian = BitConverter.IsLittleEndian ? Endianness.Little : Endianness.Big;

                        bytes.PutInteger(endian, this.m_options.TcpKeepalive, 0);
                        bytes.PutInteger(endian, this.m_options.TcpKeepaliveIdle, 4);
                        bytes.PutInteger(endian, this.m_options.TcpKeepaliveIntvl, 8);

                        this.m_s.IOControl(IOControlCode.KeepAliveValues, (byte[]) bytes, null);
                    }
                }

                // Create the engine object for this connection.
                StreamEngine engine = new StreamEngine(this.m_s, this.m_options, this.m_endpoint);

                this.m_socket.EventConnected(this.m_endpoint, this.m_s);

                this.m_s = null;

                // Attach the engine to the corresponding session object.
                this.SendAttach(this.m_session, engine);

                // Shut the connector down.
                this.Terminate();
            }
        }

        /// <summary>
        ///     This is called when the timer expires - to start trying to connect.
        /// </summary>
        /// <param name="id">The timer-id. This is not used.</param>
        public void TimerEvent(int id)
        {
            this.m_timerStarted = false;
            this.StartConnecting();
        }

        /// <summary>
        ///     Internal function to add a reconnect timer
        /// </summary>
        private void AddReconnectTimer()
        {
            int rcIvl = this.GetNewReconnectIvl();
            this.m_ioObject.AddTimer(rcIvl, TcpConnector.ReconnectTimerId);
            this.m_socket.EventConnectRetried(this.m_endpoint, rcIvl);
            this.m_timerStarted = true;
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
        ///     Close the connecting socket.
        /// </summary>
        private void Close()
        {
            Debug.Assert(this.m_s != null);
            try
            {
                this.m_s.Dispose();
                this.m_socket.EventClosed(this.m_endpoint, this.m_s);
                this.m_s = null;
            }
            catch (SocketException ex)
            {
                this.m_socket.EventCloseFailed(this.m_endpoint, ex.SocketErrorCode.ToErrorCode());
            }
        }
    }
}