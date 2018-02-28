namespace ClashersRepublic.Magic.Services.Core.Message
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public class NetMessage
    {
        protected ByteStream Stream;

        protected byte[] SessionId;
        protected byte SessionIdLength;
        protected byte ServiceNodeId;
        protected byte ServiceNodeType;

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
            this.SessionIdLength = 0;
            return tmp;
        }

        /// <summary>
        ///     Gets the session length.
        /// </summary>
        public byte GetSessionIdLength()
        {
            return this.SessionIdLength;
        }

        /// <summary>
        ///     Sets the session id.
        /// </summary>
        public void SetSessionId(byte[] value, int lenght)
        {
            if (lenght <= 0xFF)
            {
                this.SessionId = value;
                this.SessionIdLength = (byte) lenght;
            }
            else
            {
                Logging.Warning(this, "NetMessage::setSessionId session too big (" + lenght + ")");
            }
        }

        /// <summary>
        ///     Gets the sender service node type.
        /// </summary>
        public byte GetServiceNodeType()
        {
            return this.ServiceNodeType;
        }

        /// <summary>
        ///     Sets the sender service node type.
        /// </summary>
        public void SetServiceNodeType(byte value)
        {
            this.ServiceNodeType = value;
        }

        /// <summary>
        ///     Gets the sender service node id.
        /// </summary>
        public byte GetServiceNodeId()
        {
            return this.ServiceNodeId;
        }

        /// <summary>
        ///     Sets the sender service node id.
        /// </summary>
        public void SetServiceNodeId(byte value)
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
            this.SessionIdLength = 0;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode()
        {
            this.ServiceNodeType = this.Stream.ReadByte();
            this.ServiceNodeId = this.Stream.ReadByte();
            this.SessionIdLength = this.Stream.ReadByte();
            this.SessionId = this.Stream.ReadBytes(this.SessionIdLength, 0xFF);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode()
        {
            this.Stream.WriteByte(this.ServiceNodeType);
            this.Stream.WriteByte(this.ServiceNodeId);
            this.Stream.WriteByte(this.SessionIdLength);
            this.Stream.WriteBytesWithoutLength(this.SessionId, this.SessionIdLength);
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