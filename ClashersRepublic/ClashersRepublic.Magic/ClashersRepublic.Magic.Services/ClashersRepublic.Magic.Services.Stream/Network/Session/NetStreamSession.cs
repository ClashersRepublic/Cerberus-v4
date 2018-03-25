namespace ClashersRepublic.Magic.Services.Stream.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Services.Stream.Game;
    using ClashersRepublic.Magic.Services.Stream.Game.Message;

    internal class NetStreamSession : NetSession
    {
        /// <summary>
        ///     Gets the stream instance.
        /// </summary>
        internal Stream Stream { get; private set; }

        /// <summary>
        ///     Gets the piranha <see cref="MessageManager"/> instance.
        /// </summary>
        internal MessageManager PiranhaMessageManager { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetStreamSession"/> class.
        /// </summary>
        internal NetStreamSession(Stream stream, byte[] sessionId) : base(sessionId)
        {
            this.Stream = stream;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this.PiranhaMessageManager = null;
            this.Stream = null;
        }
    }
}