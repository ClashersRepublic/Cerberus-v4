namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Titan.Message;

    public class MagicServiceMessage : PiranhaMessage
    {
        public string ProxySessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MagicServiceMessage" />
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
            this.ProxySessionId = this.Stream.ReadString(64);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteString(this.ProxySessionId);
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
    }
}