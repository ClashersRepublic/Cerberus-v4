namespace ClashersRepublic.Magic.Services.Core.Message.Account
{
    public class AccountBanRevokedMessage : NetMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AccountBanRevokedMessage"/> instance.
        /// </summary>
        public AccountBanRevokedMessage() : base()
        {
            // AccountBanRevokedMessage.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20106;
        }
    }
}