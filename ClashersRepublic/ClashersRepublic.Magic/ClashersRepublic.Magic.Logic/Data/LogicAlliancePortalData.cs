namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicAlliancePortalData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAlliancePortalData" /> class.
        /// </summary>
        public LogicAlliancePortalData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAlliancePortalData.
        }

        public string TID { get; protected set; }
        public string SWF { get; protected set; }
        protected string[] ExportName { get; set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public string GetExportName(int index)
        {
            return this.ExportName[index];
        }
    }
}