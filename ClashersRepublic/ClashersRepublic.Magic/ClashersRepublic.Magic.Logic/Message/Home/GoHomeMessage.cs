namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Titan.Message;

    public class GoHomeMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GoHomeMessage" /> class.
        /// </summary>
        public GoHomeMessage() : this(0)
        {
            // GoHomeMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GoHomeMessage" /> class.
        /// </summary>
        public GoHomeMessage(short messageVersion) : base(messageVersion)
        {
            // GoHomeMessage.
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
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 14101;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }
    }
}