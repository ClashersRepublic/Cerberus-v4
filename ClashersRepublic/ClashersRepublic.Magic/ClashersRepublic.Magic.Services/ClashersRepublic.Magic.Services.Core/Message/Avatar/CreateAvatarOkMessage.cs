namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateAvatarOkMessage : NetMessage
    {
        private LogicLong _avatarId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateAvatarOkMessage"/> instance.
        /// </summary>
        public CreateAvatarOkMessage() : base()
        {
            // CreateAvatarOkMessage.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._avatarId = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._avatarId);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._avatarId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20201;
        }

        /// <summary>
        ///     Removes the avatar id.
        /// </summary>
        public LogicLong RemoveAvatarId()
        {
            LogicLong tmp = this._avatarId;
            this._avatarId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the avatar id.
        /// </summary>
        public void SetAvatarId(LogicLong value)
        {
            this._avatarId = value;
        }
    }
}