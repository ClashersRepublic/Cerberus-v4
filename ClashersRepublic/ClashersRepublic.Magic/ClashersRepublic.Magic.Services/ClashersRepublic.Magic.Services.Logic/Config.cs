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

        public static string ServiceFile;
        public static string DatabaseFile;
        public static string ConfigServer;
        
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
                LogicJSONObject resourceObject = jsonObject.GetJSONObject("resources");
                LogicJSONNumber serverIdNumber = jsonObject.GetJSONNumber("server_id");

                Config.ServerId = serverIdNumber.GetIntValue();
                
                if (resourceObject != null)
                {
                    Config.ServiceFile = LogicJSONHelper.GetJSONString(resourceObject, "service_file");
                    Config.DatabaseFile = LogicJSONHelper.GetJSONString(resourceObject, "database_file");
                    Config.ConfigServer = LogicJSONHelper.GetJSONString(resourceObject, "config_server");
                }
            }
            else
            {
                Debugger.Error("Config::loadConfig file config.json not exist");
            }
        }
    }
}