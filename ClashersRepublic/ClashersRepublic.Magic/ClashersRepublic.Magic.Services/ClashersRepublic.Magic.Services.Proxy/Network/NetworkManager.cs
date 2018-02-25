namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Proxy.Network.Udp;

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

        private static long _connectionSeed;

        private static NetworkGateway[] _gateways;
        private static ConcurrentDictionary<long, NetworkToken> _connections;

        /// <summary>
        ///     Gets the <see cref="NetworkUdpGateway"/> instance.
        /// </summary>
        internal static NetworkUdpGateway UdpGateway { get; private set; }

        /// <summary>
        ///     Gets the connection count.
        /// </summary>
        internal static int Connections
        {
            get
            {
                return NetworkManager._connections.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetworkManager._connections = new ConcurrentDictionary<long, NetworkToken>();
            NetworkManager._gateways = new NetworkGateway[NetworkManager.Ports.Length];

            for (int i = 0; i < NetworkManager._gateways.Length; i++)
            {
                NetworkManager._gateways[i] = new NetworkGateway(NetworkManager.Ports[i]);
            }

            NetworkManager.UdpGateway = new NetworkUdpGateway(9339);
        }

        /// <summary>
        ///     Tries to add the specified <see cref="NetworkToken"/> instance.
        /// </summary>
        internal static bool TryAdd(NetworkToken token)
        {
            if (token.ConnectionId == 0)
            {
                long id = Interlocked.Increment(ref NetworkManager._connectionSeed);

                if (NetworkManager._connections.TryAdd(id, token))
                {
                    token.SetConnectionId(id);
                    return true;
                }

                Logging.Warning(typeof(NetworkManager), "NetworkManager::tryAdd connection id already exist");
            }

            Logging.Warning(typeof(NetworkManager), "NetworkManager::tryAdd connection id already defined");

            return false;
        }

        /// <summary>
        ///     Tries to remove the specified <see cref="NetworkToken"/> instance.
        /// </summary>
        internal static bool TryRemove(NetworkToken token)
        {
            return NetworkManager._connections.TryRemove(token.ConnectionId, out _);
        }

        /// <summary>
        ///     Gets all connections.
        /// </summary>
        internal static NetworkToken[] Get()
        {
            var collection = NetworkManager._connections.Values;
            NetworkToken[] instances = new NetworkToken[collection.Count];
            collection.CopyTo(instances, 0);

            return instances;
        }
    }
}