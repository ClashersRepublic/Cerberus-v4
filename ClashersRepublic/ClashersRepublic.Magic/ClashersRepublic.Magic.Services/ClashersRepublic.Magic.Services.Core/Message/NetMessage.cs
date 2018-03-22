namespace ClashersRepublic.Magic.Services.Core.Message
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public class NetMessage
    {
        protected ByteStream Stream;

        protected byte[] SessionId;
        protected int ServiceNodeId;
        protected int ServiceNodeType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetMessage" /> class.
        /// </summary>
        public NetMessage()
        {
            this.Stream = new ByteStream(10);
        }

        /// <summary>
        ///     Gets the session id.
        /// </summary>
        public byte[] GetSessionId()
        {
            return this.SessionId;
        }

        /// <summary>
        ///     Removes the session id.
        /// </summary>
        public byte[] RemoveSessionId()
        {
            byte[] tmp = this.SessionId;
            this.SessionId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the session id.
        /// </summary>
        public void SetSessionId(byte[] value)
        {
            if (value != null && value.Length > 64)
            {
                Logging.Error("NetMessage::setSessionId session length is too big! (" + value.Length + ")");
            }

            this.SessionId = value;
        }

        /// <summary>
        ///     Gets the sender service node type.
        /// </summary>
        public int GetServiceNodeType()
        {
            return this.ServiceNodeType;
        }

        /// <summary>
        ///     Sets the sender service node type.
        /// </summary>
        public void SetServiceNodeType(int value)
        {
            this.ServiceNodeType = value;
        }

        /// <summary>
        ///     Gets the sender service node id.
        /// </summary>
        public int GetServiceNodeId()
        {
            return this.ServiceNodeId;
        }

        /// <summary>
        ///     Sets the sender service node id.
        /// </summary>
        public void SetServiceNodeId(int value)
        {
            this.ServiceNodeId = value;
        }

        /// <summary>
        ///     Gets the encoding length.
        /// </summary>
        public int GetEncodingLength()
        {
            return this.Stream.GetOffset();
        }

        /// <summary>
        ///     Gets the message bytes.
        /// </summary>
        public byte[] GetMessageBytes()
        {
            return this.Stream.GetByteArray();
        }

        /// <summary>
        ///     Gets the <see cref="ByteStream" /> instance.
        /// </summary>
        public ByteStream GetByteStream()
        {
            return this.Stream;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            this.Stream.Destruct();
            this.SessionId = null;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode()
        {
            this.ServiceNodeType = this.Stream.ReadVInt();
            this.ServiceNodeId = this.Stream.ReadVInt();
            this.SessionId = this.Stream.ReadBytes(this.Stream.ReadVInt(), 64);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode()
        {
            this.Stream.WriteVInt(this.ServiceNodeType);
            this.Stream.WriteVInt(this.ServiceNodeId);

            if (this.SessionId != null)
            {
                this.Stream.WriteVInt(this.SessionId.Length);
                this.Stream.WriteBytesWithoutLength(this.SessionId, 12);
            }
            else
            {
                this.Stream.WriteVInt(-1);
            }
        }

        /// <summary>
        ///     Gets the message type.
        /// </summary>
        public virtual int GetMessageType()
        {
            return 0;
        }
    }
}