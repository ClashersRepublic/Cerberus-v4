namespace ClashersRepublic.Magic.Services.Core.Message.Session
{
    using ClashersRepublic.Magic.Titan.Math;

    public class UpdateSessionSocketListMessage : NetMessage
    {
        private byte[] _sessionSocketList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSessionSocketListMessage"/> instance.
        /// </summary>
        public UpdateSessionSocketListMessage() : base()
        {
            // UpdateSessionSocketListMessage.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._sessionSocketList = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteBytesWithoutLength(this._sessionSocketList, 28);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._sessionSocketList = this.Stream.ReadBytes(28, 28);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20300;
        }

        /// <summary>
        ///     Removes the session socket list.
        /// </summary>
        public byte[] RemoveSessionSocketList()
        {
            byte[] tmp = this._sessionSocketList;
            this._sessionSocketList = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the session socket list.
        /// </summary>
        public void SetSessionSocketList(byte[] value)
        {
            this._sessionSocketList = value;
        }
    }
}