namespace ClashersRepublic.Magic.Services.Core.Database
{
    using System;
    using System.Collections.Generic;

    using Couchbase.Configuration.Client;

    public static class DatabaseManager
    {
        private static CouchbaseDatabase[] _databases;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize(string bucketName)
        {
            DatabaseManager._databases = new CouchbaseDatabase[ServiceSettings.GetDatabaseUrls().Length];

            for (int i = 0; i < DatabaseManager._databases.Length; i++)
            {
                DatabaseManager._databases[i] = new CouchbaseDatabase(i, new ClientConfiguration
                {
                    Servers = new List<Uri>
                    {
                        new Uri("http://" + ServiceSettings.GetDatabaseUrls()[i])
                    }
                }, bucketName, ServiceSettings.GetDatabaseUserName(), ServiceSettings.GetDatabasePassword());
            }
        }

        /// <summary>
        ///     Inserts the specified document.
        /// </summary>
        public static void Insert(long key, string json)
        {
            CouchbaseDatabase database = DatabaseManager._databases[key >> 32];

            if (database != null)
            {
                database.InsertDocument(key, json);
            }
        }

        /// <summary>
        ///     Updates the specified document.
        /// </summary>
        public static void Update(long key, string json)
        {
            CouchbaseDatabase database = DatabaseManager._databases[key >> 32];

            if (database != null)
            {
                database.UpdateDocument(key, json);
            }
        }

        /// <summary>
        ///     Gets the number of database.
        /// </summary>
        public static int GetDatabaseCount()
        {
            return DatabaseManager._databases.Length;
        }

        /// <summary>
        ///     Gets the database instance at the specified idx.
        /// </summary>
        public static CouchbaseDatabase GetDatabaseAt(int idx)
        {
            return DatabaseManager._databases[idx];
        }
    }
}