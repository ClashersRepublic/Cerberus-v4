namespace LineageSoft.Magic.Services.Party.Network.Session
{
    using System.Collections.Generic;

    using LineageSoft.Magic.Services.Core.Utils;
    using LineageSoft.Magic.Services.Party.Game;

    internal static class NetPartySessionManager
    {
        private static Dictionary<byte[], NetPartySession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetPartySessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetPartySessionManager._sessions = new Dictionary<byte[], NetPartySession>(new ByteArrayComparer());
        }

        /// <summary>
        ///     Creates a new session.
        /// </summary>
        internal static NetPartySession Create(PartyAccount partyAccount, byte[] sessionId)
        {
            NetPartySession session = new NetPartySession(partyAccount, sessionId);
            NetPartySessionManager._sessions.Add(sessionId, session);
            return session;
        }

        /// <summary>
        ///     Tries to get the specified session.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetPartySession session)
        {
            return NetPartySessionManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Tries to remove the specified session.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetPartySession session)
        {
            if (NetPartySessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetPartySessionManager._sessions.Remove(sessionId);
            }

            return false;
        }

        /// <summary>
        ///     Remove the specified session.
        /// </summary>
        internal static bool Remove(byte[] sessionId)
        {
            return NetPartySessionManager._sessions.Remove(sessionId);
        }
    }
}