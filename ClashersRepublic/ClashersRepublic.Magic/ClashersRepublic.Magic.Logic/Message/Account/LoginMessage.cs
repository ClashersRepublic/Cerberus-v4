namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using System;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    public class LoginMessage : PiranhaMessage
    {
        public LogicLong AccountId;
        public LogicData PreferredLanguage;

        public bool AndroidClient;
        public bool AdvertisingEnabled;

        public int ScramblerSeed;
        public int ClientMajorVersion;
        public int ClientBuildVersion;
        public int AppStore;

        public string AndroidID;
        public string ADID;
        public string Device;
        public string IMEI;
        public string MacAddress;
        public string OpenUDID;
        public string OSVersion;
        public string AdvertisingId;

        public string PassToken;
        public string PreferredDeviceLanguage;
        public string ResourceSha;
        public string UDID;

        public string KunlunSSO;
        public string KunlunUID;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginMessage" /> class.
        /// </summary>
        public LoginMessage() : this(8)
        {
            // LoginMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginMessage" /> class.
        /// </summary>
        public LoginMessage(short messageVersion) : base(messageVersion)
        {
            this.IMEI = string.Empty;
            this.AndroidID = string.Empty;
            this.KunlunSSO = string.Empty;
            this.KunlunUID = string.Empty;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this.AccountId);
            this.Stream.WriteString(this.PassToken);
            this.Stream.WriteInt(this.ClientMajorVersion);
            this.Stream.WriteInt(0);
            this.Stream.WriteInt(this.ClientBuildVersion);
            this.Stream.WriteString(this.ResourceSha);
            this.Stream.WriteString(this.UDID);
            this.Stream.WriteString(this.OpenUDID);
            this.Stream.WriteString(this.MacAddress);
            this.Stream.WriteString(this.Device);
            this.Stream.WriteDataReference(this.PreferredLanguage);
            this.Stream.WriteString(this.PreferredDeviceLanguage);
            this.Stream.WriteString(this.ADID);
            this.Stream.WriteString(this.OSVersion);
            this.Stream.WriteBoolean(this.AndroidClient);
            this.Stream.WriteStringReference(this.IMEI);
            this.Stream.WriteStringReference(this.AndroidID);
            this.Stream.WriteString("");
            this.Stream.WriteBoolean(false);
            this.Stream.WriteString("");
            this.Stream.WriteInt(this.ScramblerSeed);
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

            if (!this.Stream.IsAtEnd())
            {
                this.PreferredLanguage = this.Stream.ReadDataReference(1);
                this.PreferredDeviceLanguage = this.Stream.ReadString(900000);

                if (this.PreferredDeviceLanguage == null)
                {
                    this.PreferredDeviceLanguage = string.Empty;
                }

                if (!this.Stream.IsAtEnd())
                {
                    this.ADID = this.Stream.ReadString(900000);

                    if (!this.Stream.IsAtEnd())
                    {
                        this.OSVersion = this.Stream.ReadString(900000);

                        if (!this.Stream.IsAtEnd())
                        {
                            this.AndroidClient = this.Stream.ReadBoolean();

                            if (!this.Stream.IsAtEnd())
                            {
                                this.IMEI = this.Stream.ReadStringReference(900000);
                                this.AndroidID = this.Stream.ReadStringReference(900000);
                                
                                if (!this.Stream.IsAtEnd())
                                {
                                    this.Stream.ReadString(900000);

                                    if (!this.Stream.IsAtEnd())
                                    {
                                        this.AdvertisingEnabled = this.Stream.ReadBoolean();
                                        this.AdvertisingId = this.Stream.ReadString(900000);

                                        if (!this.Stream.IsAtEnd())
                                        {
                                            this.ScramblerSeed = this.Stream.ReadInt();

                                            if (!this.Stream.IsAtEnd())
                                            {
                                                this.AppStore = this.Stream.ReadVInt();

                                                this.Stream.ReadStringReference(900000);
                                                this.Stream.ReadStringReference(900000);

                                                if (!this.Stream.IsAtEnd())
                                                {
                                                    this.Stream.ReadStringReference(900000);

                                                    if (!this.Stream.IsAtEnd())
                                                    {
                                                        this.KunlunSSO = this.Stream.ReadStringReference(900000);
                                                        this.KunlunUID = this.Stream.ReadStringReference(900000);

                                                        this.Stream.ReadVInt();
                                                    }
                                                }
                                            }
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

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this.ADID = null;
            this.PassToken = null;
            this.Device = null;
            this.IMEI = null;
            this.MacAddress = null;
            this.OSVersion = null;
            this.AndroidID = null;
            this.OpenUDID = null;
            this.PreferredDeviceLanguage = null;
            this.ResourceSha = null;
            this.KunlunSSO = null;
            this.KunlunUID = null;
            this.UDID = null;
        }
    }
}