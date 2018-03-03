﻿namespace ClashersRepublic.Magic.Services.Account.Network.Session
{
    using System;
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Services.Account.Game;

    internal static class NetAccountSessionManager
    {
        private static ConcurrentDictionary<string, NetAccountSession> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetAccountSessionManager._sessions = new ConcurrentDictionary<string, NetAccountSession>();
        }
        
        /// <summary>
        ///     Creates a new <see cref="NetAccountSession"/> instance.
        /// </summary>
        internal static NetAccountSession TryCreate(Account account, byte[] sessionId)
        {
            string sessionName = BitConverter.ToString(sessionId).Replace("-", string.Empty).ToLower();

            NetAccountSession session = new NetAccountSession(account, sessionId, sessionName);

            if (NetAccountSessionManager._sessions.TryAdd(sessionName, session))
            {
                return session;
            }

            return null;
        }

        /// <summary>
        ///     Tries to get the <see cref="NetAccountSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(string sessionName, out NetAccountSession session)
        {
            return NetAccountSessionManager._sessions.TryGetValue(sessionName, out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetAccountSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(string sessionName, out NetAccountSession session)
        {
            return NetAccountSessionManager._sessions.TryRemove(sessionName, out session);
        }
    }
}