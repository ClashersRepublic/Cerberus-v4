namespace RivieraStudio.Magic.Services.Core.Message.Session
{
    using RivieraStudio.Magic.Titan.Math;

    public class ServerBoundMessage : NetMessage
    {
        private LogicLong _accountId;
        private int[] _endPoints;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._accountId = null;
            this._endPoints = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._accountId);

            for (byte i = 0; i < 28; i++)
            {
                this.Stream.WriteVInt(this._endPoints[i]);
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._accountId = this.Stream.ReadLong();
            this._endPoints = new int[28];

            for (byte i = 0; i < 28; i++)
            {
                this._endPoints[i] = this.Stream.ReadVInt();
            }
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10302;
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
        ///     Removes the end point array.
        /// </summary>
        public int[] RemoveEndPoints()
        {
            int[] tmp = this._endPoints;
            this._endPoints = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the end point array.
        /// </summary>
        public void SetEndPoints(int[] value)
        {
            this._endPoints = value;
        }
    }
}