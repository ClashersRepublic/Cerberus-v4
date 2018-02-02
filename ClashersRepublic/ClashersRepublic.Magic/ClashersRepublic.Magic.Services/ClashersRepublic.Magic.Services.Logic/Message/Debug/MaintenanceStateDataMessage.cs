namespace ClashersRepublic.Magic.Services.Logic.Message.Debug
{
    public class MaintenanceStateDataMessage : ServiceMessage
    {
        public int MaintenanceState;
        public int MaintenanceTime;
        public int RemainingMaintenanceTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MaintenanceStateDataMessage"/> class.
        /// </summary>
        public MaintenanceStateDataMessage() : base(0)
        {
            // MaintenanceStateDataMessage.
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
            return 20130;
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