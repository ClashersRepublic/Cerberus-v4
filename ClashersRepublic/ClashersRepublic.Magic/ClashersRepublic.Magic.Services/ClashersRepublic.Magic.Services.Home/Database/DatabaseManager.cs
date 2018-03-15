// #define USE_THREAD

namespace ClashersRepublic.Magic.Services.Home.Database
{
    using System;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Home.Game;
    using ClashersRepublic.Magic.Titan.Json;

    using Couchbase.Configuration.Client;

    #if USE_THREAD
    using System.Collections.Concurrent;
    using System.Threading;
    #endif

    internal static class DatabaseManager
    {
        private static IDatabase[] _databases;

        #if USE_THREAD
        private static Thread _insertThread;
        private static Thread _updateThread;
        private static ConcurrentQueue<Home> _insertQueue;
        private static ConcurrentQueue<Home> _updateQueue;
        #endif

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
                }, "magic-homes", ServiceSettings.GetDatabaseUserName(), ServiceSettings.GetDatabasePassword());
            }

            #if USE_THREAD
            DatabaseManager._insertQueue = new ConcurrentQueue<Home>();
            DatabaseManager._updateQueue = new ConcurrentQueue<Home>();
            DatabaseManager._insertThread = new Thread(DatabaseManager.InsertLoop);
            DatabaseManager._updateThread = new Thread(DatabaseManager.UpdateLoop);
            DatabaseManager._insertThread.Start();
            DatabaseManager._updateThread.Start();
            #endif
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

        /// <summary>
        ///     Inserts the specified home.
        /// </summary>
        internal static void Insert(Home home)
        {
            #if USE_THREAD
            DatabaseManager._insertQueue.Enqueue(home);
            #else
            DatabaseManager.InternalInsert(home);
            #endif
        }

        /// <summary>
        ///     Inserts the specified home.
        /// </summary>
        internal static void Update(Home home)
        {
            #if USE_THREAD
            DatabaseManager._updateQueue.Enqueue(home);
            #else
            DatabaseManager.InternalUpdate(home);
                        #endif
        }

        /// <summary>
        ///     Inserts the specified home on database.
        /// </summary>
        private static void InternalInsert(Home home)
        {
            IDatabase database = DatabaseManager.GetDatabase(home.Id.GetHigherInt());

            if (database != null)
            {
                database.InsertDocument(home.Id, LogicJSONParser.CreateJSONString(home.Save()));
            }
        }

        /// <summary>
        ///     Updates the specified home on database.
        /// </summary>
        private static void InternalUpdate(Home home)
        {
            IDatabase database = DatabaseManager.GetDatabase(home.Id.GetHigherInt());

            if (database != null)
            {
                database.UpdateDocument(home.Id, LogicJSONParser.CreateJSONString(home.Save()));
            }
        }

        #if USE_THREAD
        /// <summary>
        ///     Loop for the insert thread.
        /// </summary>
        private static void InsertLoop()
        {
            while (true)
            {
                if (DatabaseManager._insertQueue.TryDequeue(out Home home))
                {
                    DatabaseManager.InternalInsert(home);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Loop for the update thread.
        /// </summary>
        private static void UpdateLoop()
        {
            while (true)
            {
                if (DatabaseManager._updateQueue.TryDequeue(out Home home))
                {
                    DatabaseManager.InternalUpdate(home);
                }

                Thread.Sleep(1);
            }
        }
        #endif
    }
}