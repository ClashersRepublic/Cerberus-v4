namespace ClashersRepublic.Magic.Services.Core.Message.Account
{
    public class CreateAccountFailedMessage : NetMessage
    {
        private int _errorCode;

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
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._errorCode = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20102;
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
    }
}