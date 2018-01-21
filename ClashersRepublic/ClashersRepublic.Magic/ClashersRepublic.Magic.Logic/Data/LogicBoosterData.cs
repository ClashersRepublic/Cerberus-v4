namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicBoosterData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBoosterData" /> class.
        /// </summary>
        public LogicBoosterData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicBoosterData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string SWF { get; protected set; }
        public string ExportName { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public bool Enabled { get; protected set; }
        public int MaxItems { get; protected set; }
        public int DiamondValue { get; protected set; }
        public bool FinishTroopUpgrade { get; protected set; }
        public bool FinishBuildingUpgrade { get; protected set; }
        public bool FinishSpellUpgrade { get; protected set; }
        public bool FinishHeroUpgrade { get; protected set; }
        public bool MaxLevelArmy { get; protected set; }
        public bool BoostResource { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}