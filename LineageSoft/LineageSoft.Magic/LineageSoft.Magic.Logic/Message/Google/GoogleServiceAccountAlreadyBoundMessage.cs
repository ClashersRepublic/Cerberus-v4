namespace LineageSoft.Magic.Logic.Message.Google
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Message;

    public class GoogleServiceAccountAlreadyBoundMessage : PiranhaMessage
    {
        private LogicLong _accountId;
        private LogicClientAvatar _avatar;

        private string _googleServiceId;
        private string _passToken;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GoogleServiceAccountAlreadyBoundMessage" /> class.
        /// </summary>
        public GoogleServiceAccountAlreadyBoundMessage() : this(0)
        {
            // GoogleServiceAccountAlreadyBoundMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GoogleServiceAccountAlreadyBoundMessage" /> class.
        /// </summary>
        public GoogleServiceAccountAlreadyBoundMessage(short messageVersion) : base(messageVersion)
        {
            // GoogleServiceAccountAlreadyBoundMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._googleServiceId = this.Stream.ReadString(900000);

            if (this.Stream.ReadBoolean())
            {
                this._accountId = this.Stream.ReadLong();
            }

            this._passToken = this.Stream.ReadString(900000);
            this._avatar = new LogicClientAvatar();
            this._avatar.Decode(this.Stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteString(this._googleServiceId);

            if (!this._accountId.IsZero())
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteLong(this._accountId);
            }
            else
            {
                this.Stream.WriteBoolean(false);
            }

            this.Stream.WriteString(this._passToken);
            this._avatar.Encode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24262;
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
            this._passToken = null;
            this._avatar = null;
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
        ///     Removes the pass token.
        /// </summary>
        public string RemovePassToken()
        {
            string tmp = this._passToken;
            this._passToken = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the pass token.
        /// </summary>
        public void SetPassToken(string value)
        {
            this._passToken = value;
        }

        /// <summary>
        ///     Removes the account id.
        /// </summary>
        public LogicLong RemoveAccountId()
        {
            LogicLong tmp = this._accountId;
            this._accountId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the account id.
        /// </summary>
        public void SetAccountId(LogicLong value)
        {
            this._accountId = value;
        }

        /// <summary>
        ///     Removes the <see cref="LogicClientAvatar" /> instance.
        /// </summary>
        public LogicClientAvatar RemoveLogicClientAvatar()
        {
            LogicClientAvatar tmp = this._avatar;
            this._avatar = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar" /> instance.
        /// </summary>
        public void SetAvatar(LogicClientAvatar avatar)
        {
            this._avatar = avatar;
        }
    }
}