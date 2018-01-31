namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using System.Net.Sockets;
    using ClashersRepublic.Magic.Proxy.User;
    using ClashersRepublic.Magic.Services.Logic;

    internal class NetworkToken : IDisposable
    {
        private static readonly int MAX_AVAILABLE_BYTES = Config.BufferSize * 4;

        private bool _aborted;

        private Socket _clientSocket;
        private SocketAsyncEventArgs _readEvent;
        private NetworkMessaging _messaging;
        private Client _client;

        private byte[] _receivedBytes;
        private int _receivedOffset;

        /// <summary>
        ///     Gets the messaging instance.
        /// </summary>
        internal NetworkMessaging Messaging
        {
            get
            {
                return this._messaging;
            }
        }

        /// <summary>
        ///     Gets the client socket.
        /// </summary>
        internal Socket Socket
        {
            get
            {
                return this._clientSocket;
            }
        }

        /// <summary>
        ///     Gets the receive event.
        /// </summary>
        internal SocketAsyncEventArgs AsyncEvent
        {
            get
            {
                return this._readEvent;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool Aborted
        {
            get
            {
                return this._aborted;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkToken"/> class.
        /// </summary>
        internal NetworkToken(Socket socket, SocketAsyncEventArgs readEvent)
        {
            this._receivedBytes = new byte[NetworkToken.MAX_AVAILABLE_BYTES];
            this._client = new Client(this);
            this._messaging = new NetworkMessaging(this._client, this);

            this._clientSocket = socket;
            this._readEvent = readEvent;
            this._readEvent.UserToken = this;
        }

        /// <summary>
        ///     Adds the specified data to available data.
        /// </summary>
        internal bool AddData()
        {
            int rcvLength = this._readEvent.BytesTransferred;

            if (this._receivedOffset + rcvLength > NetworkToken.MAX_AVAILABLE_BYTES)
            {
                return false;
            }

            Array.Copy(this._readEvent.Buffer, 0, this._receivedBytes, this._receivedOffset, rcvLength);
            this._receivedOffset += rcvLength;

            return true;
        }

        /// <summary>
        ///     Handles the received data.
        /// </summary>
        internal bool HandleData()
        {
            int read = this._messaging.OnReceive(this._receivedBytes, this._receivedOffset);

            if (read != -1)
            {
                if (read != 0)
                {
                    this.RemoveData(read);
                    this.HandleData();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Removes the read data.
        /// </summary>
        internal void RemoveData(int count)
        {
            this._receivedOffset -= count;

            if (this._receivedOffset != 0)
            {
                Array.Copy(this._receivedBytes, count, this._receivedBytes, 0, this._receivedOffset);
            }
        }

        /// <summary>
        ///     Sends the specified data to client.
        /// </summary>
        internal void WriteData(byte[] packet)
        {
            if (this._aborted)
            {
                return;
            }

            if (!this.IsConnected())
            {
                return;
            }

            NetworkGateway.Send(packet, this);
        }

        /// <summary>
        ///     Gets a value indicating whether this client is connected.
        /// </summary>
        internal bool IsConnected()
        {
            return !this._aborted && this._clientSocket.Connected;
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this._aborted)
            {
                return;
            }

            this._aborted = true;

            this._clientSocket.Close();

            this._readEvent = null;
            this._messaging = null;
            this._receivedBytes = null;
            this._clientSocket = null;
            this._client = null;

            this._receivedOffset = 0;
        }
    }
}