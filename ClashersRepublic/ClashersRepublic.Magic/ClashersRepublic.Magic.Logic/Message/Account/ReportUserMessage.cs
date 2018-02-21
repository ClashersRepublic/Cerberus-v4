namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    public class ReportUserMessage : PiranhaMessage
    {
        private LogicLong _reportedAvatarId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportUserMessage" /> class.
        /// </summary>
        public ReportUserMessage() : this(0)
        {
            // ReportUserMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ReportUserMessage" /> class.
        /// </summary>
        public ReportUserMessage(short messageVersion) : base(messageVersion)
        {
            // ReportUserMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._reportedAvatarId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._reportedAvatarId);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10117;
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

        /// <summary>
        ///     Removes the reported avatar id.
        /// </summary>
        public LogicLong RemoveReportedAvatarId()
        {
            LogicLong tmp = this._reportedAvatarId;
            this._reportedAvatarId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the reported avatar id.
        /// </summary>
        public void SetReportedAvatarId(LogicLong value)
        {
            this._reportedAvatarId = value;
        }
    }
}