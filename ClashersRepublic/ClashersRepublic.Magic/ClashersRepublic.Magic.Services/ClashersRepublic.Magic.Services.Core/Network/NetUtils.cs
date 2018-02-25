namespace ClashersRepublic.Magic.Services.Core.Network
{
    public class NetUtils
    {
        /// <summary>
        ///     Gets the net port.
        /// </summary>
        public static int GetNetPort(int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < 28)
            {
                return 5000 + serviceNodeType;
            }
            
            return -1;
        }
    }
}