namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Logic.Message;

    public class DisconnectMessage : PiranhaMessage
    {
        public string SessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DisconnectMessage"/> class.
        /// </summary>
        public DisconnectMessage() : this(0)
        {
            // DisconnectMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DisconnectMessage"/> class.
        /// </summary>
        public DisconnectMessage(short messageVersion) : base(messageVersion)
        {
            // DisconnectMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.SessionId = this.Stream.ReadString(64);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this.SessionId);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20250;
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