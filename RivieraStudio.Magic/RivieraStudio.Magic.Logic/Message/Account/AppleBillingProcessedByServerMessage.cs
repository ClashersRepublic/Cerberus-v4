namespace RivieraStudio.Magic.Logic.Message.Account
{
    using RivieraStudio.Magic.Titan.Message;

    public class AppleBillingProcessedByServerMessage : PiranhaMessage
    {
        private string _tid;
        private string _prodId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppleBillingProcessedByServerMessage" /> class.
        /// </summary>
        public AppleBillingProcessedByServerMessage() : this(0)
        {
            // AppleBillingProcessedByServerMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppleBillingProcessedByServerMessage" /> class.
        /// </summary>
        public AppleBillingProcessedByServerMessage(short messageVersion) : base(messageVersion)
        {
            // AppleBillingProcessedByServerMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._tid = this.Stream.ReadString(900000);
            this._prodId = this.Stream.ReadString(900000);
            this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteString(this._tid);
            this.Stream.WriteString(this._prodId);
            this.Stream.WriteInt(0);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 20151;
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

            this._tid = null;
            this._prodId = null;
        }

        /// <summary>
        ///     Gets the billing tid.
        /// </summary>
        public string GetTID()
        {
            return this._tid;
        }

        /// <summary>
        ///     Sets the billing tid.
        /// </summary>
        public void SetTID(string value)
        {
            this._tid = value;
        }

        /// <summary>
        ///     Gets the prod id.
        /// </summary>
        public string GetProdID()
        {
            return this._prodId;
        }

        /// <summary>
        ///     Sets the prod id.
        /// </summary>
        public void SetProdID(string value)
        {
            this._prodId = value;
        }
    }
}