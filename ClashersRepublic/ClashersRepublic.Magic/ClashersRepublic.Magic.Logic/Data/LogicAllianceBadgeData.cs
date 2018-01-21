namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicAllianceBadgeData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAllianceBadgeData" /> class.
        /// </summary>
        public LogicAllianceBadgeData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAllianceBadgeData.
        }

        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string IconLayer0 { get; protected set; }
        public string IconLayer1 { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}