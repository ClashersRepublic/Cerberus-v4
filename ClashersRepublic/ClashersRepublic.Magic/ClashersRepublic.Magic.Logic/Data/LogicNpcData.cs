namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicNpcData : LogicData
    {
        private readonly LogicArrayList<LogicDataSlot> _unitCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNpcData" /> class.
        /// </summary>
        public LogicNpcData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            this._unitCount = new LogicArrayList<LogicDataSlot>();
        }

        public string MapInstanceName { get; protected set; }
        protected string[] MapDependencies { get; set; }
        public int ExpLevel { get; protected set; }
        public int UnitCount { get; protected set; }
        public string LevelFile { get; protected set; }
        public int Gold { get; protected set; }
        public int Elixir { get; protected set; }
        public bool AlwaysUnlocked { get; protected set; }
        public string PlayerName { get; protected set; }
        public string AllianceName { get; protected set; }
        public int AllianceBadge { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            int unitCountSize = this.GetArraySize("UnitType");

            if (unitCountSize > 0)
            {
                this._unitCount.EnsureCapacity(unitCountSize);

                for (int i = 0; i < unitCountSize; i++)
                {
                    int count = this.GetIntegerValue("UnitCount", i);

                    if (count > 0)
                    {
                        this._unitCount.Add(new LogicDataSlot(LogicDataTables.GetCharacterByName(this.GetValue("UnitType", i)), count));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the cloned units.
        /// </summary>
        public LogicArrayList<LogicDataSlot> GetClonedUnits()
        {
            LogicArrayList<LogicDataSlot> units = new LogicArrayList<LogicDataSlot>();

            for (int i = 0; i < this._unitCount.Count; i++)
            {
                units.Add(this._unitCount[i].Clone());
            }

            return units;
        }


        public string GetMapDependencies(int index)
        {
            return this.MapDependencies[index];
        }
    }
}