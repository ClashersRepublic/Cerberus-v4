namespace ClashersRepublic.Magic.Services.Core.Database
{
    using ClashersRepublic.Magic.Services.Core;

    using Couchbase;
    using Couchbase.Configuration.Client;
    using Couchbase.Core;
    using Couchbase.N1QL;

    using Newtonsoft.Json.Linq;

    public class CouchbaseDatabase : IDatabase
    {
        private readonly ICluster _cluster;
        private readonly IBucket _bucket;

        private readonly int _databaseId;
        private readonly string _bucketName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CouchbaseDatabase"/> class.
        /// </summary>
        public CouchbaseDatabase(int id, ClientConfiguration configuration, string bucket, string userName, string password)
        {
            this._databaseId = id;
            this._bucketName = bucket;

            this._cluster = new Cluster(configuration);
            this._cluster.Authenticate(userName, password);
            this._bucket = this._cluster.OpenBucket(bucket);
        }

        /// <summary>
        ///     Gets the database id.
        /// </summary>
        public int GetDatabaseId()
        {
            return this._databaseId;
        }

        /// <summary>
        ///     Gets the higher id.
        /// </summary>
        public int GetHigherId()
        {
            IQueryResult<dynamic> result = this._bucket.Query<dynamic>(new QueryRequest().Statement("SELECT MAX(acc_lo) FROM `" + this._bucketName + "` WHERE acc_hi == " + this._databaseId));

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
            this._bucket.Insert(id.ToString(), json);
        }

        /// <summary>
        ///     Gets the document in database.
        /// </summary>
        public string GetDocument(long id)
        {
            return this._bucket.GetDocument<string>(id.ToString()).Content;
        }

        /// <summary>
        ///     Updates the document stocked in database.
        /// </summary>
        public void UpdateDocument(long id, string json)
        {
            this._bucket.Replace(id.ToString(), json);
        }
    }
}