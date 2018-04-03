namespace LineageSoft.Magic.Services.Avatar.Network.Session
{
    using LineageSoft.Magic.Services.Core.Network.Session;

    using LineageSoft.Magic.Services.Avatar.Game;
    using LineageSoft.Magic.Services.Avatar.Game.Message;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Core.Message;
    using LineageSoft.Magic.Services.Core.Message.Session;
    using LineageSoft.Magic.Services.Core.Utils;

    internal class NetAvatarSession : NetSession
    {
        /// <summary>
        ///     Gets the <see cref="Game.AvatarAccount"/> instance.
        /// </summary>
        internal AvatarAccount AvatarAccount { get; private set; }

        /// <summary>
        ///     Gets the piranha <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager PiranhaMessageManager { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetAvatarSession"/> class.
        /// </summary>
        internal NetAvatarSession(AvatarAccount avatarAccount, byte[] sessionId) : base(sessionId)
        {
            this.AvatarAccount = avatarAccount;
            this.PiranhaMessageManager = new MessageManager(this);
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

            this.AvatarAccount = null;
        }
    }
}