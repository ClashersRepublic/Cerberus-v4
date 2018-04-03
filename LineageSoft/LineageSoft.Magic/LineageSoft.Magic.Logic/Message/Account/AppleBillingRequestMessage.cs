namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Message;

    public class AppleBillingRequestMessage : PiranhaMessage
    {
        private string _tid;
        private string _prodId;
        private string _currencyCode;
        private string _price;

        private byte[] _receiptData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppleBillingRequestMessage" /> class.
        /// </summary>
        public AppleBillingRequestMessage() : this(0)
        {
            // AppleBillingRequestMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AppleBillingRequestMessage" /> class.
        /// </summary>
        public AppleBillingRequestMessage(short messageVersion) : base(messageVersion)
        {
            // AppleBillingRequestMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._tid = this.Stream.ReadString(900000);
            this._prodId = this.Stream.ReadString(900000);
            this._currencyCode = this.Stream.ReadString(900000);
            this._price = this.Stream.ReadString(900000);

            int length = this.Stream.ReadBytesLength();

            if (length > 300000)
            {
                Debugger.Error("Illegal byte array length encountered.");
            }

            this._receiptData = this.Stream.ReadBytes(length, 900000);
            this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteString(this._tid);
            this.Stream.WriteString(this._prodId);
            this.Stream.WriteString(this._currencyCode);
            this.Stream.WriteString(this._price);
            this.Stream.WriteBytes(this._receiptData, this._receiptData.Length);
            this.Stream.WriteVInt(0);
            this.Stream.WriteInt(0);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10150;
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
            this._currencyCode = null;
            this._price = null;
            this._receiptData = null;
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

        /// <summary>
        ///     Gets the currency code.
        /// </summary>
        public string GetCurrencyCode()
        {
            return this._currencyCode;
        }

        /// <summary>
        ///     Sets the currency code.
        /// </summary>
        public void SetCurrencyCode(string value)
        {
            this._currencyCode = value;
        }

        /// <summary>
        ///     Gets the receipt data.
        /// </summary>
        public byte[] GetReceiptData()
        {
            return this._receiptData;
        }

        /// <summary>
        ///     Sets the receipt data.
        /// </summary>
        public void SetReceiptData(byte[] data, int length)
        {
            this._receiptData = null;

            if (length > -1)
            {
                this._receiptData = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    this._receiptData[i] = data[i];
                }
            }
        }
    }
}