namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Titan.Message;

    public class OutOfSyncMessage : PiranhaMessage
    {
        public int Subtick;
        public int ClientChecksum;
        public int ServerChecksum;
        public string ChecksumJson;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OutOfSyncMessage" /> class.
        /// </summary>
        public OutOfSyncMessage() : this(0)
        {
            // OutOfSyncMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OutOfSyncMessage" /> class.
        /// </summary>
        public OutOfSyncMessage(short messageVersion) : base(messageVersion)
        {
            // OutOfSyncMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.ServerChecksum = this.Stream.ReadInt();
            this.ClientChecksum = this.Stream.ReadInt();
            this.Subtick = this.Stream.ReadInt();

            if (this.Stream.ReadBoolean())
            {
                this.ChecksumJson = this.Stream.ReadString(900000);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this.ServerChecksum);
            this.Stream.WriteInt(this.ClientChecksum);
            this.Stream.WriteInt(this.Subtick);

            if (this.ChecksumJson != null)
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteString(this.ChecksumJson);
            }
            else
            {
                this.Stream.WriteBoolean(false);
            }
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24104;
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
            this.ChecksumJson = null;
        }
    }
}