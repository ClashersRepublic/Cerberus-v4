namespace ClashersRepublic.Magic.Services.Logic.Message.Session
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateSessionMessage : MagicServiceMessage
    {
        public LogicLong AccountId;

        /// <summary>
        ///     Initializes a new instance <see cref="CreateSessionMessage" /> class.
        /// </summary>
        public CreateSessionMessage() : this(0)
        {
            // CreateSessionMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="CreateSessionMessage" /> class.
        /// </summary>
        public CreateSessionMessage(short messageVersion) : base(messageVersion)
        {
            // CreateSessionMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.AccountId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this.AccountId);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10500;
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 15;
        }
    }
}