namespace RivieraStudio.Magic.Services.Core.Message.Account
{
    using RivieraStudio.Magic.Titan.Math;

    public class CreateAccountBanMessage : NetMessage
    {
        private LogicLong _accountId;
        private string _reason;
        private int _endTime;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._reason = null;
            this._accountId = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this._accountId);
            this.Stream.WriteString(this._reason);
            this.Stream.WriteVInt(this._endTime);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._accountId = this.Stream.ReadLong();
            this._reason = this.Stream.ReadString(900000);
            this._endTime = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10101;
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
        ///     Gets the end ban time.
        /// </summary>
        public int GetEndTime()
        {
            return this._endTime;
        }

        /// <summary>
        ///     Sets the end ban time.
        /// </summary>
        public void SetEndTime(int value)
        {
            this._endTime = value;
        }

        /// <summary>
        ///     Gets the ban reason.
        /// </summary>
        public string GetReason()
        {
            return this._reason;
        }

        /// <summary>
        ///     Sets the reason.
        /// </summary>
        public void SetReason(string value)
        {
            this._reason = value;
        }
    }
}