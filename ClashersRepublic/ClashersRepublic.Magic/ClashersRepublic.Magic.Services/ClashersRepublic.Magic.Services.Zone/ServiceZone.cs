namespace ClashersRepublic.Magic.Services.Zone
{
    using System.Timers;

    using ClashersRepublic.Magic.Logic.Data;
    
    using ClashersRepublic.Magic.Services.Zone.Game;
    using ClashersRepublic.Magic.Services.Zone.Network.Message;
    using ClashersRepublic.Magic.Services.Zone.Network.Session;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Zone.Handler;
    using ClashersRepublic.Magic.Services.Zone.Resource;

    internal static class ServiceZone
    {
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, new NetZoneMessageManager(), args);

            ServiceZone.InitLogic();
            ServiceZone.InitGame();
            ServiceZone.InitNetwork();

            ServiceZone._titleTimer = new Timer(200);
            ServiceZone._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceZone._titleTimer.Start();

            ServiceZone.Start();
        }

        /// <summary>
        ///     Called for start the home service node.
        /// </summary>
        internal static void Start()
        {
            ZoneAccountManager.LoadAccounts();
            ExitHandler.Initialize();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize("magic-homes");
            HomeResourceManager.Initialize();
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetZoneSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        internal static void InitGame()
        {
            ZoneAccountManager.Initialize();
        }
    }
}