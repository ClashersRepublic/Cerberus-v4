namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicEventEntryData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicEventEntryData" /> class.
        /// </summary>
        public LogicEventEntryData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicEventEntryData.
        }

        public string ItemSWF { get; protected set; }
        public string ItemExportName { get; protected set; }
        public string UpcomingItemExportName { get; protected set; }
        public bool LoadSWF { get; protected set; }
        public string TitleTID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string UpcomingTitleTID { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string ButtonTID { get; protected set; }
        public string ButtonAction { get; protected set; }
        public string ButtonActionData { get; protected set; }
        public string Button2TID { get; protected set; }
        public string Button2Action { get; protected set; }
        public string Button2ActionData { get; protected set; }
        public string ButtonLanguage { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}