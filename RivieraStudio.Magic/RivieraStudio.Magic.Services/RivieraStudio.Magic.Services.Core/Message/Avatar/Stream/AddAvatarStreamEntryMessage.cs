namespace RivieraStudio.Magic.Services.Core.Message.Avatar.Stream
{
    using RivieraStudio.Magic.Logic.Message.Avatar.Stream;

    public class AddAvatarStreamEntryMessage : NetMessage
    {
        private AvatarStreamEntry _entry;

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
            this.Stream.WriteVInt(this._entry.GetAvatarStreamEntryType());
            this._entry.Encode(this.Stream);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._entry = AvatarStreamEntryFactory.CreateStreamEntryByType(this.Stream.ReadVInt());
            this._entry.Decode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 1;
        }

        /// <summary>
        ///     Removes the <see cref="AvatarStreamEntry"/> instance.
        /// </summary>
        public AvatarStreamEntry RemoveAvatarChangeEntry()
        {
            AvatarStreamEntry tmp = this._entry;
            this._entry = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="AvatarStreamEntry"/> instance.
        /// </summary>
        public void SetAvatarChangeEntry(AvatarStreamEntry entry)
        {
            this._entry = entry;
        }
    }
}