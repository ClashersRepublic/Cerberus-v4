namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;

    public class LoginAccountMessage : MagicServiceMessage
    {
        public LogicLong AccountId;
        public string PassToken;

        /// <summary>
        ///     Initializes a new instance <see cref="LoginAccountMessage" /> class.
        /// </summary>
        public LoginAccountMessage() : this(0)
        {
            // LoginMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="LoginAccountMessage" /> class.
        /// </summary>
        public LoginAccountMessage(short messageVersion) : base(messageVersion)
        {
            // LoginMessage.
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