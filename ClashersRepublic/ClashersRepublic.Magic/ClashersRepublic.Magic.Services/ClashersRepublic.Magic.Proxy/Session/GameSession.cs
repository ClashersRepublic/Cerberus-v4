namespace ClashersRepublic.Magic.Proxy.Session
{
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.User;

    internal class GameSession
    {
        internal string SessionId { get; }

        internal Client Client { get; }
        internal GameAccount Account { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameSession" /> class.
        /// </summary>
        internal GameSession(string sessionId, Client client, GameAccount account)
        {
            this.SessionId = sessionId;
            this.Client = client;
            this.Account = account;
        }
    }
}