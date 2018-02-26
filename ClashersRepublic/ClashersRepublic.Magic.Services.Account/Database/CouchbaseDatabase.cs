namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System.Threading.Tasks;

    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Core;

    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;

    internal class CouchbaseDatabase : IDatabase
    {
        private readonly ICluster _cluster;
        private readonly IBucket _accountBucket;
        private readonly IBucket _counterBucket;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CouchbaseDatabase"/> class.
        /// </summary>
        internal CouchbaseDatabase(ClientConfiguration configuration, string userName, string password)
        {
            this._cluster = new Cluster(configuration);
            this._cluster.Authenticate(userName, password);
            this._accountBucket = this._cluster.OpenBucket("magic-accounts");
            this._counterBucket = this._cluster.OpenBucket("magic-counters");
        }

        /// <summary>
        ///     Gets the higher id.
        /// </summary>
        public int GetHigherId()
        {
            return (int) this._counterBucket.Get<ulong>("acc-counters").Value;
        }

        /// <summary>
        ///     Sets the higher id.
        /// </summary>
        public void SetHigherId(int id)
        {
            this._counterBucket.Replace(new Document<ulong>
            {
                Id = "acc-counters",
                Content = (ulong) id
            });
        }

        /// <summary>
        ///     Sets the higher id.
        /// </summary>
        public void IncrementHigherId()
        {
            this._counterBucket.Increment("acc-counters");
        }

        /// <summary>
        ///     Inserts the specified document.
        /// </summary>
        public bool InsertDocument(long id, Account account)
        {
            if (id != 0)
            {
                return this._accountBucket.Insert(new Document<Account>
                {
                    Id = id.ToString(),
                    Content = account
                }).Success;
            }
            else
            {
                Logging.Warning(this, "CouchbaseDatabase::insertDocument id is 0");
            }

            return false;
        }

        /// <summary>
        ///     Inserts the specified document in async.
        /// </summary>
        public async Task<IDocumentResult<Account>> InsertDocumentAsync(long id, Account account)
        {
            if (id != 0)
            {
                return await this._accountBucket.InsertAsync(new Document<Account>
                {
                    Id = id.ToString(),
                    Content = account
                });
            }
            else
            {
                Logging.Warning(this, "CouchbaseDatabase::insertDocument id is 0");
            }

            return null;
        }

        /// <summary>
        ///     Gets the document in database.
        /// </summary>
        public Account GetDocument(long id)
        {
            return this._accountBucket.Get<Account>(id.ToString()).Value;
        }

        /// <summary>
        ///     Gets the document in database in async.
        /// </summary>
        public async Task<IOperationResult<Account>> GetDocumentAsync(long id)
        {
            return await this._accountBucket.GetAsync<Account>(id.ToString());
        }

        /// <summary>
        ///     Updates the document stocked in database.
        /// </summary>
        public void UpdateDocument(long id, Account account)
        {
            this._accountBucket.Replace(new Document<Account>
            {
                Id = id.ToString(),
                Content = account
            });
        }

        /// <summary>
        ///     Updates the document stocked in database in async.
        /// </summary>
        public void UpdateDocumentAsync(long id, Account account)
        {
            this._accountBucket.ReplaceAsync(new Document<Account>
            {
                Id = id.ToString(),
                Content = account
            });
        }
    }
}