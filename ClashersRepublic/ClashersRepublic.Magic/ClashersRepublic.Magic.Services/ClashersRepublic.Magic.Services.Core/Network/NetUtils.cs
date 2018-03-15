namespace ClashersRepublic.Magic.Services.Core.Network
{
    public class NetUtils
    {
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
                case 9: return "Stream";
                case 10: return "Home";
                case 11: return "Alliance";
                case 13: return "League";
                case 27: return "Battle";
                default: return string.Empty;
            }
        }
    }
}