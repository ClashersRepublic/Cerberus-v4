namespace ClashersRepublic.Magic.Proxy
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Database;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.Service.Api;
    using ClashersRepublic.Magic.Proxy.Session;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Services.Logic.Service;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class Resources
    {
        internal static LogicMersenneTwisterRandom Random;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ResourceManager.Initialize();

            Resources.InitializeConfigs();
            Resources.InitializeLogics();
            Resources.InitializeGames();
            Resources.InitializeServices();
            Resources.InitializeNetworks();
        }

        /// <summary>
        ///     Initializes the config part.
        /// </summary>
        private static void InitializeConfigs()
        {
            Config.Initialize();
            ServiceConfig.Initialize();
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

            GameDatabaseManager.Initialize();
            GameAccountManager.Initialize();
            GameSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the service part.
        /// </summary>
        private static void InitializeServices()
        {
            GoogleServiceManager.Initialize();
            ServiceProcessor.Initialize();
            ServiceManager.Initialize();
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        private static void InitializeNetworks()
        {
            NetworkManager.Initialize();
        }
    }
}