namespace ClashersRepublic.Magic.Services.Home.Game
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Titan.Math;

    internal static class HomeManager
    {
        private static int[] _homeCounters;
        private static Dictionary<long, Home> _homes;

        /// <summary>
        ///     Gets the total homes.
        /// </summary>
        internal static int TotalHomes
        {
            get
            {
                return HomeManager._homes.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            HomeManager._homeCounters = new int[DatabaseManager.GetDatabaseCount()];
            HomeManager._homes = new Dictionary<long, Home>();
        }

        /// <summary>
        ///     Loads all homes from database.
        /// </summary>
        internal static void LoadHomes()
        {
            for (int i = 0; i < DatabaseManager.GetDatabaseCount(); i++)
            {
                CouchbaseDatabase database = DatabaseManager.GetDatabase(i);

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
                                Home home = new Home();
                                home.Load(json);

                                lock (locker)
                                {
                                    HomeManager._homes.Add(home.Id, home);
                                    HomeManager._homeCounters[highId] = LogicMath.Max(id, HomeManager._homeCounters[highId]);
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
        ///     Tries to get the instance of the specified <see cref="Home"/> id.
        /// </summary>
        internal static bool TryGet(LogicLong avatarId, out Home home)
        {
            return HomeManager._homes.TryGetValue(avatarId, out home);
        }
        
        /// <summary>
        ///     Create a new <see cref="Home"/> instance.
        /// </summary>
        internal static Home CreateHome(LogicLong homeId)
        {
            Home home = new Home(homeId);
            DatabaseManager.Insert(home);
            HomeManager._homes.Add(homeId, home);

            return home;
        }
    }
}