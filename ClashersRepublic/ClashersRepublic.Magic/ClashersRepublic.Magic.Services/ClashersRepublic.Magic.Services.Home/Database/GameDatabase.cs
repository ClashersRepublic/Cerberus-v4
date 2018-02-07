namespace ClashersRepublic.Magic.Services.Home.Database
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Home.Player;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Titan.Math;
    using MongoDB.Bson;
    using MongoDB.Driver;

    internal static class GameDatabase
    {
        private static IMongoCollection<GamePlayer>[] _players;
        private static IMongoCollection<BsonDocument>[] _counters;

        private static Thread _saveThread;
        private static ConcurrentQueue<GamePlayer> _saveAccountQueue;

        private static int _nextCollectionIndex;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameDatabase._players = new IMongoCollection<GamePlayer>[Config.MongodServers.Length];
            GameDatabase._counters = new IMongoCollection<BsonDocument>[Config.MongodServers.Length];

            for (int i = 0; i < Config.MongodServers.Length; i++)
            {
                var client = new MongoClient("mongodb://" + (!string.IsNullOrEmpty(Config.MongodUser) ? Config.MongodUser + ":" + Config.MongodPassword + "@" : "") + Config.MongodServers[i] + ":27017");
                var database = client.GetDatabase(Config.MongodDbName);
                
                GameDatabase._players[i] = database.GetCollection<GamePlayer>("Players");
                GameDatabase._counters[i] = database.GetCollection<BsonDocument>("Counters");

                if (GameDatabase._counters[i] == null)
                {
                    database.CreateCollection("Counters");

                    GameDatabase._counters[i] = database.GetCollection<BsonDocument>("Counters");
                    GameDatabase._counters[i].InsertOne(BsonDocument.Parse("{\"_id\":\"Players\",\"last_id\":0}"));
                }

                GameDatabase._players[i].Indexes.CreateOne(Builders<GamePlayer>.IndexKeys.Combine(
                        Builders<GamePlayer>.IndexKeys.Ascending(T => T.HighId),
                        Builders<GamePlayer>.IndexKeys.Descending(T => T.LowId)),
                    new CreateIndexOptions
                    {
                        Name = "entityIds",
                        Background = true
                    }
                );
            }
            
            GameDatabase._saveAccountQueue = new ConcurrentQueue<GamePlayer>();
            GameDatabase._saveThread = new Thread(GameDatabase.SaveTask);
            GameDatabase._saveThread.Start();
        }

        /// <summary>
        ///     Gets the higher player id.
        /// </summary>
        internal static int GetHigherAccountId(int dbId)
        {
            return GameDatabase._counters[dbId].Find(T => T["_id"] == "Accounts").Limit(1).Single()["last_id"].AsInt32;
        }

        /// <summary>
        ///     Inserts the specified player to database.
        /// </summary>
        internal static void InsertPlayer(GamePlayer player)
        {
            if (player.HighId == 0 && player.LowId == 0)
            {
                player.HighId = GameDatabase._nextCollectionIndex;
                player.LowId = GameDatabase._counters[player.HighId].FindOneAndUpdate(T => T["_id"] == "Accounts", Builders<BsonDocument>.Update.Inc("last_id", 1))["last_id"].AsInt32 + 1;
                    
                GameDatabase._nextCollectionIndex = ++GameDatabase._nextCollectionIndex % GameDatabase._players.Length;
            }

            GameDatabase._players[player.HighId].InsertOne(player);
        }

        /// <summary>
        ///     Inserts the specified player to database.
        /// </summary>
        internal static void SaveAccount(GamePlayer player)
        {
            GamePlayer savePlayer = new GamePlayer
            {
                _id = player._id,

                AvatarDocument = player.AvatarDocument,
                HomeDocument = player.HomeDocument,

                LastSaveTime = player.LastSaveTime
            };

            GameDatabase._saveAccountQueue.Enqueue(savePlayer);
        }

        /// <summary>
        ///     Loads the specified player from database.
        /// </summary>
        internal static GamePlayer LoadPlayer(LogicLong accountId)
        {
            return GameDatabase._players[accountId.GetHigherInt()].Find(T => T.HighId == accountId.GetHigherInt() && T.LowId == accountId.GetLowerInt()).Limit(1).SingleOrDefault();
        }

        /// <summary>
        ///     Tasks for save thread.
        /// </summary>
        private static void SaveTask()
        {
            while (true)
            {
                while (GameDatabase._saveAccountQueue.TryDequeue(out GamePlayer player))
                {
                    GameDatabase._players[player.HighId].ReplaceOne(T => T._id == player._id, player);
                }

                Thread.Sleep(1);
            }
        }
    }
}