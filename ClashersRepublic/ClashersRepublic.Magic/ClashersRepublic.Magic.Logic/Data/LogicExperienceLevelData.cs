namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicExperienceLevelData : LogicData
    {
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
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}