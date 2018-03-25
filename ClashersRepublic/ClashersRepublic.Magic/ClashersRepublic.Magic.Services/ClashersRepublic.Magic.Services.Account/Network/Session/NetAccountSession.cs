namespace ClashersRepublic.Magic.Services.Account.Network.Session
{
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Titan.Json;

    internal class NetAccountSession : NetSession
    {
        /// <summary>
        ///     Gets the account.
        /// </summary>
        internal Account Account { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetAccountSession"/> class.
        /// </summary>
        internal NetAccountSession(Account account, byte[] sessionId) : base(sessionId)
        {
            this.Account = account;
        }

        /// <summary>
        ///     Saves the account.
        /// </summary>
        public void SaveAccount()
        {
            DatabaseManager.Update(0, this.Account.Id, LogicJSONParser.CreateJSONString(this.Account.Save()));
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this.Account = null;
        }
    }
}