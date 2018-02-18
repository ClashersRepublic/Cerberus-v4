namespace ClashersRepublic.Magic.Proxy.Session
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;

    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Log;
    using ClashersRepublic.Magic.Proxy.User;

    using ClashersRepublic.Magic.Services.Logic;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class GameSessionManager
    {
        private static int _processId;
        private static int _sessionCounter;

        private static LogicRandom _rndSession;
        private static ConcurrentDictionary<string, GameSession> _sessions;

        /// <summary>
        ///     Gets the total sessions in memory.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return GameSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameSessionManager._sessions = new ConcurrentDictionary<string, GameSession>();
            
            GameSessionManager._processId = Process.GetCurrentProcess().Id;
            GameSessionManager._sessionCounter = Resources.Random.NextInt() & 0x7FFFFFFF;
            GameSessionManager._rndSession = new LogicRandom(GameSessionManager._sessionCounter);
        }

        /// <summary>
        ///     Generates a session id.
        /// </summary>
        internal static string GenerateSessionId()
        {
            int sessionCounter = GameSessionManager._sessionCounter++;
            int timestamp = LogicTimeUtil.GetTimestamp();
            int rnd = GameSessionManager._rndSession.Rand(0x1000000);

            byte[] sessionId = new byte[12];
            
            sessionId[0] = (byte) (Config.ServerId);
            sessionId[1] = (byte) (GameSessionManager._processId >> 8);
            sessionId[2] = (byte) (GameSessionManager._processId);
            sessionId[3] = (byte) (timestamp >> 8);
            sessionId[4] = (byte) (timestamp);
            sessionId[5] = (byte) (sessionCounter >> 24);
            sessionId[6] = (byte) (sessionCounter >> 16);
            sessionId[7] = (byte) (sessionCounter >> 8);
            sessionId[8] = (byte) (sessionCounter);
            sessionId[9] = (byte) (rnd >> 16);
            sessionId[10] = (byte) (rnd >> 8);
            sessionId[11] = (byte) (rnd);

            return BitConverter.ToString(sessionId).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        ///     Creates a new session for specified client.
        /// </summary>
        internal static void CreateSession(Client client, GameAccount account)
        {
            if (client.GameSession == null)
            {
                GameSession session = new GameSession(GameSessionManager.GenerateSessionId(), client, account);

                if (GameSessionManager._sessions.TryAdd(session.SessionId, session))
                {
                    client.GameSession = session;
                    client.NetworkToken.Messaging.MessageManager.SendLoginOkMessage(account);
                    
                    session.SetServerIDs(10, account.HighId);
                    session.SetServerIDs(9, account.HighId);

                    account.SetSession(session);
                }
            }
            else
            {
                Logging.Warning(typeof(GameSessionManager), "GameSessionManager::createSession session already created");
            }
        }

        /// <summary>
        ///     Gets a session by id.
        /// </summary>
        internal static GameSession GetSession(string sessionId)
        {
            return GameSessionManager._sessions.TryGetValue(sessionId, out GameSession session) ? session : null;
        }

        /// <summary>
        ///     Gets a session by id.
        /// </summary>
        internal static bool GetSession(string sessionId, out GameSession session)
        {
            return GameSessionManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Closes the game session.
        /// </summary>
        internal static void CloseSession(GameSession session)
        {
            if (GameSessionManager._sessions.TryRemove(session.SessionId, out _))
            {
                session.ClearServerIDs();
                session.Account.SetSession(null);
            }
            else
            {
                Logging.Warning(typeof(GameSession), "GameSessionManager::closeSession session already closed");
            }
        }
    }
}