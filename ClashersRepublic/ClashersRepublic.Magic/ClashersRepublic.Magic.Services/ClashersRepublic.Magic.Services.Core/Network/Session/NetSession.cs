namespace ClashersRepublic.Magic.Services.Core.Network.Session
{
    public class NetSession
    {
        protected readonly NetSocket[] _serviceNodeSockets;
        
        /// <summary>
        ///     Gets the session id.
        /// </summary>
        public byte[] SessionId { get; private set; }

        /// <summary>
        ///     Gets the session name.
        /// </summary>
        public string SessionName { get; private set; }
        
        /// <summary>
        ///     Gets the id of all servers.
        /// </summary>
        public int[] ServiceNodeIDs
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
        public NetSession(byte[] sessionId, string sessionName)
        {
            this.SessionId = sessionId;
            this.SessionName = sessionName;

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
            this.SessionName = null;
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
    }
}