namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicAllianceBadgeLayerData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAllianceBadgeLayerData" /> class.
        /// </summary>
        public LogicAllianceBadgeLayerData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAllianceBadgeLayerData.
        }

        public string Type { get; protected set; }
        public string SWF { get; protected set; }
        public string ExportName { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}