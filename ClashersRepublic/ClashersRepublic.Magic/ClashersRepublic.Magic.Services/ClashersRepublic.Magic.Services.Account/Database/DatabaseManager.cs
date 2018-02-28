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
                    new Uri("http://127.0.0.1")
                }
            };
            DatabaseManager._database = new CouchbaseDatabase(configuration, "MagicServer", "HlB18qOxGj1DPLYbQof4cjoAN9SxMpuwoOymYxrQs13QtTbB2313JNkltbZAF7pp");
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