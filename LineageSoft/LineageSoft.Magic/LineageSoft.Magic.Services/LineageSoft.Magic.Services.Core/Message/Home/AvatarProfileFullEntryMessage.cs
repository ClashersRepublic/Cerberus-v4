namespace LineageSoft.Magic.Services.Core.Message.Home
{
    using LineageSoft.Magic.Logic.Message.Avatar;

    public class AvatarProfileFullEntryMessage : NetMessage
    {
        private AvatarProfileFullEntry _entry;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._entry != null)
            {
                this._entry.Destruct();
                this._entry = null;
            }
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
            this._entry = new AvatarProfileFullEntry();
            this._entry.Decode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20520;
        }

        /// <summary>
        ///     Removes the <see cref="AvatarProfileFullEntry"/> instance.
        /// </summary>
        public AvatarProfileFullEntry RemoveAvatarProfileFullEntry()
        {
            AvatarProfileFullEntry tmp = this._entry;
            this._entry = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="AvatarProfileFullEntry"/> instance.
        /// </summary>
        public void SetAvatarProfileFullEntry(AvatarProfileFullEntry entry)
        {
            this._entry = entry;
        }
    }
}