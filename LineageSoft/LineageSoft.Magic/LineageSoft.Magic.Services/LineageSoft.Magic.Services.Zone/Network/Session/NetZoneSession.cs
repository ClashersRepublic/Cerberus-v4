namespace LineageSoft.Magic.Services.Zone.Network.Session
{
    using LineageSoft.Magic.Services.Core.Network.Session;

    using LineageSoft.Magic.Services.Zone.Game;
    using LineageSoft.Magic.Services.Zone.Game.Message;

    internal class NetZoneSession : NetSession
    {
        /// <summary>
        ///     Gets the <see cref="Game.ZoneAccount"/> instance.
        /// </summary>
        internal ZoneAccount ZoneAccount { get; private set; }

        /// <summary>
        ///     Gets the <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager PiranhaMessageManager { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetZoneSession"/> class.
        /// </summary>
        internal NetZoneSession(ZoneAccount zoneAccount, byte[] sessionId) : base(sessionId)
        {
            this.ZoneAccount = zoneAccount;
            this.PiranhaMessageManager = new MessageManager(zoneAccount.GameMode);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this.PiranhaMessageManager != null)
            {
                this.PiranhaMessageManager.Destruct();
                this.PiranhaMessageManager = null;
            }

            if (this.ZoneAccount != null)
            {
                this.ZoneAccount.SetSession(null);
                this.ZoneAccount = null;
            }
        }
    }
}