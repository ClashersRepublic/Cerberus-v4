namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Message;

    public class KeepAliveMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeepAliveMessage" /> class.
        /// </summary>
        public KeepAliveMessage() : this(0)
        {
            // KeepAliveMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginFailedMessage" /> class.
        /// </summary>
        public KeepAliveMessage(short messageVersion) : base(messageVersion)
        {
            // KeepAliveMessage.
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
            return 10108;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 1;
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