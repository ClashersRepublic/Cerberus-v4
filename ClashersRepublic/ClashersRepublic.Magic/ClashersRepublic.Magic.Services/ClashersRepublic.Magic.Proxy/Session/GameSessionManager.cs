namespace ClashersRepublic.Magic.Proxy.Session
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using ClashersRepublic.Magic.Proxy.User;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class GameSessionManager
    {
        private static int _processId;
        private static long _sessionNum;
        
        private static ConcurrentDictionary<string, GameSession> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameSessionManager._sessions = new ConcurrentDictionary<string, GameSession>();

            GameSessionManager._processId = Process.GetCurrentProcess().Id;
            GameSessionManager._sessionNum = new LogicLong(Resources.Random.NextInt() % 256, Resources.Random.NextInt() & 0x7FFFFFFF);
        }

        /// <summary>
        ///     Generates a session id.
        /// </summary>
        internal static string GenerateSessionId()
        {
            int timestamp = LogicTimeUtil.GetTimestamp();
            long sessionNumber = GameSessionManager._sessionNum++;
            byte[] sessionId = new byte[16];

            sessionId[0] = (byte) (GameSessionManager._processId >> 16);
            sessionId[1] = (byte) (timestamp);
            sessionId[2] = (byte) (timestamp >> 8);
            sessionId[3] = (byte) (GameSessionManager._processId >> 24);
            sessionId[4] = (byte) (sessionNumber >> 40);
            sessionId[5] = (byte) (sessionNumber >> 24);
            sessionId[6] = (byte) (sessionNumber >> 16);
            sessionId[7] = (byte) (sessionNumber >> 48);
            sessionId[8] = (byte) (timestamp >> 16);
            sessionId[9] = (byte) (GameSessionManager._processId >> 8);
            sessionId[10] = (byte) (sessionNumber >> 8);
            sessionId[11] = (byte) (Config.ServerId);
            sessionId[12] = (byte) (sessionNumber);
            sessionId[13] = (byte) (sessionNumber >> 32);
            sessionId[14] = (byte) (GameSessionManager._processId);
            sessionId[15] = (byte) (sessionNumber >> 56);

            return BitConverter.ToString(sessionId).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        ///     Creates a new session for specified client.
        /// </summary>
        internal static void CreateSession(Client client)
        {
            if (client.GameSession == null)
            {
                if (client.GameAccount != null)
                {
                    client.GameSession = new GameSession();
                    client.GameSession.SetData(GameSessionManager.GenerateSessionId(), client);
                }
            }
        }
    }
}