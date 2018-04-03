namespace RivieraStudio.Magic.Logic.Message.Security
{
    using RivieraStudio.Magic.Titan.Message.Security;

    public class ExtendedSetEncryptionMessage : SetEncryptionMessage
    {
        public const string INTEGRATION_NONCE = "77035c098d0a04753b77167c7133cdd4b7052813ed47c461";
        public const string STAGE_NONCE = "4c444a4b4c396876736b6c3b6473766b666c73676a90fbef";
        public const string DEFAULT_NONCE = "nonce";

        private int _nonceMethod;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedSetEncryptionMessage" /> class.
        /// </summary>
        public ExtendedSetEncryptionMessage() : this(0)
        {
            // ExtendedSetEncryptionMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtendedSetEncryptionMessage" /> class.
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
            this._nonceMethod = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._nonceMethod);
        }
        
        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
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