namespace RivieraStudio.Magic.Logic.Message.Avatar
{
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Message;

    public class AskForAvatarProfileMessage : PiranhaMessage
    {
        private LogicLong _avatarId;
        private LogicLong _homeId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AskForAvatarProfileMessage" /> class.
        /// </summary>
        public AskForAvatarProfileMessage() : this(0)
        {
            // AskForAvatarProfileMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AskForAvatarProfileMessage" /> class.
        /// </summary>
        public AskForAvatarProfileMessage(short messageVersion) : base(messageVersion)
        {
            // AskForAvatarProfileMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._avatarId = this.Stream.ReadLong();

            if (this.Stream.ReadBoolean())
            {
                this._homeId = this.Stream.ReadLong();
            }

            this.Stream.ReadBoolean();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._avatarId);

            if (this._homeId != null)
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteLong(this._homeId);
            }
            else
            {
                this.Stream.WriteBoolean(false);
            }

            this.Stream.WriteBoolean(false);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 14325;
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

            this._avatarId = null;
            this._homeId = null;
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