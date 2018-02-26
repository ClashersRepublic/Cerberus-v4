namespace ClashersRepublic.Magic.Services.Proxy.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Network.Session;

    internal class NetProxySession : NetSession
    {
        /// <summary>
        ///     Gets the <see cref="NetworkClient"/> instance.
        /// </summary>
        internal NetworkClient Client { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetProxySession"/> class.
        /// </summary>
        internal NetProxySession(NetworkClient client, byte[] sessionId, string sessionName) : base(sessionId, sessionName)
        {
            this.Client = client;
        }
    }
}