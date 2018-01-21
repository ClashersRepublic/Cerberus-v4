namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Logic.Message;

    public class StopSessionMessage : PiranhaMessage
    {
        public string SessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StopSessionMessage"/> class.
        /// </summary>
        public StopSessionMessage() : this(0)
        {
            // StopSessionMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StopSessionMessage"/> class.
        /// </summary>
        public StopSessionMessage(short messageVersion) : base(messageVersion)
        {
            // StopSessionMessage.
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
            return 10210;
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