namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Account;

    public class StartSessionOkMessage : PiranhaMessage
    {
        public string SessionId;
        public GameAccount Account;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartSessionOkMessage"/> class.
        /// </summary>
        public StartSessionOkMessage() : this(0)
        {
            // StartSessionOkMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StartSessionOkMessage"/> class.
        /// </summary>
        public StartSessionOkMessage(short messageVersion) : base(messageVersion)
        {
            // StartSessionOkMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.SessionId = this.Stream.ReadString(64);
            this.Account = new GameAccount();
            this.Account.Decode(this.Stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this.SessionId);
            this.Account.Encode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20200;
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