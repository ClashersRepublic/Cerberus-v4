namespace RivieraStudio.Magic.Services.Zone
{
    using System.Timers;

    using RivieraStudio.Magic.Logic.Data;
    
    using RivieraStudio.Magic.Services.Zone.Game;
    using RivieraStudio.Magic.Services.Zone.Network.Message;
    using RivieraStudio.Magic.Services.Zone.Network.Session;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Services.Zone.Handler;
    using RivieraStudio.Magic.Services.Zone.Resource;

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
            DatabaseManager.Initialize("magic-zone");
            DatabaseManagerNew.Initialize(1, 1);
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