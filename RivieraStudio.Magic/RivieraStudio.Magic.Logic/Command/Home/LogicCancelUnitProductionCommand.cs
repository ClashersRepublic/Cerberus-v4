namespace RivieraStudio.Magic.Logic.Command.Home
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Math;

    public sealed class LogicCancelUnitProductionCommand : LogicCommand
    {
        private LogicCombatItemData _unitData;

        private int _unitType;
        private int _unitCount;
        private int _gameObjectId;
        private int _slotId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCancelUnitProductionCommand" /> class.
        /// </summary>
        public LogicCancelUnitProductionCommand()
        {
            // LogicCancelConstructionCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTrainUnitCommand" /> class.
        /// </summary>
        public LogicCancelUnitProductionCommand(int count, LogicCombatItemData combatItemData, int gameObjectId, int slotId)
        {
            this._unitCount = count;
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
            this._unitData = (LogicCombatItemData) stream.ReadDataReference(this._unitType != 0 ? 25 : 3);
            this._unitCount = stream.ReadInt();
            this._slotId = stream.ReadInt();

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
            encoder.WriteInt(this._unitCount);
            encoder.WriteInt(this._slotId);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 509;
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
            this._unitCount = 0;
            this._unitData = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (!LogicDataTables.GetGlobals().UseNewTraining())
            {
                return -1; // TODO: Implement this.
            }

            return this.NewTrainingUnit(level);
        }

        /// <summary>
        ///     Trains the unit with new training.
        /// </summary>
        public int NewTrainingUnit(LogicLevel level)
        {
            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                if (this._unitData != null)
                {
                    LogicUnitProduction unitProduction = this._unitData.GetCombatItemType() == 1
                        ? level.GetGameObjectManager().GetSpellProduction()
                        : level.GetGameObjectManager().GetUnitProduction();

                    if (!unitProduction.IsLocked())
                    {
                        if (this._unitCount > 0)
                        {
                            if (this._unitData.GetDataType() == unitProduction.GetUnitProductionType())
                            {
                                LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                                LogicResourceData trainingResourceData = this._unitData.GetTrainingResource();
                                Int32 trainingCost = level.GetGameMode().GetCalendar().GetUnitTrainingCost(this._unitData, playerAvatar.GetUnitUpgradeLevel(this._unitData));
                                Int32 refundCount = LogicMath.Max(trainingCost * (this._unitData.GetDataType() != 3
                                                                      ? LogicDataTables.GetGlobals().GetSpellCancelMultiplier()
                                                                      : LogicDataTables.GetGlobals().GetTrainCancelMultiplier()) / 100, 0);
                                
                                while (unitProduction.RemoveUnit(this._unitData, this._slotId))
                                {
                                    playerAvatar.CommodityCountChangeHelper(0, trainingResourceData, refundCount);

                                    if (--this._unitCount <= 0)
                                    {
                                        break;
                                    }
                                }

                                return 0;
                            }
                        }

                        return -1;
                    }

                    return -23;
                }

                return -1;
            }

            return -99;
        }
    }
}