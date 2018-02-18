namespace ClashersRepublic.Magic.Services.Logic.Service
{
    using System.Net;

    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Logic.Service.Setting;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public static class ServiceConfig
    {
        private static string _version;
        private static string _environment;
        
        private static LogicArrayList<string> _proxyNodes;
        private static LogicArrayList<string> _globalChatNodes;
        private static LogicArrayList<string> _avatarNodes;
        private static LogicArrayList<string> _homeNodes;
        private static LogicArrayList<string> _allianceNodes;
        private static LogicArrayList<string> _leagueNodes;
        private static LogicArrayList<string> _battleNodes;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            ServiceConfig._proxyNodes = new LogicArrayList<string>();
            ServiceConfig._globalChatNodes = new LogicArrayList<string>();
            ServiceConfig._avatarNodes = new LogicArrayList<string>();
            ServiceConfig._homeNodes = new LogicArrayList<string>();
            ServiceConfig._allianceNodes = new LogicArrayList<string>();
            ServiceConfig._leagueNodes = new LogicArrayList<string>();
            ServiceConfig._battleNodes = new LogicArrayList<string>();

            using (WebClient www = new WebClient())
            {
                ServiceConfig.LoadConfig(www.DownloadString(Config.ServiceFile));
            }
        }

        /// <summary>
        ///     Loads the config file.
        /// </summary>
        private static void LoadConfig(string file)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(file);

            ServiceConfig._version = LogicJSONHelper.GetJSONString(jsonObject, "version");
            ServiceConfig._environment = LogicJSONHelper.GetJSONString(jsonObject, "env");

            LogicJSONObject nodeObject = jsonObject.GetJSONObject("nodes");

            if (nodeObject != null)
            {
                LogicJSONArray proxyArray = nodeObject.GetJSONArray("proxy");

                if (proxyArray != null)
                {
                    for (int i = 0; i < proxyArray.Size(); i++)
                    {
                        ServiceConfig._proxyNodes.Add(proxyArray.GetJSONString(i).GetStringValue());
                    }
                }

                LogicJSONArray globalChatArray = nodeObject.GetJSONArray("global_chat");

                if (globalChatArray != null)
                {
                    for (int i = 0; i < globalChatArray.Size(); i++)
                    {
                        ServiceConfig._globalChatNodes.Add(globalChatArray.GetJSONString(i).GetStringValue());
                    }
                }

                LogicJSONArray avatarArray = nodeObject.GetJSONArray("avatar");

                if (avatarArray != null)
                {
                    for (int i = 0; i < avatarArray.Size(); i++)
                    {
                        ServiceConfig._avatarNodes.Add(avatarArray.GetJSONString(i).GetStringValue());
                    }
                }

                LogicJSONArray homeArray = nodeObject.GetJSONArray("home");

                if (homeArray != null)
                {
                    for (int i = 0; i < homeArray.Size(); i++)
                    {
                        ServiceConfig._homeNodes.Add(homeArray.GetJSONString(i).GetStringValue());
                    }
                }

                LogicJSONArray allianceArray = nodeObject.GetJSONArray("alliance");

                if (allianceArray != null)
                {
                    for (int i = 0; i < allianceArray.Size(); i++)
                    {
                        ServiceConfig._allianceNodes.Add(allianceArray.GetJSONString(i).GetStringValue());
                    }
                }

                LogicJSONArray leagueArray = nodeObject.GetJSONArray("league");

                if (leagueArray != null)
                {
                    for (int i = 0; i < leagueArray.Size(); i++)
                    {
                        ServiceConfig._leagueNodes.Add(leagueArray.GetJSONString(i).GetStringValue());
                    }
                }

                LogicJSONArray battleArray = nodeObject.GetJSONArray("battle");

                if (battleArray != null)
                {
                    for (int i = 0; i < battleArray.Size(); i++)
                    {
                        ServiceConfig._battleNodes.Add(battleArray.GetJSONString(i).GetStringValue());
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the service config file.
        /// </summary>
        public static void LoadServiceConfig(string file)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(file);

            LogicJSONObject netObject = jsonObject.GetJSONObject("net");

            if (netObject != null)
            {
                ServiceNetConfig.LoadConfig(netObject);
            }

            LogicJSONObject logicObject = jsonObject.GetJSONObject("logic");

            if (logicObject != null)
            {
                ServiceLogicConfig.LoadConfig(logicObject);
            }

            LogicJSONObject securityObject = jsonObject.GetJSONObject("security");

            if (securityObject != null)
            {
                ServiceSecurityConfig.LoadConfig(securityObject);
            }

            LogicJSONObject stockageObject = jsonObject.GetJSONObject("stockage");

            if (stockageObject != null)
            {
                ServiceStockageConfig.LoadConfig(stockageObject);
            }
        }

        /// <summary>
        ///     Gets the service version.
        /// </summary>
        public static string GetVersion()
        {
            return ServiceConfig._version;
        }

        /// <summary>
        ///     Gets the service environment.
        /// </summary>
        public static string GetEnvironment()
        {
            return ServiceConfig._environment;
        }

        /// <summary>
        ///     Gets all service host for specified service type.
        /// </summary>
        public static LogicArrayList<string> GetServiceHosts(int serviceType)
        {
            switch (serviceType)
            {
                case 1: return ServiceConfig._proxyNodes;
                case 6: return ServiceConfig._globalChatNodes;
                case 9: return ServiceConfig._avatarNodes;
                case 10: return ServiceConfig._homeNodes;
                case 11: return ServiceConfig._allianceNodes;
                case 13: return ServiceConfig._leagueNodes;
                case 27: return ServiceConfig._battleNodes;
                default: return null;
            }
        }
    }
}