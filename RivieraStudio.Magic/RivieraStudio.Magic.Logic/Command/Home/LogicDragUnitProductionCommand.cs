namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicDragUnitProductionCommand : LogicCommand
    {
        private bool _spellProduction;
        private int _slotIdx;
        private int _dragIdx;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDragUnitProductionCommand"/> class.
        /// </summary>
        public LogicDragUnitProductionCommand()
        {
            // LogicDragUnitProductionCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDragUnitProductionCommand"/> class.
        /// </summary>
        public LogicDragUnitProductionCommand(bool spellProduction, int slotIdx, int dragIdx)
        {
            this._spellProduction = spellProduction;
            this._slotIdx = slotIdx;
            this._dragIdx = dragIdx;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._spellProduction = stream.ReadBoolean();
            this._slotIdx = stream.ReadInt();
            this._dragIdx = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteBoolean(this._spellProduction);
            encoder.WriteInt(this._slotIdx);
            encoder.WriteInt(this._dragIdx);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 576;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                if (!LogicDataTables.GetGlobals().UseDragInTraining() &&
                    !LogicDataTables.GetGlobals().UseDragInTrainingFix() &&
                    !LogicDataTables.GetGlobals().UseDragInTrainingFix2())
                {
                    return -51;
                }

                LogicUnitProduction unitProduction = this._spellProduction ? level.GetGameObjectManager().GetSpellProduction() : level.GetGameObjectManager().GetUnitProduction();

                if (unitProduction.GetSlotCount() > this._slotIdx)
                {
                    if (unitProduction.GetSlotCount() >= this._dragIdx)
                    {
                        if (this._slotIdx >= 0)
                        {
                            if (this._dragIdx >= 0)
                            {
                                return unitProduction.DragSlot(this._slotIdx, this._dragIdx) ? 0 : -5;
                            }

                            return -4;
                        }

                        return -3;
                    }

                    return -2;
                }

                return -1;
            }

            return -50;
        }
    }
}