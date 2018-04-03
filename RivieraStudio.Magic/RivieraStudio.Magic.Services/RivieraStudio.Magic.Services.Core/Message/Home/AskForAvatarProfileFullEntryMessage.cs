namespace RivieraStudio.Magic.Services.Core.Message.Home
{
    using RivieraStudio.Magic.Titan.Math;

    public class AskForAvatarProfileFullEntryMessage : NetMessage
    {
        private LogicLong _avatarId;
        private LogicLong _homeId;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._avatarId = null;
            this._homeId = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._avatarId);
            this.Stream.WriteLong(this._homeId);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._avatarId = this.Stream.ReadLong();
            this._homeId = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10520;
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
        public void SetAvatarId(LogicLong id)
        {
            this._avatarId = id;
        }

        /// <summary>
        ///     Removes the home id.
        /// </summary>
        public LogicLong RemoveHomeId()
        {
            LogicLong tmp = this._homeId;
            this._homeId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the home id.
        /// </summary>
        public void SetHomeId(LogicLong id)
        {
            this._homeId = id;
        }
    }
}