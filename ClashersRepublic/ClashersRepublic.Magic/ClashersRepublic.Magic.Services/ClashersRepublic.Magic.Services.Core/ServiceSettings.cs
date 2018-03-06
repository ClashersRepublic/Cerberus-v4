namespace ClashersRepublic.Magic.Services.Core
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Core.Web;
    using ClashersRepublic.Magic.Titan.Json;

    public static class ServiceSettings
    {
        private static string _serviceVersion;
        private static string _serviceEnvironment;
        private static string[][] _serviceIPs;

        private static string _databaseUser;
        private static string _databasePassword;
        private static string[] _databaseUrls;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceSettings._databaseUrls = new string[0];
            ServiceSettings._serviceIPs = new string[28][];

            for (int i = 0; i < 28; i++)
            {
                ServiceSettings._serviceIPs[i] = new string[0];
            }

            ServiceSettings.LoadConfig();
        }

        /// <summary>
        ///     Loads the service config.
        /// </summary>
        private static void LoadConfig()
        {
            string json = WebManager.DownloadConfigFile("/services.json");

            if (json != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject)LogicJSONParser.Parse(json);

                LogicJSONString versionObject = jsonObject.GetJSONString("version");

                if (versionObject != null)
                {
                    ServiceSettings._serviceVersion = versionObject.GetStringValue();
                }

                LogicJSONString environmentObject = jsonObject.GetJSONString("environment");

                if (environmentObject != null)
                {
                    ServiceSettings._serviceEnvironment = environmentObject.GetStringValue();
                }

                LogicJSONObject nodeObject = jsonObject.GetJSONObject("nodes");

                if (nodeObject != null)
                {
                    LogicJSONArray proxyArray = nodeObject.GetJSONArray("proxy");

                    if (proxyArray != null)
                    {
                        ServiceSettings._serviceIPs[1] = new string[proxyArray.Size()];

                        for (int i = 0; i < proxyArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[1][i] = proxyArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray accountArray = nodeObject.GetJSONArray("account");

                    if (accountArray != null)
                    {
                        ServiceSettings._serviceIPs[2] = new string[accountArray.Size()];

                        for (int i = 0; i < accountArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[2][i] = accountArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray avatarArray = nodeObject.GetJSONArray("avatar");

                    if (avatarArray != null)
                    {
                        ServiceSettings._serviceIPs[3] = new string[avatarArray.Size()];

                        for (int i = 0; i < avatarArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[3][i] = avatarArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray globalArray = nodeObject.GetJSONArray("global_chat");

                    if (globalArray != null)
                    {
                        ServiceSettings._serviceIPs[6] = new string[globalArray.Size()];

                        for (int i = 0; i < globalArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[6][i] = globalArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray streamArray = nodeObject.GetJSONArray("stream");

                    if (streamArray != null)
                    {
                        ServiceSettings._serviceIPs[9] = new string[streamArray.Size()];

                        for (int i = 0; i < streamArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[9][i] = streamArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray homeArray = nodeObject.GetJSONArray("home");

                    if (homeArray != null)
                    {
                        ServiceSettings._serviceIPs[10] = new string[homeArray.Size()];

                        for (int i = 0; i < homeArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[10][i] = homeArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray allianceArray = nodeObject.GetJSONArray("alliance");

                    if (allianceArray != null)
                    {
                        ServiceSettings._serviceIPs[11] = new string[allianceArray.Size()];

                        for (int i = 0; i < allianceArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[11][i] = allianceArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray leagueArray = nodeObject.GetJSONArray("league");

                    if (leagueArray != null)
                    {
                        ServiceSettings._serviceIPs[13] = new string[leagueArray.Size()];

                        for (int i = 0; i < leagueArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[13][i] = leagueArray.GetJSONString(i).GetStringValue();
                        }
                    }

                    LogicJSONArray battleArray = nodeObject.GetJSONArray("battle");

                    if (battleArray != null)
                    {
                        ServiceSettings._serviceIPs[27] = new string[battleArray.Size()];

                        for (int i = 0; i < battleArray.Size(); i++)
                        {
                            ServiceSettings._serviceIPs[27][i] = battleArray.GetJSONString(i).GetStringValue();
                        }
                    }
                }

                LogicJSONObject databaseObject = jsonObject.GetJSONObject("database");

                if (databaseObject != null)
                {
                    LogicJSONArray urls = databaseObject.GetJSONArray("urls");

                    if (urls != null)
                    {
                        ServiceSettings._databaseUrls = new string[urls.Size()];

                        for (int i = 0; i < urls.Size(); i++)
                        {
                            ServiceSettings._databaseUrls[i] = urls.GetJSONString(i).GetStringValue();
                        }
                    }

                    ServiceSettings._databaseUser = LogicJSONHelper.GetJSONString(databaseObject, "user");
                    ServiceSettings._databasePassword = LogicJSONHelper.GetJSONString(databaseObject, "pass");
                }
            }
        }

        /// <summary>
        ///     Gets the database username.
        /// </summary>
        public static string GetDatabaseUserName()
        {
            return ServiceSettings._databaseUser;
        }

        /// <summary>
        ///     Gets the database password.
        /// </summary>
        public static string GetDatabasePassword()
        {
            return ServiceSettings._databasePassword;
        }

        /// <summary>
        ///     Gets the database urls.
        /// </summary>
        public static string[] GetDatabaseUrls()
        {
            return ServiceSettings._databaseUrls;
        }

        /// <summary>
        ///     Gets all the ips of the servers
        /// </summary>
        public static string[][] GetServerIPs()
        {
            return ServiceSettings._serviceIPs;
        }
    }
}