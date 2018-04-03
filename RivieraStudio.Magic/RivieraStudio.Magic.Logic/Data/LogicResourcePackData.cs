namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicResourcePackData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicResourcePackData" /> class.
        /// </summary>
        public LogicResourcePackData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicResourcePackData.
        }

        public string TID { get; protected set; }
        public string Resource { get; protected set; }
        public int CapacityPercentage { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}