namespace ClashersRepublic.Magic.Services.Stream.Network.Session
{
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Stream.Game;

    internal static class NetStreamSessionManager
    {
        private static Dictionary<byte[], NetStreamSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetStreamSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetStreamSessionManager._sessions = new Dictionary<byte[], NetStreamSession>(new ByteArrayComparer());
        }

        /// <summary>
        ///     Creates a new session.
        /// </summary>
        internal static NetStreamSession Create(Stream stream, byte[] sessionId)
        {
            NetStreamSession session = new NetStreamSession(stream, sessionId);
            NetStreamSessionManager._sessions.Add(sessionId, session);
            return session;
        }

        /// <summary>
        ///     Tries to get the specified session.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetStreamSession session)
        {
            return NetStreamSessionManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Tries to remove the specified session.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetStreamSession session)
        {
            if (NetStreamSessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetStreamSessionManager._sessions.Remove(sessionId);
            }

            return false;
        }
    }
}