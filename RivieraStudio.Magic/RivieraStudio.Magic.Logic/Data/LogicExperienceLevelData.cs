namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicExperienceLevelData : LogicData
    {
        private int _expPoints;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicExperienceLevelData" /> class.
        /// </summary>
        public LogicExperienceLevelData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicExperienceLevelData.
        }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            this._expPoints = this.GetIntegerValue("ExpPoints", 0);
        }

        /// <summary>
        ///     Get the number of max exp points.
        /// </summary>
        public int GetMaxExpPoints()
        {
            return this._expPoints;
        }

        /// <summary>
        ///     Gets the level cap.
        /// </summary>
        public static int GetLevelCap()
        {
            return LogicDataTables.GetTable(10).GetItemCount();
        }
    }
}