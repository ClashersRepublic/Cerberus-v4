namespace ClashersRepublic.Magic.Proxy.Account
{
    using ClashersRepublic.Magic.Proxy.Session;
    using ClashersRepublic.Magic.Services.Logic.Math;
    using ClashersRepublic.Magic.Titan.Math;
    using Newtonsoft.Json;

    internal class GameAccount
    {
        [JsonConverter(typeof(LogicLongSerializer))] public LogicLong Id;

        public string PassToken;
        public string AccountCreationDate;

        /// <summary>
        ///     Gets the current session instance.
        /// </summary>
        internal GameSession CurrentSession { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount" /> class.
        /// </summary>
        public GameAccount()
        {
            // GameAccount.
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