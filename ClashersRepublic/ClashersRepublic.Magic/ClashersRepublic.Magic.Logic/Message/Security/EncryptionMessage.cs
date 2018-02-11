namespace ClashersRepublic.Magic.Logic.Message.Security
{
    using ClashersRepublic.Magic.Titan.Message;

    public class EncryptionMessage : PiranhaMessage
    {
        public byte[] ServerNonce;
        public int Version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptionMessage"/> class.
        /// </summary>
        public EncryptionMessage() : this(0)
        {
            // EncryptionMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EncryptionMessage"/> class.
        /// </summary>
        public EncryptionMessage(short messageVersion) : base(messageVersion)
        {
            this.ServerNonce = new byte[0];
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.ServerNonce = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 900000);
            this.Version = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteBytes(this.ServerNonce, this.ServerNonce.Length);
            this.Stream.WriteInt(this.Version);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20000;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 1;
        }
    }
}