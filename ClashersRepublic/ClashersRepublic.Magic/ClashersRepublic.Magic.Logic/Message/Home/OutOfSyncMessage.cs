namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Titan.Message;

    public class OutOfSyncMessage : PiranhaMessage
    {
        private int _subtick;
        private int _clientChecksum;
        private int _serverChecksum;
        private string _checksumJson;

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

            this._serverChecksum = this.Stream.ReadInt();
            this._clientChecksum = this.Stream.ReadInt();
            this._subtick = this.Stream.ReadInt();

            if (this.Stream.ReadBoolean())
            {
                this._checksumJson = this.Stream.ReadString(900000);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this._serverChecksum);
            this.Stream.WriteInt(this._clientChecksum);
            this.Stream.WriteInt(this._subtick);

            if (this._checksumJson != null)
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteString(this._checksumJson);
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
            base.Destruct();
            this._checksumJson = null;
        }

        /// <summary>
        ///     Gets the server checksum.
        /// </summary>
        public int GetServerChecksum()
        {
            return this._serverChecksum;
        }

        /// <summary>
        ///     Sets the server checksum.
        /// </summary>
        public void SetServerChecksum(int value)
        {
            this._serverChecksum = value;
        }

        /// <summary>
        ///     Gets the client checksum.
        /// </summary>
        public int GetClientChecksum()
        {
            return this._clientChecksum;
        }

        /// <summary>
        ///     Sets the client checksum.
        /// </summary>
        public void SetClientChecksum(int value)
        {
            this._clientChecksum = value;
        }

        /// <summary>
        ///     Gets the subtick.
        /// </summary>
        public int GetSubtick()
        {
            return this._subtick;
        }

        /// <summary>
        ///     Sets the subtick.
        /// </summary>
        public void SetSubtick(int value)
        {
            this._subtick = value;
        }

        /// <summary>
        ///     Removes the checksum json.
        /// </summary>
        public string RemoveChecksumJson()
        {
            string tmp = this._checksumJson;
            this._checksumJson = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the checksum json.
        /// </summary>
        public void SetChecksumJson(string json)
        {
            this._checksumJson = json;
        }
    }
}