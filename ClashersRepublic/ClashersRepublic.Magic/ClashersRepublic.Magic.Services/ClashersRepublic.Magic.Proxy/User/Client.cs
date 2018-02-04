namespace ClashersRepublic.Magic.Proxy.User
{
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Session;

    internal class Client
    {
        internal NetworkToken NetworkToken;
        internal GameSession GameSession;
        internal ClientDefines Defines;

        internal int State;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        internal Client(NetworkToken token)
        {
            this.NetworkToken = token;
        }
    }
}