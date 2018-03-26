namespace ClashersRepublic.Magic.Services.Avatar.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Network.Session;

    using ClashersRepublic.Magic.Services.Avatar.Game;
    using ClashersRepublic.Magic.Services.Avatar.Game.Message;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Utils;

    internal class NetAvatarSession : NetSession
    {
        /// <summary>
        ///     Gets the <see cref="Game.AvatarAccount"/> instance.
        /// </summary>
        internal AvatarAccount AvatarAccount { get; private set; }

        /// <summary>
        ///     Gets the piranha <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager PiranhaMessageManager { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetAvatarSession"/> class.
        /// </summary>
        internal NetAvatarSession(AvatarAccount avatarAccount, byte[] sessionId) : base(sessionId)
        {
            this.AvatarAccount = avatarAccount;
            this.PiranhaMessageManager = new MessageManager(this);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this.PiranhaMessageManager != null)
            {
                this.PiranhaMessageManager.Destruct();
                this.PiranhaMessageManager = null;
            }

            this.AvatarAccount = null;
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

                        message.SetAccountId(this.AvatarAccount.Id);
                        message.SetEndPoints(serverIDs);

                        NetMessageManager.SendMessage(serviceNodeType, serviceNodeId, this.SessionId, message);
                    }

                    if (updateBoundServers)
                    {
                        for (int i = 0; i < this._serviceNodeSockets.Length; i++)
                        {
                            if (i != NetUtils.SERVICE_NODE_TYPE_ACCOUNT_DIRECTORY && i != NetUtils.SERVICE_NODE_TYPE_AVATAR_CONTAINER)
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
                Logging.Warning("NetAvatarSession::bindServer serviceNodeType out of bands " + serviceNodeType + "/" + this._serviceNodeSockets.Length);
            }
        }
    }
}