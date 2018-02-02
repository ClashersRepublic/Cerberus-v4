namespace ClashersRepublic.Magic.Services.Logic.Message.Network
{
    public class KeepAliveServerMessage : ServiceMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="KeepAliveServerMessage"/> class.
        /// </summary>
        public KeepAliveServerMessage() : base(0)
        {
            // KeepAliveServerMessage.
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
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20108;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 0;
        }
    }
}