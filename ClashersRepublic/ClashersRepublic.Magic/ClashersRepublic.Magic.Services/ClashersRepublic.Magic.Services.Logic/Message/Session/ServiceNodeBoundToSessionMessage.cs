namespace ClashersRepublic.Magic.Services.Logic.Message.Session
{
    using ClashersRepublic.Magic.Titan.Math;

    public class ServiceNodeBoundToSessionMessage : ServiceMessage
    {
        public LogicLong AccountId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceNodeBoundToSessionMessage"/> class.
        /// </summary>
        public ServiceNodeBoundToSessionMessage()
        {
            // ServiceNodeBoundToSessionMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.AccountId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this.AccountId);
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
            return 10201;
        }
    }
}