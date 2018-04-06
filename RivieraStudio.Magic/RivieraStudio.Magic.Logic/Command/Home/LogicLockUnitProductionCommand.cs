namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicLockUnitProductionCommand : LogicCommand
    {
        private bool _disabled;
        private int _index;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLockUnitProductionCommand"/> class.
        /// </summary>
        public LogicLockUnitProductionCommand()
        {
            // LogicLockUnitProductionCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLockUnitProductionCommand"/> class.
        /// </summary>
        public LogicLockUnitProductionCommand(bool disabled, int index)
        {
            this._disabled = disabled;
            this._index = index;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._disabled = stream.ReadBoolean();
            this._index = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteBoolean(this._disabled);
            encoder.WriteInt(this._index);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 585;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (level.GetVillageType() == 0)
            {
                LogicUnitProduction unitProduction = null;

                switch (this._index)
                {
                    case 1:
                        unitProduction = level.GetGameObjectManagerAt(0).GetUnitProduction();
                        break;
                    case 2:
                        unitProduction = level.GetGameObjectManagerAt(0).GetSpellProduction();
                        break;
                }

                if (unitProduction == null)
                {
                    return -1;
                }

                unitProduction.SetLocked(this._disabled);

                return 0;
            }

            return -32;
        }
    }
}