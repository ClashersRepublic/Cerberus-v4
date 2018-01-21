namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Math;

    public class LoginMessage : PiranhaMessage
    {
        public LogicLong AccountId;
        public LogicData PreferredLanguage;

        public string PassToken;
        public string ResourceSha;
        public string UDID;
        public string OpenUDID;
        public string ADID;
        public string IMEI;
        public string OSVersion;
        public string MacAddress;
        public string Device;
        public string PreferredDeviceLanguage;
        public string AndroidID;

        public int ClientMajorVersion;
        public int ClientBuildVersion;
        public int ScramblerSeed;

        public bool AndroidClient;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginMessage"/> class.
        /// </summary>
        public LoginMessage() : this(8)
        {
            // LoginMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginMessage"/> class.
        /// </summary>
        public LoginMessage(short messageVersion) : base(messageVersion)
        {
            this.IMEI = string.Empty;
            this.AndroidID = string.Empty;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.AccountId = this.Stream.ReadLong();
            this.PassToken = this.Stream.ReadString(900000);
            this.ClientMajorVersion = this.Stream.ReadInt();
            this.Stream.ReadInt();
            this.ClientBuildVersion = this.Stream.ReadInt();
            this.ResourceSha = this.Stream.ReadString(900000);
            this.UDID = this.Stream.ReadString(900000);
            this.OpenUDID = this.Stream.ReadString(900000);
            this.MacAddress = this.Stream.ReadString(900000);
            this.Device = this.Stream.ReadString(900000);

            if (!this.Stream.IsAtEnd)
            {
                this.PreferredLanguage = this.Stream.ReadDataReference(1);
                this.PreferredDeviceLanguage = this.Stream.ReadString(900000);

                if (this.PreferredDeviceLanguage == null)
                {
                    this.PreferredDeviceLanguage = string.Empty;
                }

                if (!this.Stream.IsAtEnd)
                {
                    this.ADID = this.Stream.ReadString(900000);

                    if (!this.Stream.IsAtEnd)
                    {
                        this.OSVersion = this.Stream.ReadString(900000);

                        if (!this.Stream.IsAtEnd)
                        {
                            this.AndroidClient = this.Stream.ReadBoolean();

                            if (!this.Stream.IsAtEnd)
                            {
                                this.IMEI = this.Stream.ReadStringReference(900000);
                                this.AndroidID = this.Stream.ReadStringReference(900000);

                                if (!this.Stream.IsAtEnd)
                                {
                                    this.Stream.ReadString(900000);

                                    if (!this.Stream.IsAtEnd)
                                    {
                                        this.Stream.ReadBoolean();
                                        this.Stream.ReadString(900000);

                                        if (!this.Stream.IsAtEnd)
                                        {
                                            this.ScramblerSeed = this.Stream.ReadInt();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10101;
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 1;
        }
    }
}