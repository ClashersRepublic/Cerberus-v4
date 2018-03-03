namespace ClashersRepublic.Magic.Services.Core.Message.GlobalChat
{
    public class GlobalChatAvatarEntryMessage : NetMessage
    {
        private GlobalChatAvatarEntry _entry;

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
            this._entry = new GlobalChatAvatarEntry();
            this._entry.Decode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10201;
        }

        /// <summary>
        ///     Removes the <see cref="GlobalChatAvatarEntry"/> instance.
        /// </summary>
        public GlobalChatAvatarEntry RemoveAvatarId()
        {
            GlobalChatAvatarEntry tmp = this._entry;
            this._entry = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="GlobalChatAvatarEntry"/> instance.
        /// </summary>
        public void SetGlobalChatAvatarEntry(GlobalChatAvatarEntry value)
        {
            this._entry = value;
        }
    }
}