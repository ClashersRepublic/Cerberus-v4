namespace ClashersRepublic.Magic.Services.Logic.Message.Game
{
    public class CreateDataMessage : ServiceMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateDataMessage"/> class.
        /// </summary>
        public CreateDataMessage() : base()
        {
            // CreateDataMessage.
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
            return 10100;
        }
    }
}