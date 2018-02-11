namespace ClashersRepublic.Magic.Proxy
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Database;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.Service.Api;
    using ClashersRepublic.Magic.Proxy.Session;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class Resources
    {
        private static readonly int[] GATEWAY_PORTS =
        {
            9339,
            1863,
            3724,
            30000,
            843
        };
        
        internal static NetworkGateway[] Gateways;
        internal static LogicMersenneTwisterRandom Random;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ResourceManager.Initialize();
            Resources.InitializeLogics();
            Resources.InitializeGames();
            Resources.InitializeServices();
            Resources.InitializeNetworks();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        private static void InitializeLogics()
        {
            LogicDataTables.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        private static void InitializeGames()
        {
            Resources.Random = new LogicMersenneTwisterRandom(LogicTimeUtil.GetTimestamp());

            GameDatabase.Initialize();
            GameAccountManager.Initialize();
            GameSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the service part.
        /// </summary>
        private static void InitializeServices()
        {
            ServiceProcessor.Initialize();
            ServiceMessaging.Initialize();
            ServiceGateway.Initialize();

            GoogleServiceManager.Initialize();
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        private static void InitializeNetworks()
        {
            NetworkManager.Initialize();

            Resources.Gateways = new NetworkGateway[Resources.GATEWAY_PORTS.Length];

            for (int i = 0; i < Resources.GATEWAY_PORTS.Length; i++)
            {
                Resources.Gateways[i] = new NetworkGateway(Resources.GATEWAY_PORTS[i]);
            }
        }
    }
}