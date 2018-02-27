namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    using ClashersRepublic.Magic.Logic.Avatar;

    public class AvatarDataMessage : NetMessage
    {
        private LogicClientAvatar _logicClientAvatar;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._logicClientAvatar != null)
            {
                this._logicClientAvatar.Destruct();
                this._logicClientAvatar = null;
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this._logicClientAvatar.Encode(this.Stream);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._logicClientAvatar.Decode(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20203;
        }

        /// <summary>
        ///     Removes the <see cref="LogicClientAvatar" /> instance.
        /// </summary>
        public LogicClientAvatar RemoveLogicClientAvatar()
        {
            LogicClientAvatar tmp = this._logicClientAvatar;
            this._logicClientAvatar = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar" /> instance.
        /// </summary>
        public void SetLogicClientAvatar(LogicClientAvatar instance)
        {
            this._logicClientAvatar = instance;
        }
    }
}