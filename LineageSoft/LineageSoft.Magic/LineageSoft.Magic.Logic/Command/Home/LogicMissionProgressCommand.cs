namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Logic.Mission;
    using LineageSoft.Magic.Titan.DataStream;

    public sealed class LogicMissionProgressCommand : LogicCommand
    {
        private LogicMissionData _missionData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionProgressCommand" /> class.
        /// </summary>
        public LogicMissionProgressCommand()
        {
            // LogicMissionProgressCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionData" /> class.
        /// </summary>
        public LogicMissionProgressCommand(LogicMissionData missionData)
        {
            this._missionData = missionData;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._missionData = (LogicMissionData) stream.ReadDataReference(20);
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteDataReference(this._missionData);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 519;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._missionData = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (this._missionData != null)
            {
                LogicMission mission = level.GetMissionManager().GetMissionByData(this._missionData);

                if (mission != null)
                {
                    mission.StateChangeConfirmed();
                    return 0;
                }

                return -2;
            }

            return -1;
        }
    }
}