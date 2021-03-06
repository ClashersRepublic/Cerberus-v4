﻿namespace RivieraStudio.Magic.Services.Core.Utils
{
    public class NetUtils
    {
        public const int SERVICE_NODE_TYPE_PROXY_CONTAINER = 1;
        public const int SERVICE_NODE_TYPE_ACCOUNT_DIRECTORY = 2;
        public const int SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER = 6;
        public const int SERVICE_NODE_TYPE_AVATAR_CONTAINER = 9;
        public const int SERVICE_NODE_TYPE_ZONE_CONTAINER = 10;
        public const int SERVICE_NODE_TYPE_PARTY_CONTAINER = 11;

        /// <summary>
        ///     Gets the net port.
        /// </summary>
        public static int GetNetPort(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < 28)
            {
                return 5000 + 512 * serviceNodeType + serviceNodeId;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the service node name.
        /// </summary>
        public static string GetServiceName(int serviceNodeType)
        {
            switch (serviceNodeType)
            {
                case 1: return "Proxy";
                case 2: return "Account";
                case 6: return "Global Chat";
                case 9: return "Avatar";
                case 10: return "Zone";
                case 11: return "Party";
                case 13: return "Ranking";
                case 27: return "Battle";
                default: return string.Empty;
            }
        }
    }
}