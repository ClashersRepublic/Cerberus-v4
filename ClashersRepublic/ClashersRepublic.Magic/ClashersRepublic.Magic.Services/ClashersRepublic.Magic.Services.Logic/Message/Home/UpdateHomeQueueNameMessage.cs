namespace ClashersRepublic.Magic.Services.Logic.Message.Home
{
    public class UpdateHomeQueueNameMessage : ServiceMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateHomeQueueNameMessage"/> class.
        /// </summary>
        public UpdateHomeQueueNameMessage() : base(0)
        {
            // SetHomeQueueNameMessage.
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
            return 20190;
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