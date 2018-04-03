namespace RivieraStudio.Magic.Logic.Command.Server
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Math;

    public class LogicLeaveAllianceCommand : LogicServerCommand
    {
        private LogicLong _allianceId;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._allianceId = null;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._allianceId = stream.ReadLong();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._allianceId);
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
                if (playerAvatar.IsInAlliance())
                {
                    if (playerAvatar.GetAllianceId().Equals(this._allianceId))
                    {
                        playerAvatar.SetAllianceId(null);
                        playerAvatar.SetAllianceName(null);
                        playerAvatar.SetAllianceBadge(-1);
                        playerAvatar.SetAllianceExpLevel(-1);
                        playerAvatar.GetChangeListener().AllianceLeft();
                    }
                }

                level.GetGameListener().AllianceLeft();

                return 0;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the command type.
        /// </summary>
        public override int GetCommandType()
        {
            return 2;
        }
    }
}