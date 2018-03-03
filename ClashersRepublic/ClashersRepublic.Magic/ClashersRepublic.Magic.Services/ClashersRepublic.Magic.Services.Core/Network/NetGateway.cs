namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System.Threading;

    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Sockets;

    public static class NetGateway
    {
        private static NetMQSocket _listener;
        private static Thread _receiveThread;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            NetGateway._listener = new DealerSocket("@tcp://*:" + NetUtils.GetNetPort(ServiceCore.ServiceNodeType, ServiceCore.ServiceNodeId));
            NetGateway._receiveThread = new Thread(NetGateway.Receive);
            NetGateway._receiveThread.Start();
        }

        /// <summary>
        ///     Receives packets from socket.
        /// </summary>
        private static void Receive()
        {
            while (true)
            {
                NetGateway.ProcessReceive(NetGateway._listener.ReceiveFrameBytes());
            }
        }

        /// <summary>
        ///     Processes the received packet.
        /// </summary>
        private static void ProcessReceive(byte[] buffer)
        {
            if (buffer != null)
            {
                NetMessaging.OnReceive(buffer, buffer.Length);
            }
        }
    }
}