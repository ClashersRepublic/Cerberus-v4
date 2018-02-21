namespace ClashersRepublic.Magic.Logic.Command.Server
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicLeaveAllianceCommand : LogicServerCommand
    {
        private LogicLong _allianceId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLeaveAllianceCommand"/> class.
        /// </summary>
        public LogicLeaveAllianceCommand()
        {
            // LogicLeaveAllianceCommand.
        }

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
            LogicClientAvatar playerAvatar = (LogicClientAvatar)level.GetPlayerAvatar();

            if (playerAvatar != null)
            {
                if (playerAvatar.IsInAlliance())
                {

                }

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