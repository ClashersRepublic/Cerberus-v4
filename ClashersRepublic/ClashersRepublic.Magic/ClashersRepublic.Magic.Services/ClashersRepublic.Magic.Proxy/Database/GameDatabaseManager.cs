namespace ClashersRepublic.Magic.Proxy.Database
{
    using System.Net;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Log;

    using ClashersRepublic.Magic.Titan.Json;

    internal static class GameDatabaseManager
    {
        private static GameDatabase[] _databases;
        private static int _dbScrambler;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            using (WebClient www = new WebClient())
            {
                GameDatabaseManager.LoadConfig(www.DownloadString(Config.DatabaseFile));
            }
        }

        /// <summary>
        ///     Loads the config file.
        /// </summary>
        internal static void LoadConfig(string file)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(file);
            LogicJSONArray serverArray = jsonObject.GetJSONArray("servers");
            LogicJSONString userString = jsonObject.GetJSONString("user");
            LogicJSONString passwordString = jsonObject.GetJSONString("password");

            if (serverArray != null)
            {
                GameDatabaseManager._databases = new GameDatabase[serverArray.Size()];

                for (int i = 0; i < GameDatabaseManager._databases.Length; i++)
                {
                    GameDatabaseManager._databases[i] = new GameDatabase(i, serverArray.GetJSONString(i).GetStringValue(), userString.GetStringValue(), passwordString.GetStringValue());
                }
            }
        }

        /// <summary>
        ///     Gets the specified database instance.
        /// </summary>
        internal static GameDatabase GetDatabase(int databaseId)
        {
            if (databaseId >= 0 && databaseId < GameDatabaseManager._databases.Length)
            {
                return GameDatabaseManager._databases[databaseId];
            }

            Logging.Error(typeof(GameDatabaseManager), "GameDatabaseManager::getDatabase index are out of bands " + databaseId + "/" + GameDatabaseManager._databases.Length);

            return null;
        }

        /// <summary>
        ///     Gets random database instance.
        /// </summary>
        internal static GameDatabase GetRandomDatabase()
        {
            GameDatabase database = GameDatabaseManager._databases[GameDatabaseManager._dbScrambler];
            GameDatabaseManager._dbScrambler = (GameDatabaseManager._dbScrambler + 1) % GameDatabaseManager._databases.Length;
            return database;
        }
    }
}