namespace ClashersRepublic.Magic.Services.Home.Sessions
{
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Home.Home;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Log;

    using ClashersRepublic.Magic.Titan.Math;

    internal static class GameSessionManager
    {
        private static Dictionary<string, GameSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSession
        {
            get
            {
                if (GameSessionManager._sessions != null)
                {
                    return GameSessionManager._sessions.Count;
                }

                return 0;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameSessionManager._sessions = new Dictionary<string, GameSession>(40000);
        }

        /// <summary>
        ///     Try to add the specified <see cref="GameSession"/> instance.
        /// </summary>
        private static bool TryAdd(string id, GameSession gameSession)
        {
            bool success = !GameSessionManager._sessions.ContainsKey(id);

            if (success)
            {
                GameSessionManager._sessions.Add(id, gameSession);
            }

            return success;
        }

        /// <summary>
        ///     Try to get the specified <see cref="GameSession"/> instance.
        /// </summary>
        private static bool TryGet(string id, out GameSession gameSession)
        {
            bool success = GameSessionManager._sessions.ContainsKey(id);
            gameSession = success ? GameSessionManager._sessions[id] : null;
            return success;
        }

        /// <summary>
        ///     Try to update the specified <see cref="GameSession"/> instance.
        /// </summary>
        private static bool TryUpdate(string id, GameSession newGameSession)
        {
            bool success = GameSessionManager._sessions.ContainsKey(id);

            if (success)
            {
                GameSessionManager._sessions[id] = newGameSession;
            }

            return success;
        }

        /// <summary>
        ///     Try to remove the specified <see cref="GameSession"/> instance.
        /// </summary>
        private static bool TryRemove(string id)
        {
            return GameSessionManager._sessions.Remove(id);
        }

        /// <summary>
        ///     Creates a new <see cref="GameSession"/> instance.
        /// </summary>
        internal static GameSession CreateGameSession(string sessionId, LogicLong homeId)
        {
            GameHome gameHome = GameHomeManager.GetGameHome(homeId);

            if (gameHome != null)
            {
                GameSession session = new GameSession(sessionId, gameHome);

                if (GameSessionManager.TryAdd(sessionId, session))
                {
                    return session;
                }

                Logging.Error(typeof(GameSessionManager), "GameSessionManager::createGameSession cannot create a new game session id " + sessionId);
            }
            else
            {
                Logging.Error(typeof(GameSessionManager), "GameSessionManager::createGameSession null game home");
            }

            return null;
        }

        /// <summary>
        ///     Gets the specified <see cref="GameSession"/> instance.
        /// </summary>
        internal static GameSession GetGameSession(string sessionId)
        {
            if (!GameSessionManager.TryGet(sessionId, out GameSession gameSession))
            {
                // Logging.Warning(typeof(GameSessionManager), "GameSessionManager::getGameSession cannot get the game session id " + sessionId);
            }

            return gameSession;
        }

        /// <summary>
        ///     Updates the specified <see cref="GameSession"/> instance.
        /// </summary>
        internal static bool UpdateGameSession(string sessionId, GameSession newGameSession)
        {
            return GameSessionManager.TryUpdate(sessionId, newGameSession);
        }

        /// <summary>
        ///     Removes the specified <see cref="GameSession"/> instance.
        /// </summary>
        internal static bool RemoveGameSession(string sessionId)
        {
            return GameSessionManager.TryRemove(sessionId);
        }

        /// <summary>
        ///     Called when a service node has been bouded to session.
        /// </summary>
        internal static void ServiceNodeBoundToSession(string sessionId, LogicLong accountId)
        {
            if (!GameSessionManager._sessions.TryGetValue(sessionId, out _))
            {
                if (accountId.GetHigherInt() == Config.ServerId)
                {
                    GameSession gameSession = GameSessionManager.CreateGameSession(sessionId, accountId);

                    if (gameSession == null)
                    {
                        Logging.Warning(typeof(GameSessionManager), "GameSessionManager::serviceNodeBoundToSession session creation error");
                    }
                }
                else
                {
                    Logging.Warning(typeof(GameSessionManager), "GameSessionManager::serviceNodeBoundToSession server id mismatch");
                }
            }
            else
            {
                Logging.Warning(typeof(GameSessionManager), "GameSessionManager::serviceNodeBoundToSession service node already bound to this session id " + sessionId);
            }
        }
    }
}