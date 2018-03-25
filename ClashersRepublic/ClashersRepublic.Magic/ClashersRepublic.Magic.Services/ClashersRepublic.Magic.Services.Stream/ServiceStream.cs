namespace ClashersRepublic.Magic.Services.Stream
{
    using System.Timers;

    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Stream.Game;
    using ClashersRepublic.Magic.Services.Stream.Handler;
    using ClashersRepublic.Magic.Services.Stream.Network.Message;
    using ClashersRepublic.Magic.Services.Stream.Network.Session;

    internal static class ServiceStream
    {
        private static Timer _titleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(10, new NetStreamMessageManager(), args);

            ServiceStream.InitLogic();
            ServiceStream.InitGame();
            ServiceStream.InitNetwork();

            ServiceStream._titleTimer = new Timer(200);
            ServiceStream._titleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceStream._titleTimer.Start();

            ServiceStream.Start();
        }

        /// <summary>
        ///     Called for start the home service node.
        /// </summary>
        internal static void Start()
        {
            StreamManager.LoadStreams();
            ExitHandler.Initialize();
            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            DatabaseManager.Initialize("magic-streams");
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetStreamSessionManager.Initialize();
        }

        /// <summary>
        ///     Initializes the game part.
        /// </summary>
        internal static void InitGame()
        {
            StreamManager.Initialize();
        }
    }
}