namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicHintData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHintData" /> class.
        /// </summary>
        public LogicHintData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicHintData.
        }

        public string TID { get; protected set; }
        public int TownHallLevelMin { get; protected set; }
        public int TownHallLevelMax { get; protected set; }
        public int VillageType { get; protected set; }
        public string iOSTID { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}