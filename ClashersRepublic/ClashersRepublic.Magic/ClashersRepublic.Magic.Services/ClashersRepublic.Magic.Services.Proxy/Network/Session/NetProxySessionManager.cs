namespace ClashersRepublic.Magic.Services.Proxy.Network.Session
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class NetProxySessionManager
    {
        private static int _processId;
        private static long _sessionCounter;

        private static LogicRandom _random;
        private static ConcurrentDictionary<string, NetProxySession> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetProxySessionManager._processId = Process.GetCurrentProcess().Id;
            NetProxySessionManager._random = new LogicRandom(LogicTimeUtil.GetTimestamp());
            NetProxySessionManager._sessions = new ConcurrentDictionary<string, NetProxySession>();
        }

        /// <summary>
        ///     Generates a random session id.
        /// </summary>
        internal static byte[] GenerateSessionId()
        {
            long sessionCounter = Interlocked.Increment(ref NetProxySessionManager._sessionCounter);
            int random = 0;

            byte[] sessionId = new byte[10];

            for (int i = 0; i < sessionId.Length; i++)
            {
                if (i % 4 == 0)
                {
                    random = NetProxySessionManager._random.Rand();
                }

                sessionId[i] = (byte)(random >>= 8);
            }

            sessionId[0] ^= (byte)(NetProxySessionManager._processId >> 8);
            sessionId[1] ^= (byte)(NetProxySessionManager._processId);
            sessionId[2] ^= (byte)(ServiceCore.ServiceNodeId);
            sessionId[3] ^= (byte)(sessionCounter >> 48);
            sessionId[4] ^= (byte)(sessionCounter >> 40);
            sessionId[5] ^= (byte)(sessionCounter >> 32);
            sessionId[6] ^= (byte)(sessionCounter >> 24);
            sessionId[7] ^= (byte)(sessionCounter >> 16);
            sessionId[8] ^= (byte)(sessionCounter >> 8);
            sessionId[9] ^= (byte)(sessionCounter);

            return sessionId;
        }

        /// <summary>
        ///     Converts the session id to session name.
        /// </summary>
        internal static string ConvertSessionIdToSessionName(byte[] sessionId)
        {
            return BitConverter.ToString(sessionId).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        ///     Creates a new <see cref="NetSession"/> instance.
        /// </summary>
        internal static NetProxySession TryCreate(NetworkClient client)
        {
            byte[] sessionId = NetProxySessionManager.GenerateSessionId();
            string sessionName = NetProxySessionManager.ConvertSessionIdToSessionName(sessionId);

            NetProxySession session = new NetProxySession(client, sessionId, sessionName);

            if (NetProxySessionManager._sessions.TryAdd(sessionName, session))
            {
                return session;
            }

            return null;
        }

        /// <summary>
        ///     Tries to get the <see cref="NetProxySession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(string sessionName, out NetProxySession session)
        {
            return NetProxySessionManager._sessions.TryGetValue(sessionName, out session);
        }

        /// <summary>
        ///     Tries to get the <see cref="NetProxySession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetProxySession session)
        {
            return NetProxySessionManager._sessions.TryGetValue(NetProxySessionManager.ConvertSessionIdToSessionName(sessionId), out session);
        }

        /// <summary>
        ///     Tries to remove the <see cref="NetProxySession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(string sessionName, out NetProxySession session)
        {
            return NetProxySessionManager._sessions.TryRemove(sessionName, out session);
        }
    }
}