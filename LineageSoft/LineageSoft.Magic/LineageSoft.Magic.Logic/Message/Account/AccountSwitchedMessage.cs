namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Message;

    public class AccountSwitchedMessage : PiranhaMessage
    {
        private LogicLong _switchedToAccountId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountSwitchedMessage" /> class.
        /// </summary>
        public AccountSwitchedMessage() : this(0)
        {
            // AccountSwitchedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountSwitchedMessage" /> class.
        /// </summary>
        public AccountSwitchedMessage(short messageVersion) : base(messageVersion)
        {
            // AccountSwitchedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._switchedToAccountId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._switchedToAccountId);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10118;
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
        ///     Removes the switched to account id.
        /// </summary>
        public LogicLong RemoveSwitchedToAccountId()
        {
            LogicLong tmp = this._switchedToAccountId;
            this._switchedToAccountId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the switched to account id.
        /// </summary>
        public void SetSwitchedToAccountId(LogicLong id)
        {
            this._switchedToAccountId = id;
        }
    }
}