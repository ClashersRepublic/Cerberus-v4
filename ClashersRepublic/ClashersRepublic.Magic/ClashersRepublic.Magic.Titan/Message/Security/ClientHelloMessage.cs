namespace ClashersRepublic.Magic.Titan.Message.Security
{
    public class ClientHelloMessage : PiranhaMessage
    {
        public int Protocol;
        public int KeyVersion;
        public int MajorVersion;
        public int MinorVersion;
        public int BuildVersion;
        public int DeviceType;
        public int AppStore;

        public string ContentHash;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientHelloMessage"/> message.
        /// </summary>
        public ClientHelloMessage() : this(0)
        {
            // ClientHelloMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientHelloMessage"/> message.
        /// </summary>
        public ClientHelloMessage(short messageVersion) : base(messageVersion)
        {
            this.ContentHash = string.Empty;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this.Protocol);
            this.Stream.WriteInt(this.KeyVersion);
            this.Stream.WriteInt(this.MajorVersion);
            this.Stream.WriteInt(this.MinorVersion);
            this.Stream.WriteInt(this.BuildVersion);
            this.Stream.WriteStringReference(this.ContentHash);
            this.Stream.WriteInt(this.DeviceType);
            this.Stream.WriteInt(this.AppStore);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.Protocol = this.Stream.ReadInt();
            this.KeyVersion = this.Stream.ReadInt();
            this.MajorVersion = this.Stream.ReadInt();
            this.MinorVersion = this.Stream.ReadInt();
            this.BuildVersion = this.Stream.ReadInt();
            this.ContentHash = this.Stream.ReadStringReference(900000);
            this.DeviceType = this.Stream.ReadInt();
            this.AppStore = this.Stream.ReadInt();
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
        public void Destruct()
        {
            this.ContentHash = string.Empty;
        }

        /// <summary>
        ///     Deconstructor of this instance.
        /// </summary>
        ~ClientHelloMessage()
        {
            this.ContentHash = null;
        }
    }
}