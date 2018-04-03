namespace RivieraStudio.Magic.Services.Party
{
    using System.Timers;

    using RivieraStudio.Magic.Logic.Data;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Services.Party.Game;
    using RivieraStudio.Magic.Services.Party.Handler;
    using RivieraStudio.Magic.Services.Party.Network.Message;
    using RivieraStudio.Magic.Services.Party.Network.Session;

    internal static class ServiceParty
    {
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(NetUtils.SERVICE_NODE_TYPE_PARTY_CONTAINER, new NetPartyMessageManager(), args);

            ServiceParty.InitLogic();
            ServiceParty.InitGame();
            ServiceParty.InitNetwork();

            ServiceParty._titleTimer = new Timer(200);
            ServiceParty._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceParty._titleTimer.Start();

            ServiceParty.Start();
        }

        /// <summary>
        ///     Called for start the home service node.
        /// </summary>
        internal static void Start()
        {
            PartyAccountManager.LoadAccounts();
            ExitHandler.Initialize();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize("magic-party_avatar");
            DatabaseManager.Initialize("magic-party_alliance");
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetPartySessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        internal static void InitGame()
        {
            PartyAccountManager.Initialize();
        }
    }
}