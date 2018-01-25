namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Util;

    public class LoginFailedMessage : PiranhaMessage
    {
        public bool BannedShowHelpshiftContact;
        public string ContentUrl;

        public LogicArrayList<string> ContentUrlList;
        public int EndMaintenanceTime;
        public int ErrorCode;
        public string Reason;
        public string RedirectDomain;

        public string ResourceFingerprintContent;

        public byte[] ResourceFingerprintData;
        public string UpdateUrl;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginFailedMessage" />
        /// </summary>
        public LoginFailedMessage() : this(9)
        {
            // LoginFailedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginFailedMessage" />
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
    }
}