namespace LineageSoft.Magic.Services.Proxy.Network.ServerSocket
{
    using System.Net;
    using System.Net.Sockets;

    internal class NetworkServerSocket
    {
        protected readonly Socket _listener;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkServerSocket"/> class.
        /// </summary>
        internal NetworkServerSocket(int port)
        {
            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(new IPEndPoint(IPAddress.Any, port));
            this._listener.Listen(500);
        }
    }
}