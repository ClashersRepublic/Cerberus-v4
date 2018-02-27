namespace ClashersRepublic.Magic.Services.Core.Message.Session
{
    public class SetSessionSocketMessage : NetMessage
    {
        private byte _sessionSocketType;
        private byte _sessionSocketId;

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
            this.Stream.WriteByte(this._sessionSocketId);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._sessionSocketType = this.Stream.ReadByte();
            this._sessionSocketId = this.Stream.ReadByte();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10301;
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

        /// <summary>
        ///     Gets the session socket id.
        /// </summary>
        public byte GetSessionSocketId()
        {
            return this._sessionSocketId;
        }

        /// <summary>
        ///     Sets the session socket id.
        /// </summary>
        public void SetSessionSocketId(byte value)
        {
            this._sessionSocketId = value;
        }
    }
}