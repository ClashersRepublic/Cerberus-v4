namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System;
    using System.Collections.Generic;
    using Couchbase.Configuration.Client;

    internal static class DatabaseManager
    {
        private static IDatabase _database;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ClientConfiguration configuration = new ClientConfiguration
            {
                Servers = new List<Uri>
                {
                    new Uri("127.0.0.1")
                }
            };
            DatabaseManager._database = new CouchbaseDatabase(configuration);
        }

        /// <summary>
        ///     Gets the <see cref="IDatabase"/> instance.
        /// </summary>
        internal static IDatabase GetDatabase()
        {
            return DatabaseManager._database;
        }
    }
}