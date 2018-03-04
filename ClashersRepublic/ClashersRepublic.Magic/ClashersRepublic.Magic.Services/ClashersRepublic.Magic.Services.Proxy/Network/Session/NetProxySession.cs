namespace ClashersRepublic.Magic.Services.Proxy.Network.Session
{
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Titan.Math;

    internal class NetProxySession : NetSession
    {
        /// <summary>
        ///     Gets the <see cref="NetworkClient"/> instance.
        /// </summary>
        internal NetworkClient Client { get; }

        /// <summary>
        ///     Gets the account id.
        /// </summary>
        internal LogicLong AccountId { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetProxySession"/> class.
        /// </summary>
        internal NetProxySession(NetworkClient client, byte[] sessionId, string sessionName) : base(sessionId, sessionName)
        {
            this.Client = client;
        }

        /// <summary>
        ///     Sets the account id.
        /// </summary>
        internal void SetAccountId(LogicLong accountId)
        {
            this.AccountId = accountId;
        }

        /// <summary>
        ///     Binds the specified server.
        /// </summary>
        internal void BindServer(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (serviceNodeId > -1)
                {
                    base.SetServiceNodeId(serviceNodeType, serviceNodeId);

                    int[] serverIDs = this.ServiceNodeIDs;

                    ServerBoundMessage message = new ServerBoundMessage();

                    message.SetAccountId(this.AccountId);
                    message.SetEndPoints(serverIDs);

                    NetMessageManager.SendMessage(serviceNodeType, serviceNodeId, this.SessionId, this.SessionId.Length, message);

                    for (int i = 0; i < this._serviceNodeSockets.Length; i++)
                    {
                        if (i != serviceNodeType && i != ServiceCore.ServiceNodeType && i != 2)
                        {
                            if (this._serviceNodeSockets[i] != null)
                            {
                                UpdateServerEndPointMessage updateMessage = new UpdateServerEndPointMessage();

                                updateMessage.SetServerType(serviceNodeType);
                                updateMessage.SetServerId(serviceNodeId);

                                NetMessageManager.SendMessage(this._serviceNodeSockets[i], this.SessionId, this.SessionId.Length, updateMessage);
                            }
                        }
                    }
                }
            }
            else
            {
                Logging.Warning(this, "NetProxySession::bindServer serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }

        /// <summary>
        ///     Unbinds the specified service node.
        /// </summary>
        internal void UnbindServer(int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (this._serviceNodeSockets[serviceNodeType] != null)
                {
                    NetMessageManager.SendMessage(this._serviceNodeSockets[serviceNodeType], this.SessionId, this.SessionId.Length, new ServerUnboundMessage());
                }

                for (int i = 0; i < this._serviceNodeSockets.Length; i++)
                {
                    if (i != serviceNodeType && i != ServiceCore.ServiceNodeType && i != 2)
                    {
                        if (this._serviceNodeSockets[i] != null)
                        {
                            UpdateServerEndPointMessage updateMessage = new UpdateServerEndPointMessage();

                            updateMessage.SetServerType(serviceNodeType);
                            updateMessage.SetServerId(-1);

                            NetMessageManager.SendMessage(this._serviceNodeSockets[i], this.SessionId, this.SessionId.Length, updateMessage);
                        }
                    }
                }

                base.SetServiceNodeId(serviceNodeType, -1);
            }
            else
            {
                Logging.Warning(this, "NetProxySession::unbindServer serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }

        /// <summary>
        ///     Unbinds all servers.
        /// </summary>
        internal void UnbindAllServers()
        {
            for (int i = 0; i < this._serviceNodeSockets.Length; i++)
            {
                if (this._serviceNodeSockets[i] != null)
                {
                    NetMessageManager.SendMessage(this._serviceNodeSockets[i], this.SessionId, this.SessionId.Length, new ServerUnboundMessage());
                }
            }
        }
    }
}