namespace ClashersRepublic.Magic.Services.Logic
{
    using System.IO;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;

    public static class Config
    {
        private static bool _initialized;

        public static int ServerId;

        public static int BufferSize;
        public static int MaxPlayers;

        public static string ServerVersion;
        public static string ServerEnvironment;

        public static string RabbitServer;
        public static string RabbitUser;
        public static string RabbitPassword;

        public static string MongodServer;
        public static string MongodUser;
        public static string MongodPassword;
        public static string MongodDbName;
        public static string MongodDbCollection;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            if (Config._initialized)
            {
                return;
            }

            Config._initialized = true;
            Config.Load();
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        private static void Load()
        {
            Config.LoadConfig();
        }

        /// <summary>
        ///     Loads the config file.
        /// </summary>
        private static void LoadConfig()
        {
            if (File.Exists("config.json"))
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(File.ReadAllText("config.json"));

                LogicJSONObject rabbitObject = jsonObject.GetJSONObject("rabbit");
                LogicJSONObject mongodObject = jsonObject.GetJSONObject("mongod");
                LogicJSONObject networkObject = jsonObject.GetJSONObject("network");

                Config.ServerId = LogicJSONHelper.GetJSONNumber(jsonObject, "serverId");
                Config.ServerVersion = LogicJSONHelper.GetJSONString(jsonObject, "serverVersion");
                Config.ServerEnvironment = LogicJSONHelper.GetJSONString(jsonObject, "serverEnvironment");

                if (rabbitObject != null)
                {
                    Config.RabbitServer = LogicJSONHelper.GetJSONString(rabbitObject, "server");
                    Config.RabbitUser = LogicJSONHelper.GetJSONString(rabbitObject, "user");
                    Config.RabbitPassword = LogicJSONHelper.GetJSONString(rabbitObject, "password");
                }

                if (mongodObject != null)
                {
                    Config.MongodServer = LogicJSONHelper.GetJSONString(mongodObject, "server");
                    Config.MongodUser = LogicJSONHelper.GetJSONString(mongodObject, "user");
                    Config.MongodPassword = LogicJSONHelper.GetJSONString(mongodObject, "password");
                    Config.MongodDbName = LogicJSONHelper.GetJSONString(mongodObject, "db_name");
                    Config.MongodDbCollection = LogicJSONHelper.GetJSONString(mongodObject, "db_collection_name");
                }

                if (networkObject != null)
                {
                    Config.BufferSize = LogicJSONHelper.GetJSONNumber(networkObject, "buffer_size");
                }
            }
            else
            {
                Debugger.Error("Config::loadConfig file config.json not exist");
            }
        }
    }
}