namespace ClashersRepublic.Magic.Services.Home.Game
{
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;

    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    internal static class HomeManager
    {
        private static int[] _homeCounters;
        private static ConcurrentDictionary<long, Home> _homes;

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
            HomeManager._homes = new ConcurrentDictionary<long, Home>();
        }

        /// <summary>
        ///     Loads all homes from database.
        /// </summary>
        internal static void LoadHomes()
        {
            for (int highId = 0; highId < DatabaseManager.GetDatabaseCount(); highId++)
            {
                IDatabase database = DatabaseManager.GetDatabase(highId);

                if (database != null)
                {
                    int maxLowId = database.GetHigherId();
                    
                    Parallel.For(1, maxLowId + 1, new ParallelOptions { MaxDegreeOfParallelism = 3 }, id =>
                    {
                        string json = database.GetDocument(new LogicLong(highId, id));

                        if (json != null)
                        {
                            Home home = new Home();
                            home.Load(json);
                            HomeManager.TryAdd(home);

                            HomeManager._homeCounters[highId] = id;
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
        ///     Tries to add the specified <see cref="Home"/> instance.
        /// </summary>
        private static bool TryAdd(Home home)
        {
            return HomeManager._homes.TryAdd(home.Id, home);
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="Home"/> id.
        /// </summary>
        private static bool TryGet(LogicLong avatarId, out Home home)
        {
            return HomeManager._homes.TryGetValue(avatarId, out home);
        }

        /// <summary>
        ///     Tries to remove the specified <see cref="Home"/> instance.
        /// </summary>
        private static bool TryRemove(LogicLong avatarId, out Home home)
        {
            return HomeManager._homes.TryRemove(avatarId, out home);
        }
        
        /// <summary>
        ///     Tries to create a new <see cref="Home"/> instance.
        /// </summary>
        internal static bool TryCreateHome(LogicLong homeId, out Home home)
        {
            IDatabase database = DatabaseManager.GetDatabase(homeId.GetHigherInt());

            if (database != null)
            {
                home = new Home(homeId);

                bool success = HomeManager.TryAdd(home);

                if (success)
                {
                    database.InsertDocument(homeId, LogicJSONParser.CreateJSONString(home.Save()));
                }

                return success;
            }

            home = null;

            return false;
        }

        /// <summary>
        ///     Tries to get the <see cref="Home"/> instance with id.
        /// </summary>
        internal static bool TryGetHome(LogicLong avatarId, out Home home)
        {
            return HomeManager.TryGet(avatarId, out home);
        }
    }
}