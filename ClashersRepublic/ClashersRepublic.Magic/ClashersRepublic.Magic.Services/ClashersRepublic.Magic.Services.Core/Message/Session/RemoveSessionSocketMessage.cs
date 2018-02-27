namespace ClashersRepublic.Magic.Services.Core.Message.Session
{
    public class RemoveSessionSocketMessage : NetMessage
    {
        private byte _sessionSocketType;

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
            this.Stream.WriteByte(this._sessionSocketType);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._sessionSocketType = this.Stream.ReadByte();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10300;
        }

        /// <summary>
        ///     Gets the session socket type.
        /// </summary>
        public byte GetSessionSocketType()
        {
            return this._sessionSocketType;
        }

        /// <summary>
        ///     Sets the session socket type.
        /// </summary>
        public void SetSessionSocketType(byte value)
        {
            this._sessionSocketType = value;
        }
    }
}