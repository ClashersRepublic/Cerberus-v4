namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Titan.Message;

    public class ServerErrorMessage : PiranhaMessage
    {
        private string _errorMessage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerErrorMessage" /> class.
        /// </summary>
        public ServerErrorMessage() : this(0)
        {
            // ServerErrorMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServerErrorMessage" /> class.
        /// </summary>
        public ServerErrorMessage(short messageVersion) : base(messageVersion)
        {
            // ServerErrorMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._errorMessage = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this._errorMessage);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24115;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._errorMessage = null;
        }

        /// <summary>
        ///     Gets the error message.
        /// </summary>
        public string GetErrorMessage()
        {
            return this._errorMessage;
        }

        /// <summary>
        ///     Sets the error message.
        /// </summary>
        public void SetErrorMessage(string value)
        {
            this._errorMessage = value;
        }
    }
}