namespace ClashersRepublic.Magic.Services.Account
{
    using System.Timers;

    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Services.Account.Database;
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Account.Network.Message;
    using ClashersRepublic.Magic.Services.Account.Network.Session;
    using ClashersRepublic.Magic.Services.Core;
    
    internal static class ServiceAccount
    {
        private const int ServiceNodeType = 2;    
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(ServiceAccount.ServiceNodeType, new NetAccountMessageManager(), args);

            ServiceAccount.InitLogic();
            ServiceAccount.InitGame();
            ServiceAccount.InitNetwork();

            ServiceAccount._titleTimer = new Timer(50);
            ServiceAccount._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceAccount._titleTimer.Start();

            ServiceAccount.Start();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Called for start the account service node.
        /// </summary>
        internal static void Start()
        {
            AccountManager.LoadAccounts();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize();
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