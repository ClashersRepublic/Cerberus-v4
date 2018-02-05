namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using System.Net.Sockets;
    using ClashersRepublic.Magic.Proxy.User;
    using ClashersRepublic.Magic.Services.Logic;

    internal class NetworkToken : IDisposable
    {
        private static readonly int MAX_AVAILABLE_BYTES = Config.BufferSize * 4;

        private byte[] _receivedBytes;
        private int _receivedOffset;

        /// <summary>
        ///     Gets the client instance.
        /// </summary>
        internal Client Client { get; }

        /// <summary>
        ///     Gets or sets the connection id.
        /// </summary>
        internal long ConnectionId { get; set; }

        /// <summary>
        ///     Gets the messaging instance.
        /// </summary>
        internal NetworkMessaging Messaging { get; }

        /// <summary>
        ///     Gets the client socket.
        /// </summary>
        internal Socket Socket { get; }

        /// <summary>
        ///     Gets the receive event.
        /// </summary>
        internal SocketAsyncEventArgs AsyncEvent { get; }

        /// <summary>
        ///     Gets a value indicating whether this instance is disposed.
        /// </summary>
        internal bool Aborted { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkToken" /> class.
        /// </summary>
        internal NetworkToken(Socket socket, SocketAsyncEventArgs readEvent)
        {
            this._receivedBytes = new byte[Config.BufferSize];
            this.Client = new Client(this);
            this.Messaging = new NetworkMessaging(this.Client, this);

            this.Socket = socket;
            this.AsyncEvent = readEvent;
            this.AsyncEvent.UserToken = this;
        }

        /// <summary>
        ///     Adds the specified data to available data.
        /// </summary>
        internal bool AddData()
        {
            int rcvLength = this.AsyncEvent.BytesTransferred;
            int requireCapacity = rcvLength + this._receivedOffset;

            if (requireCapacity > this._receivedBytes.Length)
            {
                if (requireCapacity > NetworkToken.MAX_AVAILABLE_BYTES)
                {
                    return false;
                }

                this.EnsureCapacity(requireCapacity);
            }

            Array.Copy(this.AsyncEvent.Buffer, 0, this._receivedBytes, this._receivedOffset, rcvLength);
            this._receivedOffset += rcvLength;

            return true;
        }

        /// <summary>
        ///     Ensures the capacity of the receive byte array.
        /// </summary>
        internal void EnsureCapacity(int capacity)
        {
            byte[] tmp = this._receivedBytes;
            this._receivedBytes = new byte[capacity + 100];
            Array.Copy(tmp, this._receivedBytes, this._receivedOffset);
        }
        
        /// <summary>
        ///     Handles the received data.
        /// </summary>
        internal bool HandleData()
        {
            int read = this.Messaging.OnReceive(this._receivedBytes, this._receivedOffset);

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
            if (!this.Aborted)
            {
                if (this.IsConnected())
                {
                    NetworkGateway.Send(packet, this);
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this client is connected.
        /// </summary>
        internal bool IsConnected()
        {
            return !this.Aborted && this.Socket.Connected;
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this.Aborted)
            {
                return;
            }

            this.Aborted = true;

            this.Socket.Close();
            this.Client.Dispose();
        }
    }
}