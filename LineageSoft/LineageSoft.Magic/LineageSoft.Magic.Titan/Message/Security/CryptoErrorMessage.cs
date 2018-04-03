namespace LineageSoft.Magic.Titan.Message.Security
{
    public class CryptoErrorMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CryptoErrorMessage" /> message.
        /// </summary>
        public CryptoErrorMessage() : this(0)
        {
            // CryptoErrorMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CryptoErrorMessage" /> message.
        /// </summary>
        public CryptoErrorMessage(short messageVersion) : base(messageVersion)
        {
            // CryptoErrorMessage.
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(0);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 29997;
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