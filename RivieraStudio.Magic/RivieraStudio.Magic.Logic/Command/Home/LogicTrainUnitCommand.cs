namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicTrainUnitCommand : LogicCommand
    {
        private LogicData _unitData;

        private int _unitType;
        private int _trainCount;
        private int _gameObjectId;
        private int _slotId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTrainUnitCommand" /> class.
        /// </summary>
        public LogicTrainUnitCommand()
        {
            // LogicTrainUnitCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTrainUnitCommand" /> class.
        /// </summary>
        public LogicTrainUnitCommand(int count, LogicCombatItemData combatItemData, int gameObjectId, int slotId)
        {
            this._trainCount = count;
            this._unitData = combatItemData;
            this._gameObjectId = gameObjectId;
            this._slotId = slotId;
            this._unitType = this._unitData.GetDataType();
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObjectId = stream.ReadInt();
            this._unitType = stream.ReadInt();
            this._unitData = stream.ReadDataReference(this._unitType != 0 ? 25 : 3);
            this._trainCount = stream.ReadInt();
            this._slotId = stream.ReadInt();

            LogicGlobals globals = LogicDataTables.GetGlobals();

            if (!globals.UseDragInTraining() && !globals.UseDragInTrainingFix() && !globals.UseDragInTrainingFix2())
            {
                this._slotId = -1;
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectId);
            encoder.WriteInt(this._unitType);
            encoder.WriteDataReference(this._unitData);
            encoder.WriteInt(this._trainCount);
            encoder.WriteInt(this._slotId);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 508;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._gameObjectId = 0;
            this._unitType = 0;
            this._slotId = 0;
            this._trainCount = 0;
            this._unitData = null;
        }
    }
}