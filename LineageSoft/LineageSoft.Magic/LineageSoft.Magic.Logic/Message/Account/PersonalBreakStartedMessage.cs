namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Message;

    public class PersonalBreakStartedMessage : PiranhaMessage
    {
        private int _secondsUntilBreak;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonalBreakStartedMessage" /> class.
        /// </summary>
        public PersonalBreakStartedMessage() : this(0)
        {
            // PersonalBreakStartedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonalBreakStartedMessage" /> class.
        /// </summary>
        public PersonalBreakStartedMessage(short messageVersion) : base(messageVersion)
        {
            // PersonalBreakStartedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._secondsUntilBreak = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._secondsUntilBreak);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20171;
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
        ///     Gets seconds until break.
        /// </summary>
        public int GetSecondsUntilBreak()
        {
            return this._secondsUntilBreak;
        }

        /// <summary>
        ///     Sets seconds until break.
        /// </summary>
        public void SetSecondsUntilBreak(int value)
        {
            this._secondsUntilBreak = value;
        }
    }
}