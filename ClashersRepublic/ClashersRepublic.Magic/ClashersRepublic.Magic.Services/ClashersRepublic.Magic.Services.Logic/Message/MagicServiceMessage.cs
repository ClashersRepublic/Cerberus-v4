namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Titan.Message;

    public class MagicServiceMessage : PiranhaMessage
    {
        private string _proxySessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MagicServiceMessage" /> class.
        /// </summary>
        public MagicServiceMessage(short messageVersion) : base(messageVersion)
        {
            // MagicServiceMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return base.GetMessageType();
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return base.GetServiceNodeType();
        }

        /// <summary>
        ///     Gets the proxy session id.
        /// </summary>
        public string GetProxySessionId()
        {
            return this._proxySessionId;
        }

        /// <summary>
        ///     Sets the proxy session id.
        /// </summary>
        public void SetProxySessionId(string sessionId)
        {
            this._proxySessionId = sessionId;
        }
    }
}