namespace ClashersRepublic.Magic.Services.Core.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Titan.Message;

    public class NetSession
    {
        protected readonly NetSocket[] _serviceNodeSockets;
        
        /// <summary>
        ///     Gets the session id.
        /// </summary>
        public byte[] SessionId { get; private set; }
        
        /// <summary>
        ///     Gets the id of all servers.
        /// </summary>
        protected int[] ServiceNodeIDs
        {
            get
            {
                int[] tmp = new int[28];

                for (int i = 0; i < 28; i++)
                {
                    tmp[i] = this._serviceNodeSockets[i] != null ? this._serviceNodeSockets[i].Id : -1;
                }

                return tmp;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSession" /> class.
        /// </summary>
        public NetSession(byte[] sessionId)
        {
            this.SessionId = sessionId;
            this._serviceNodeSockets = new NetSocket[28];
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            for (int i = 0; i < 28; i++)
            {
                this._serviceNodeSockets[i] = null;
            }

            this.SessionId = null;
        }

        /// <summary>
        ///     Gets the service node end point.
        /// </summary>
        public NetSocket GetServiceNodeEndPoint(int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                return this._serviceNodeSockets[serviceNodeType];
            }
            else
            {
                Logging.Warning("NetSession::setServiceNodeId serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }

            return null;
        }

        /// <summary>
        ///     Sets the service node id.
        /// </summary>
        public void SetServiceNodeId(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (serviceNodeId > -1)
                {
                    NetSocket socket = NetManager.GetServiceNodeEndPoint(serviceNodeType, serviceNodeId);

                    if (socket != null)
                    {
                        this._serviceNodeSockets[serviceNodeType] = socket;
                    }
                }
                else
                {
                    this._serviceNodeSockets[serviceNodeType] = null;
                }
            }
            else
            {
                Logging.Warning("NetSession::setServiceNodeId serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }

        /// <summary>
        ///     Sends the specified <see cref="NetMessage"/> to the service.
        /// </summary>
        public void SendMessage(int serviceNodeType, NetMessage message)
        {
            NetSocket socket = this._serviceNodeSockets[serviceNodeType];

            if (socket != null)
            {
                NetMessageManager.SendMessage(socket, this.SessionId, message);
            }
            else
            {
                Logging.Warning("NetSession::sendMessage server is not set, nodeType: " + serviceNodeType);
            }
        }

        /// <summary>
        ///     Forwards the specified <see cref="PiranhaMessage"/> to the service.
        /// </summary>
        public void  SendPiranhaMessage(int serviceNodeType, PiranhaMessage message)
        {
            NetSocket socket = this._serviceNodeSockets[serviceNodeType];

            if (socket != null)
            {
                if (message.GetEncodingLength() == 0)
                {
                    message.Encode();
                }

                ForwardPiranhaMessage forwardPiranhaMessage = new ForwardPiranhaMessage();
                forwardPiranhaMessage.SetPiranhaMessage(message);
                NetMessageManager.SendMessage(socket, this.SessionId, forwardPiranhaMessage);
            }
        }

        /// <summary>
        ///     Forwards the specified <see cref="PiranhaMessage"/> to the service.
        /// </summary>
        public void SendErrorPiranhaMessage(int serviceNodeType, PiranhaMessage message)
        {
            NetSocket socket = this._serviceNodeSockets[serviceNodeType];

            if (socket != null)
            {
                if (message.GetEncodingLength() == 0)
                {
                    message.Encode();
                }

                this.SendPiranhaMessage(serviceNodeType, message);
                this.SendMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, new UnbindServerMessage());
            }
        }
    }
}