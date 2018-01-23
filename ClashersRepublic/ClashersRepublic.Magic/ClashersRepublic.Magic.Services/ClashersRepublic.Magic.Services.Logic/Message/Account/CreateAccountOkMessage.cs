namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateAccountOkMessage : MagicServiceMessage
    {
        public GameAccount Account;
        
        /// <summary>
        ///     Initializes a new instance <see cref="CreateAccountOkMessage"/> class.
        /// </summary>
        public CreateAccountOkMessage() : this(0)
        {
            // CreateAccountOkMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAccountOkMessage"/> class.
        /// </summary>
        public CreateAccountOkMessage(short messageVersion) : base(messageVersion)
        {
            // CreateAccountOkMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.Account = new GameAccount();
            this.Account.Decode(this.Stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Account.Encode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 20101;
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 2;
        }
    }
}