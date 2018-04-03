namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicTaskData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTaskData" /> class.
        /// </summary>
        public LogicTaskData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicTaskData.
        }

        public string TaskType { get; protected set; }
        public string ProgressType { get; protected set; }
        public string Category { get; protected set; }
        public string Set { get; protected set; }
        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public int MinScore { get; protected set; }
        public int MaxScore { get; protected set; }
        public int MinDurationMinutes { get; protected set; }
        public int MaxDurationMinutes { get; protected set; }
        public int MinQuantity { get; protected set; }
        public int MaxQuantity { get; protected set; }
        public int MinQuantity2 { get; protected set; }
        public int MaxQuantity2 { get; protected set; }
        public string Data1 { get; protected set; }
        public string Data2 { get; protected set; }
        public int SelectionWeight { get; protected set; }
        public int MinLeague { get; protected set; }
        public int MaxLeague { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}