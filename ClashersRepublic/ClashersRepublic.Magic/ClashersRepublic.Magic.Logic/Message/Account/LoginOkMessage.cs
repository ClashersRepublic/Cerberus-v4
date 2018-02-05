namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    public class LoginOkMessage : PiranhaMessage
    {
        public LogicLong AccountId;
        public LogicLong HomeId;

        public string PassToken;
        public string FacebookId;
        public string GamecenterId;
        public string ServerEnvironment;
        public string FacebookAppId;
        public string ServerTime;
        public string AccountCreatedDate;
        public string GoogleServiceId;
        public string Region;

        public int ServerMajorVersion;
        public int ServerBuildVersion;
        public int ContentVersion;
        public int SessionCount;
        public int PlayTimeSeconds;
        public int DaysSinceStartedPlaying;
        public int StartupCooldownSeconds;

        public LogicArrayList<string> ContentUrlList;
        public LogicArrayList<string> ChronosContentUrlList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginOkMessage" />
        /// </summary>
        public LoginOkMessage() : this(1)
        {
            // LoginOkMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LoginOkMessage" />
        /// </summary>
        public LoginOkMessage(short messageVersion) : base(messageVersion)
        {
            // LoginOkMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.AccountId = this.Stream.ReadLong();
            this.HomeId = this.Stream.ReadLong();
            this.PassToken = this.Stream.ReadString(900000);
            this.FacebookId = this.Stream.ReadString(900000);
            this.GamecenterId = this.Stream.ReadString(900000);
            this.ServerMajorVersion = this.Stream.ReadInt();
            this.ServerBuildVersion = this.Stream.ReadInt();
            this.ContentVersion = this.Stream.ReadInt();
            this.ServerEnvironment = this.Stream.ReadString(900000);
            this.SessionCount = this.Stream.ReadInt();
            this.PlayTimeSeconds = this.Stream.ReadInt();
            this.DaysSinceStartedPlaying = this.Stream.ReadInt();
            this.FacebookAppId = this.Stream.ReadString(900000);
            this.ServerTime = this.Stream.ReadString(900000);
            this.AccountCreatedDate = this.Stream.ReadString(900000);
            this.StartupCooldownSeconds = this.Stream.ReadInt();
            this.GoogleServiceId = this.Stream.ReadString(9000);
            this.Region = this.Stream.ReadString(9000);
            this.Stream.ReadString(9000);
            this.Stream.ReadInt();
            this.Stream.ReadString(9000);
            this.Stream.ReadString(9000);
            this.Stream.ReadString(9000);

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

            int chronosContentUrlListSize = this.Stream.ReadInt();

            if (chronosContentUrlListSize != -1)
            {
                this.ChronosContentUrlList = new LogicArrayList<string>(chronosContentUrlListSize);

                if (chronosContentUrlListSize != 0)
                {
                    for (int i = 0; i < chronosContentUrlListSize; i++)
                    {
                        this.ChronosContentUrlList.Add(this.Stream.ReadString(900000));
                    }
                }
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this.AccountId);
            this.Stream.WriteLong(this.HomeId);
            this.Stream.WriteString(this.PassToken);
            this.Stream.WriteString(this.FacebookId);
            this.Stream.WriteString(this.GamecenterId);
            this.Stream.WriteInt(this.ServerMajorVersion);
            this.Stream.WriteInt(this.ServerBuildVersion);
            this.Stream.WriteInt(this.ContentVersion);
            this.Stream.WriteString(this.ServerEnvironment);
            this.Stream.WriteInt(this.SessionCount);
            this.Stream.WriteInt(this.PlayTimeSeconds);
            this.Stream.WriteInt(this.DaysSinceStartedPlaying);
            this.Stream.WriteString(this.FacebookAppId);
            this.Stream.WriteString(this.ServerTime);
            this.Stream.WriteString(this.AccountCreatedDate);
            this.Stream.WriteInt(this.StartupCooldownSeconds);
            this.Stream.WriteString(this.GoogleServiceId);
            this.Stream.WriteString(this.Region);
            this.Stream.WriteString(null);
            this.Stream.WriteInt(1);
            this.Stream.WriteString(null);
            this.Stream.WriteString(null);
            this.Stream.WriteString(null);

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

            if (this.ChronosContentUrlList != null)
            {
                this.Stream.WriteInt(this.ChronosContentUrlList.Count);

                for (int i = 0; i < this.ChronosContentUrlList.Count; i++)
                {
                    this.Stream.WriteString(this.ChronosContentUrlList[i]);
                }
            }
            else
            {
                this.Stream.WriteInt(-1);
            }
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20104;
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
            this.ChronosContentUrlList = null;
            this.ContentUrlList = null;

            this.PassToken = null;
            this.FacebookId = null;
            this.GamecenterId = null;
            this.ServerEnvironment = null;
            this.FacebookAppId = null;
            this.ServerTime = null;
            this.AccountCreatedDate = null;
            this.GoogleServiceId = null;
            this.Region = null;
        }
    }
}