namespace RivieraStudio.Magic.Services.Core.Message.Avatar
{
    public class AvatarEntryMessage : NetMessage
    {
        private AvatarEntry _entry;

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
            this._entry.Encode(this.Stream);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._entry = new AvatarEntry();
            this._entry.Decode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20211;
        }

        /// <summary>
        ///     Removes the <see cref="AvatarEntry"/> instance.
        /// </summary>
        public AvatarEntry RemoveAvatarEntry()
        {
            AvatarEntry tmp = this._entry;
            this._entry = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="AvatarEntry"/> instance.
        /// </summary>
        public void SetAvatarEntry(AvatarEntry entry)
        {
            this._entry = entry;
        }
    }
}