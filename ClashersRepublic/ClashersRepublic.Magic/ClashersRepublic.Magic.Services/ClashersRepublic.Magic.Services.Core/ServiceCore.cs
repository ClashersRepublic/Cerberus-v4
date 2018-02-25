namespace ClashersRepublic.Magic.Services.Core
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Core.Web;

    public static class ServiceCore
    {
        private static bool _initialized;
        
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
        public static void Initialize(int serviceNodeType, INetMessageManager messageManager, string[] args)
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

            ServiceCore.InitConfig();
            ServiceCore.InitLogic();
            ServiceCore.InitNet(messageManager);
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
        }

        /// <summary>
        ///     Initializes the net part.
        /// </summary>
        private static void InitNet(INetMessageManager messageManager)
        {
            WebManager.Initialize();
            NetManager.Initialize();
            NetMessaging.Initialize();
            NetMessaging.SetMessageManager(messageManager);
            NetGateway.Initialize();
        }
    }
}