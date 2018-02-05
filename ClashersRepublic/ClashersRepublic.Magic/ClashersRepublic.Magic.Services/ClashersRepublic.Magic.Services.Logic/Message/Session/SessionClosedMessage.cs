namespace ClashersRepublic.Magic.Services.Logic.Message.Session
{
    public class SessionClosedMessage : ServiceMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SessionClosedMessage"/> class.
        /// </summary>
        public SessionClosedMessage() : base(0)
        {
            // SessionClosedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20110;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 0;
        }
    }
}