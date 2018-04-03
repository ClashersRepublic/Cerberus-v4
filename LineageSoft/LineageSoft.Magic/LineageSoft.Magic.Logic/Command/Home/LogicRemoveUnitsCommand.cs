namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Level;

    using LineageSoft.Magic.Titan.DataStream;
    using LineageSoft.Magic.Titan.Util;

    public sealed class LogicRemoveUnitsCommand : LogicCommand
    {
        private LogicArrayList<int> _unitsIdx;
        private LogicArrayList<int> _unitsCount;

        private LogicArrayList<LogicData> _unitsData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicRemoveUnitsCommand"/> class.
        /// </summary>
        public LogicRemoveUnitsCommand()
        {
            // LogicRemoveUnitCommand.
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                this._unitsIdx.Add(stream.ReadInt());

                if (stream.ReadInt() != 0)
                {
                    this._unitsData.Add(stream.ReadDataReference(25));
                }
                else
                {
                    this._unitsData.Add(stream.ReadDataReference(3));
                }

                this._unitsCount.Add(stream.ReadInt());
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 550;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            return 0;
        }
    }
}