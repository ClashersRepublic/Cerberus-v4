namespace ClashersRepublic.Magic.Services.Core.Message.Account
{
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateAccountOkMessage : NetMessage
    {
        private LogicLong _accountId;
        private LogicLong _homeId;
        private string _passToken;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateAccountOkMessage"/> instance.
        /// </summary>
        public CreateAccountOkMessage() : base()
        {
            // CreateAccountOkMessage.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._accountId = null;
            this._homeId = null;
            this._passToken = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this._accountId);
            this.Stream.WriteLong(this._homeId);
            this.Stream.WriteString(this._passToken);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._accountId = this.Stream.ReadLong();
            this._homeId = this.Stream.ReadLong();
            this._passToken = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20101;
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
        ///     Removes the home id.
        /// </summary>
        public LogicLong RemoveHomeId()
        {
            LogicLong tmp = this._homeId;
            this._homeId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the home id.
        /// </summary>
        public void SetHomeId(LogicLong value)
        {
            this._homeId = value;
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
    }
}