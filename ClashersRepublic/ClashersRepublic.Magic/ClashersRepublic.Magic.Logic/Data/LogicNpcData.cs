namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicNpcData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNpcData" /> class.
        /// </summary>
        public LogicNpcData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicNpcData.
        }

        public string MapInstanceName { get; protected set; }
        protected string[] MapDependencies { get; set; }
        public string TID { get; protected set; }
        public int ExpLevel { get; protected set; }
        public string UnitType { get; protected set; }
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
            // LoadingFinished.
        }

        public string GetMapDependencies(int index)
        {
            return this.MapDependencies[index];
        }
    }
}