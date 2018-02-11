namespace ClashersRepublic.Magic.Services.Home.Session
{
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Message.Home;

    using ClashersRepublic.Magic.Services.Home.Player;
    using ClashersRepublic.Magic.Services.Home.Service;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal class GameSessionManager
    {
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
        }

        /// <summary>
        ///     Called when a client connects to the server
        /// </summary>
        internal static void OnClientConnected(int serverId, string sessionId, LogicLong accountId, bool isNewClient)
        {
            GamePlayer player = GamePlayerManager.GetPlayer(accountId, isNewClient);

            if (player != null)
            {
                if (player.CurrentSessionId != null)
                {
                    GameSessionManager.CloseSession(player.CurrentSessionId);
                }

                GameSession session = new GameSession(serverId, sessionId, player);

                if (GameSessionManager._sessions.TryAdd(sessionId, session))
                {
                    GameSessionManager.SendGameData(session);
                }
            }
        }

        /// <summary>
        ///     Called when the client is disconnected from server.
        /// </summary>
        internal static void OnClientDisconnected(string sessionId)
        {
            if (GameSessionManager._sessions.TryRemove(sessionId, out GameSession session))
            {
                GameSessionManager.OnSessionRemoved(session);
            }
        }

        /// <summary>
        ///     Called when a session has been removed.
        /// </summary>
        internal static void OnSessionRemoved(GameSession session)
        {
            LogicAvatar avatar = session.LogicGameMode.GetLevel().GetHomeOwnerAvatar();

            if (avatar != null)
            {
                session.LogicGameMode.GetLevel().GetHomeOwnerAvatar().SetLevel(null);

                if (session.LogicGameMode.GetLevel().GetHome() != null)
                {
                    GamePlayerManager.SavePlayer(session.Player, session.LogicGameMode);
                }
            }
        }

        /// <summary>
        ///     Forces the closure of the session.
        /// </summary>
        internal static void CloseSession(string sessionId)
        {
            if (GameSessionManager._sessions.TryRemove(sessionId, out GameSession session))
            {
                session.SendToProxy(new SessionClosedMessage());
                GameSessionManager.OnSessionRemoved(session);
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
        ///     Sends the game data to client.
        /// </summary>
        internal static void SendGameData(GameSession session)
        {
            GamePlayer player = session.Player;
            LogicClientHome logicClientHome = player.LogicClientHome;
            LogicClientAvatar logicClientAvatar = player.LogicClientAvatar;

            logicClientHome.SetGlobalJSON("{}");
            logicClientHome.SetEventJSON("{}");

            int currentTimestamp = LogicTimeUtil.GetTimestamp();
            int secondsSinceLastSave = currentTimestamp - player.LastSaveTime;

            session.LogicGameMode.LoadHomeState(logicClientHome, logicClientAvatar, secondsSinceLastSave, currentTimestamp);
            session.SendToProxy(new OwnHomeDataMessage
            {
                LogicClientAvatar = logicClientAvatar,
                LogicClientHome = logicClientHome,
                SecondsSinceLastSave = secondsSinceLastSave,
                CurrentTimestamp = currentTimestamp
            });
        }
    }
}