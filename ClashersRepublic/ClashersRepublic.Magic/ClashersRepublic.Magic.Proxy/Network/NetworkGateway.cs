namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    internal static class NetworkGateway
    {
        private static Socket _listener;

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="NetworkGateway" /> has been already initialized.
        /// </summary>
        internal static bool Initialized { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkGateway" /> class.
        /// </summary>
        internal static void Initialize()
        {
            if (NetworkGateway.Initialized)
            {
                return;
            }

            NetworkGateway._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            NetworkGateway._listener.NoDelay = true;
            NetworkGateway._listener.Blocking = false;

            NetworkGateway.Initialized = true;

            NetworkGateway._listener.Bind(new IPEndPoint(IPAddress.Any, 9339));
            NetworkGateway._listener.Listen(300);

            Logging.Info(typeof(NetworkGateway), "Listener has been bound to " + NetworkGateway._listener.LocalEndPoint + ".");

            SocketAsyncEventArgs acceptEvent = new SocketAsyncEventArgs();
            acceptEvent.Completed += NetworkGateway.OnAcceptCompleted;
            acceptEvent.DisconnectReuseSocket = true;

            NetworkGateway.StartAccept(acceptEvent);
        }

        /// <summary>
        ///     Accepts a TCP Request.
        /// </summary>
        /// <param name="acceptEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void StartAccept(SocketAsyncEventArgs acceptEvent)
        {
            acceptEvent.AcceptSocket = null;

            if (!NetworkGateway._listener.AcceptAsync(acceptEvent))
            {
                NetworkGateway.OnAcceptCompleted(null, acceptEvent);
            }
        }

        /// <summary>
        ///     Called when the client has been accepted.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void OnAcceptCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                NetworkGateway.ProcessAccept(asyncEvent);
            }
            else
            {
                Logging.Warning(typeof(NetworkGateway), asyncEvent.SocketError + ", SocketError != Success at OnAcceptCompleted(Sender, AsyncEvent).");
            }

            NetworkGateway.StartAccept(asyncEvent);
        }

        /// <summary>
        ///     Accept the new client and store it in memory.
        /// </summary>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private static void ProcessAccept(SocketAsyncEventArgs asyncEvent)
        {
            Logging.Info(typeof(NetworkGateway), "Connection from " + ((IPEndPoint) asyncEvent.AcceptSocket.RemoteEndPoint).Address + ".");

            if (asyncEvent.AcceptSocket.Connected)
            {
                SocketAsyncEventArgs readEvent = new SocketAsyncEventArgs();

                readEvent.SetBuffer(new byte[Config.BufferSize], 0, Config.BufferSize);
                readEvent.Completed += NetworkGateway.OnReceiveCompleted;

                NetworkToken token = new NetworkToken(readEvent, asyncEvent.AcceptSocket);

                if (!token.Socket.ReceiveAsync(readEvent))
                {
                    NetworkGateway.OnReceiveCompleted(null, readEvent);
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
                    token.AddData();

                    try
                    {
                        if (token.Socket.Available == 0)
                        {
                            token.ProcessData();
                        }

                        if (!token.Aborting)
                        {
                            if (!token.Socket.ReceiveAsync(asyncEvent))
                            {
                                NetworkGateway.ProcessReceive(asyncEvent);
                            }
                        }
                    }
                    catch (Exception)
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
        /// <param name="Sender">The sender.</param>
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

                writeEvent.DisconnectReuseSocket = false;
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
            if (asyncEvent.SocketError != SocketError.Success)
            {
                NetworkGateway.Disconnect(asyncEvent);
            }

            asyncEvent.Dispose();
        }

        /// <summary>
        ///     Closes the specified client's socket.
        /// </summary>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        internal static void Disconnect(SocketAsyncEventArgs asyncEvent)
        {
            NetworkToken token = (NetworkToken) asyncEvent.UserToken;

            if (token != null)
            {
                if (!token.Aborting)
                {
                    token.Dispose();
                }
            }

            asyncEvent.Dispose();
        }
    }
}