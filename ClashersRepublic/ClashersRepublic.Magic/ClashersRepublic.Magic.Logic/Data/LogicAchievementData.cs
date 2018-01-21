namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicAchievementData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAchievementData" /> class.
        /// </summary>
        public LogicAchievementData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAchievementData.
        }

        public int Level { get; protected set; }
        public int LevelCount { get; protected set; }
        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string Action { get; protected set; }
        public int ActionCount { get; protected set; }
        public string ActionData { get; protected set; }
        public int ExpReward { get; protected set; }
        public int DiamondReward { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string CompletedTID { get; protected set; }
        public bool ShowValue { get; protected set; }
        public string AndroidID { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}