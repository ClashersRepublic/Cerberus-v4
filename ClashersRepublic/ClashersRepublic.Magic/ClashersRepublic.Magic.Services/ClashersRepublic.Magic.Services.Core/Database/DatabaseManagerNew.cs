namespace ClashersRepublic.Magic.Services.Core.Database
{
    using System.Collections.Concurrent;
    using System.Threading;

    public static class DatabaseManagerNew
    {
        private static Thread[] _insertThreads;
        private static Thread[] _updateThreads;
        private static ConcurrentQueue<Queue> _insertQueue;
        private static ConcurrentQueue<Queue> _updateQueue;
        
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize(int insertThread, int updateThread)
        {
            DatabaseManagerNew._insertQueue = new ConcurrentQueue<Queue>();
            DatabaseManagerNew._updateQueue = new ConcurrentQueue<Queue>();
            DatabaseManagerNew._insertThreads = new Thread[insertThread];
            DatabaseManagerNew._updateThreads = new Thread[updateThread];

            for (int i = 0; i < DatabaseManagerNew._insertThreads.Length; i++)
            {
                DatabaseManagerNew._insertThreads[i] = new Thread(() =>
                {
                    while (true)
                    {
                        if (DatabaseManagerNew._insertQueue.TryDequeue(out Queue item))
                        {
                            DatabaseManager.Insert(item.BucketIdx, item.Key, item.Json);
                        }

                        Thread.Sleep(1);
                    }
                });

                DatabaseManagerNew._insertThreads[i].Start();
            }

            for (int i = 0; i < DatabaseManagerNew._updateThreads.Length; i++)
            {
                DatabaseManagerNew._updateThreads[i] = new Thread(() =>
                {
                    while (true)
                    {
                        if (DatabaseManagerNew._updateQueue.TryDequeue(out Queue item))
                        {
                            DatabaseManager.Update(item.BucketIdx, item.Key, item.Json);
                        }

                        Thread.Sleep(1);
                    }
                });

                DatabaseManagerNew._updateThreads[i].Start();
            }
        }

        /// <summary>
        ///     Inserts the specified document.
        /// </summary>
        public static void Insert(int bucketIdx, long key, string json)
        {
            DatabaseManagerNew._insertQueue.Enqueue(new Queue
            {
                BucketIdx = bucketIdx,
                Key = key,
                Json = json
            });
        }

        /// <summary>
        ///     Updates the specified document.
        /// </summary>
        public static void Update(int bucketIdx, long key, string json)
        {
            DatabaseManagerNew._updateQueue.Enqueue(new Queue
            {
                BucketIdx = bucketIdx,
                Key = key,
                Json = json
            });
        }

        /// <summary>
        ///     Gets the number of database.
        /// </summary>
        public static int GetDatabaseCount(int bucketIdx)
        {
            return DatabaseManager.GetDatabaseCount(bucketIdx);
        }

        /// <summary>
        ///     Gets the database instance at the specified idx.
        /// </summary>
        public static CouchbaseDatabase GetDatabaseAt(int bucketIdx, int idx)
        {
            return DatabaseManager.GetDatabaseAt(bucketIdx, idx);
        }

        private struct Queue
        {
            internal int BucketIdx;
            internal long Key;
            internal string Json;
        }
    }
}