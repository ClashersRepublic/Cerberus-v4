namespace ClashersRepublic.Magic.Services.Home.Database
{
    using System;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Home.Home;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Titan.Math;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;

    internal class GameDatabase
    {
        private int _id;

        private IBucket _homeBucket;
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

            this._homeBucket = this._cluster.OpenBucket("magic-homes");
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
        ///     Inserts a new home.
        /// </summary>
        internal void InsertHome(GameHome home)
        {
            if (home.Id.IsZero())
            {
                Logging.Error(this, "GameDatabase::insertHome id not set");
                return;
            }

            IDocumentResult result = this._homeBucket.Insert(new Document<GameHome>
            {
                Id = home.Id.GetLowerInt().ToString(),
                Content = home
            });

            if (!result.Success)
            {
                Logging.Error(this, "GameDatabase::insertHome insert failed, status: " + result.Status);
            }
        }

        /// <summary>
        ///     Gets the specified document.
        /// </summary>
        internal GameHome GetAccount(int lowId)
        {
            return this._homeBucket.GetDocument<GameHome>(lowId.ToString()).Content;
        }
    }
}