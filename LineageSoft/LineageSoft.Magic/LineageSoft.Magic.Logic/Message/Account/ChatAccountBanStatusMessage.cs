namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Message.Account;

    public class ChatAccountBanStatusMessage : TitanDisconnectedMessage
    {
        private int _banSecs;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatAccountBanStatusMessage" /> class.
        /// </summary>
        public ChatAccountBanStatusMessage() : this(0)
        {
            // ChatAccountBanStatusMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChatAccountBanStatusMessage" /> class.
        /// </summary>
        public ChatAccountBanStatusMessage(short messageVersion) : base(messageVersion)
        {
            // ChatAccountBanStatusMessage.
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
        ///     Gets the ban seconds.
        /// </summary>
        public int GetBanSeconds()
        {
            return this._banSecs;
        }

        /// <summary>
        ///     Sets the ban seconds.
        /// </summary>
        public void SetBanSeconds(int value)
        {
            this._banSecs = value;
        }
    }
}