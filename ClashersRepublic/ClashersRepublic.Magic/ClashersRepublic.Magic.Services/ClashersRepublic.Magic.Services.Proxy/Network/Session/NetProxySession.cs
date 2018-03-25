namespace ClashersRepublic.Magic.Services.Proxy.Network.Session
{
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Services.Core.Utils;
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
        internal NetProxySession(NetworkClient client, byte[] sessionId) : base(sessionId)
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
        internal void BindServer(int serviceNodeType, int serviceNodeId, bool sendBound = true, bool sendUnbound = true, bool updateBoundServers = true)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (serviceNodeId > -1)
                {
                    if (this._serviceNodeSockets[serviceNodeType] != null)
                    {
                        if (sendUnbound)
                        {
                            NetMessageManager.SendMessage(this._serviceNodeSockets[serviceNodeType], this.SessionId, new ServerUnboundMessage());
                        }

                        this._serviceNodeSockets[serviceNodeType] = null;
                    }

                    base.SetServiceNodeId(serviceNodeType, serviceNodeId);

                    if (sendBound)
                    {
                        int[] serverIDs = this.ServiceNodeIDs;

                        ServerBoundMessage message = new ServerBoundMessage();

                        message.SetAccountId(this.AccountId);
                        message.SetEndPoints(serverIDs);

                        NetMessageManager.SendMessage(serviceNodeType, serviceNodeId, this.SessionId, message);
                    }

                    if (updateBoundServers)
                    {
                        for (int i = 0; i < this._serviceNodeSockets.Length; i++)
                        {
                            if (i != NetUtils.SERVICE_NODE_TYPE_ACCOUNT_DIRECTORY && i != NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER)
                            {
                                if (this._serviceNodeSockets[i] != null)
                                {
                                    UpdateServerEndPointMessage updateMessage = new UpdateServerEndPointMessage();

                                    updateMessage.SetServerType(serviceNodeType);
                                    updateMessage.SetServerId(serviceNodeId);

                                    NetMessageManager.SendMessage(this._serviceNodeSockets[i], this.SessionId, updateMessage);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Logging.Warning("NetProxySession::bindServer serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }

        /// <summary>
        ///     Unbinds the specified service node.
        /// </summary>
        internal void UnbindServer(int serviceNodeType, bool sendUnbound = true, bool updateBoundServer = true)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serviceNodeSockets.Length)
            {
                if (this._serviceNodeSockets[serviceNodeType] != null)
                {
                    this._serviceNodeSockets[serviceNodeType] = null;

                    if (updateBoundServer)
                    {
                        for (int i = 3; i < this._serviceNodeSockets.Length; i++)
                        {
                            if (this._serviceNodeSockets[i] != null)
                            {
                                UpdateServerEndPointMessage updateMessage = new UpdateServerEndPointMessage();

                                updateMessage.SetServerType(serviceNodeType);
                                updateMessage.SetServerId(-1);

                                NetMessageManager.SendMessage(this._serviceNodeSockets[i], this.SessionId, updateMessage);
                            }
                        }
                    }

                    base.SetServiceNodeId(serviceNodeType, -1);

                    if (sendUnbound)
                    {
                        NetMessageManager.SendMessage(this._serviceNodeSockets[serviceNodeType], this.SessionId, new ServerUnboundMessage());
                    }
                }
            }
            else
            {
                Logging.Warning("NetProxySession::unbindServer serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
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
                    NetMessageManager.SendMessage(this._serviceNodeSockets[i], this.SessionId, new ServerUnboundMessage());
                }
            }
        }
    }
}