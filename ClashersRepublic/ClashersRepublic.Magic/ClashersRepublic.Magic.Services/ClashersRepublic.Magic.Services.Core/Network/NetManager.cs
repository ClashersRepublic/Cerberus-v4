namespace ClashersRepublic.Magic.Services.Core.Network
{
    using ClashersRepublic.Magic.Services.Core.Web;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public static class NetManager
    {
        private static string _netVersion;
        private static string _netEnvironment;

        private static int[] _scrambler;
        private static LogicArrayList<NetSocket>[] _endPoints;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            NetManager._scrambler = new int[28];
            NetManager._endPoints = new LogicArrayList<NetSocket>[28];

            for (int i = 0; i < 28; i++)
            {
                NetManager._endPoints[i] = new LogicArrayList<NetSocket>(10);
            }

            NetManager.LoadConfig();
        }
        
        /// <summary>
        ///     Loads the config.
        /// </summary>
        private static void LoadConfig()
        {
            string json = WebManager.DownloadString("https://raw.githubusercontent.com/Mimi8298/services/master/services.json");

            if (json != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

                LogicJSONString versionObject = jsonObject.GetJSONString("version");

                if (versionObject != null)
                {
                    NetManager._netVersion = versionObject.GetStringValue();
                }

                LogicJSONString environmentObject = jsonObject.GetJSONString("environment");

                if (environmentObject != null)
                {
                    NetManager._netEnvironment = environmentObject.GetStringValue();
                }

                LogicJSONObject nodeObject = jsonObject.GetJSONObject("nodes");

                if (nodeObject != null)
                {
                    LogicJSONArray proxyArray = nodeObject.GetJSONArray("proxy");

                    if (proxyArray != null)
                    {
                        for (int i = 0; i < proxyArray.Size(); i++)
                        {
                            NetManager.CreateSocket(proxyArray.GetJSONString(i).GetStringValue(), 1);
                        }
                    }

                    LogicJSONArray accountArray = nodeObject.GetJSONArray("account");

                    if (accountArray != null)
                    {
                        for (int i = 0; i < accountArray.Size(); i++)
                        {
                            NetManager.CreateSocket(accountArray.GetJSONString(i).GetStringValue(), 2);
                        }
                    }

                    LogicJSONArray globalArray = nodeObject.GetJSONArray("global_chat");

                    if (globalArray != null)
                    {
                        for (int i = 0; i < globalArray.Size(); i++)
                        {
                            NetManager.CreateSocket(globalArray.GetJSONString(i).GetStringValue(), 6);
                        }
                    }

                    LogicJSONArray avatarArray = nodeObject.GetJSONArray("avatar");

                    if (avatarArray != null)
                    {
                        for (int i = 0; i < avatarArray.Size(); i++)
                        {
                            NetManager.CreateSocket(avatarArray.GetJSONString(i).GetStringValue(), 9);
                        }
                    }

                    LogicJSONArray homeArray = nodeObject.GetJSONArray("home");

                    if (homeArray != null)
                    {
                        for (int i = 0; i < homeArray.Size(); i++)
                        {
                            NetManager.CreateSocket(homeArray.GetJSONString(i).GetStringValue(), 10);
                        }
                    }

                    LogicJSONArray allianceArray = nodeObject.GetJSONArray("alliance");

                    if (allianceArray != null)
                    {
                        for (int i = 0; i < allianceArray.Size(); i++)
                        {
                            NetManager.CreateSocket(allianceArray.GetJSONString(i).GetStringValue(), 11);
                        }
                    }

                    LogicJSONArray leagueArray = nodeObject.GetJSONArray("league");

                    if (leagueArray != null)
                    {
                        for (int i = 0; i < leagueArray.Size(); i++)
                        {
                            NetManager.CreateSocket(leagueArray.GetJSONString(i).GetStringValue(), 13);
                        }
                    }

                    LogicJSONArray battleArray = nodeObject.GetJSONArray("battle");

                    if (battleArray != null)
                    {
                        for (int i = 0; i < battleArray.Size(); i++)
                        {
                            NetManager.CreateSocket(battleArray.GetJSONString(i).GetStringValue(), 28);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a new socket for the specified service node.
        /// </summary>
        private static void CreateSocket(string host, int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                NetManager._endPoints[serviceNodeType].Add(new NetSocket(serviceNodeType, NetManager._endPoints[serviceNodeType].Count, host));
            }
        }

        /// <summary>
        ///     Gets the <see cref="NetSocket" /> of the specified service node.
        /// </summary>
        public static NetSocket GetServiceNodeEndPoint(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                return NetManager._endPoints[serviceNodeType][serviceNodeId];
            }

            Logging.Warning(typeof(NetManager), "NetManager::getServiceNodeEndPoint serviceNodeType out of bands " + serviceNodeType + "/" + NetManager._endPoints.Length);

            return null;
        }

        /// <summary>
        ///     Gets a random <see cref="NetSocket" /> instance.
        /// </summary>
        public static NetSocket GetRandomEndPoint(int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                if (NetManager._endPoints[serviceNodeType].Count != 0)
                {
                    NetSocket socket = NetManager._endPoints[serviceNodeType][NetManager._scrambler[serviceNodeType]];
                    NetManager._scrambler[serviceNodeType] = NetManager._scrambler[serviceNodeType] % NetManager._endPoints[serviceNodeType].Count;
                    return socket;
                }
            }
            else
            {
                Logging.Warning(typeof(NetManager), "NetManager::getRandomEndPoint serviceNodeType out of bands " + serviceNodeType + "/" + NetManager._endPoints.Length);
            }

            return null;
        }
    }
}