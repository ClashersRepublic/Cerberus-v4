namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Message;

    public class ReportUserStatusMessage : PiranhaMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportUserStatusMessage" /> class.
        /// </summary>
        public ReportUserStatusMessage() : this(0)
        {
            // ReportUserStatusMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportUserStatusMessage" /> class.
        /// </summary>
        public ReportUserStatusMessage(short messageVersion) : base(messageVersion)
        {
            // ReportUserStatusMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.Stream.ReadInt();
            this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(0);
            this.Stream.WriteInt(0);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20117;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }
    }
}