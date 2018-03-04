namespace ClashersRepublic.Magic.Services.Account.Network.Session
{
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Core.Network.Session;

    internal class NetAccountSession : NetSession
    {
        private readonly Account _account;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetAccountSession"/> class.
        /// </summary>
        internal NetAccountSession(Account account, byte[] sessionId, string sessionName) : base(sessionId, sessionName)
        {
            this._account = account;
        }
    }
}