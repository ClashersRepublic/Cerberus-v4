namespace LineageSoft.Magic.Services.Party.Network.Session
{
    using LineageSoft.Magic.Logic.Message.Avatar.Stream;
    using LineageSoft.Magic.Services.Core.Network.Session;
    using LineageSoft.Magic.Services.Core.Utils;
    using LineageSoft.Magic.Services.Party.Game;
    using LineageSoft.Magic.Services.Party.Game.Message;

    internal class NetPartySession : NetSession
    {
        /// <summary>
        ///     Gets the partyAccount instance.
        /// </summary>
        internal PartyAccount PartyAccount { get; private set; }

        /// <summary>
        ///     Gets the piranha <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager PiranhaMessageManager { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPartySession"/> class.
        /// </summary>
        internal NetPartySession(PartyAccount partyAccount, byte[] sessionId) : base(sessionId)
        {
            this.PartyAccount = partyAccount;
        }

        /// <summary>
        ///     Sends the avatar entry to client.
        /// </summary>
        internal void SendAvatarStreamMessage()
        {
            AvatarStreamMessage message = new AvatarStreamMessage();
            message.SetStreamEntries(this.PartyAccount.GetStreamEntries());
            this.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, message);
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

            this.PartyAccount = null;
        }
    }
}