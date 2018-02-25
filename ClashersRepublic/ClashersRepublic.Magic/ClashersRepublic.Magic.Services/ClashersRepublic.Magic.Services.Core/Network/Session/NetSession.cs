namespace ClashersRepublic.Magic.Services.Core.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ;

    public class NetSession
    {
        private byte[] _sessionId;
        private string _sessionName;

        private readonly NetMQSocket[] _serviceNodeSockets;

        /// <summary>
        ///     Gets the session name.
        /// </summary>
        public string SessionName
        {
            get
            {
                return this._sessionName;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSession"/> class.
        /// </summary>
        public NetSession(byte[] sessionId, string sessionName)
        {
            this._sessionId = sessionId;
            this._sessionName = sessionName;
            this._serviceNodeSockets = new NetMQSocket[28];
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            for (int i = 0; i < 28; i++)
            {
                this._serviceNodeSockets[i] = null;
            }

            this._sessionId = null;
            this._sessionName = null;
        }

        /// <summary>
        ///     Sets the service node socket.
        /// </summary>
        public void SetServiceNodeSocket(NetMQSocket socket, int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                this._serviceNodeSockets[serviceNodeType] = socket;
            }
            else
            {
                Logging.Warning(this, "NetSession::setServiceNodeSocket serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }

        /// <summary>
        ///     Sets the service node id.
        /// </summary>
        public void SetServiceNodeId(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (serviceNodeType <= 0xFF)
                {
                    NetMQSocket socket = NetManager.GetServiceNodeEndPoint(serviceNodeType, serviceNodeId);

                    if (socket != null)
                    {
                        this._serviceNodeSockets[serviceNodeType] = socket;
                    }
                }
                else
                {
                    Logging.Warning(this, "NetSession::setServiceNodeId serverNodeId too big (" + serviceNodeId + ")");
                }
            }
            else
            {
                Logging.Warning(this, "NetSession::setServiceNodeId serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }
    }
}