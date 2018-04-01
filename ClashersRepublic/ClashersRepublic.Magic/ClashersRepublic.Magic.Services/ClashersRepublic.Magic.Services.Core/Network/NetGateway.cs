namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System.Collections.Generic;
    using System.Threading;
    
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Net;

    public class NetGateway : NetServerListener
    {
        private NetServer _serverSocket;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetGateway"/> class.
        /// </summary>
        public NetGateway()
        {
            this._serverSocket = new NetServer(NetUtils.GetNetPort(ServiceCore.ServiceNodeType, ServiceCore.ServiceNodeId));
            this._serverSocket.SetListener(this);
        }

        /// <summary>
        ///     Starts the gateway.
        /// </summary>
        public void Start()
        {
            this._serverSocket.StartAccept();
        }

        /// <summary>
        ///     Called when a packet is received.
        /// </summary>
        public override void OnReceive(byte[] buffer, int length)
        {
            NetMessaging.OnReceive(buffer, length);
        }
    }
}