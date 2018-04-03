namespace LineageSoft.Magic.Services.Core.Message.Account
{
    public class LoginClientFailedMessage : NetMessage
    {
        private int _errorCode;
        private int _remainingTime;

        private string _reason;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(this._errorCode);
            this.Stream.WriteVInt(this._remainingTime);
            this.Stream.WriteString(this._reason);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._errorCode = this.Stream.ReadVInt();
            this._remainingTime = this.Stream.ReadVInt();
            this._reason = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20100;
        }

        /// <summary>
        ///     Gets the error code.
        /// </summary>
        public int GetErrorCode()
        {
            return this._errorCode;
        }

        /// <summary>
        ///     Sets the error code.
        /// </summary>
        public void SetErrorCode(int value)
        {
            this._errorCode = value;
        }

        /// <summary>
        ///     Gets the reason.
        /// </summary>
        public string GetReason()
        {
            return this._reason;
        }

        /// <summary>
        ///     Sets the reason.
        /// </summary>
        public void SetReason(string message)
        {
            this._reason = message;
        }

        /// <summary>
        ///     Gets the remaining time.
        /// </summary>
        public int GetRemainingTime()
        {
            return this._remainingTime;
        }

        /// <summary>
        ///     Sets the remaining time.
        /// </summary>
        public void SetRemainingTime(int value)
        {
            this._remainingTime = value;
        }
    }
}