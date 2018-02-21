namespace ClashersRepublic.Magic.Logic.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    public class ResetAccountMessage : PiranhaMessage
    {
        private int _accountPreset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResetAccountMessage" /> class.
        /// </summary>
        public ResetAccountMessage() : this(0)
        {
            // ResetAccountMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResetAccountMessage" /> class.
        /// </summary>
        public ResetAccountMessage(short messageVersion) : base(messageVersion)
        {
            // ResetAccountMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._accountPreset = this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteInt(this._accountPreset);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10116;
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
        ///     Gets the account preset.
        /// </summary>
        public int GetAccountPreset()
        {
            return this._accountPreset;
        }

        /// <summary>
        ///     Sets the account preset.
        /// </summary>
        public void SetAccountPreset(int value)
        {
            this._accountPreset = value;
        }
    }
}