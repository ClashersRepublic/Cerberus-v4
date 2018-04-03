namespace LineageSoft.Magic.Services.Proxy.Network
{
    using LineageSoft.Magic.Services.Core.Utils;
    using LineageSoft.Magic.Services.Proxy.Network.Session;

    internal class NetworkClient
    {
        private NetProxySession _session;

        /// <summary>
        ///     Gets the client state.
        /// </summary>
        internal int State { get; set; }

        /// <summary>
        ///     Gets or Sets the device model.
        /// </summary>
        internal string DeviceModel { get; set; }

        /// <summary>
        ///     Gets the <see cref="NetworkMessaging"/> instance.
        /// </summary>
        internal NetworkMessaging Messaging { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        internal NetworkClient(NetworkMessaging messaging)
        {
            this.State = 1;
            this.Messaging = messaging;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this.State = -1;

            if (this._session != null)
            {
                NetProxySessionManager.TryRemove(this._session.SessionId, out _);

                this._session.SetServiceNodeId(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, -1);
                this._session.UnbindAllServers();
                this._session.Destruct();
                this._session = null;
            }

            this.Messaging = null;
        }

        /// <summary>
        ///     Gets the client address.
        /// </summary>
        internal string GetAddress()
        {
            return this.Messaging.Connection.GetAddress();
        }

        /// <summary>
        ///     Gets the <see cref="NetProxySession"/> instance.
        /// </summary>
        internal NetProxySession GetSession()
        {
            return this._session;
        }

        /// <summary>
        ///     Sets the <see cref="NetProxySession"/> instance.
        /// </summary>
        internal void SetSession(NetProxySession session)
        {
            this._session = session;
        }
    }
}