namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    using ClashersRepublic.Magic.Proxy.Logic;

    internal class NetworkToken : IDisposable
    {
        private readonly List<byte> _packet;

        internal bool Aborting;
        internal SocketAsyncEventArgs AsyncEvent;

        internal Client Client;
        internal NetworkMessaging Messaging;
        internal Socket Socket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkToken" /> class.
        /// </summary>
        /// <param name="asyncEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        /// <param name="socket">The socket.</param>
        internal NetworkToken(SocketAsyncEventArgs asyncEvent, Socket socket)
        {
            this.Socket = socket;
            this.AsyncEvent = asyncEvent;
            this.AsyncEvent.UserToken = this;

            this._packet = new List<byte>(Config.BufferSize * 4);
            this.Messaging = new NetworkMessaging(this);
            this.Client = new Client(this);
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (!this.Aborting)
            {
                this.Aborting = true;

                this.Socket.Close();
                this._packet.Clear();
                this.Client.Dispose();
            }
        }

        /// <summary>
        ///     Processes the data.
        /// </summary>
        internal void AddData()
        {
            byte[] tmpData = new byte[this.AsyncEvent.BytesTransferred];
            Buffer.BlockCopy(this.AsyncEvent.Buffer, 0, tmpData, 0, this.AsyncEvent.BytesTransferred);
            this._packet.AddRange(tmpData);
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is connected.
        /// </summary>
        internal bool IsConnected()
        {
            if (!this.Aborting)
            {
                return this.Socket.Connected;
            }

            return false;
        }

        /// <summary>
        ///     Processes the received data.
        /// </summary>
        internal void ProcessData()
        {
            byte[] packet = this._packet.ToArray();

            if (packet.Length >= 7)
            {
                if (packet.Length <= Config.BufferSize * 4)
                {
                    this.Messaging.OnReceiveMessage(packet);
                }
                else
                {
                    NetworkGateway.Disconnect(this.AsyncEvent);
                }
            }
        }

        /// <summary>
        ///     Removes the number of specified bytes.
        /// </summary>
        internal void RemoveData(int count)
        {
            this._packet.RemoveRange(0, count);
        }
    }
}