namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Message;

    public class SetDeviceTokenMessage : PiranhaMessage
    {
        private byte[] _deviceToken;
        private int _deviceTokenLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetDeviceTokenMessage" /> class.
        /// </summary>
        public SetDeviceTokenMessage() : this(0)
        {
            // SetDeviceTokenMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SetDeviceTokenMessage" /> class.
        /// </summary>
        public SetDeviceTokenMessage(short messageVersion) : base(messageVersion)
        {
            // SetDeviceTokenMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._deviceTokenLength = this.Stream.ReadBytesLength();

            if (this._deviceTokenLength > 1000)
            {
                Debugger.Error("Illegal byte array length encountered.");
            }

            this._deviceToken = this.Stream.ReadBytes(this._deviceTokenLength, 900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteBytes(this._deviceToken, this._deviceTokenLength);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10113;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._deviceToken = null;
        }

        /// <summary>
        ///     Gets the device token.
        /// </summary>
        public byte[] GetDeviceToken()
        {
            return this._deviceToken;
        }

        /// <summary>
        ///     Gets the device token length.
        /// </summary>
        public int GetDeviceTokenLength()
        {
            return this._deviceTokenLength;
        }

        /// <summary>
        ///     Sets the device token.
        /// </summary>
        public void SetDeviceToken(byte[] value, int length)
        {
            this._deviceToken = value;
            this._deviceTokenLength = length;
        }
    }
}