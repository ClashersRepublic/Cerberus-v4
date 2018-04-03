namespace RivieraStudio.Magic.Logic.Message.Avatar
{
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Message;

    public class ChangeAvatarNameMessage : PiranhaMessage
    {
        private string _avatarName;
        private bool _nameSetByUser;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeAvatarNameMessage" /> class.
        /// </summary>
        public ChangeAvatarNameMessage() : this(0)
        {
            // ChangeAvatarNameMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChangeAvatarNameMessage" /> class.
        /// </summary>
        public ChangeAvatarNameMessage(short messageVersion) : base(messageVersion)
        {
            // ChangeAvatarNameMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._avatarName = this.Stream.ReadString(900000);
            this._nameSetByUser = this.Stream.ReadBoolean();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteString(this._avatarName);
            this.Stream.WriteBoolean(this._nameSetByUser);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10212;
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
            this._avatarName = null;
        }

        /// <summary>
        ///     Removes the avatar name.
        /// </summary>
        public string RemoveAvatarName()
        {
            string tmp = this._avatarName;
            this._avatarName = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the avatar name.
        /// </summary>
        public void SetAvatarName(string name)
        {
            this._avatarName = name;
        }

        /// <summary>
        ///     Gets if the name is set by user.
        /// </summary>
        public bool GetNameSetByUser()
        {
            return this._nameSetByUser;
        }

        /// <summary>
        ///     Sets if the name is set by user.
        /// </summary>
        public void SetNameSetByUser(bool set)
        {
            this._nameSetByUser = set;
        }
    }
}