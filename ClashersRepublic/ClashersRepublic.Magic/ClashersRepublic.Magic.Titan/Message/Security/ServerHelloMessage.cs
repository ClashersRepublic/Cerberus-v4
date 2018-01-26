namespace ClashersRepublic.Magic.Titan.Message.Security
{
    public class ServerHelloMessage : PiranhaMessage
    {
        public byte[] ServerKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerHelloMessage"/> message.
        /// </summary>
        public ServerHelloMessage() : this(0)
        {
            // ServerHelloMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerHelloMessage"/> message.
        /// </summary>
        public ServerHelloMessage(short messageVersion) : base(messageVersion)
        {
            // ServerHelloMessage.
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteBytes(this.ServerKey, this.ServerKey?.Length ?? 0);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.ServerKey = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 24);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 20100;
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
        public void Destruct()
        {
            this.ServerKey = null;
        }

        /// <summary>
        ///     Deconstructor of this instance.
        /// </summary>
        ~ServerHelloMessage()
        {
            this.ServerKey = null;
        }
    }
}