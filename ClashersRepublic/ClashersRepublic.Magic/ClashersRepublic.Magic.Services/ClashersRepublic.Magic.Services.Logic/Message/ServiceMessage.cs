namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Titan.Message;

    public class ServiceMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceMessage" /> class.
        /// </summary>
        public ServiceMessage(short messageVersion) : base(messageVersion)
        {
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
    }
}