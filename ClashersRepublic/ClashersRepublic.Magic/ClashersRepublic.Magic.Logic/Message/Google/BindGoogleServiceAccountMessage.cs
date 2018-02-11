namespace ClashersRepublic.Magic.Logic.Message.Google
{
    using ClashersRepublic.Magic.Titan.Message;

    public class BindGoogleServiceAccountMessage : PiranhaMessage
    {
        public bool Force;
        public string GoogleServiceId;
        public string AuthCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindGoogleServiceAccountMessage" /> class.
        /// </summary>
        public BindGoogleServiceAccountMessage() : this(0)
        {
            // BindGoogleServiceAccountMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BindGoogleServiceAccountMessage" /> class.
        /// </summary>
        public BindGoogleServiceAccountMessage(short messageVersion) : base(messageVersion)
        {
            // BindGoogleServiceAccountMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.Force = this.Stream.ReadBoolean();
            this.GoogleServiceId = this.Stream.ReadString(900000);
            this.AuthCode = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteBoolean(this.Force);
            this.Stream.WriteString(this.GoogleServiceId);
            this.Stream.WriteString(this.AuthCode);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 14262;
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
            this.GoogleServiceId = null;
            this.AuthCode = null;
        }
    }
}