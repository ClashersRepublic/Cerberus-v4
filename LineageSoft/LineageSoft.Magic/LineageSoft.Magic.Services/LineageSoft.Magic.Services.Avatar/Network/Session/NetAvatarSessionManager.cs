namespace LineageSoft.Magic.Services.Avatar.Network.Session
{
    using System.Collections.Generic;

    using LineageSoft.Magic.Services.Core.Utils;
    using LineageSoft.Magic.Services.Avatar.Game;

    internal static class NetAvatarSessionManager
    {
        private static Dictionary<byte[], NetAvatarSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetAvatarSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetAvatarSessionManager._sessions = new Dictionary<byte[], NetAvatarSession>(new ByteArrayComparer());
        }

        /// <summary>
        ///     Creates a new session.
        /// </summary>
        internal static NetAvatarSession Create(AvatarAccount avatarAccount, byte[] sessionId)
        {
            NetAvatarSession session = new NetAvatarSession(avatarAccount, sessionId);
            NetAvatarSessionManager._sessions.Add(sessionId, session);
            return session;
        }

        /// <summary>
        ///     Tries to get the specified session.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetAvatarSession session)
        {
            return NetAvatarSessionManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Tries to remove the specified session.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetAvatarSession session)
        {
            if (NetAvatarSessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetAvatarSessionManager._sessions.Remove(sessionId);
            }

            return false;
        }

        /// <summary>
        ///     Remove the specified session.
        /// </summary>
        internal static bool Remove(byte[] sessionId)
        {
            return NetAvatarSessionManager._sessions.Remove(sessionId);
        }
    }
}