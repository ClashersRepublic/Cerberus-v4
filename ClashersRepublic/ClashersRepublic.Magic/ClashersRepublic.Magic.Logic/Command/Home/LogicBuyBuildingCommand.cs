namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicBuyBuildingCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private LogicData _buildingData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicBuyBuildingCommand()
        {
            // LogicBuyBuildingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicBuyBuildingCommand(int x, int y, LogicBuildingData buildingData)
        {
            this._x = x;
            this._y = y;
            this._buildingData = buildingData;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._x = stream.ReadInt();
            this._y = stream.ReadInt();
            this._buildingData = stream.ReadDataReference(0);

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._x);
            encoder.WriteInt(this._y);
            encoder.WriteDataReference(this._buildingData);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 500;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._x = 0;
            this._y = 0;
            this._buildingData = null;
        }
    }
}