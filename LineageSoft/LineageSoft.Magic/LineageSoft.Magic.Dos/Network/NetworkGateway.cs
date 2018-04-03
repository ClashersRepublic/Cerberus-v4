namespace LineageSoft.Magic.Dos.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using LineageSoft.Magic.Dos.Bot;
    using LineageSoft.Magic.Dos.Debug;

    internal class NetworkGateway
    {
        private Socket _socket;
        private Client _client;

        private byte[] _receivedBytes;
        private int _receivedOffset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkGateway"/> class.
        /// </summary>
        internal NetworkGateway(Client client)
        {
            this._client = client;
            this._receivedBytes = new byte[8192];
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        /// <summary>
        ///     Gets a value indicating whether the socket is connected.
        /// </summary>
        internal bool IsConnected()
        {
            return this._socket.Connected;
        }

        /// <summary>
        ///     Connects the client to specified host.
        /// </summary>
        internal void Connect(string host, int port)
        {
            if (!this._socket.Connected)
            {
                try
                {
                    this._socket.Connect(host, port);

                    SocketAsyncEventArgs receivedEvent = new SocketAsyncEventArgs();

                    receivedEvent.Completed += this.OnReceiveCompleted;
                    receivedEvent.SetBuffer(new byte[8192], 0, 8192);

                    if (!this._socket.ReceiveAsync(receivedEvent))
                    {
                        this.OnReceiveCompleted(this, receivedEvent);
                    }

                    this._client.OnConnect();
                }
                catch
                {
                    this._client.OnConnectFailed();
                }
            }
        }

        /// <summary>
        ///     Connects the client to specified host.
        /// </summary>
        internal void ConnectAsync(string host, int port)
        {
            if (!this._socket.Connected)
            {
                SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();
                connectEvent.Completed += this.OnConnectCompleted;
                connectEvent.RemoteEndPoint = new DnsEndPoint(host, port);

                if (!this._socket.ConnectAsync(connectEvent))
                {
                    this.OnConnectCompleted(this, connectEvent);
                }
            }
        }

        /// <summary>
        ///     Called when the connect event has been completed.
        /// </summary>
        private void OnConnectCompleted(object sender, SocketAsyncEventArgs connectEvent)
        {
            if (connectEvent.SocketError == SocketError.Success)
            {
                SocketAsyncEventArgs receivedEvent = new SocketAsyncEventArgs();

                receivedEvent.Completed += this.OnReceiveCompleted;
                receivedEvent.SetBuffer(new byte[8192], 0, 8192);

                if (!this._socket.ReceiveAsync(receivedEvent))
                {
                    this.OnReceiveCompleted(this, receivedEvent);
                }

                this._client.OnConnect();
            }
            else
            {
                this._client.OnConnectFailed();
            }
        }

        /// <summary>
        ///     Called when the receive event has been completed.
        /// </summary>
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs receiveEvent)
        {
            if (receiveEvent.SocketError == SocketError.Success)
            {
                if (receiveEvent.BytesTransferred != 0)
                {
                    this.AddData(receiveEvent);

                    try
                    {
                        if (this._socket.Available == 0)
                        {
                            this.ProcessReceive();
                        }

                        if (this._socket.Connected)
                        {
                            if (!this._socket.ReceiveAsync(receiveEvent))
                            {
                                this.OnReceiveCompleted(this, receiveEvent);
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Logging.Error(this, "NetworkGateway::onReceiveCompleted array handle exception, trace: " + exception);
                    }
                }
            }
            else
            {
                this._client.OnDisconnect();
            }
        }

        /// <summary>
        ///     Process the received data.
        /// </summary>
        internal void ProcessReceive()
        {
            int read = this._client.OnReceive(this._receivedBytes, this._receivedOffset);

            if (read != -1)
            {
                if (read != 0)
                {
                    this.RemoveBlocking(read);
                    this.ProcessReceive();
                }
            }
        }

        /// <summary>
        ///     Sends the packet to server.
        /// </summary>
        internal void Send(byte[] packet, int length)
        {
            if (this._socket.Connected)
            {
                SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();

                sendEvent.SetBuffer(packet, 0, length);
                sendEvent.Completed += this.OnSendCompleted;

                if (!this._socket.SendAsync(sendEvent))
                {
                    this.OnSendCompleted(this, sendEvent);
                }
            }
        }

        /// <summary>
        ///     Called when the send event has been completed.
        /// </summary>
        private void OnSendCompleted(object sender, SocketAsyncEventArgs sendEvent)
        {
            sendEvent.Dispose();
        }

        /// <summary>
        ///     Adds the specified data to available data.
        /// </summary>
        internal bool AddData(SocketAsyncEventArgs receiveEvent)
        {
            int rcvLength = receiveEvent.BytesTransferred;

            if (this._receivedOffset + rcvLength > this._receivedBytes.Length)
            {
                byte[] tmp = this._receivedBytes;
                this._receivedBytes = new byte[this._receivedOffset + rcvLength + 100];
                Array.Copy(tmp, this._receivedBytes, this._receivedOffset);
            }

            Array.Copy(receiveEvent.Buffer, 0, this._receivedBytes, this._receivedOffset, rcvLength);
            this._receivedOffset += rcvLength;

            return true;
        }

        /// <summary>
        ///     Removes a block of bytes.
        /// </summary>
        internal void RemoveBlocking(int blockSize)
        {
            this._receivedOffset -= blockSize;
            Array.Copy(this._receivedBytes, blockSize, this._receivedBytes, 0, this._receivedOffset);
        }

        /// <summary>
        ///     Disconnects the connection.
        /// </summary>
        internal void Disconnect()
        {
            this._socket.Close();
        }
    }
}