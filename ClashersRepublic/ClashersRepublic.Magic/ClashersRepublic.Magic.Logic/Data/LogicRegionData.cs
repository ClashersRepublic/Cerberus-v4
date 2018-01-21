namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicRegionData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicRegionData" /> class.
        /// </summary>
        public LogicRegionData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicRegionData.
        }

        public string TID { get; protected set; }
        public string DisplayName { get; protected set; }
        public bool IsCountry { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}