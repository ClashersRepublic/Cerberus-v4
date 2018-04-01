namespace ClashersRepublic.Magic.Services.Net
{
    using System;
    using System.Collections.Generic;

    using System.Net;
    using System.Net.Sockets;

    public class NetServer
    {
        private readonly Socket _socket;
        private readonly List<Client> _clients;

        private NetServerListener _listener;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetServer"/> class.
        /// </summary>
        public NetServer(int port)
        {
            this._clients = new List<Client>();
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._socket.Bind(new IPEndPoint(IPAddress.Any, port));
            this._socket.Listen(150);
        }

        /// <summary>
        ///     Starts the accept.
        /// </summary>
        public void StartAccept()
        {
            SocketAsyncEventArgs acceptEvent = new SocketAsyncEventArgs();

            acceptEvent.Completed += this.OnAcceptCompleted;
            acceptEvent.DisconnectReuseSocket = true;

            this.StartAccept(acceptEvent);
        }

        /// <summary>
        ///     Sets the listener.
        /// </summary>
        public void SetListener(NetServerListener listener)
        {
            this._listener = listener;
        }

        /// <summary>
        ///     Accepts a TCP Request.
        /// </summary>
        /// <param name="acceptEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEvent)
        {
            acceptEvent.AcceptSocket = null;

            if (!this._socket.AcceptAsync(acceptEvent))
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
                Client client = new Client(socket, this._listener);

                readEvent.SetBuffer(new byte[2048], 0, 2048);
                readEvent.Completed += this.OnReceiveCompleted;
                readEvent.UserToken = client;

                this._clients.Add(client);

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
            Client client = (Client) asyncEvent.UserToken;

            if (asyncEvent.BytesTransferred > 0)
            {
                client.Write(asyncEvent.Buffer, asyncEvent.BytesTransferred);

                if (client.Socket.Connected)
                {
                    try
                    {
                        if (!client.Socket.ReceiveAsync(asyncEvent))
                        {
                            this.OnReceiveCompleted(null, asyncEvent);
                        }
                    }
                    catch (Exception)
                    {
                        client.Socket.Close();
                    }
                }
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
                if (asyncEvent.UserToken != null)
                {
                    this._clients.Remove((Client) asyncEvent.UserToken);
                }
            }
        }
        
        private class Client
        {
            internal Socket Socket { get; }

            private readonly NetBuffer _buffer;
            private readonly NetServerListener _listener;

            /// <summary>
            ///     Initializes a new instance of the <see cref="Client"/> class.
            /// </summary>
            internal Client(Socket socket, NetServerListener listener)
            {
                this.Socket = socket;
                this._listener = listener;

                this._buffer = new NetBuffer();
            }

            /// <summary>
            ///     Writes the received bytes.
            /// </summary>
            internal void Write(byte[] buffer, int length)
            {
                this._buffer.Write(buffer, length);
                this.Decode();
            }

            /// <summary>
            ///     Decodes the received packet.
            /// </summary>
            private void Decode()
            {
                if (this._buffer.GetLength() >= 4)
                {
                    byte[] tmp = this._buffer.GetBuffer();
                    int messageLength = tmp[3] << 24 | tmp[2] << 16 | tmp[1] << 8 | tmp[0];

                    if (this._buffer.GetLength() - 4 >= messageLength)
                    {
                        byte[] messageBytes = new byte[messageLength];

                        Array.Copy(tmp, 4, messageBytes, 0, messageLength);

                        this._listener.OnReceive(messageBytes, messageLength);
                        this._buffer.Remove(4 + messageLength);

                        if (this._buffer.GetLength() >= 4)
                        {
                            this.Decode();
                        }
                    }
                }
            }
        }
    }
}