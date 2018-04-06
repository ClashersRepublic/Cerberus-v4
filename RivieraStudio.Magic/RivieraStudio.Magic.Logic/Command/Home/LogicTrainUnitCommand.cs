namespace RivieraStudio.Magic.Logic.Command.Home
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;

    public sealed class LogicTrainUnitCommand : LogicCommand
    {
        private LogicCombatItemData _unitData;

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
            this._unitData = (LogicCombatItemData) stream.ReadDataReference(this._unitType != 0 ? 25 : 3);
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

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (level.GetVillageType() == 0)
            {
                if (!LogicDataTables.GetGlobals().UseNewTraining())
                {
                    if (this._gameObjectId == 0)
                    {
                        return -1;
                    }

                    // TODO: Implement this.
                }
                else
                {
                    return this.NewTrainingUnit(level);
                }
            }

            return -32;
        }

        /// <summary>
        ///     Trains the unit with new training.
        /// </summary>
        public int NewTrainingUnit(LogicLevel level)
        {
            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                if (this._trainCount <= 100)
                {
                    LogicUnitProduction unitProduction = this._unitType == 1
                        ? level.GetGameObjectManagerAt(0).GetSpellProduction()
                        : level.GetGameObjectManagerAt(0).GetUnitProduction();

                    if (this._trainCount > 0)
                    {
                        if (this._unitData != null)
                        {
                            LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                            Int32 trainingCost = level.GetGameMode().GetCalendar().GetUnitTrainingCost(this._unitData, playerAvatar.GetUnitUpgradeLevel(this._unitData));

                            for (int i = 0; i < this._trainCount; i++)
                            {
                                if (!unitProduction.CanAddUnitToQueue(this._unitData, false))
                                {
                                    return -40;
                                }

                                if (true) // unk slot.
                                {
                                    if (!playerAvatar.HasEnoughResources(this._unitData.GetTrainingResource(), trainingCost, true, this, false))
                                    {
                                        return -30;
                                    }

                                    playerAvatar.CommodityCountChangeHelper(0, this._unitData.GetTrainingResource(), -trainingCost);
                                }
                                else
                                {
                                    playerAvatar.CommodityCountChangeHelper(9, this._unitData, -1);
                                }

                                if (this._slotId == -1)
                                {
                                    this._slotId = unitProduction.GetSlotCount();
                                }

                                unitProduction.AddUnitToQueue(this._unitData, this._slotId, false);
                            }

                            return 0;
                        }
                    }

                    return -50;
                }
                else
                {
                    Debugger.Error("LogicTraingUnitCommand - Count is too high");
                }

                return -20;
            }

            return -99;
        }
    }
}