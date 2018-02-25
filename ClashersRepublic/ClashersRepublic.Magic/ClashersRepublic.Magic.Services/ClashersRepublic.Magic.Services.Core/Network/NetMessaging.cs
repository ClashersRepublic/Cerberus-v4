namespace ClashersRepublic.Magic.Services.Core.Network
{
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Network.Handler;
    using ClashersRepublic.Magic.Titan.DataStream;

    public static class NetMessaging
    {
        private static NetHandler _messageHandler;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetMessaging._messageHandler = new NetHandler();
        }

        /// <summary>
        ///     Called when a packet is received.
        /// </summary>
        internal static void OnReceive(byte[] buffer, int length)
        {
            NetPacket packet = new NetPacket(buffer, length);

            if (packet.GetNetMessageCount() != 0)
            {
                NetMessaging._messageHandler.Receive(packet);
            }
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        public static void Send(int serviceNodeType, int serviceNodeId, byte[] sessionId, int sessionLength, NetMessage message)
        {
            NetMQSocket destinationSocket = NetManager.GetServiceNodeEndPoint(serviceNodeType, serviceNodeId);

            if (destinationSocket != null)
            {
                NetPacket netPacket = new NetPacket();

                netPacket.AddMessage(message);
                netPacket.SetSessionId(sessionId, sessionLength);
                netPacket.SetServiceNodeId(ServiceCore.ServiceNodeId);
                netPacket.SetServiceNodeType(ServiceCore.ServiceNodeType);

                NetMessaging._messageHandler.Send(destinationSocket, netPacket);
            }
        }

        /// <summary>
        ///     Sends the specified <see cref="NetPacket"/> instance.
        /// </summary>
        internal static void InternalSend(NetMQSocket serverSocket, NetPacket packet)
        {
            ByteStream byteStream = new ByteStream(10);

            packet.Encode(byteStream);

            int encodingLength = byteStream.GetOffset();
            byte[] encodingByteArray = byteStream.GetByteArray();

            NetGateway.Send(encodingByteArray, encodingLength, serverSocket);

            byteStream.Destruct();
        }

        /// <summary>
        ///     Sets the <see cref="INetMessageManager"/> instance.
        /// </summary>
        public static void SetMessageManager(INetMessageManager manager)
        {
            NetMessaging._messageHandler.SetMessageManager(manager);
        }
    }
}