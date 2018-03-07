namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    using ClashersRepublic.Magic.Services.Proxy.Network.Message;
    using ClashersRepublic.Magic.Services.Proxy.Network.Session;

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
        ///     Gets the <see cref="Network.NetworkToken"/> instance.
        /// </summary>
        internal NetworkToken NetworkToken { get; private set; }

        /// <summary>
        ///     Gets the <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager MessageManager
        {
            get
            {
                return this.NetworkToken.Messaging.MessageManager;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        internal NetworkClient(NetworkToken token)
        {
            this.State = 1;
            this.NetworkToken = token;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this.State = -1;

            if (this._session != null)
            {
                NetProxySessionManager.TryRemove(this._session.SessionName, out _);

                this._session.UnbindAllServers();
                this._session.Destruct();
                this._session = null;
            }

            this.NetworkToken = null;
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