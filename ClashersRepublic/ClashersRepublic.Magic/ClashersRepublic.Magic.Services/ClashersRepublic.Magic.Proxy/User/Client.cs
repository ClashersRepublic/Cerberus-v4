namespace ClashersRepublic.Magic.Proxy.User
{
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Message;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Session;

    internal class Client
    {
        internal NetworkToken NetworkToken;
        internal MessageManager MessageManager;
        internal GameAccount GameAccount;
        internal GameSession GameSession;

        internal int State;
        internal int DeviceType;

        internal bool AndroidClient;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        internal Client(NetworkToken token)
        {
            this.NetworkToken = token;
            this.MessageManager = new MessageManager(this);
            this.State = 0;
        }
    }
}