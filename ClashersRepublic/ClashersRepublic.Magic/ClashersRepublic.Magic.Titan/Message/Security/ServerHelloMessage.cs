namespace ClashersRepublic.Magic.Titan.Message.Security
{
    public class ServerHelloMessage : PiranhaMessage
    {
        private byte[] _serverNonce;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerHelloMessage" /> message.
        /// </summary>
        public ServerHelloMessage() : this(0)
        {
            // ServerHelloMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerHelloMessage" /> message.
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
            this.Stream.WriteBytes(this._serverNonce, this._serverNonce.Length);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._serverNonce = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 24);
        }

        /// <summary>
        ///     Removes the server nonce.
        /// </summary>
        public byte[] RemoveServerNonce()
        {
            byte[] tmp = this._serverNonce;
            this._serverNonce = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the server nonce
        /// </summary>
        public void SetServerNonce(byte[] value)
        {
            this._serverNonce = value;
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
        public override void Destruct()
        {
            this._serverNonce = null;
        }
    }
}