namespace ClashersRepublic.Magic.Services.Core.Message.Session
{
    public class UpdateSessionSocketListMessage : NetMessage
    {
        private bool[] _isSetList;
        private byte[] _sessionSocketList;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateSessionSocketListMessage" /> instance.
        /// </summary>
        public UpdateSessionSocketListMessage()
        {
            this._isSetList = new bool[28];
            this._sessionSocketList = new byte[28];
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._isSetList = null;
            this._sessionSocketList = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            for (int i = 0; i < 28; i++)
            {
                this.Stream.WriteBoolean(this._isSetList[i]);
            }

            for (int i = 0; i < 28; i++)
            {
                if (this._isSetList[i])
                {
                    this.Stream.WriteByte(this._sessionSocketList[i]);
                }
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            for (int i = 0; i < 28; i++)
            {
                this._isSetList[i] = this.Stream.ReadBoolean();
            }

            for (int i = 0; i < 28; i++)
            {
                if (this._isSetList[i])
                {
                    this._sessionSocketList[i] = this.Stream.ReadByte();
                }
            }
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

        /// <summary>
        ///     Removes the set socket list.
        /// </summary>
        public bool[] RemoveIsSetList()
        {
            bool[] tmp = this._isSetList;
            this._isSetList = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the set list.
        /// </summary>
        public void SetIsSetList(bool[] value)
        {
            this._isSetList = value;
        }
    }
}