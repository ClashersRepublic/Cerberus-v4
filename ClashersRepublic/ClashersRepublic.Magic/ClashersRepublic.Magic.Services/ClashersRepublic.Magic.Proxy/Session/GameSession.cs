namespace ClashersRepublic.Magic.Proxy.Session
{
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.User;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;

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

        /// <summary>
        ///     Sends the <see cref="ServiceMessage"/> to home service.
        /// </summary>
        internal void SendToHomeService(ServiceMessage message)
        {
            ServiceMessageManager.SendMessage(message, ServiceExchangeName.SERVICE_HOME_NAME, this.Account.HighId, this.SessionId);
        }
    }
}