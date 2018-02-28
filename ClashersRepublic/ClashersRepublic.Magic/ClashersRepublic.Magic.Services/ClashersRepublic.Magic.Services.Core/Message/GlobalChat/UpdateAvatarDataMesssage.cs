namespace ClashersRepublic.Magic.Services.Core.Message.GlobalChat
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Titan.Math;

    public class UpdateAvatarDataMesssage : NetMessage
    {
        private LogicLong _avatarId;
        private LogicLong _homeId;
        private string _name;
        private int _expLevel;
        private int _league;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._avatarId = null;
            this._homeId = null;
            this._name = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this._avatarId);
            this.Stream.WriteLong(this._homeId);
            this.Stream.WriteString(this._name);
            this.Stream.WriteVInt(this._expLevel);
            this.Stream.WriteVInt(this._league);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._avatarId = this.Stream.ReadLong();
            this._homeId = this.Stream.ReadLong();
            this._name = this.Stream.ReadString(900000);
            this._expLevel = this.Stream.ReadVInt();
            this._league = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20203;
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
            LogicLong tmp = this._avatarId;
            this._avatarId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the home id.
        /// </summary>
        public void SetHomeId(LogicLong id)
        {
            this._avatarId = id;
        }

        /// <summary>
        ///     Removes the avatar name.
        /// </summary>
        public string RemoveName()
        {
            string tmp = this._name;
            this._name = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the avatar name.
        /// </summary>
        public void SetName(string name)
        {
            this._name = name;
        }
    }
}