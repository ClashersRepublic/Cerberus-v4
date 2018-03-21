namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicNpcData : LogicData
    {
        private readonly LogicArrayList<LogicNpcData> _dependencies;
        private readonly LogicArrayList<LogicDataSlot> _unitCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNpcData" /> class.
        /// </summary>
        public LogicNpcData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            this._dependencies = new LogicArrayList<LogicNpcData>();
            this._unitCount = new LogicArrayList<LogicDataSlot>();
        }

        public string MapInstanceName { get; protected set; }
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

            int mapDependencySize = this.GetArraySize("MapDependencies");

            for (int i = 0; i < mapDependencySize; i++)
            {
                LogicNpcData data = LogicDataTables.GetNpcByName(this.GetValue("MapDependencies", i));

                if (data != null)
                {
                    this._dependencies.Add(data);
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

        
        /// <summary>
        ///     Gets if this <see cref="LogicNpcData"/> is unlocked on the map of specified avatar.
        /// </summary>
        public bool IsUnlockedInMap(LogicClientAvatar avatar)
        {
            if (!this.AlwaysUnlocked)
            {
                if (!string.IsNullOrEmpty(this.MapInstanceName))
                {
                    if (this._dependencies != null)
                    {
                        for (int i = 0; i < this._dependencies.Count; i++)
                        {
                            if (avatar.GetNpcStars(this._dependencies[i]) > 0)
                            {
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return true;
        }
    }
}