namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Core;
    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;
    using Couchbase.N1QL;
    using Newtonsoft.Json.Linq;

    internal class CouchbaseDatabase : IDatabase
    {
        private readonly ICluster _cluster;
        private readonly IBucket _accountBucket;

        private readonly string _bucketName;

        private readonly Thread _saveThread;
        private readonly Thread _insertThread;
        private readonly ConcurrentQueue<Item> _saveQueue;
        private readonly ConcurrentQueue<Item> _insertQueue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CouchbaseDatabase"/> class.
        /// </summary>
        internal CouchbaseDatabase(ClientConfiguration configuration, string bucket, string userName, string password)
        {
            this._bucketName = bucket;

            this._cluster = new Cluster(configuration);
            this._cluster.Authenticate(userName, password);
            this._accountBucket = this._cluster.OpenBucket(bucket);

            this._saveQueue = new ConcurrentQueue<Item>();
            this._insertQueue = new ConcurrentQueue<Item>();
            this._insertThread = new Thread(() =>
            {
                while (true)
                {
                    while (this._insertQueue.TryDequeue(out Item document))
                    {
                        this._accountBucket.Insert(document.Key, document.Json);
                    }

                    Thread.Sleep(1);
                }
            });
            this._saveThread = new Thread(() =>
            {
                while (true)
                {
                    while (this._saveQueue.TryDequeue(out Item document))
                    {
                        this._accountBucket.Replace(document.Key, document.Json);
                    }

                    Thread.Sleep(1);
                }
            });

            this._insertThread.Start();
            this._saveThread.Start();
        }

        /// <summary>
        ///     Gets the higher id.
        /// </summary>
        public int GetHigherId()
        {
            IQueryResult<dynamic> result = this._accountBucket.Query<dynamic>(new QueryRequest().Statement("SELECT MAX(acc_lo) FROM `" + this._bucketName + "` WHERE acc_hi == " + ServiceCore.ServiceNodeId));

            if (result.Success)
            {
                if (result.Rows.Count != 0)
                {
                    JValue value = result.Rows[0]["$1"];

                    if (value == null || value.Type == JTokenType.Null)
                    {
                        return 0;
                    }

                    return (int) value;
                }
            }

            return 0;
        }
        
        /// <summary>
        ///     Inserts the specified document.
        /// </summary>
        public void InsertDocument(long id, string json)
        {
            if (id != 0)
            {
                this._insertQueue.Enqueue(new Item(id.ToString(), json));
            }
            else
            {
                Logging.Warning(this, "CouchbaseDatabase::insertDocument id is 0");
            }
        }

        /// <summary>
        ///     Gets the document in database.
        /// </summary>
        public string GetDocument(long id)
        {
            return this._accountBucket.GetDocument<string>(id.ToString()).Content;
        }

        /// <summary>
        ///     Updates the document stocked in database.
        /// </summary>
        public void UpdateDocument(long id, string json)
        {
            this._saveQueue.Enqueue(new Item(id.ToString(), json));
        }

        private struct Item
        {
            /// <summary>
            ///     Gets the key.
            /// </summary>
            internal string Key { get; }

            /// <summary>
            ///     Gets the json value.
            /// </summary>
            internal string Json { get; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="Item"/> struct.
            /// </summary>
            internal Item(string key, string json)
            {
                this.Key = key;
                this.Json = json;
            }
        }
    }
}