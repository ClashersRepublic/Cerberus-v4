namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System;

    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Network.Handler;

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

            if (packet.GetNetMessage() != null)
            {
                NetMessaging._messageHandler.Receive(packet);
            }
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        internal static void Send(int serviceNodeType, int serviceNodeId, NetMessage message, byte[] sessionId = null, int sessionIdLength = 0)
        {
            NetSocket destinationSocket = NetManager.GetServiceNodeEndPoint(serviceNodeType, serviceNodeId);

            if (destinationSocket != null)
            {
                NetMessaging.Send(destinationSocket, message, sessionId, sessionIdLength);
            }
        }
        
        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        internal static void Send(NetSocket destinationSocket, NetMessage message, byte[] sessionId = null, int sessionIdLength = 0)
        {
            if (destinationSocket == null)
            {
                throw new ArgumentNullException("destinationSocket");
            }

            message.SetServiceNodeType(ServiceCore.ServiceNodeType);
            message.SetServiceNodeId(ServiceCore.ServiceNodeId);
            message.SetSessionId(sessionId, sessionIdLength);
            message.Encode();

            NetMessaging._messageHandler.Send(destinationSocket, new NetPacket(message));
        }
        
        /// <summary>
        ///     Sets the <see cref="NetMessageManager" /> instance.
        /// </summary>
        public static void SetMessageManager(NetMessageManager manager)
        {
            NetMessaging._messageHandler.SetMessageManager(manager);
        }
    }
}