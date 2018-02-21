namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    public class LoginFailedMessage : PiranhaMessage
    {
        public LogicArrayList<string> ContentUrlList;

        public bool BannedShowHelpshiftContact;

        public int ErrorCode;
        public int EndMaintenanceTime;

        public string ResourceFingerprintContent;
        public string RedirectDomain;
        public string Reason;
        public string UpdateUrl;
        public string ContentUrl;

        public byte[] ResourceFingerprintData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginFailedMessage" /> class.
        /// </summary>
        public LoginFailedMessage() : this(9)
        {
            // LoginFailedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginFailedMessage" /> class.
        /// </summary>
        public LoginFailedMessage(short messageVersion) : base(messageVersion)
        {
            this.ResourceFingerprintData = new byte[0];
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.ErrorCode = this.Stream.ReadInt();
            this.ResourceFingerprintContent = this.Stream.ReadString(900000);
            this.RedirectDomain = this.Stream.ReadString(900000);
            this.ContentUrl = this.Stream.ReadString(900000);
            this.UpdateUrl = this.Stream.ReadString(900000);
            this.Reason = this.Stream.ReadString(900000);
            this.EndMaintenanceTime = this.Stream.ReadInt();
            this.BannedShowHelpshiftContact = this.Stream.ReadBoolean();
            this.ResourceFingerprintData = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 900000);

            int contentUrlListSize = this.Stream.ReadInt();

            if (contentUrlListSize != -1)
            {
                this.ContentUrlList = new LogicArrayList<string>(contentUrlListSize);

                if (contentUrlListSize != 0)
                {
                    for (int i = 0; i < contentUrlListSize; i++)
                    {
                        this.ContentUrlList.Add(this.Stream.ReadString(900000));
                    }
                }
            }

            this.Stream.ReadInt();
            this.Stream.ReadInt();
            this.Stream.ReadString(900000);
            this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this.ErrorCode);
            this.Stream.WriteString(this.ResourceFingerprintContent);
            this.Stream.WriteString(this.RedirectDomain);
            this.Stream.WriteString(this.ContentUrl);
            this.Stream.WriteString(this.UpdateUrl);
            this.Stream.WriteString(this.Reason);
            this.Stream.WriteInt(this.EndMaintenanceTime);
            this.Stream.WriteBoolean(this.BannedShowHelpshiftContact);
            this.Stream.WriteBytes(this.ResourceFingerprintData, this.ResourceFingerprintData.Length);

            if (this.ContentUrlList != null)
            {
                this.Stream.WriteInt(this.ContentUrlList.Count);

                for (int i = 0; i < this.ContentUrlList.Count; i++)
                {
                    this.Stream.WriteString(this.ContentUrlList[i]);
                }
            }
            else
            {
                this.Stream.WriteInt(-1);
            }

            this.Stream.WriteInt(0);
            this.Stream.WriteInt(0);
            this.Stream.WriteString(null);
            this.Stream.WriteInt(-1);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20103;
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

            this.ContentUrl = null;
            this.ContentUrlList = null;
            this.Reason = null;
            this.RedirectDomain = null;
            this.ResourceFingerprintContent = null;
            this.UpdateUrl = null;
        }
    }
}