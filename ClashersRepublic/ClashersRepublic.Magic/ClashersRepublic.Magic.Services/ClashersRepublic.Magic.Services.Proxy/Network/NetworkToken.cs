namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using ClashersRepublic.Magic.Services.Core;

    internal class NetworkToken
    {
        private const int MaxAllocBufferSize = 32768;
        private const int InitAllocBufferSize = 1024;

        private byte[] _receiveBytes;
        private int _receiveBytesLength;

        /// <summary>
        ///     Gets the <see cref="System.Net.Sockets.Socket"/> instance.
        /// </summary>
        internal Socket Socket { get; }

        /// <summary>
        ///     Gets the <see cref="NetworkClient"/> instance.
        /// </summary>
        internal NetworkClient Client { get; private set; }

        /// <summary>
        ///     Gets the <see cref="NetworkMessaging"/> instance.
        /// </summary>
        internal NetworkMessaging Messaging { get; }

        /// <summary>
        ///     Gets the <see cref="SocketAsyncEventArgs"/> read event.
        /// </summary>
        internal SocketAsyncEventArgs ReadEvent { get; }

        /// <summary>
        ///     Gets the connection id.
        /// </summary>
        internal long ConnectionId { get; private set; }

        /// <summary>
        ///     Gets the client ip.
        /// </summary>
        internal string ClientIP { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkToken"/> class.
        /// </summary>
        internal NetworkToken(Socket socket, SocketAsyncEventArgs readEvent)
        {
            this.Socket = socket;
            this.ReadEvent = readEvent;
            this.ClientIP = ((IPEndPoint) socket.RemoteEndPoint).Address.ToString();
            this.Client = new NetworkClient(this);
            this.Messaging = new NetworkMessaging(this);
            this._receiveBytes = new byte[NetworkToken.InitAllocBufferSize];
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            if (this.Client != null)
            {
                this.Client.Destruct();
                this.Client = null;
            }

            this.ClientIP = null;
            this._receiveBytes = null;
            this._receiveBytesLength = 0;
            this.ConnectionId = 0;
        }

        /// <summary>
        ///     Gets if the <see cref="NetworkToken"/> instance is connected.
        /// </summary>
        internal bool IsConnected()
        {
            return this.ConnectionId != 0 && this.Socket.Connected;
        }

        /// <summary>
        ///     Receives the available data on the <see cref="System.Net.Sockets.Socket"/> instance.
        /// </summary>
        internal bool ReceiveData()
        {
            int receivedDataLength = this.ReadEvent.BytesTransferred;
            int requiredDataLength = this._receiveBytesLength + receivedDataLength;

            if (this._receiveBytes.Length <= requiredDataLength)
            {
                if (requiredDataLength <= NetworkToken.MaxAllocBufferSize)
                {
                    byte[] tmp = this._receiveBytes;
                    this._receiveBytes = new byte[requiredDataLength];
                    Array.Copy(tmp, 0, this._receiveBytes, 0, this._receiveBytesLength);
                }
                else
                {
                    return false;
                }
            }

            Array.Copy(this.ReadEvent.Buffer, 0, this._receiveBytes, this._receiveBytesLength, receivedDataLength);
            this._receiveBytesLength += receivedDataLength;

            return true;
        }

        /// <summary>
        ///     Sends the packet to client.
        /// </summary>
        internal void SendData(byte[] packet, int length)
        {
            NetworkGateway.Send(packet, length, this);
        }

        /// <summary>
        ///     Handles the available data.
        /// </summary>
        internal bool HandleData()
        {
            if (this.ConnectionId != 0)
            {
                int read = this.Messaging.Receive(this._receiveBytes, this._receiveBytesLength);

                if (read != -1)
                {
                    if (read != 0)
                    {
                        this.RemoveData(this._receiveBytesLength);

                        if (this._receiveBytesLength != 0)
                        {
                            return this.HandleData();
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Removes the available data.
        /// </summary>
        internal void RemoveData(int length)
        {
            if (this.ConnectionId != 0)
            {
                if (this._receiveBytesLength != length)
                {
                    Array.Copy(this._receiveBytes, length, this._receiveBytes, 0, this._receiveBytesLength - length);
                }

                this._receiveBytesLength -= length;
            }
        }

        /// <summary>
        ///     Sets the connection id.
        /// </summary>
        internal void SetConnectionId(long connectionId)
        {
            if (this.ConnectionId == 0)
            {
                this.ConnectionId = connectionId;
            }
            else
            {
                Logging.Warning(this, "NetworkToken::setConnectionId connectionId already setted");
            }
        }
    }
}