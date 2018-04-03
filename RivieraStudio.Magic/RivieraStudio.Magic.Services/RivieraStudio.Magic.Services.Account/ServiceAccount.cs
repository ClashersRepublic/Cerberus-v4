namespace RivieraStudio.Magic.Services.Account
{
    using System.Timers;

    using RivieraStudio.Magic.Logic.Data;

    using RivieraStudio.Magic.Services.Account.Game;
    using RivieraStudio.Magic.Services.Account.Handler;
    using RivieraStudio.Magic.Services.Account.Network.Message;
    using RivieraStudio.Magic.Services.Account.Network.Session;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;

    internal static class ServiceAccount
    { 
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(2, new NetAccountMessageManager(), args);

            ServiceAccount.InitLogic();
            ServiceAccount.InitGame();
            ServiceAccount.InitNetwork();

            ServiceAccount._titleTimer = new Timer(200);
            ServiceAccount._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceAccount._titleTimer.Start();

            ServiceAccount.Start();
        }

        /// <summary>
        ///     Called for start the account service node.
        /// </summary>
        internal static void Start()
        {
            AccountManager.LoadAccounts();
            ExitHandler.Initialize();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize("magic-account");
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetAccountSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        internal static void InitGame()
        {
            AccountManager.Initialize();
        }
    }
}