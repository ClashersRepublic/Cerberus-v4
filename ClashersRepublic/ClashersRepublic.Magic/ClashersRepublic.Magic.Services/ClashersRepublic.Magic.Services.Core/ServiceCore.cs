namespace ClashersRepublic.Magic.Services.Core
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Core.Web;

    public static class ServiceCore
    {
        private static bool _initialized;
        private static bool _started;

        /// <summary>
        ///     Gets the service node type.
        /// </summary>
        public static int ServiceNodeType { get; private set; }

        /// <summary>
        ///     Gets the service node id.
        /// </summary>
        public static int ServiceNodeId { get; private set; }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize(int serviceNodeType, NetMessageManager messageManager, string[] args)
        {
            if (ServiceCore._initialized)
            {
                return;
            }

            ServiceCore._initialized = true;
            ServiceCore.ServiceNodeType = serviceNodeType;

            if (args.Length > 0)
            {
                ServiceCore.ServiceNodeId = int.Parse(args[0]);
            }

            WebManager.Initialize();

            ServiceCore.InitConfig();
            ServiceCore.InitLogic();
            ServiceCore.InitNet(messageManager);
        }

        /// <summary>
        ///     Starts the gateway.
        /// </summary>
        public static void Start()
        {
            if (ServiceCore._started)
            {
                return;
            }

            NetGateway.Initialize();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        private static void InitLogic()
        {
            ResourceManager.Initialize();
        }

        /// <summary>
        ///     Initializes the config part.
        /// </summary>
        private static void InitConfig()
        {
            Logging.Initialize();
            ServiceSettings.Initialize();
        }

        /// <summary>
        ///     Initializes the net part.
        /// </summary>
        private static void InitNet(NetMessageManager messageManager)
        {
            NetManager.Initialize();
            NetMessaging.Initialize();
            NetMessaging.SetMessageManager(messageManager);
        }
    }
}