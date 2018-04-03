namespace RivieraStudio.Magic.Services.Zone.Network.Session
{
    using System.Collections.Generic;

    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Services.Zone.Game;

    internal static class NetZoneSessionManager
    {
        private static Dictionary<byte[], NetZoneSession> _sessions;

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal static int TotalSessions
        {
            get
            {
                return NetZoneSessionManager._sessions.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetZoneSessionManager._sessions = new Dictionary<byte[], NetZoneSession>(new ByteArrayComparer());
        }
        
        /// <summary>
        ///     Creates a new <see cref="NetZoneSession"/> instance.
        /// </summary>
        internal static NetZoneSession Create(ZoneAccount home, byte[] sessionId)
        {
            NetZoneSession session = new NetZoneSession(home, sessionId);
            NetZoneSessionManager._sessions.Add(sessionId, session);
            return session;
        }
        
        /// <summary>
        ///     Tries to get the <see cref="NetZoneSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetZoneSession session)
        {
            return NetZoneSessionManager._sessions.TryGetValue(sessionId, out session);
        }
        
        /// <summary>
        ///     Tries to remove the <see cref="NetZoneSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetZoneSession session)
        {
            if (NetZoneSessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetZoneSessionManager._sessions.Remove(sessionId);
            }

            return false;
        }

        /// <summary>
        ///     Removes the <see cref="NetZoneSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool Remove(byte[] sessionId)
        {
            return NetZoneSessionManager._sessions.Remove(sessionId);
        }
    }
}