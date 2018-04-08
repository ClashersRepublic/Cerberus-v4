namespace RivieraStudio.Magic.Services.Core
{
    using RivieraStudio.Magic.Services.Core.Message;
    using RivieraStudio.Magic.Services.Core.Network;
    using RivieraStudio.Magic.Services.Core.Web;
    
    public static class ServiceCore
    {
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
        ///     Gets the configuration server.
        /// </summary>
        public static string ConfigurationServer { get; private set; }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize(int serviceNodeType, NetMessageManager messageManager, string[] args)
        {
            ServiceCore.ServiceNodeType = serviceNodeType;
            ServiceCore.ServiceEnvironment = "internal";
            ServiceCore.ConfigurationServer = "http://127.0.0.1/";
            ServiceCore.ServiceNodeId = 0;

            if (args.Length > 0)
            {
                if (args.Length % 2 == 0)
                {
                    for (int i = 0; i < args.Length; i += 2)
                    {
                        string name = args[i];
                        string value = args[i + 1];
                        
                        switch (name)
                        {
                            case "-env":
                                if (!value.Equals("integration") &&
                                    !value.StartsWith("integration-") &&
                                    !value.Equals("stage") &&
                                    !value.Equals("prod") &&
                                    !value.Equals("internal") &&
                                    !value.Equals("content-stage"))
                                {
                                    Logging.Warning("ServiceCore::initialize unknown server environment: " + ServiceCore.ServiceEnvironment);
                                }

                                ServiceCore.ServiceEnvironment = value;
                                break;
                            case "-conf":
                                if (value.Length > 0)
                                {
                                    if (value.StartsWith("http://") || value.StartsWith("https://"))
                                    {
                                        if (!value.EndsWith("/"))
                                        {
                                            value += "/";
                                        }

                                        ServiceCore.ConfigurationServer = value;
                                    }
                                    else
                                    {
                                        Logging.Warning("ServiceCore::initialize invalid server url: " + value);
                                    }
                                }
                                else
                                {
                                    Logging.Warning("ServiceCore::initialize server url is empty");
                                }

                                break;
                            case "-id":
                                ServiceCore.ServiceNodeId = int.Parse(value);
                                break;
                            default:
                                Logging.Warning("ServiceCore::initialize invalid args name");
                                break;
                        }
                    }
                }
                else
                {
                    Logging.Warning("ServiceCore::initialize invalid args length");
                }
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