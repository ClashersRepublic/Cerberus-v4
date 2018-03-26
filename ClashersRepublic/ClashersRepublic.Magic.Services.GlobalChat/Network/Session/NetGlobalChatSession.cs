namespace ClashersRepublic.Magic.Services.GlobalChat.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Services.GlobalChat.Game;
    using ClashersRepublic.Magic.Services.GlobalChat.Game.Message;
    
    internal class NetGlobalChatSession : NetSession
    {
        /// <summary>
        ///     Gets the piranha <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager PiranhaMessageManager { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Core.Message.Avatar.AvatarEntry"/> instnace.
        /// </summary>
        internal AvatarEntry AvatarEntry { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Game.Room"/> instance.
        /// </summary>
        internal Room Room { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetGlobalChatSession"/> class.
        /// </summary>
        internal NetGlobalChatSession(byte[] sessionId) : base(sessionId)
        {
            this.AvatarEntry = new AvatarEntry();
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

            if (this.Room != null)
            {
                this.Room.RemoveSession(this);
                this.Room = null;
            }

            this.AvatarEntry = null;
        }

        /// <summary>
        ///     Sets the room instance.
        /// </summary>
        public void SetRoom(Room room)
        {
            this.Room = room;
        }
    }
}