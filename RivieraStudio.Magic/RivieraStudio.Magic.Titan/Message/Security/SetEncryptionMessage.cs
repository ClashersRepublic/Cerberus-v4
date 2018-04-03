namespace RivieraStudio.Magic.Titan.Message.Security
{
    using RivieraStudio.Magic.Titan.Message;

    public class SetEncryptionMessage : PiranhaMessage
    {
        private byte[] _nonce;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetEncryptionMessage" /> class.
        /// </summary>
        public SetEncryptionMessage() : this(0)
        {
            // SetEncryptionMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetEncryptionMessage" /> class.
        /// </summary>
        public SetEncryptionMessage(short messageVersion) : base(messageVersion)
        {
            // SetEncryptionMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._nonce = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteBytes(this._nonce, this._nonce.Length);
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
    }
}