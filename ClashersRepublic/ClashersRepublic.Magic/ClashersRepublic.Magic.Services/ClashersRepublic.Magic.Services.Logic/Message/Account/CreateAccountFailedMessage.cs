namespace ClashersRepublic.Magic.Services.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateAccountFailedMessage : MagicServiceMessage
    {
        public int ErrorCode;

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAccountFailedMessage"/> class.
        /// </summary>
        public CreateAccountFailedMessage() : this(0)
        {
            // CreateAccountFailedMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAccountFailedMessage"/> class.
        /// </summary>
        public CreateAccountFailedMessage(short messageVersion) : base(messageVersion)
        {
            // CreateAccountFailedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.ErrorCode = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this.ErrorCode);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 20102;
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