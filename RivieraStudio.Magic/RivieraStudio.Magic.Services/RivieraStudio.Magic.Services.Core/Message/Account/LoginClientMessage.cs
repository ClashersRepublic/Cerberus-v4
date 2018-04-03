namespace RivieraStudio.Magic.Services.Core.Message.Account
{
    using RivieraStudio.Magic.Titan.Math;

    public class LoginClientMessage : NetMessage
    {
        private LogicLong _accountId;
        private string _passToken;
        private string _ipAddress;
        private string _deviceModel;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._accountId = null;
            this._passToken = null;
            this._ipAddress = null;
            this._deviceModel = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this._accountId);
            this.Stream.WriteString(this._passToken);
            this.Stream.WriteString(this._ipAddress);
            this.Stream.WriteString(this._deviceModel);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._accountId = this.Stream.ReadLong();
            this._passToken = this.Stream.ReadString(900000);
            this._ipAddress = this.Stream.ReadString(900000);
            this._deviceModel = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10100;
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

        /// <summary>
        ///     Gets the ip address.
        /// </summary>
        public string GetIPAddress()
        {
            return this._ipAddress;
        }

        /// <summary>
        ///     Sets the ip address.
        /// </summary>
        public void SetIPAddress(string value)
        {
            this._ipAddress = value;
        }

        /// <summary>
        ///     Gets the device model.
        /// </summary>
        public string GetDeviceModel()
        {
            return this._deviceModel;
        }

        /// <summary>
        ///     Sets the device model.
        /// </summary>
        public void SetDeviceModel(string value)
        {
            this._deviceModel = value;
        }
    }
}