namespace ClashersRepublic.Magic.Services.Logic.Message.Debug
{
    public class AskForMaintenanceStateMessage : MagicServiceMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AskForMaintenanceStateMessage"/> class.
        /// </summary>
        public AskForMaintenanceStateMessage() : base(0)
        {
            // AskForMaintenanceStateMessage.
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
            return 10130;
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