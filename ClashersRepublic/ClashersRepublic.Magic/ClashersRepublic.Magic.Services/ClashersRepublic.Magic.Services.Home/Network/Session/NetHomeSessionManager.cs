namespace ClashersRepublic.Magic.Services.Home.Network.Session
{
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Home.Game;

    internal static class NetHomeSessionManager
    {
        private static Dictionary<byte[], NetHomeSession> _sessions;

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
            NetHomeSessionManager._sessions = new Dictionary<byte[], NetHomeSession>(new ByteArrayComparer());
        }
        
        /// <summary>
        ///     Creates a new <see cref="NetHomeSession"/> instance.
        /// </summary>
        internal static NetHomeSession Create(Home home, byte[] sessionId)
        {
            NetHomeSession session = new NetHomeSession(home, sessionId);
            NetHomeSessionManager._sessions.Add(sessionId, session);
            return session;
        }
        
        /// <summary>
        ///     Tries to get the <see cref="NetHomeSession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetHomeSession session)
        {
            return NetHomeSessionManager._sessions.TryGetValue(sessionId, out session);
        }
        
        /// <summary>
        ///     Tries to remove the <see cref="NetHomeSession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetHomeSession session)
        {
            if (NetHomeSessionManager._sessions.TryGetValue(sessionId, out session))
            {
                return NetHomeSessionManager._sessions.Remove(sessionId);
            }

            return false;
        }
    }
}