namespace ClashersRepublic.Magic.Services.Home
{
    using System.Timers;

    using ClashersRepublic.Magic.Logic.Data;
    
    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Home.Game;
    using ClashersRepublic.Magic.Services.Home.Network.Message;
    using ClashersRepublic.Magic.Services.Home.Network.Session;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Home.Resource;

    internal static class ServiceHome
    {
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(10, new NetHomeMessageManager(), args);

            ServiceHome.InitLogic();
            ServiceHome.InitGame();
            ServiceHome.InitNetwork();

            ServiceHome._titleTimer = new Timer(150);
            ServiceHome._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceHome._titleTimer.Start();

            ServiceHome.Start();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Called for start the home service node.
        /// </summary>
        internal static void Start()
        {
            HomeManager.LoadHomes();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize();
            HomeResourceManager.Initialize();
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetHomeSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        internal static void InitGame()
        {
            HomeManager.Initialize();
        }
    }
}