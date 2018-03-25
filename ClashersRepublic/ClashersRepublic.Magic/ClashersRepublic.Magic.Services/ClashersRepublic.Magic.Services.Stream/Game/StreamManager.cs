namespace ClashersRepublic.Magic.Services.Stream.Game
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    internal static class StreamManager
    {
        private static int[] _streamCounters;
        private static Dictionary<long, Stream> _streams;

        /// <summary>
        ///     Gets the total streams.
        /// </summary>
        internal static int TotalStreams
        {
            get
            {
                return StreamManager._streams.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            StreamManager._streamCounters = new int[DatabaseManager.GetDatabaseCount()];
            StreamManager._streams = new Dictionary<long, Stream>();
        }

        /// <summary>
        ///     Loads all homes from database.
        /// </summary>
        internal static void LoadStreams()
        {
            for (int i = 0; i < DatabaseManager.GetDatabaseCount(); i++)
            {
                CouchbaseDatabase database = DatabaseManager.GetDatabaseAt(i);

                if (database != null)
                {
                    int highId = i;
                    int maxLowId = database.GetHigherId();

                    object locker = new object();

                    Parallel.For(1, maxLowId + 1, new ParallelOptions { MaxDegreeOfParallelism = 4 }, id =>
                    {
                        if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, id) == ServiceCore.ServiceNodeId)
                        {
                            string json = database.GetDocument(new LogicLong(highId, id));

                            if (json != null)
                            {
                                Stream stream = new Stream();
                                stream.Load(json);

                                lock (locker)
                                {
                                    StreamManager._streams.Add(stream.Id, stream);
                                    StreamManager._streamCounters[highId] = LogicMath.Max(id, StreamManager._streamCounters[highId]);
                                }
                            }
                        }
                    });
                }
                else
                {
                    Logging.Warning("HomeManager::loadHomes pDatabase->NULL");
                }
            }
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="Stream"/> id.
        /// </summary>
        internal static bool TryGet(LogicLong avatarId, out Stream home)
        {
            return StreamManager._streams.TryGetValue(avatarId, out home);
        }

        /// <summary>
        ///     Create a new <see cref="Stream"/> instance.
        /// </summary>
        internal static Stream CreateStream(LogicLong homeId)
        {
            Stream stream = new Stream(homeId);
            DatabaseManager.Insert(homeId, LogicJSONParser.CreateJSONString(stream.Save()));
            StreamManager._streams.Add(homeId, stream);

            return stream;
        }
    }
}