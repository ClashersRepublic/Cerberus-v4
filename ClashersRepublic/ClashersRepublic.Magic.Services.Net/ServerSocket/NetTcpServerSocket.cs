namespace ClashersRepublic.Magic.Services.Net.ServerSocket
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    public class NetTcpServerGateway
    {
        /// <summary>
        ///     Gets the socket listener instance.
        /// </summary>
        public Socket ServerSocket { get; }

        /// <summary>
        ///     Gets the server listener.
        /// </summary>
        public NetListener Listener { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetTcpServerGateway"/> class.
        /// </summary>
        public NetTcpServerGateway(NetListener listener, int port)
        {
            this.Listener = listener;

            this.ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.ServerSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            this.ServerSocket.Listen(50);
        }

        /// <summary>
        ///     Accepts a TCP Request.
        /// </summary>
        public void StartAccept()
        {
            SocketAsyncEventArgs acceptEvent = new SocketAsyncEventArgs();

            acceptEvent.Completed += this.OnAcceptCompleted;
            acceptEvent.DisconnectReuseSocket = true;

            this.StartAccept(acceptEvent);
        }

        /// <summary>
        ///     Accepts a TCP Request.
        /// </summary>
        /// <param name="acceptEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEvent)
        {
            acceptEvent.AcceptSocket = null;

            if (!this.ServerSocket.AcceptAsync(acceptEvent))
            {
                this.OnAcceptCompleted(null, acceptEvent);
            }
        }

        /// <summary>
        ///     Called when the a is accepted.
        /// </summary>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                this.ProcessAccept(asyncEvent);
            }

            this.StartAccept(asyncEvent);
        }

        /// <summary>
        ///     Accept the new client and store it in memory.
        /// </summary>
        private void ProcessAccept(SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.AcceptSocket.Connected)
            {
                Socket socket = asyncEvent.AcceptSocket;
                SocketAsyncEventArgs readEvent = new SocketAsyncEventArgs();

                readEvent.SetBuffer(new byte[2048], 0, 2048);
                readEvent.Completed += this.OnReceiveCompleted;
                readEvent.UserToken = new NetConnection(socket, this.Listener, readEvent);

                if (!socket.ReceiveAsync(readEvent))
                {
                    this.ProcessReceive(readEvent);
                }
            }
        }

        /// <summary>
        ///     Processes the received data.
        /// </summary>
        private void ProcessReceive(SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.BytesTransferred > 0)
            {
                NetConnection connection = (NetConnection) asyncEvent.UserToken;

                if (!connection.IsDestructed())
                {
                    if (connection.TryReceive())
                    {
                        if (connection.Socket.Available == 0)
                        {
                            try
                            {
                                if (connection.Process())
                                {
                                    if (!connection.Socket.ReceiveAsync(asyncEvent))
                                    {
                                        this.ProcessReceive(asyncEvent);
                                    }
                                }
                                else
                                {
                                    NetTcpServerGateway.Disconnect(asyncEvent);
                                }
                            }
                            catch (Exception)
                            {
                                NetTcpServerGateway.Disconnect(asyncEvent);
                            }
                        }
                        else
                        {
                            if (!connection.Socket.ReceiveAsync(asyncEvent))
                            {
                                this.ProcessReceive(asyncEvent);
                            }
                        }
                    }
                    else
                    {
                        NetTcpServerGateway.Disconnect(asyncEvent);
                    }
                }
            }
            else
            {
                NetTcpServerGateway.Disconnect(asyncEvent);
            }
        }

        /// <summary>
        ///     Called when a receive event is completed.
        /// </summary>
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                this.ProcessReceive(asyncEvent);
            }
            else
            {
                NetTcpServerGateway.Disconnect(asyncEvent);
            }
        }
        
        /// <summary>
        ///     Disconnects the specified socket.
        /// </summary>
        private static void Disconnect(SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.UserToken != null)
            {
                ((NetConnection) asyncEvent.UserToken).Destruct();
                asyncEvent.Dispose();
            }
        }
    }
}