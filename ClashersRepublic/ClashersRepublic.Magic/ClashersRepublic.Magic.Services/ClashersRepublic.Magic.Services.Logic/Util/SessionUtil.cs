namespace ClashersRepublic.Magic.Services.Logic.Util
{
    using System;

    public static class SessionUtil
    {
        /// <summary>
        ///     Creates a new session id.
        /// </summary>
        public static string CreateSessionId(int proxyId, long connectionId)
        {
            byte[] tmp = new byte[12];

            tmp[0] = (byte) (proxyId >> 24);
            tmp[1] = (byte) (proxyId >> 16);
            tmp[2] = (byte) (proxyId >> 8);
            tmp[3] = (byte) (proxyId);

            tmp[4] = (byte) (connectionId >> 56);
            tmp[5] = (byte) (connectionId >> 48);
            tmp[6] = (byte) (connectionId >> 40);
            tmp[7] = (byte) (connectionId >> 32);
            tmp[8] = (byte) (connectionId >> 24);
            tmp[9] = (byte) (connectionId >> 16);
            tmp[10] = (byte) (connectionId >> 8);
            tmp[11] = (byte) (connectionId);
            
            return Convert.ToBase64String(tmp);
        }

        /// <summary>
        ///     Decodes the session id.
        /// </summary>
        public static void DecodeSessionId(string sessionId, out int proxyId, out long connectionId)
        {
            byte[] tmp = Convert.FromBase64String(sessionId);

            proxyId = (tmp[0] << 24 | tmp[1] << 16 | tmp[2] << 8 | tmp[3]);
            connectionId = (tmp[4] << 56 | tmp[5] << 48 | tmp[6] << 40 | tmp[7] << 32 | tmp[8] << 24 | tmp[9] << 16 | tmp[10] << 8 | tmp[11]);
        }
    }
}