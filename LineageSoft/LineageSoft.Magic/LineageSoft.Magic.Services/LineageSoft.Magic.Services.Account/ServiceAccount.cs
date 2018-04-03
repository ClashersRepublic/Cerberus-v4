namespace LineageSoft.Magic.Services.Account
{
    using System.Timers;

    using LineageSoft.Magic.Logic.Data;

    using LineageSoft.Magic.Services.Account.Game;
    using LineageSoft.Magic.Services.Account.Handler;
    using LineageSoft.Magic.Services.Account.Network.Message;
    using LineageSoft.Magic.Services.Account.Network.Session;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Core.Database;

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