namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System.Collections.Generic;
    using System.Threading;
    
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Net;

    public static class NetGateway
    {
        private static NetServer _listener;
        private static Thread _receiveThread;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            NetGateway._listener = new NetServer(NetUtils.GetNetPort(ServiceCore.ServiceNodeType, ServiceCore.ServiceNodeId));
            NetGateway._receiveThread = new Thread(NetGateway.Receive);
        }

        /// <summary>
        ///     Starts the receive thread.
        /// </summary>
        public static void Start()
        {
            NetGateway._receiveThread.Start();
        }

        /// <summary>
        ///     Receives packets from socket.
        /// </summary>
        private static void Receive()
        {
            Queue<byte[]> messages = new Queue<byte[]>();

            while (true)
            {
                if (NetGateway._listener.Read(ref messages))
                {
                    while (messages.Count != 0)
                    {
                        byte[] message = messages.Dequeue();

                        if (message == null)
                        {
                            break;
                        }

                        NetMessaging.OnReceive(message, message.Length);
                    }
                }

                Thread.Sleep(1);
            }
        }
    }
}