namespace ClashersRepublic.Magic.Titan.Message.Security
{
    public class ClientHelloMessage : PiranhaMessage
    {
        private int _protocol;
        private int _keyVersion;
        private int _majorVersion;
        private int _minorVersion;
        private int _buildVersion;
        private int _deviceType;
        private int _appStore;

        private string _contentHash;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientHelloMessage" /> message.
        /// </summary>
        public ClientHelloMessage() : this(0)
        {
            // ClientHelloMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientHelloMessage" /> message.
        /// </summary>
        public ClientHelloMessage(short messageVersion) : base(messageVersion)
        {
            this._contentHash = string.Empty;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this._protocol);
            this.Stream.WriteInt(this._keyVersion);
            this.Stream.WriteInt(this._majorVersion);
            this.Stream.WriteInt(this._minorVersion);
            this.Stream.WriteInt(this._buildVersion);
            this.Stream.WriteStringReference(this._contentHash);
            this.Stream.WriteInt(this._deviceType);
            this.Stream.WriteInt(this._appStore);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._protocol = this.Stream.ReadInt();
            this._keyVersion = this.Stream.ReadInt();
            this._majorVersion = this.Stream.ReadInt();
            this._minorVersion = this.Stream.ReadInt();
            this._buildVersion = this.Stream.ReadInt();
            this._contentHash = this.Stream.ReadStringReference(900000);
            this._deviceType = this.Stream.ReadInt();
            this._appStore = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10100;
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
            this._contentHash = null;
        }

        /// <summary>
        ///     Gets the protocol version.
        /// </summary>
        public int GetProtocol()
        {
            return this._protocol;
        }

        /// <summary>
        ///     Sets the protocol version.
        /// </summary>
        public void SetProtocol(int value)
        {
            this._protocol = value;
        }

        /// <summary>
        ///     Gets the key version.
        /// </summary>
        public int GetKeyVersion()
        {
            return this._keyVersion;
        }

        /// <summary>
        ///     Sets the key version.
        /// </summary>
        public void SetKeyVersion(int value)
        {
            this._keyVersion = value;
        }

        /// <summary>
        ///     Gets the major version.
        /// </summary>
        public int GetMajorVersion()
        {
            return this._majorVersion;
        }

        /// <summary>
        ///     Sets the major version.
        /// </summary>
        public void SetMajorVersion(int value)
        {
            this._majorVersion = value;
        }

        /// <summary>
        ///     Gets the minor version.
        /// </summary>
        public int GetMinorVersion()
        {
            return this._minorVersion;
        }

        /// <summary>
        ///     Sets the minor version.
        /// </summary>
        public void SetMinorVersion(int value)
        {
            this._minorVersion = value;
        }

        /// <summary>
        ///     Gets the build version.
        /// </summary>
        public int GetBuildVersion()
        {
            return this._buildVersion;
        }

        /// <summary>
        ///     Sets the build version.
        /// </summary>
        public void SetBuildVersion(int value)
        {
            this._buildVersion = value;
        }

        /// <summary>
        ///     Gets the content hash.
        /// </summary>
        public string GetContentHash()
        {
            return this._contentHash;
        }

        /// <summary>
        ///     Sets the content hash.
        /// </summary>
        public void SetContentHash(string value)
        {
            this._contentHash = value;
        }

        /// <summary>
        ///     Gets the device type.
        /// </summary>
        public int GetDeviceType()
        {
            return this._deviceType;
        }

        /// <summary>
        ///     Sets the device type.
        /// </summary>
        public void SetDeviceType(int value)
        {
            this._deviceType = value;
        }

        /// <summary>
        ///     Gets the app store.
        /// </summary>
        public int GetAppStore()
        {
            return this._appStore;
        }

        /// <summary>
        ///     Sets the app store.
        /// </summary>
        public void SetAppStore(int value)
        {
            this._appStore = value;
        }
    }
}