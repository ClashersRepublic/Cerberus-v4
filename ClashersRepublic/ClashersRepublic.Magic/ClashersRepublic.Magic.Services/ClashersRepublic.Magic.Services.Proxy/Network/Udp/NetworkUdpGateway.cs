namespace ClashersRepublic.Magic.Services.Proxy.Network.Udp
{
    using System.Net;
    using System.Net.Sockets;

    using ClashersRepublic.Magic.Services.Core;

    internal class NetworkUdpGateway
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
        ///     Initializes a new instance of the <see cref="NetworkUdpGateway"/> class.
        /// </summary>
        internal NetworkUdpGateway(int port)
        {
            this.Port = port;

            this.Listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.Listener.Bind(new IPEndPoint(IPAddress.Any, port));

            Logging.Debug(this, "Server listens on UDP port " + port);

            SocketAsyncEventArgs receiveEvent = new SocketAsyncEventArgs();

            receiveEvent.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            receiveEvent.Completed += this.ReceiveCompleted;
            receiveEvent.UserToken = this.Listener;
            receiveEvent.SetBuffer(new byte[NetworkUdpGateway.BufferSize], 0, NetworkUdpGateway.BufferSize);

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
            Logging.Debug(this, "Receives UDP packet from " + receiveEvent.RemoteEndPoint + ".");

            if (!this.Listener.ReceiveFromAsync(receiveEvent))
            {
                this.ReceiveCompleted(this, receiveEvent);
            }
        }

        /// <summary>
        ///     Called when the send event has been completed.
        /// </summary>
        private static void SendCompleted(object sender, SocketAsyncEventArgs sendEvent)
        {
            sendEvent.Dispose();
        }

        /// <summary>
        ///     Sends the buffer to specified endpoint.
        /// </summary>
        internal static void Send(byte[] buffer, int length, EndPoint endPoint)
        {
            SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();

            sendEvent.Completed += NetworkUdpGateway.SendCompleted;
            sendEvent.RemoteEndPoint = endPoint;
            sendEvent.SetBuffer(buffer, 0, length);

            if (!NetworkManager.UdpGateway.Listener.SendToAsync(sendEvent))
            {
                NetworkUdpGateway.SendCompleted(null, sendEvent);
            }
        }
    }
}