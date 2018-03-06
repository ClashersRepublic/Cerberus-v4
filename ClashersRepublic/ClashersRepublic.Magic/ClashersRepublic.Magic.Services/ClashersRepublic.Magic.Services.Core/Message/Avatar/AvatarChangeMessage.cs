namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    using ClashersRepublic.Magic.Logic.Avatar.Change;
    using ClashersRepublic.Magic.Services.Core.Game.Avatar.Change;

    public class AvatarChangeMessage : NetMessage
    {
        private LogicAvatarChange _entry;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._entry = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(this._entry.GetAvatarChangeType());
            this._entry.Encode(this.Stream);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._entry = AvatarChangeFactory.CreateAvatarChangeByType(this.Stream.ReadVInt());
            this._entry.Decode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20210;
        }

        /// <summary>
        ///     Removes the <see cref="LogicAvatarChange"/> instance.
        /// </summary>
        public LogicAvatarChange RemoveAvatarChangeEntry()
        {
            LogicAvatarChange tmp = this._entry;
            this._entry = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicAvatarChange"/> instance.
        /// </summary>
        public void SetAvatarChangeEntry(LogicAvatarChange entry)
        {
            this._entry = entry;
        }
    }
}