namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    public class CreateAccountMessage : MagicServiceMessage
    {
        public bool StartSession;

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAccountMessage"/> class.
        /// </summary>
        public CreateAccountMessage() : this(0)
        {
            // CreateAccountMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAccountMessage"/> class.
        /// </summary>
        public CreateAccountMessage(short messageVersion) : base(messageVersion)
        {
            // CreateAccountMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.StartSession = this.Stream.ReadBoolean();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteBoolean(this.StartSession);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10105;
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