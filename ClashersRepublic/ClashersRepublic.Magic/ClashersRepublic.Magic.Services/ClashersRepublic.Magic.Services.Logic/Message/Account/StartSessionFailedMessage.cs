namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Logic.Message;

    public class StartSessionFailedMessage : PiranhaMessage
    {
        public int ErrorCode;
        public string SessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartSessionFailedMessage"/> class.
        /// </summary>
        public StartSessionFailedMessage() : this(0)
        {
            // StartSessionFailedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartSessionOkMessage"/> class.
        /// </summary>
        public StartSessionFailedMessage(short messageVersion) : base(messageVersion)
        {
            // StartSessionFailedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.SessionId = this.Stream.ReadString(64);
            this.ErrorCode = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this.SessionId);
            this.Stream.WriteVInt(this.ErrorCode);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20201;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 2;
        }
    }
}