namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;

    public class LoginAccountOkMessage : MagicServiceMessage
    {
        public LogicLong AccountId;
        public LogicLong HomeId;

        public string PassToken;



        /// <summary>
        ///     Initializes a new instance <see cref="LoginAccountOkMessage" /> class.
        /// </summary>
        public LoginAccountOkMessage() : this(0)
        {
            // LoginAccountOkMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="LoginAccountOkMessage" /> class.
        /// </summary>
        public LoginAccountOkMessage(short messageVersion) : base(messageVersion)
        {
            // LoginAccountOkMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.AccountId = this.Stream.ReadLong();
            this.PassToken = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this.AccountId);
            this.Stream.WriteString(this.PassToken);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10150;
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