namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Core;
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
            DatabaseManager._databases = new IDatabase[ServiceSettings.GetDatabaseUrls().Length];

            for (int i = 0; i < DatabaseManager._databases.Length; i++)
            {
                DatabaseManager._databases[i] = new CouchbaseDatabase(i, new ClientConfiguration
                {
                    Servers = new List<Uri>
                    {
                        new Uri("http://" + ServiceSettings.GetDatabaseUrls()[i])
                    }
                }, "magic-accounts", ServiceSettings.GetDatabaseUserName(), ServiceSettings.GetDatabasePassword());
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
            if (idx > -1 && idx < DatabaseManager._databases.Length)
            {
                return DatabaseManager._databases[idx];
            }

            Logging.Warning(string.Format("DatabaseManager::getDatabase idx out of bands {0}/{1}", idx, DatabaseManager._databases.Length));

            return null;
        }
    }
}