namespace ClashersRepublic.Magic.Services.Proxy.Network.Session
{
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Utils;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class NetProxySessionManager
    {
        private static int _processId;
        private static long _sessionCounter;

        private static LogicRandom _random;
        private static ConcurrentDictionary<byte[], NetProxySession> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetProxySessionManager._processId = Process.GetCurrentProcess().Id;
            NetProxySessionManager._random = new LogicRandom(LogicTimeUtil.GetTimestamp());
            NetProxySessionManager._sessions = new ConcurrentDictionary<byte[], NetProxySession>(new ByteArrayComparer());
        }

        /// <summary>
        ///     Generates a random session id.
        /// </summary>
        internal static byte[] GenerateSessionId()
        {
            long sessionCounter = Interlocked.Increment(ref NetProxySessionManager._sessionCounter);
            int random = 0;

            byte[] sessionId = new byte[12];

            for (int i = 0; i < sessionId.Length; i++)
            {
                if (i % 4 == 0)
                {
                    random = NetProxySessionManager._random.Rand();
                }

                sessionId[i] = (byte) (random >>= 8);
            }

            sessionId[0] ^= (byte) (NetProxySessionManager._processId >> 16);
            sessionId[1] ^= (byte) (NetProxySessionManager._processId >> 8);
            sessionId[2] ^= (byte) (NetProxySessionManager._processId);
            sessionId[3] ^= (byte) (ServiceCore.ServiceNodeId);
            sessionId[4] ^= (byte) (sessionCounter >> 56);
            sessionId[5] ^= (byte) (sessionCounter >> 48);
            sessionId[6] ^= (byte) (sessionCounter >> 40);
            sessionId[7] ^= (byte) (sessionCounter >> 32);
            sessionId[8] ^= (byte) (sessionCounter >> 24);
            sessionId[9] ^= (byte) (sessionCounter >> 16);
            sessionId[10] ^= (byte) (sessionCounter >> 8);
            sessionId[11] ^= (byte) (sessionCounter);

            return sessionId;
        }

        /// <summary>
        ///     Creates a new <see cref="NetProxySession"/> instance.
        /// </summary>
        internal static NetProxySession Create(NetworkClient client)
        {
            byte[] sessionId = NetProxySessionManager.GenerateSessionId();

            NetProxySession session = new NetProxySession(client, sessionId);

            if (NetProxySessionManager._sessions.TryAdd(sessionId, session))
            {
                return session;
            }

            return null;
        }
        
        /// <summary>
        ///     Tries to get the <see cref="NetProxySession"/> instance with his id.
        /// </summary>
        internal static bool TryGet(byte[] sessionId, out NetProxySession session)
        {
            return NetProxySessionManager._sessions.TryGetValue(sessionId, out session);
        }
        
        /// <summary>
        ///     Tries to remove the <see cref="NetProxySession"/> instance associed with the specified id.
        /// </summary>
        internal static bool TryRemove(byte[] sessionId, out NetProxySession session)
        {
            return NetProxySessionManager._sessions.TryRemove(sessionId, out session);
        }
    }
}