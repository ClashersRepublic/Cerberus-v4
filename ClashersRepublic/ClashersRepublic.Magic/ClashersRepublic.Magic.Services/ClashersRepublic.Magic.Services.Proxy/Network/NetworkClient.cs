namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    using ClashersRepublic.Magic.Services.Proxy.Network.Message;
    using ClashersRepublic.Magic.Services.Proxy.Network.Session;

    internal class NetworkClient
    {
        private NetworkToken _networkToken;
        private NetProxySession _session;

        /// <summary>
        ///     Gets the client state.
        /// </summary>
        internal int State { get; set; }

        /// <summary>
        ///     Gets the <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager MessageManager
        {
            get
            {
                return this._networkToken.Messaging.MessageManager;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        internal NetworkClient(NetworkToken token)
        {
            this.State = 1;
            this._networkToken = token;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            if (this._session != null)
            {
                NetProxySessionManager.TryRemove(this._session.SessionName, out _);

                this._session.UnbindAllServers();
                this._session.Destruct();
                this._session = null;
            }

            this._networkToken = null;
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