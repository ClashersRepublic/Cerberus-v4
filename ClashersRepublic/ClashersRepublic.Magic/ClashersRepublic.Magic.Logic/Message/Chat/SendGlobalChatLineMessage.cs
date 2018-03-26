namespace ClashersRepublic.Magic.Logic.Message.Chat
{
    using ClashersRepublic.Magic.Titan.Message;

    public class SendGlobalChatLineMessage : PiranhaMessage
    {
        private string _message;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SendGlobalChatLineMessage" /> class.
        /// </summary>
        public SendGlobalChatLineMessage() : this(0)
        {
            // SendGlobalChatLineMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SendGlobalChatLineMessage" /> class.
        /// </summary>
        public SendGlobalChatLineMessage(short messageVersion) : base(messageVersion)
        {
            // SendGlobalChatLineMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._message = this.Stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this._message);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 14715;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 6;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._message = null;
        }

        /// <summary>
        ///     Removes the message.
        /// </summary>
        public string RemoveMessage()
        {
            string tmp = this._message;
            this._message = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the message.
        /// </summary>
        public void SetMessage(string message)
        {
            this._message = message;
        }
    }
}