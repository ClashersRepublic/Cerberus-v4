namespace ClashersRepublic.Magic.Logic.Message.Avatar
{
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    public class AvatarProfileMessage : PiranhaMessage
    {
        private AvatarProfileFullEntry _entry;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarProfileMessage" /> class.
        /// </summary>
        public AvatarProfileMessage() : this(0)
        {
            // AvatarProfileMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarProfileMessage" /> class.
        /// </summary>
        public AvatarProfileMessage(short messageVersion) : base(messageVersion)
        {
            // AvatarProfileMessage.
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
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this._entry.Encode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24334;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 9;
        }

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