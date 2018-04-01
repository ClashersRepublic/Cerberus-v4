namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System;
    using ClashersRepublic.Magic.Services.Core.Utils;

    using ClashersRepublic.Magic.Services.Net;
    using ClashersRepublic.Magic.Services.Net.ServerSocket;

    public class NetGateway : NetListener
    {
        private readonly NetTcpServerGateway _serverSocket;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetGateway"/> class.
        /// </summary>
        public NetGateway()
        {
            this._serverSocket = new NetTcpServerGateway(this, NetUtils.GetNetPort(ServiceCore.ServiceNodeType, ServiceCore.ServiceNodeId));
        }

        /// <summary>
        ///     Starts the server socket.
        /// </summary>
        public void Start()
        {
            this._serverSocket.StartAccept();
        }

        /// <summary>
        ///     Called when a packet is received.
        /// </summary>
        public override int OnReceive(byte[] buffer, int length)
        {
            if (length >= 4)
            {
                int messageLength = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];

                if (length - 4 >= messageLength)
                {
                    byte[] messageBytes = new byte[length - 4];

                    Array.Copy(buffer, 4, messageBytes, 0, messageLength);
                    NetMessaging.OnReceive(messageBytes, messageLength);

                    return 4 + messageLength;
                }
            }

            return 0;
        }
    }
}