namespace LineageSoft.Magic.Titan.Message.Account
{
    using LineageSoft.Magic.Titan.Message;

    public class TitanDisconnectedMessage : PiranhaMessage
    {
        private int _reason;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TitanDisconnectedMessage" /> class.
        /// </summary>
        public TitanDisconnectedMessage() : this(0)
        {
            // TitanDisconnectedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TitanDisconnectedMessage" /> class.
        /// </summary>
        public TitanDisconnectedMessage(short messageVersion) : base(messageVersion)
        {
            // TitanDisconnectedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._reason = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._reason);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 25892;
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
        ///     Gets the reason.
        /// </summary>
        public int GetReason()
        {
            return this._reason;
        }

        /// <summary>
        ///     Sets the reason;
        /// </summary>
        public void SetReason(int value)
        {
            this._reason = value;
        }
    }
}