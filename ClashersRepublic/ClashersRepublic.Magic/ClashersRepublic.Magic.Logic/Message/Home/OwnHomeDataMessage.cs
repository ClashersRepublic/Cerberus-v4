namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Titan.Message;

    public class OwnHomeDataMessage : PiranhaMessage
    {
        public int CurrentTimestamp;
        public int SecondsSinceLastSave;

        public LogicClientAvatar LogicClientAvatar;
        public LogicClientHome LogicClientHome;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OwnHomeDataMessage" /> class.
        /// </summary>
        public OwnHomeDataMessage() : this(0)
        {
            // OwnHomeDataMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OwnHomeDataMessage" /> class.
        /// </summary>
        public OwnHomeDataMessage(short messageVersion) : base(messageVersion)
        {
            // OwnHomeDataMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.LogicClientHome = new LogicClientHome();
            this.LogicClientAvatar = new LogicClientAvatar();

            this.CurrentTimestamp = this.Stream.ReadInt();
            this.Stream.ReadInt();
            this.SecondsSinceLastSave = this.Stream.ReadInt();

            this.LogicClientHome.Decode(this.Stream);
            this.LogicClientAvatar.Decode(this.Stream);

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this.CurrentTimestamp);
            this.Stream.WriteInt(-1);
            this.Stream.WriteInt(this.SecondsSinceLastSave);

            this.LogicClientHome.Encode(this.Stream);
            this.LogicClientAvatar.Encode(this.Stream);

            this.Stream.WriteInt(352);
            this.Stream.WriteInt(1190797808);

            this.Stream.WriteInt(352);
            this.Stream.WriteInt(1192597808);

            this.Stream.WriteInt(352);
            this.Stream.WriteInt(1192597808);

            this.Stream.WriteInt(1);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24101;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            this.LogicClientHome = null;
            this.LogicClientAvatar = null;
        }
    }
}