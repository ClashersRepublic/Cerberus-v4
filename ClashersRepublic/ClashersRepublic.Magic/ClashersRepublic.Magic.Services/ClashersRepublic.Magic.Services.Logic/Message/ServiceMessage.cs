namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public class ServiceMessage
    {
        protected ByteStream Stream;

        protected string SessionId;

        protected int ServiceType;
        protected int ServerId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceMessage" /> class.
        /// </summary>
        public ServiceMessage()
        {
            this.Stream = new ByteStream(20);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode()
        {
            this.ServiceType = this.Stream.ReadVInt();
            this.ServerId = this.Stream.ReadVInt();
            this.SessionId = this.Stream.ReadString(24);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode()
        {
            this.Stream.WriteVInt(this.ServiceType);
            this.Stream.WriteVInt(this.ServerId);
            this.Stream.WriteString(this.SessionId);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public virtual short GetMessageType()
        {
            return 0;
        }
        
        /// <summary>
        ///     Destructs this message.
        /// </summary>
        public virtual void Destruct()
        {
            this.Stream.Destruct();
            this.SessionId = null;
        }

        /// <summary>
        ///     Gets the service type.
        /// </summary>
        public int GetServiceType()
        {
            return this.ServiceType;
        }

        /// <summary>
        ///     Sets the service type.
        /// </summary>
        public void SetServiceType(int value)
        {
            this.ServiceType = value;
        }

        /// <summary>
        ///     Gets the session id.
        /// </summary>
        public string GetSessionId()
        {
            return this.SessionId;
        }

        /// <summary>
        ///     Sets the session id.
        /// </summary>
        public void SetSessionId(string value)
        {
            this.SessionId = value;
        }

        /// <summary>
        ///     Gets the server id.
        /// </summary>
        public int GetServerId()
        {
            return this.ServerId;
        }

        /// <summary>
        ///     Sets the server id.
        /// </summary>
        public void SetServerId(int value)
        {
            this.ServerId = value;
        }

        /// <summary>
        ///     Gets the encoding length.
        /// </summary>
        public int GetEncodingLength()
        {
            return this.Stream.GetLength();
        }

        /// <summary>
        ///     Gets the <see cref="ByteStream"/> instance.
        /// </summary>
        public ByteStream GetByteStream()
        {
            return this.Stream;
        }
    }
}