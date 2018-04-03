namespace LineageSoft.Magic.Services.Account.Network.Session
{
    using System.Collections.Generic;

    using LineageSoft.Magic.Services.Account.Game;
    using LineageSoft.Magic.Services.Core.Utils;

    internal static class NetAccountSessionManager
    {
        private static Dictionary<byte[], NetAccountSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetAccountSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetAccountSessionManager._sessions = new Dictionary<byte[], NetAccountSession>(new ByteArrayComparer());
        }

        /// <summary>
        ///     Creates a new <see cref="NetAccountSession"/> instance.
        /// </summary>
        internal static NetAccountSession Create(Account account, byte[] sessionId)
        {
            NetAccountSession session = new NetAccountSession(account, sessionId);
            NetAccountSessionManager._sessions.Add(sessionId, session);
            return session;
        }
        
        /// <summary>
        ///     Tries to get the <see cref="NetAccountSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetAccountSession session)
        {
            return NetAccountSessionManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetAccountSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetAccountSession session)
        {
            if (NetAccountSessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetAccountSessionManager._sessions.Remove(sessionId);
            }

            return false;
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetAccountSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId)
        {
            return NetAccountSessionManager._sessions.Remove(sessionId);
        }
    }
}