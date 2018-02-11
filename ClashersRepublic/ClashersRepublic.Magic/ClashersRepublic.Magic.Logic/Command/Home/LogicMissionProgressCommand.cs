namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicMissionProgressCommand : LogicCommand
    {
        private LogicData _missionData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionProgressCommand"/> class.
        /// </summary>
        public LogicMissionProgressCommand()
        {
            // LogicMissionProgressCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand"/> class.
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
            this._missionData = stream.ReadDataReference(20);
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
    }
}