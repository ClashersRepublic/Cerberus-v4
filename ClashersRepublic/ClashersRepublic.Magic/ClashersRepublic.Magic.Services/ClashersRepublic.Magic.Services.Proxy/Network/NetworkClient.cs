namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    internal class NetworkClient
    {
        private NetworkToken _networkToken;

        /// <summary>
        ///     Gets the client state.
        /// </summary>
        internal int State { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkClient"/> class.
        /// </summary>
        internal NetworkClient(NetworkToken token)
        {
            this._networkToken = token;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        private void Destruct()
        {
            this._networkToken = null;
        }
    }
}