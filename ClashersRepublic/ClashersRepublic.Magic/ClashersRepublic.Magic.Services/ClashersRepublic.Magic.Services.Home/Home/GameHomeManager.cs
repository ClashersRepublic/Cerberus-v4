namespace ClashersRepublic.Magic.Services.Home.Home
{
    using System.Collections.Generic;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Log;

    using ClashersRepublic.Magic.Titan.Math;

    internal static class GameHomeManager
    {
        private static Thread _updateThread;
        private static Dictionary<long, GameHome> _homes;

        /// <summary>
        ///     Gets the total homes.
        /// </summary>
        internal static int TotalHomes
        {
            get
            {
                if (GameHomeManager._homes != null)
                {
                    return GameHomeManager._homes.Count;
                }

                return 0;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameHomeManager._homes = new Dictionary<long, GameHome>(100000);
            GameHomeManager._updateThread = new Thread(GameHomeManager.Update);
            GameHomeManager._updateThread.Start();
            GameHomeManager.LoadAllDatas();
        }

        /// <summary>
        ///     Loads all datas.
        /// </summary>
        private static void LoadAllDatas()
        {
            GameDatabase database = GameDatabaseManager.GetDatabase(Config.ServerId);

            if (database == null)
            {
                Logging.Error(typeof(GameHomeManager), "GameHomeManager::loadAllDatas database doesn't exist");
            }
            else
            {
                int counters = database.GetHigherId();

                for (int i = 1; i <= counters; i++)
                {
                    GameHome gameHome = database.GetAccount(i);

                    if (gameHome != null)
                    {
                        gameHome.ReloadDatas();

                        if (!GameHomeManager.TryAdd(new LogicLong(Config.ServerId, i), gameHome))
                        {
                            Logging.Error(typeof(GameHomeManager), "GameHomeManager::loadAllDatas game home already exist");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Try to add the specified <see cref="GameHome"/> instance.
        /// </summary>
        private static bool TryAdd(long id, GameHome gameHome)
        {
            bool success = !GameHomeManager._homes.ContainsKey(id);

            if (success)
            {
                GameHomeManager._homes.Add(id, gameHome);
            }

            return success;
        }

        /// <summary>
        ///     Try to get the specified <see cref="GameHome"/> instance.
        /// </summary>
        private static bool TryGet(long id, out GameHome home)
        {
            bool success = GameHomeManager._homes.ContainsKey(id);
            home = success ? GameHomeManager._homes[id] : null;
            return success;
        }

        /// <summary>
        ///     Try to update the specified <see cref="GameHome"/> instance.
        /// </summary>
        private static bool TryUpdate(long id, GameHome newGameHome)
        {
            bool success = GameHomeManager._homes.ContainsKey(id);

            if (success)
            {
                GameHomeManager._homes[id] = newGameHome;
            }

            return success;
        }

        /// <summary>
        ///     Try to remove the specified <see cref="GameHome"/> instance.
        /// </summary>
        private static bool TryRemove(long id)
        {
            return GameHomeManager._homes.Remove(id);
        }

        /// <summary>
        ///     Creates a new <see cref="GameHome"/> instance.
        /// </summary>
        internal static GameHome CreateGameHome(LogicLong id)
        {
            if (id.GetHigherInt() == Config.ServerId)
            {
                GameHome gameHome = GameHome.GetDefaultGameHome(id);

                if (GameHomeManager.TryAdd(id, gameHome))
                {
                    GameDatabaseManager.GetDatabase(id.GetHigherInt()).InsertHome(gameHome);
                    return gameHome;
                }

                Logging.Error(typeof(GameHomeManager), "GameHomeManager::createGameHome cannot create a new game home, id: " + id);
            }
            else
            {
                Logging.Error(typeof(GameHomeManager), "GameHomeManager::createGameHome server id mismatch");
            }

            return null;
        }

        /// <summary>
        ///     Gets the specified <see cref="GameHome"/> instance.
        /// </summary>
        internal static GameHome GetGameHome(LogicLong id)
        {
            if (id.GetHigherInt() == Config.ServerId)
            {
                if (!GameHomeManager.TryGet(id, out GameHome gameHome))
                {
                    Logging.Debug(typeof(GameHomeManager), "GameHomeManager::getGameHome cannot get the game home id " + id);
                }

                return gameHome;
            }
            else
            {
                Logging.Error(typeof(GameHomeManager), "GameHomeManager::getGameHome server id mismatch");
            }

            return null;
        }

        /// <summary>
        ///     Updates the specified <see cref="GameHome"/> instance.
        /// </summary>
        internal static bool UpdateGameHome(LogicLong id, GameHome newInstance)
        {
            if (id.GetHigherInt() == Config.ServerId)
            {
                if (newInstance.Id == id)
                {
                    return GameHomeManager.TryUpdate(id, newInstance);
                }

                Logging.Error(typeof(GameHomeManager), "GameHomeManager::updateGameHome id mismatch");
            }
            else
            {
                Logging.Error(typeof(GameHomeManager), "GameHomeManager::updateGameHome server id mismatch");
            }

            return false;
        }

        /// <summary>
        ///     Removes the specified <see cref="GameHome"/> instance.
        /// </summary>
        internal static bool RemoveGameHome(LogicLong id)
        {
            Logging.Debug(typeof(GameHomeManager), "Trying to remove game home instance");

            if (id.GetHigherInt() == Config.ServerId)
            {
                return GameHomeManager.TryRemove(id);
            }

            Logging.Error(typeof(GameHomeManager), "GameHomeManager::removeGameHome server id mismatch");

            return false;
        }

        /// <summary>
        ///     Task for the update thread.
        /// </summary>
        private static void Update()
        {
            while (true)
            {
                Program.UpdateConsoleTitle();
                Thread.Sleep(16); // 1T
            }
        }
    }
}