namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    using ClashersRepublic.Magic.Services.Proxy.Network.ServerSocket;

    internal static class NetworkManager
    {
        private static readonly int[] Ports =
        {
            9339,
            1863,
            3724,
            30000,
            843
        };
        
        private static NetworkTcpServerGateway[] _tcpServerGateways;

        /// <summary>
        ///     Gets the <see cref="NetworkUdpServerSocket"/> instance.
        /// </summary>
        internal static NetworkUdpServerSocket UdpGateway { get; private set; }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetworkManager._tcpServerGateways = new NetworkTcpServerGateway[NetworkManager.Ports.Length];

            for (int i = 0; i < NetworkManager._tcpServerGateways.Length; i++)
            {
                NetworkManager._tcpServerGateways[i] = new NetworkTcpServerGateway(NetworkManager.Ports[i]);
            }

            NetworkManager.UdpGateway = new NetworkUdpServerSocket(9339);
        }
    }
}