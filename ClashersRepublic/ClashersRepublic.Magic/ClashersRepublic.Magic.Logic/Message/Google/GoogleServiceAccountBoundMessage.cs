namespace ClashersRepublic.Magic.Logic.Message.Google
{
    using ClashersRepublic.Magic.Titan.Message;

    public class GoogleServiceAccountBoundMessage : PiranhaMessage
    {
        private int _resultCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GoogleServiceAccountBoundMessage" /> class.
        /// </summary>
        public GoogleServiceAccountBoundMessage() : this(0)
        {
            // GoogleServiceAccountBoundMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GoogleServiceAccountBoundMessage" /> class.
        /// </summary>
        public GoogleServiceAccountBoundMessage(short messageVersion) : base(messageVersion)
        {
            // GoogleServiceAccountBoundMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._resultCode = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._resultCode);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24261;
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
        ///     Removes the result code.
        /// </summary>
        public int GetResultCode()
        {
            return this._resultCode;
        }

        /// <summary>
        ///     Sets the result code.
        /// </summary>
        public void SetResultCode(int value)
        {
            this._resultCode = value;
        }
    }
}