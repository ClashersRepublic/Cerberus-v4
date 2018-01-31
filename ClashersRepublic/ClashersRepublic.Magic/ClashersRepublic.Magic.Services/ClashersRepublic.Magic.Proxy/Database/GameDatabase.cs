namespace ClashersRepublic.Magic.Proxy.Database
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Titan.Math;
    using MongoDB.Bson;
    using MongoDB.Driver;

    internal static class GameDatabase
    {
        private static IMongoCollection<GameAccount>[] _accounts;
        private static IMongoCollection<BsonDocument>[] _counters;

        private static Thread _saveThread;
        private static ConcurrentQueue<GameAccount> _saveAccountQueue;

        private static int _nextCollectionIndex;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameDatabase._accounts = new IMongoCollection<GameAccount>[Config.MongodServers.Length];
            GameDatabase._counters = new IMongoCollection<BsonDocument>[Config.MongodServers.Length];

            for (int i = 0; i < Config.MongodServers.Length; i++)
            {
                var client = new MongoClient("mongodb://" + (!string.IsNullOrEmpty(Config.MongodUser) ? Config.MongodUser + ":" + Config.MongodPassword + "@" : "") + Config.MongodServers[i] + ":27017");
                var database = client.GetDatabase(Config.MongodDbName);
                
                GameDatabase._accounts[i] = database.GetCollection<GameAccount>(Config.MongodDbCollection);
                GameDatabase._counters[i] = database.GetCollection<BsonDocument>("Counters");

                GameDatabase._accounts[i].Indexes.CreateOne(Builders<GameAccount>.IndexKeys.Combine(
                        Builders<GameAccount>.IndexKeys.Ascending(T => T.HighId),
                        Builders<GameAccount>.IndexKeys.Descending(T => T.LowId)),
                    new CreateIndexOptions
                    {
                        Name = "entityIds",
                        Background = true
                    }
                );
            }
            
            GameDatabase._saveAccountQueue = new ConcurrentQueue<GameAccount>();
            GameDatabase._saveThread = new Thread(GameDatabase.SaveTask);
            GameDatabase._saveThread.Start();
        }

        /// <summary>
        ///     Gets the higher account id.
        /// </summary>
        internal static int GetHigherAccountId(int dbId)
        {
            return GameDatabase._counters[dbId].Find(T => T["_id"] == "Accounts").Limit(1).Single()["last_id"].AsInt32;
        }

        /// <summary>
        ///     Inserts the specified account to database.
        /// </summary>
        internal static void InsertAccount(GameAccount account)
        {
            if (account.HighId == 0 && account.LowId == 0)
            {
                account.HighId = GameDatabase._nextCollectionIndex;
                account.LowId = GameDatabase._counters[account.HighId].FindOneAndUpdate(T => T["_id"] == "Accounts", Builders<BsonDocument>.Update.Inc("last_id", 1))["last_id"].AsInt32 + 1;
                    
                GameDatabase._nextCollectionIndex = ++GameDatabase._nextCollectionIndex % GameDatabase._accounts.Length;

                Console.WriteLine(account.HighId + "-" + account.LowId);
            }

            GameDatabase._accounts[account.HighId].InsertOne(account);
        }

        /// <summary>
        ///     Inserts the specified account to database.
        /// </summary>
        internal static void SaveAccount(GameAccount account)
        {
            GameDatabase._saveAccountQueue.Enqueue(account);
        }

        /// <summary>
        ///     Loads the specified account from database.
        /// </summary>
        internal static GameAccount LoadAccount(LogicLong accountId)
        {
            return GameDatabase._accounts[accountId.GetHigherInt()].Find(T => T.HighId == accountId.GetHigherInt() && T.LowId == accountId.GetLowerInt()).Limit(1).SingleOrDefault();
        }

        /// <summary>
        ///     Tasks for save thread.
        /// </summary>
        private static void SaveTask()
        {
            while (true)
            {
                while (GameDatabase._saveAccountQueue.TryDequeue(out GameAccount account))
                {
                    GameDatabase._accounts[account.HighId].ReplaceOne(T => T._id == account._id, account);
                }

                Thread.Sleep(5);
            }
        }
    }
}