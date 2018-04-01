namespace ClashersRepublic.Magic.Services.Net.ServerSocket
{
    using System.Net;
    using System.Net.Sockets;

    internal class NetUdpServerSocket
    {
        private const int BufferSize = 2048;

        /// <summary>
        ///     Gets the udp socket port.
        /// </summary>
        internal int Port { get; }

        /// <summary>
        ///     Gets the socket listener instance.
        /// </summary>
        internal Socket Listener { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetUdpServerSocket"/> class.
        /// </summary>
        internal NetUdpServerSocket(int port)
        {
            this.Port = port;

            this.Listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Listener.Bind(new IPEndPoint(IPAddress.Any, port));
            
            SocketAsyncEventArgs receiveEvent = new SocketAsyncEventArgs();

            receiveEvent.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            receiveEvent.Completed += this.ReceiveCompleted;
            receiveEvent.UserToken = this.Listener;
            receiveEvent.SetBuffer(new byte[NetUdpServerSocket.BufferSize], 0, NetUdpServerSocket.BufferSize);

            if (!this.Listener.ReceiveFromAsync(receiveEvent))
            {
                this.ReceiveCompleted(this, receiveEvent);
            }
        }

        /// <summary>
        ///     Called when the receive event has been completed.
        /// </summary>
        private void ReceiveCompleted(object sender, SocketAsyncEventArgs receiveEvent)
        {
            if (!this.Listener.ReceiveFromAsync(receiveEvent))
            {
                this.ReceiveCompleted(this, receiveEvent);
            }
        }

        /// <summary>
        ///     Called when the send event has been completed.
        /// </summary>
        private void SendCompleted(object sender, SocketAsyncEventArgs sendEvent)
        {
            sendEvent.Dispose();
        }

        /// <summary>
        ///     Sends the buffer to specified endpoint.
        /// </summary>
        internal void Send(byte[] buffer, int length, EndPoint endPoint)
        {
            SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();

            sendEvent.Completed += this.SendCompleted;
            sendEvent.RemoteEndPoint = endPoint;
            sendEvent.SetBuffer(buffer, 0, length);

            if (!this.Listener.SendToAsync(sendEvent))
            {
                this.SendCompleted(null, sendEvent);
            }
        }
    }
}