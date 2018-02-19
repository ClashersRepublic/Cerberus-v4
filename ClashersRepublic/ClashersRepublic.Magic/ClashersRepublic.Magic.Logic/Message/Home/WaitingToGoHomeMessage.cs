namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Titan.Message;

    public class WaitingToGoHomeMessage : PiranhaMessage
    {
        private int _estimatedTimeSeconds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaitingToGoHomeMessage" /> class.
        /// </summary>
        public WaitingToGoHomeMessage() : this(0)
        {
            // WaitingToGoHomeMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="WaitingToGoHomeMessage" /> class.
        /// </summary>
        public WaitingToGoHomeMessage(short messageVersion) : base(messageVersion)
        {
            // WaitingToGoHomeMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._estimatedTimeSeconds = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._estimatedTimeSeconds);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24112;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Gets the estimated time seconds.
        /// </summary>
        public int GetEstimatedTimeSeconds()
        {
            return this._estimatedTimeSeconds;
        }

        /// <summary>
        ///     Sets the estimated time seconds.
        /// </summary>
        public void SetEstimatedTimeSeconds(int value)
        {
            this._estimatedTimeSeconds = value;
        }
    }
}