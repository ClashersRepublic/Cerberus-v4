namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Titan.Math;

    public class StartSessionMessage : PiranhaMessage
    {
        public LogicLong AccountId;

        public string PassToken;
        public string SessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartSessionMessage"/> class.
        /// </summary>
        public StartSessionMessage() : this(0)
        {
            // StartSessionMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartSessionMessage"/> class.
        /// </summary>
        public StartSessionMessage(short messageVersion) : base(messageVersion)
        {
            // StartSessionMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.SessionId = this.Stream.ReadString(64);
            this.AccountId = this.Stream.ReadLong();
            this.PassToken = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this.SessionId);
            this.Stream.WriteLong(this.AccountId);
            this.Stream.WriteString(this.PassToken);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10200;
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