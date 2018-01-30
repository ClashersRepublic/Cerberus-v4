namespace ClashersRepublic.Magic.Services.Home.Database
{
    using ClashersRepublic.Magic.Services.Home.Game;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Titan.Math;

    using MongoDB.Driver;

    internal class GameDatabase
    {
        private static bool _initialized;

        private static IMongoClient _client;
        private static IMongoDatabase _database;
        private static IMongoCollection<GamePlayer> _players;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (GameDatabase._initialized)
            {
                return;
            }

            GameDatabase._initialized = true;
            
            GameDatabase._client = new MongoClient("mongodb://" + Config.MongodUser + ":" + Config.MongodPassword + "@" + Config.MongodServer + ":27017");
            GameDatabase._database = GameDatabase._client.GetDatabase(Config.MongodDbName);
            GameDatabase._players = GameDatabase._database.GetCollection<GamePlayer>(Config.MongodDbCollection);

            if (GameDatabase._players == null)
            {
                GameDatabase._database.CreateCollection(Config.MongodDbCollection);
                GameDatabase._players = GameDatabase._database.GetCollection<GamePlayer>(Config.MongodDbCollection);
            }

            GameDatabase._players.Indexes.CreateOne(Builders<GamePlayer>.IndexKeys.Combine(
                    Builders<GamePlayer>.IndexKeys.Ascending(T => T.HighId),
                    Builders<GamePlayer>.IndexKeys.Descending(T => T.LowId)),
                new CreateIndexOptions
                {
                    Name = "entityIds",
                    Background = true
                }
            );
        }

        /// <summary>
        ///     Gets the higher low id.
        /// </summary>
        internal static int GetHigherLowId()
        {
            GamePlayer account = GameDatabase._players.Find(T => T.HighId == Config.ServerId).SortByDescending(T => T.LowId).Limit(1).SingleOrDefault();

            if (account != null)
            {
                return account.LowId;
            }

            return 0;
        }

        /// <summary>
        ///     Loads player from database.
        /// </summary>
        internal static GamePlayer LoadPlayer(LogicLong accountId)
        {
            return GameDatabase._players.Find(T => T.HighId == accountId.GetHigherInt() && T.LowId == accountId.GetLowerInt()).Limit(1).SingleOrDefault();
        }

        /// <summary>
        ///     Inserts the specified player to database.
        /// </summary>
        internal static void InsertPlayer(GamePlayer player)
        {
            GameDatabase._players.InsertOne(player);
        }

        /// <summary>
        ///     Saves the specified player.
        /// </summary>
        internal static void SaveAccount(GamePlayer player)
        {
            GameDatabase._players.ReplaceOne(T => T._id == player._id, player);
        }
    }
}