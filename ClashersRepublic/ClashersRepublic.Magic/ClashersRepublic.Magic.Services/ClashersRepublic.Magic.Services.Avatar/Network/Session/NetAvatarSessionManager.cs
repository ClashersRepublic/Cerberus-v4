namespace ClashersRepublic.Magic.Services.Avatar.Network.Session
{
    using System;
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Services.Avatar.Game;

    internal static class NetAvatarSessionManager
    {
        private static ConcurrentDictionary<string, NetAvatarSession> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetAvatarSessionManager._sessions = new ConcurrentDictionary<string, NetAvatarSession>();
        }

        /// <summary>
        ///     Converts the session id to session name.
        /// </summary>
        internal static string ConvertSessionIdToSessionName(byte[] sessionId)
        {
            return BitConverter.ToString(sessionId).Replace("-", string.Empty).ToLower();
        }
        
        /// <summary>
        ///     Creates a new <see cref="NetAvatarSession"/> instance.
        /// </summary>
        internal static NetAvatarSession TryCreate(Avatar avatar, byte[] sessionId)
        {
            string sessionName = NetAvatarSessionManager.ConvertSessionIdToSessionName(sessionId);

            NetAvatarSession session = new NetAvatarSession(avatar, sessionId, sessionName);

            if (NetAvatarSessionManager._sessions.TryAdd(sessionName, session))
            {
                return session;
            }

            return null;
        }

        /// <summary>
        ///     Tries to get the <see cref="NetAvatarSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(string sessionName, out NetAvatarSession session)
        {
            return NetAvatarSessionManager._sessions.TryGetValue(sessionName, out session);
        }

        /// <summary>
        ///     Tries to get the <see cref="NetAvatarSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetAvatarSession session)
        {
            return NetAvatarSessionManager._sessions.TryGetValue(NetAvatarSessionManager.ConvertSessionIdToSessionName(sessionId), out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetAvatarSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(string sessionName, out NetAvatarSession session)
        {
            return NetAvatarSessionManager._sessions.TryRemove(sessionName, out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetAvatarSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetAvatarSession session)
        {
            return NetAvatarSessionManager._sessions.TryRemove(NetAvatarSessionManager.ConvertSessionIdToSessionName(sessionId), out session);
        }
    }
}