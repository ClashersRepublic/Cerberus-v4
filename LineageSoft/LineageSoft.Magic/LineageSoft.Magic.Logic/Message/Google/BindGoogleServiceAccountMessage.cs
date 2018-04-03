namespace LineageSoft.Magic.Logic.Message.Google
{
    using LineageSoft.Magic.Titan.Message;

    public class BindGoogleServiceAccountMessage : PiranhaMessage
    {
        private bool _force;
        private string _googleServiceId;
        private string _accessToken;

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

            this._force = this.Stream.ReadBoolean();
            this._googleServiceId = this.Stream.ReadString(900000);
            this._accessToken = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteBoolean(this._force);
            this.Stream.WriteString(this._googleServiceId);
            this.Stream.WriteString(this._accessToken);
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
            base.Destruct();

            this._googleServiceId = null;
            this._accessToken = null;
        }

        /// <summary>
        ///     Removes the google service id.
        /// </summary>
        public string RemoveGoogleServiceId()
        {
            string tmp = this._googleServiceId;
            this._googleServiceId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the google service id.
        /// </summary>
        public void SetGoogleServiceId(string value)
        {
            this._googleServiceId = value;
        }

        /// <summary>
        ///     Removes the access token.
        /// </summary>
        public string RemoveAccessToken()
        {
            string tmp = this._accessToken;
            this._accessToken = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the access token.
        /// </summary>
        public void SetAccessToken(string value)
        {
            this._accessToken = value;
        }
    }
}