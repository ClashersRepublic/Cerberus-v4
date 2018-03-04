namespace ClashersRepublic.Magic.Services.Account.Network.Session
{
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
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

        /// <summary>
        ///     Sets the service node id.
        /// </summary>
        public override void SetServiceNodeId(int serviceNodeType, int serviceNodeId)
        {
            base.SetServiceNodeId(serviceNodeType, serviceNodeId);

            AskForBindServerMessage ask = new AskForBindServerMessage();
            ask.SetAccountId(this._account.Id);
            NetMessageManager.SendMessage(serviceNodeType, serviceNodeId, this.SessionId, this.SessionId.Length, ask);
        }
    }
}