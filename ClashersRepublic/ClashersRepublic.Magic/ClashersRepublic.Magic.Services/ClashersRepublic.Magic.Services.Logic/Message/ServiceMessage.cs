namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Titan.Message;

    public class ServiceMessage : PiranhaMessage
    {
        protected string ServiceType;
        protected string SessionId;

        protected int ServerId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceMessage" /> class.
        /// </summary>
        public ServiceMessage(short messageVersion) : base(messageVersion)
        {
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.ServiceType = this.Stream.ReadString(64);
            this.SessionId = this.Stream.ReadString(64);
            this.ServerId = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteString(this.ServiceType);
            this.Stream.WriteString(this.SessionId);
            this.Stream.WriteVInt(this.ServerId);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return base.GetMessageType();
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return base.GetServiceNodeType();
        }

        /// <summary>
        ///     Destructs this message.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this.ServiceType = null;
            this.SessionId = null;
        }

        /// <summary>
        ///     Gets the service type.
        /// </summary>
        public string GetServiceType()
        {
            return this.ServiceType;
        }

        /// <summary>
        ///     Sets the service type.
        /// </summary>
        public void SetSeviceType(string value)
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
    }
}