namespace ClashersRepublic.Magic.Services.Home.Database
{
    using System;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Home.Debug;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;

    internal class GameDatabase
    {
        private int _id;
        private IBucket _bucket;
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
            this._bucket = this._cluster.OpenBucket("magic-accounts");

            if (!this._bucket.Exists("counters"))
            {
                this._bucket.Insert("counters", 0);
            }
        }

        /// <summary>
        ///     Gets the higher id.
        /// </summary>
        internal int GetHigherId()
        {
            return (int) this._bucket.Get<ulong>("counters").Value;
        }

        /// <summary>
        ///     Inserts a new account.
        /// </summary>
        internal void InsertAccount(GameAccount account)
        {
            if (account.HighId == 0 && account.LowId == 0)
            {
                account.HighId = this._id;
                account.LowId = (int) this._bucket.Increment("counters").Value;
            }

            IDocumentResult result = this._bucket.Insert(new Document<GameAccount>
            {
                Id = account.LowId.ToString(),
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
            return this._bucket.Get<GameAccount>(lowId.ToString()).Value;
        }
    }
}