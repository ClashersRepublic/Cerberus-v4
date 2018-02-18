namespace ClashersRepublic.Magic.Proxy.Account
{
    using ClashersRepublic.Magic.Proxy.Session;
    
    internal class GameAccount
    {
        public int HighId;
        public int LowId;

        public string PassToken;
        public string AccountCreationDate;

        /// <summary>
        ///     Gets the current session instance.
        /// </summary>
        internal GameSession CurrentSession { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount" /> class.
        /// </summary>
        internal GameAccount()
        {
            // GameAccount.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount" /> class.
        /// </summary>
        internal GameAccount(int highId, int lowId, string passToken)
        {
            this.HighId = highId;
            this.LowId = lowId;
            this.PassToken = passToken;
        }

        /// <summary>
        ///     Sets the session instance.
        /// </summary>
        internal void SetSession(GameSession session)
        {
            this.CurrentSession = session;
        }
    }
}