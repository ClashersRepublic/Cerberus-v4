namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateAvatarMessage : NetMessage
    {
        private LogicLong _accountId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateAvatarMessage"/> instance.
        /// </summary>
        public CreateAvatarMessage() : base()
        {
            // CreateAvatarMessage.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._accountId = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._accountId);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._accountId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10200;
        }
    }
}