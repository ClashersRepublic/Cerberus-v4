namespace ClashersRepublic.Magic.Proxy.Session
{
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.User;

    internal class GameSession
    {
        internal string SessionId;

        internal Client Client;
        internal GameAccount Account;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameSession"/> class.
        /// </summary>
        internal GameSession()
        {
            this.SessionId = null;
        }

        /// <summary>
        ///     Gets a value indicating whether this session is empty.
        /// </summary>
        /// <returns></returns>
        internal bool IsEmpty()
        {
            return this.SessionId == null;
        }

        /// <summary>
        ///     Sets the session data.
        /// </summary>
        internal void SetData(string sessionId, Client client)
        {
            this.SessionId = sessionId;
            this.Client = client;
            this.Account = client.GameAccount;
        }
    }
}