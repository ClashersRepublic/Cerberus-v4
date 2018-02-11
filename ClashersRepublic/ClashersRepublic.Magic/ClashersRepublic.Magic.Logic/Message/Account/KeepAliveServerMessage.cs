namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Message;

    public class KeepAliveServerMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeepAliveServerMessage" /> class.
        /// </summary>
        public KeepAliveServerMessage() : this(0)
        {
            // KeepAliveServerMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="KeepAliveServerMessage" /> class.
        /// </summary>
        public KeepAliveServerMessage(short messageVersion) : base(messageVersion)
        {
            // KeepAliveServerMessage.
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
            return 20108;
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

        /// <summary>
        ///     Destructors of this instance.
        /// </summary>
        ~KeepAliveServerMessage()
        {
            this.Stream = null;
            this.Version = 0;
        }
    }
}