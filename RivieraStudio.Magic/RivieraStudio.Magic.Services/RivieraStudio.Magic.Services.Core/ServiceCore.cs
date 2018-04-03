namespace RivieraStudio.Magic.Services.Core
{
    using System.IO;

    using RivieraStudio.Magic.Services.Core.Message;
    using RivieraStudio.Magic.Services.Core.Network;
    using RivieraStudio.Magic.Services.Core.Web;

    using RivieraStudio.Magic.Titan.Json;

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
        ///     Gets the service environment.
        /// </summary>
        public static string ServiceEnvironment { get; private set; }
        
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
                ServiceCore.ServiceEnvironment = args[0];

                if (!string.IsNullOrEmpty(ServiceCore.ServiceEnvironment))
                {
                    ServiceCore.ServiceEnvironment = "internal";
                }

                if (args.Length > 1)
                {
                    ServiceCore.ServiceNodeId = int.Parse(args[1]);
                }
            }
            else
            {
                ServiceCore.ServiceEnvironment = "internal";
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
            
            NetManager.Start();
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