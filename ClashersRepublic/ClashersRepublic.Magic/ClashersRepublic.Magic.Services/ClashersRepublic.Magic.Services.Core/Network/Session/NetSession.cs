namespace ClashersRepublic.Magic.Services.Core.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Message.Session;

    public class NetSession
    {
        private byte[] _sessionId;
        private string _sessionName;
        
        private readonly NetSocket[] _serviceNodeSockets;


        /// <summary>
        ///     Gets the session id.
        /// </summary>
        public byte[] SessionId
        {
            get
            {
                return this._sessionId;
            }
        }

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
        ///     Gets the id of all servers.
        /// </summary>
        public byte[] ServiceNodeIDs
        {
            get
            {
                byte[] tmp = new byte[28];

                for (int i = 0; i < 28; i++)
                {
                    if (this._serviceNodeSockets[i] != null)
                    {
                        tmp[i] = (byte) this._serviceNodeSockets[i].Id;
                    }
                }

                return tmp;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSession"/> class.
        /// </summary>
        public NetSession(byte[] sessionId, string sessionName)
        {
            this._sessionId = sessionId;
            this._sessionName = sessionName;

            this._serviceNodeSockets = new NetSocket[28];
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
        ///     Sets the service node id.
        /// </summary>
        public void SetServiceNodeId(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (serviceNodeType <= 0xFF)
                {
                    NetSocket socket = NetManager.GetServiceNodeEndPoint(serviceNodeType, serviceNodeId);

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

        /// <summary>
        ///     Sends the <see cref="UpdateSessionSocketListMessage"/> message to all service nodes.
        /// </summary>
        public void SendSessionUpdateListMessage()
        {
            bool[] setSocketList = new bool[28];
            byte[] sessionSocketList = new byte[28];

            for (int i = 0; i < 28; i++)
            {
                if (this._serviceNodeSockets[i] != null)
                {
                    setSocketList[i] = true;
                    sessionSocketList[i] = (byte) this._serviceNodeSockets[i].Id;
                }
            }

            for (int i = 0; i < 28; i++)
            {
                NetSocket socket = this._serviceNodeSockets[i];

                if (socket != null)
                {
                    UpdateSessionSocketListMessage message = new UpdateSessionSocketListMessage();

                    message.SetIsSetList(setSocketList);
                    message.SetSessionSocketList(sessionSocketList);

                    NetMessaging.Send(i, sessionSocketList[i], this._sessionId, this._sessionId.Length, message);
                }
            }
        }
    }
}