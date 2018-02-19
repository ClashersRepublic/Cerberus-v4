namespace ClashersRepublic.Magic.Logic.Message.Security
{
    using ClashersRepublic.Magic.Titan.Message;

    public class ExtendedSetEncryptionMessage : PiranhaMessage
    {
        private byte[] _nonce;
        private int _nonceMethod;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedSetEncryptionMessage"/> class.
        /// </summary>
        public ExtendedSetEncryptionMessage() : this(0)
        {
            // ExtendedSetEncryptionMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedSetEncryptionMessage"/> class.
        /// </summary>
        public ExtendedSetEncryptionMessage(short messageVersion) : base(messageVersion)
        {
            // ExtendedSetEncryptionMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._nonce = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 900000);
            this._nonceMethod = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteBytes(this._nonce, this._nonce.Length);
            this.Stream.WriteInt(this._nonceMethod);
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

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._nonce = null;
        }

        /// <summary>
        ///     Removes the nonce.
        /// </summary>
        public byte[] RemoveNonce()
        {
            byte[] tmp = this._nonce;
            this._nonce = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the nonce.
        /// </summary>
        public void SetNonce(byte[] value)
        {
            this._nonce = value;
        }

        /// <summary>
        ///     Gets the nonce method.
        /// </summary>
        public int GetNonceMethod()
        {
            return this._nonceMethod;
        }

        /// <summary>
        ///     Sets the nonce method.
        /// </summary>
        public void SetNonceMethod(int value)
        {
            this._nonceMethod = value;
        }
    }
}