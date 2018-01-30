namespace ClashersRepublic.Magic.Services.Home.Game
{
    using System;
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Logic;

    using ClashersRepublic.Magic.Titan.Math;

    internal static class GameManager
    {
        private static bool _initialized;
        
        private static ConcurrentDictionary<long, GamePlayer> _players;
        private static ConcurrentDictionary<string, GamePlayer> _sessions;
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (GameManager._initialized)
            {
                return;
            }

            GameManager._initialized = true;
            
            GameManager._players = new ConcurrentDictionary<long, GamePlayer>();
            GameManager._sessions = new ConcurrentDictionary<string, GamePlayer>();

            GameManager.LoadPlayersFromDB();
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadPlayersFromDB()
        {
            int lastLowId = GameDatabase.GetHigherLowId();

            for (int i = 1; i <= lastLowId; i++)
            {
                GameManager.GetPlayer(new LogicLong(Config.ServerId, i));
            }

            Console.WriteLine("In-Memory Players : " + GameManager._players.Count);
        }
        
        /// <summary>
        ///     Creates a new account.
        /// </summary>
        internal static GamePlayer CreatePlayer(LogicLong accountId)
        {
            GamePlayer player = new GamePlayer(accountId);

            if (!GameManager._players.TryAdd(accountId, player))
            {
                Logging.Error(typeof(GameManager), "CreatePlayer account id " + accountId + " already exist");
                return null;
            }

            GameDatabase.InsertPlayer(player);

            return player;
        }

        /// <summary>
        ///     Creates a new session.
        /// </summary>
        internal static void CreateSession(string sessionId, LogicLong accountId)
        {
            if (!GameManager._sessions.ContainsKey(sessionId))
            {
                if (GameManager._players.TryGetValue(accountId, out GamePlayer player))
                {
                    if (player.CurrentSessionId != null)
                    {
                        GameManager.RemoveSession(player.CurrentSessionId);
                    }
                    
                    if (GameManager._sessions.TryAdd(sessionId, player))
                    {
                        player.CurrentSessionId = sessionId;
                    }
                    else
                    {
                        Logging.Error(typeof(GamePlayer), "CreateSession unable to add session id " + sessionId);
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the specified session.
        /// </summary>
        internal static void RemoveSession(string sessionId)
        {
            if (GameManager._sessions.TryRemove(sessionId, out GamePlayer player))
            {
                player.CurrentSessionId = null;
            }
        }

        /// <summary>
        ///     Gets the player instance by id.
        /// </summary>
        internal static GamePlayer GetPlayer(LogicLong accountId)
        {
            if (!GameManager._players.TryGetValue(accountId, out GamePlayer player))
            {
                player = GameManager.LoadPlayerFromDB(accountId);

                if (player != null)
                {
                    if (!GameManager._players.TryAdd(accountId, player))
                    {
                        Logging.Error(typeof(GameManager), "GetPlayer account id " + accountId + " already added");
                    }
                }
            }

            return player;
        }

        /// <summary>
        ///     Gets the player instance by session id.
        /// </summary>
        internal static GamePlayer GetPlayer(string sessionId)
        {
            return GameManager._sessions.TryGetValue(sessionId, out GamePlayer account) ? account : null;
        }

        /// <summary>
        ///     Loads the player from database.
        /// </summary>
        internal static GamePlayer LoadPlayerFromDB(LogicLong accountId)
        {
            GamePlayer player = GameDatabase.LoadPlayer(accountId);

            if (player == null)
            {
                Logging.Warning(typeof(GameManager), "LoadPlayerFromDB account id " + accountId + " not exist");
            }

            return player;
        }
    }
}