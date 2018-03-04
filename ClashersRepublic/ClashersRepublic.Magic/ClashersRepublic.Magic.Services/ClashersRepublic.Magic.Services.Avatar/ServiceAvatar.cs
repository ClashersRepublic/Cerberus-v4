namespace ClashersRepublic.Magic.Services.Avatar
{
    using System.Timers;

    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Services.Avatar.Database;
    using ClashersRepublic.Magic.Services.Avatar.Game;
    using ClashersRepublic.Magic.Services.Avatar.Network.Message;
    using ClashersRepublic.Magic.Services.Avatar.Network.Session;
    using ClashersRepublic.Magic.Services.Core;
    
    internal static class ServiceAvatar
    {
        private const int ServiceNodeType = 3;    
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(ServiceAvatar.ServiceNodeType, new NetAvatarMessageManager(), args);

            ServiceAvatar.InitLogic();
            ServiceAvatar.InitGame();
            ServiceAvatar.InitNetwork();

            ServiceAvatar._titleTimer = new Timer(50);
            ServiceAvatar._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceAvatar._titleTimer.Start();

            ServiceAvatar.Start();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Called for start the account service node.
        /// </summary>
        internal static void Start()
        {
            AvatarManager.LoadAvatars();
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
            NetAvatarSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        internal static void InitGame()
        {
            AvatarManager.Initialize();
        }
    }
}