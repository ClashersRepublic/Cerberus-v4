namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Message;

    public class ShutdownStartedMessage : PiranhaMessage
    {
        private int _secondsUntilShutdown;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShutdownStartedMessage" /> class.
        /// </summary>
        public ShutdownStartedMessage() : this(0)
        {
            // ShutdownStartedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ShutdownStartedMessage" /> class.
        /// </summary>
        public ShutdownStartedMessage(short messageVersion) : base(messageVersion)
        {
            // ShutdownStartedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._secondsUntilShutdown = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._secondsUntilShutdown);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20161;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Gets the device token.
        /// </summary>
        public int GetSecondsUntilShutdown()
        {
            return this._secondsUntilShutdown;
        }

        /// <summary>
        ///     Sets the device token.
        /// </summary>
        public void SetSecondsUntilShutdown(int value)
        {
            this._secondsUntilShutdown = value;
        }
    }
}