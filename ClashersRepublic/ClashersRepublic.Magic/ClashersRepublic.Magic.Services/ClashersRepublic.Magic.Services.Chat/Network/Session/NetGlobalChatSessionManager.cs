namespace ClashersRepublic.Magic.Services.Chat.Network.Session
{
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Core.Utils;

    internal static class NetGlobalChatSessionManager
    {
        private static Dictionary<byte[], NetGlobalChatSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetGlobalChatSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetGlobalChatSessionManager._sessions = new Dictionary<byte[], NetGlobalChatSession>(new ByteArrayComparer());
        }

        /// <summary>
        ///     Creates a new session.
        /// </summary>
        internal static NetGlobalChatSession Create(byte[] sessionId)
        {
            NetGlobalChatSession session = new NetGlobalChatSession(sessionId);
            NetGlobalChatSessionManager._sessions.Add(sessionId, session);
            return session;
        }

        /// <summary>
        ///     Tries to get the specified session.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetGlobalChatSession session)
        {
            return NetGlobalChatSessionManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Tries to remove the specified session.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetGlobalChatSession session)
        {
            if (NetGlobalChatSessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetGlobalChatSessionManager._sessions.Remove(sessionId);
            }

            return false;
        }

        /// <summary>
        ///     Remove the specified session.
        /// </summary>
        internal static bool Remove(byte[] sessionId)
        {
            return NetGlobalChatSessionManager._sessions.Remove(sessionId);
        }
    }
}