namespace LineageSoft.Magic.Logic.Message.Account
{
    using LineageSoft.Magic.Titan.Message.Account;

    public class DisconnectedMessage : TitanDisconnectedMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DisconnectedMessage" /> class.
        /// </summary>
        public DisconnectedMessage() : this(0)
        {
            // DisconnectedMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DisconnectedMessage" /> class.
        /// </summary>
        public DisconnectedMessage(short messageVersion) : base(messageVersion)
        {
            // DisconnectedMessage.
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
    }
}