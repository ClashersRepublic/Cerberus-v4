namespace ClashersRepublic.Magic.Services.Logic.Message.Session
{
    using ClashersRepublic.Magic.Titan.Math;

    public class ServiceNodeUnboundToSessionMessage : ServiceMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceNodeUnboundToSessionMessage"/> class.
        /// </summary>
        public ServiceNodeUnboundToSessionMessage()
        {
            // ServiceNodeUnboundToSessionMessage.
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
            return 10202;
        }
    }
}