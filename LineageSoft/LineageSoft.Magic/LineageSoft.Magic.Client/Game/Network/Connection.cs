namespace ClashersRepublic.Magic.Client.Game.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    internal class Connection
    {
        private Socket _socket;
        private Messaging _listener;

        private readonly List<byte> _blocks;

        /// <summary>
        ///     Gets the connection buffer.
        /// </summary>
        public byte[] Buffer
        {
            get
            {
                return this._blocks.ToArray();
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Connection" /> class.
        /// </summary>
        internal Connection()
        {
            this._blocks = new List<byte>(8192);
        }

        /// <summary>
        ///     Starts the connection.
        /// </summary>
        internal void Start()
        {
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._listener.OnStart(this);
        }

        /// <summary>
        ///     Closes the connection.
        /// </summary>
        internal void Close()
        {
            if (this._socket != null)
            {
                this._socket.Close();
                this._socket = null;
            }
        }

        /// <summary>
        ///     Connects the instance to specified host.
        /// </summary>
        internal void Connect(string host, int port)
        {
            SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();

            connectEvent.RemoteEndPoint = new DnsEndPoint(host, port);
            connectEvent.Completed += this.OnConnectCompleted;

            if (!this._socket.ConnectAsync(connectEvent))
            {
                this.OnConnectCompleted(this, connectEvent);
            }
        }

        /// <summary>
        ///     Sets the connection listener.
        /// </summary>
        internal void SetListener(Messaging listener)
        {
            this._listener = listener;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is connected.
        /// </summary>
        internal bool IsConnected()
        {
            return this._socket != null && this._socket.Connected;
        }

        /// <summary>
        ///     Called when the connect event has been completed.
        /// </summary>
        private void OnConnectCompleted(object sender, SocketAsyncEventArgs connectEvent)
        {
            if (connectEvent.SocketError == SocketError.Success)
            {
                SocketAsyncEventArgs receiveEvent = new SocketAsyncEventArgs();
                receiveEvent.Completed += this.OnReceiveCompleted;
                receiveEvent.SetBuffer(new byte[4096], 0, 4096);

                this._listener.OnConnect(this);

                if (!this._socket.ReceiveAsync(receiveEvent))
                {
                    this.OnReceiveCompleted(this, receiveEvent);
                }
            }
            else
            {
                this._listener.OnConnectionFailed(this);
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
                    byte[] rcv = new byte[receiveEvent.BytesTransferred];
                    Array.Copy(receiveEvent.Buffer, 0, rcv, 0, rcv.Length);
                    this._blocks.AddRange(rcv);

                    if (this._socket.Available == 0)
                    {
                        this._listener.OnReceive(this);
                    }

                    if (this._socket.Connected)
                    {
                        if (!this._socket.ReceiveAsync(receiveEvent))
                        {
                            this.OnConnectCompleted(null, receiveEvent);
                        }
                    }
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
        ///     Writes a block of bytes.
        /// </summary>
        internal void WriteBlocking(byte[] buffer, int length)
        {
            SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();

            sendEvent.SetBuffer(buffer, 0, length);
            sendEvent.Completed += this.OnSendCompleted;

            if (!this._socket.SendAsync(sendEvent))
            {
                this.OnSendCompleted(null, sendEvent);
            }
        }

        /// <summary>
        ///     Removes the number of specified bytes.
        /// </summary>
        internal void RemoveBlocking(int count)
        {
            this._blocks.RemoveRange(0, count);
        }
    }
}