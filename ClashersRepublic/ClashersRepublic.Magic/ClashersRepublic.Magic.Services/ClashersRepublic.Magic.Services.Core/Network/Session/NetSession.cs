namespace ClashersRepublic.Magic.Services.Core.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Titan.Math;

    public class NetSession
    {
        private readonly NetSocket[] _serviceNodeSockets;


        /// <summary>
        ///     Gets the session id.
        /// </summary>
        public byte[] SessionId { get; private set; }

        /// <summary>
        ///     Gets the session name.
        /// </summary>
        public string SessionName { get; private set; }

        /// <summary>
        ///     Gets the account id.
        /// </summary>
        public LogicLong AccountId { get; private set; }

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
        ///     Initializes a new instance of the <see cref="NetSession" /> class.
        /// </summary>
        public NetSession(LogicLong accountId, byte[] sessionId, string sessionName)
        {
            this.AccountId = accountId;
            this.SessionId = sessionId;
            this.SessionName = sessionName;

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

            this.SessionId = null;
            this.SessionName = null;
        }

        /// <summary>
        ///     Sets the service node id.
        /// </summary>
        public void SetServiceNodeId(int serviceNodeType, int serviceNodeId, bool askMessage = false)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (serviceNodeId <= 0xFF)
                {
                    NetSocket socket = NetManager.GetServiceNodeEndPoint(serviceNodeType, serviceNodeId);

                    if (socket != null)
                    {
                        this._serviceNodeSockets[serviceNodeType] = socket;

                        if (askMessage)
                        {
                            AskForBindServerMessage ask = new AskForBindServerMessage();
                            ask.SetAccountId(this.AccountId);
                            NetMessageManager.SendMessage(socket, this.SessionId, this.SessionId.Length, ask);
                        }
                    }
                }
                else
                {
                    Logging.Warning(this, "NetSession::setServiceNodeId serviceNodeId too big (" + serviceNodeId + ")");
                }
            }
            else
            {
                Logging.Warning(this, "NetSession::setServiceNodeId serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }
    }
}