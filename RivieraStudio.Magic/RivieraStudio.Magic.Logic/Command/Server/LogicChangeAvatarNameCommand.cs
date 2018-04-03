namespace RivieraStudio.Magic.Logic.Command.Server
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;

    public class LogicChangeAvatarNameCommand : LogicServerCommand
    {
        private string _avatarName;
        private int _nameChangeState;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._avatarName = null;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._avatarName = stream.ReadString(900000);
            this._nameChangeState = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteString(this._avatarName);
            encoder.WriteInt(this._nameChangeState);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            LogicClientAvatar playerAvatar = (LogicClientAvatar) level.GetPlayerAvatar();

            if (playerAvatar != null)
            {
                playerAvatar.SetName(this._avatarName);
                playerAvatar.SetNameSetByUser(true);
                playerAvatar.SetNameChangeState(this._nameChangeState);

                return 0;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the command type.
        /// </summary>
        public override int GetCommandType()
        {
            return 3;
        }

        /// <summary>
        ///     Sets the avatar name.
        /// </summary>
        public void SetAvatarName(string avatarName)
        {
            this._avatarName = avatarName;
        }

        /// <summary>
        ///     Sets the avatar name change state.
        /// </summary>
        public void SetAvatarNameChangeState(int state)
        {
            this._nameChangeState = state;
        }
    }
}