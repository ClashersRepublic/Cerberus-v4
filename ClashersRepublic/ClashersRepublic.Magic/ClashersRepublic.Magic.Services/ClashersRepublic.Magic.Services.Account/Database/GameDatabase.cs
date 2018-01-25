﻿namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System;
    using ClashersRepublic.Magic.Services.Logic;
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
            
            GameDatabase._client = new MongoClient("mongodb://" + Config.MongodUser + ":" + Config.MongodPassword + "@" + Config.MongodServer + ":27017");
            GameDatabase._database = GameDatabase._client.GetDatabase(Config.MongodDbName);
            GameDatabase._accounts = GameDatabase._database.GetCollection<GameAccount>(Config.MongodDbCollection);

            if (GameDatabase._accounts == null)
            {
                GameDatabase._database.CreateCollection(Config.MongodDbCollection);
                GameDatabase._accounts = GameDatabase._database.GetCollection<GameAccount>(Config.MongodDbCollection);
            }

            GameDatabase._accounts.Indexes.CreateOne(Builders<GameAccount>.IndexKeys.Combine(
                    Builders<GameAccount>.IndexKeys.Ascending(T => T.HighId),
                    Builders<GameAccount>.IndexKeys.Descending(T => T.LowId)),
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