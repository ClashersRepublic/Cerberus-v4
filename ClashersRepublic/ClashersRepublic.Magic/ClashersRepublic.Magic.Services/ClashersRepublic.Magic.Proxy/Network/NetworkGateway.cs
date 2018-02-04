namespace ClashersRepublic.Magic.Proxy.Network
{
    using System.Net;
    using System.Net.Sockets;
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Services.Logic;

    internal class NetworkGateway
    {
        private readonly Socket _listener;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkGateway" />
        /// </summary>
        internal NetworkGateway(int port)
        {
            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(new IPEndPoint(IPAddress.Any, port));
            this._listener.Listen(500);

            Logging.Debug(this, "Server has been bound to " + this._listener.LocalEndPoint + ".");

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

            if (!this._listener.AcceptAsync(acceptEvent))
            {
                this.OnAcceptCompleted(null, acceptEvent);
            }
        }

        /// <summary>
        ///     Called when the client has been accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                NetworkGateway.ProcessAccept(asyncEvent);
            }
            else
            {
                Logging.Warning(typeof(NetworkGateway), asyncEvent.SocketError + ", SocketError != Success at OnAcceptCompleted(Sender, AsyncEvent).");
            }

            this.StartAccept(asyncEvent);
        }

        /// <summary>
        ///     Accept the new client and store it in memory.
        /// </summary>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void ProcessAccept(SocketAsyncEventArgs asyncEvent)
        {
            Logging.Debug(typeof(NetworkGateway), "Connection from " + ((IPEndPoint) asyncEvent.AcceptSocket.RemoteEndPoint).Address + ".");

            if (asyncEvent.AcceptSocket.Connected)
            {
                Socket socket = asyncEvent.AcceptSocket;

                SocketAsyncEventArgs readEvent = new SocketAsyncEventArgs();
                readEvent.SetBuffer(new byte[Config.BufferSize], 0, Config.BufferSize);
                readEvent.Completed += NetworkGateway.OnReceiveCompleted;

                NetworkToken token = new NetworkToken(socket, readEvent);

                if (NetworkManager.AddConnection(token))
                {
                    if (!socket.ReceiveAsync(readEvent))
                    {
                        NetworkGateway.ProcessReceive(readEvent);
                    }
                }
            }
        }

        /// <summary>
        ///     Receives data from the specified client.
        /// </summary>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void ProcessReceive(SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.BytesTransferred > 0)
            {
                NetworkToken token = (NetworkToken) asyncEvent.UserToken;

                if (token.IsConnected())
                {
                    if (token.AddData())
                    {
                        if (token.Socket.Available == 0)
                        {
                            if (!token.HandleData())
                            {
                                NetworkGateway.Disconnect(asyncEvent);
                            }
                        }

                        if (!token.Aborted)
                        {
                            if (!token.Socket.ReceiveAsync(asyncEvent))
                            {
                                NetworkGateway.ProcessReceive(asyncEvent);
                            }
                        }
                    }
                    else
                    {
                        NetworkGateway.Disconnect(asyncEvent);
                    }
                }
            }
            else
            {
                NetworkGateway.Disconnect(asyncEvent);
            }
        }

        /// <summary>
        ///     Called when [receive completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void OnReceiveCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                NetworkGateway.ProcessReceive(asyncEvent);
            }
            else
            {
                NetworkGateway.Disconnect(asyncEvent);
            }
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        internal static void Send(byte[] packet, NetworkToken token)
        {
            if (token.IsConnected())
            {
                SocketAsyncEventArgs writeEvent = new SocketAsyncEventArgs();

                writeEvent.Completed += NetworkGateway.OnSendCompleted;
                writeEvent.UserToken = token;
                writeEvent.SetBuffer(packet, 0, packet.Length);

                if (!token.Socket.SendAsync(writeEvent))
                {
                    NetworkGateway.OnSendCompleted(null, writeEvent);
                }
            }
            else
            {
                NetworkGateway.Disconnect(token.AsyncEvent);
            }
        }

        /// <summary>
        ///     Called when [send completed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void OnSendCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            asyncEvent.Dispose();
        }

        /// <summary>
        ///     Disconnects the specified socket.
        /// </summary>
        internal static void Disconnect(SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.UserToken != null)
            {
                NetworkToken token = (NetworkToken) asyncEvent.UserToken;

                if (token.ConnectionId != 0)
                {
                    if (NetworkManager.RemoveConnection(token))
                    {
                        if (!token.Aborted)
                        {
                            token.Dispose();
                        }

                        asyncEvent.Dispose();
                    }
                }
            }
        }
    }
}