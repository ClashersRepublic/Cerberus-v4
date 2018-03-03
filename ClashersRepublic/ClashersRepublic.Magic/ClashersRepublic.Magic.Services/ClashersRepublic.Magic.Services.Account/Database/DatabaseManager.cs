namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Network;

    using Couchbase.Configuration.Client;

    internal static class DatabaseManager
    {
        private static int _scrambler;
        private static IDatabase[] _databases;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            DatabaseManager._databases = new IDatabase[NetManager.GetDatabaseUrls().Length];

            for (int i = 0; i < DatabaseManager._databases.Length; i++)
            {
                DatabaseManager._databases[i] = new CouchbaseDatabase(i, new ClientConfiguration
                {
                    Servers = new List<Uri>
                    {
                        new Uri("http://" + NetManager.GetDatabaseUrls()[i])
                    }
                }, "magic-accounts", NetManager.GetDatabaseUserName(), NetManager.GetDatabasePassword());
            }
        }

        /// <summary>
        ///     Iterates the scrambler.
        /// </summary>
        internal static int IterateScrambler()
        {
            int tmp = DatabaseManager._scrambler;
            DatabaseManager._scrambler = (DatabaseManager._scrambler + 1) % DatabaseManager._databases.Length;
            return tmp;
        }

        /// <summary>
        ///     Gets the number of database.
        /// </summary>
        internal static int GetDatabaseCount()
        {
            return DatabaseManager._databases.Length;
        }

        /// <summary>
        ///     Gets the <see cref="IDatabase"/> instance.
        /// </summary>
        internal static IDatabase GetDatabase(int idx)
        {
            return DatabaseManager._databases[idx];
        }
    }
}