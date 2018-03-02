namespace ClashersRepublic.Magic.Titan.Message.Security
{
    public class ClientCryptoErrorMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientCryptoErrorMessage" /> message.
        /// </summary>
        public ClientCryptoErrorMessage() : this(0)
        {
            // ClientCryptoErrorMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientCryptoErrorMessage" /> message.
        /// </summary>
        public ClientCryptoErrorMessage(short messageVersion) : base(messageVersion)
        {
            // ClientCryptoErrorMessage.
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(0);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.Stream.ReadInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10099;
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