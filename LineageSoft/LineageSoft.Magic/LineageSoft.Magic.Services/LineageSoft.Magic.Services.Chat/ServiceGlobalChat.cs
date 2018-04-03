namespace LineageSoft.Magic.Services.Chat
{
    using System.Text.RegularExpressions;
    using System.Timers;

    using LineageSoft.Magic.Logic.Data;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Core.Utils;
    using LineageSoft.Magic.Services.Chat.Game;
    using LineageSoft.Magic.Services.Chat.Handler;
    using LineageSoft.Magic.Services.Chat.Network.Message;
    using LineageSoft.Magic.Services.Chat.Network.Session;

    internal static class ServiceGlobalChat
    {
        private static Timer _titleTimer;
        internal static Regex Regex;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(NetUtils.SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER, new NetGlobalChatMessageManager(), args);

            ServiceGlobalChat.InitLogic();
            ServiceGlobalChat.InitNetwork();

            ServiceGlobalChat._titleTimer = new Timer(200);
            ServiceGlobalChat._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceGlobalChat._titleTimer.Start();

            ServiceGlobalChat.Regex = new Regex(@"\s+", RegexOptions.Compiled);

            ServiceGlobalChat.Start();
        }

        /// <summary>
        ///     Called for start the home service node.
        /// </summary>
        internal static void Start()
        {
            ExitHandler.Initialize();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            RoomManager.Initialize();
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetGlobalChatSessionManager.Initialize();
        }
    }
}