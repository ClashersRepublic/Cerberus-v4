namespace ClashersRepublic.Magic.Services.Account.Database
{
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Titan.Math;

    using MongoDB.Driver;

    internal class GameDatabase
    {
        private static bool _initialized;

        private static IMongoClient _client;
        private static IMongoDatabase _database;
        private static IMongoCollection<GameAccount> _accounts;

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

            GameDatabase._client = new MongoClient(Config.GameDatabaseUrl);
            GameDatabase._database = GameDatabase._client.GetDatabase(Config.GameDatabaseName);
            GameDatabase._accounts = GameDatabase._database.GetCollection<GameAccount>(Config.GameDatabaseCollection);
        }

        /// <summary>
        ///     Gets the higher low id.
        /// </summary>
        internal static int GetHigherLowId()
        {
            GameAccount account = GameDatabase._accounts.Find(T => T.HighId == Config.ServerId).SortByDescending(T => T.LowId).Limit(1).SingleOrDefault();

            if (account != null)
            {
                return account.LowId;
            }

            return 0;
        }

        /// <summary>
        ///     Loads account from database.
        /// </summary>
        internal static GameAccount LoadAccount(LogicLong accountId)
        {
            return GameDatabase._accounts.Find(T => T.HighId == accountId.GetHigherInt() && T.LowId == accountId.GetLowerInt()).Limit(1).SingleOrDefault();
        }

        /// <summary>
        ///     Inserts the specified account to database.
        /// </summary>
        internal static void InsertAccount(GameAccount account)
        {
            GameDatabase._accounts.InsertOne(account);
        }

        /// <summary>
        ///     Saves the specified account.
        /// </summary>
        internal static void SaveAccount(GameAccount account)
        {
            GameDatabase._accounts.ReplaceOne(T => T._id == account._id, account);
        }
    }
}