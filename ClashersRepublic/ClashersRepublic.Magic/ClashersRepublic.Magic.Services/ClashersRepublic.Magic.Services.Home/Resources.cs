namespace ClashersRepublic.Magic.Services.Home
{
    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Home.Player;
    using ClashersRepublic.Magic.Services.Home.Service;
    using ClashersRepublic.Magic.Services.Home.Session;
    using ClashersRepublic.Magic.Services.Logic.Resource;

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
            Resources.InitializeLogics();
            Resources.InitializeGames();
            Resources.InitializeServices();
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
            GameSessionManager.Initialize();
            GamePlayerManager.Initialize();
        }

        /// <summary>
        ///     Initializes the service part.
        /// </summary>
        private static void InitializeServices()
        {
            ServiceProcessor.Initialize();
            ServiceMessaging.Initialize();
            ServiceGateway.Initialize();
        }
    }
}