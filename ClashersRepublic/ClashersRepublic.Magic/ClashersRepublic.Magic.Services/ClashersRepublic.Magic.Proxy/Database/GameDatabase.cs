namespace ClashersRepublic.Magic.Proxy.Database
{
    using System;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Titan.Math;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;

    internal class GameDatabase
    {
        private int _id;

        private IBucket _accountBucket;
        private IBucket _counterBucket;

        private Cluster _cluster;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameDatabase"/> class.
        /// </summary>
        internal GameDatabase(int id, string serverUrl, string userName, string passToken)
        {
            this._id = id;

            this._cluster = new Cluster(new ClientConfiguration
            {
                Servers = new List<Uri>
                {
                    new Uri("http://" + serverUrl)
                }
            });
            this._cluster.Authenticate(userName, passToken);

            this._accountBucket = this._cluster.OpenBucket("magic-accounts");
            this._counterBucket = this._cluster.OpenBucket("magic-counters");

            if (!this._counterBucket.Exists("acc_counters"))
            {
                this._counterBucket.Insert("acc_counters", 0);
            }
        }

        /// <summary>
        ///     Gets the higher id.
        /// </summary>
        internal int GetHigherId()
        {
            return (int) this._counterBucket.Get<ulong>("acc_counters").Value;
        }

        /// <summary>
        ///     Inserts a new account.
        /// </summary>
        internal void InsertAccount(GameAccount account)
        {
            if (account.Id.IsZero())
            {
                account.Id = new LogicLong(Config.ServerId, (int) this._counterBucket.Increment("acc_counters").Value);
            }

            IDocumentResult result = this._accountBucket.Insert(new Document<GameAccount>
            {
                Id = account.Id.GetLowerInt().ToString(),
                Content = account
            });

            if (!result.Success)
            {
                Logging.Error(this, "GameDatabase::insertAccount insert failed, status: " + result.Status);
            }
        }

        /// <summary>
        ///     Gets the specified document.
        /// </summary>
        internal GameAccount GetAccount(int lowId)
        {
            return this._accountBucket.Get<GameAccount>(lowId.ToString()).Value;
        }
    }
}