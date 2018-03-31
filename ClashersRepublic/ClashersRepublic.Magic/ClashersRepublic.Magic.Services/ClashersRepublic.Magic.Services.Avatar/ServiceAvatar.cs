namespace ClashersRepublic.Magic.Services.Avatar
{
    using System.Text.RegularExpressions;
    using System.Timers;

    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Avatar.Game;
    using ClashersRepublic.Magic.Services.Avatar.Handler;
    using ClashersRepublic.Magic.Services.Avatar.Network.Message;
    using ClashersRepublic.Magic.Services.Avatar.Network.Session;

    internal static class ServiceAvatar
    {
        private static Timer _titleTimer;
        internal static Regex Regex;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(NetUtils.SERVICE_NODE_TYPE_AVATAR_CONTAINER, new NetAvatarMessageManager(), args);

            ServiceAvatar.InitLogic();
            ServiceAvatar.InitGame();
            ServiceAvatar.InitNetwork();

            ServiceAvatar._titleTimer = new Timer(200);
            ServiceAvatar._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceAvatar._titleTimer.Start();

            ServiceAvatar.Regex = new Regex(@"\s+", RegexOptions.Compiled);

            ServiceAvatar.Start();
        }

        /// <summary>
        ///     Called for start the home service node.
        /// </summary>
        internal static void Start()
        {
            AvatarAccountManager.LoadAccounts();
            ExitHandler.Initialize();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize("magic-avatar");
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
            AvatarAccountManager.Initialize();
        }
    }
}