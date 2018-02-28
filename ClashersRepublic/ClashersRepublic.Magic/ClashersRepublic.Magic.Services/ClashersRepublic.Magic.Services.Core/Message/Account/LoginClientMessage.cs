namespace ClashersRepublic.Magic.Services.Core.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;

    public class LoginClientMessage : NetMessage
    {
        private LogicLong _accountId;
        private string _passToken;
        private string _ip;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._accountId = null;
            this._passToken = null;
            this._ip = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this._accountId);
            this.Stream.WriteString(this._passToken);
            this.Stream.WriteString(this._ip);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._accountId = this.Stream.ReadLong();
            this._passToken = this.Stream.ReadString(900000);
            this._ip = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10102;
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