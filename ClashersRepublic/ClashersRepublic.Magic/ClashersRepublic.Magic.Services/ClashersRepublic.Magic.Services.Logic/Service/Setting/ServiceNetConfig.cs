namespace ClashersRepublic.Magic.Services.Logic.Service.Setting
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Json;

    public static class ServiceNetConfig
    {
        private static bool _udpEnabled;

        private static int _tcpBufferSize;
        private static int _tcpBufferAlloc;

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void LoadConfig(LogicJSONObject jsonObject)
        {
            LogicJSONObject udpObject = jsonObject.GetJSONObject("udp");

            if (udpObject != null)
            {
                ServiceNetConfig._udpEnabled = LogicJSONHelper.GetJSONBoolean(udpObject, "enabled");
            }

            LogicJSONObject tcpObject = jsonObject.GetJSONObject("tcp");

            if (tcpObject != null)
            {
                ServiceNetConfig._tcpBufferSize = LogicJSONHelper.GetJSONNumber(tcpObject, "buffer_size");
                ServiceNetConfig._tcpBufferAlloc = LogicJSONHelper.GetJSONNumber(tcpObject, "buffer_alloc");
            }
        }

        /// <summary>
        ///     Gets if udp is enabled.
        /// </summary>
        public static bool UdpEnabled()
        {
            return ServiceNetConfig._udpEnabled;
        }

        /// <summary>
        ///     Gets the tcp buffer size.
        /// </summary>
        public static int GetTcpBufferSize()
        {
            return ServiceNetConfig._tcpBufferSize;
        }

        /// <summary>
        ///     Gets the tcp buffer allocation size.
        /// </summary>
        public static int GetTcpBufferAllocationSize()
        {
            return ServiceNetConfig._tcpBufferAlloc;
        }
    }
}