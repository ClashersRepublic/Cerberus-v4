namespace ClashersRepublic.Magic.Services.Home.Network.Session
{
    using System;
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Services.Home.Game;

    internal static class NetHomeSessionManager
    {
        private static ConcurrentDictionary<string, NetHomeSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetHomeSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetHomeSessionManager._sessions = new ConcurrentDictionary<string, NetHomeSession>();
        }

        /// <summary>
        ///     Converts the session id to session name.
        /// </summary>
        internal static string ConvertSessionIdToSessionName(byte[] sessionId)
        {
            return BitConverter.ToString(sessionId).Replace("-", string.Empty).ToLower();
        }
        
        /// <summary>
        ///     Creates a new <see cref="NetHomeSession"/> instance.
        /// </summary>
        internal static NetHomeSession TryCreate(Home home, byte[] sessionId)
        {
            string sessionName = NetHomeSessionManager.ConvertSessionIdToSessionName(sessionId);

            NetHomeSession session = new NetHomeSession(home, sessionId, sessionName);

            if (NetHomeSessionManager._sessions.TryAdd(sessionName, session))
            {
                return session;
            }

            return null;
        }

        /// <summary>
        ///     Tries to get the <see cref="NetHomeSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(string sessionName, out NetHomeSession session)
        {
            return NetHomeSessionManager._sessions.TryGetValue(sessionName, out session);
        }

        /// <summary>
        ///     Tries to get the <see cref="NetHomeSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetHomeSession session)
        {
            return NetHomeSessionManager._sessions.TryGetValue(NetHomeSessionManager.ConvertSessionIdToSessionName(sessionId), out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetHomeSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(string sessionName, out NetHomeSession session)
        {
            return NetHomeSessionManager._sessions.TryRemove(sessionName, out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetHomeSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetHomeSession session)
        {
            return NetHomeSessionManager._sessions.TryRemove(NetHomeSessionManager.ConvertSessionIdToSessionName(sessionId), out session);
        }
    }
}