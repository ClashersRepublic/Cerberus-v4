namespace RivieraStudio.Magic.Services.Chat
{
    using System.Text.RegularExpressions;
    using System.Timers;

    using RivieraStudio.Magic.Logic.Data;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Services.Chat.Game;
    using RivieraStudio.Magic.Services.Chat.Handler;
    using RivieraStudio.Magic.Services.Chat.Network.Message;
    using RivieraStudio.Magic.Services.Chat.Network.Session;

    internal static class ServiceChat
    {
        private static Timer _titleTimer;
        internal static Regex Regex;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(NetUtils.SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER, new NetChatMessageManager(), args);

            ServiceChat.InitLogic();
            ServiceChat.InitNetwork();

            ServiceChat._titleTimer = new Timer(200);
            ServiceChat._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceChat._titleTimer.Start();

            ServiceChat.Regex = new Regex(@"\s+", RegexOptions.Compiled);

            ServiceChat.Start();
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