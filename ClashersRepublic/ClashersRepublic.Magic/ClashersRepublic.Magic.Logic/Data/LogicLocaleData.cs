namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicLocaleData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLocaleData" /> class.
        /// </summary>
        public LogicLocaleData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicLocaleData.
        }

        public string FileName { get; protected set; }
        public string LocalizedName { get; protected set; }
        public bool HasEvenSpaceCharacters { get; protected set; }
        public bool isRTL { get; protected set; }
        public string UsedSystemFont { get; protected set; }
        public string HelpshiftSDKLanguage { get; protected set; }
        public string HelpshiftSDKLanguageAndroid { get; protected set; }
        public int SortOrder { get; protected set; }
        public bool TestLanguage { get; protected set; }
        protected string[] TestExcludes { get; set; }
        public bool BoomboxEnabled { get; protected set; }
        public string BoomboxUrl { get; protected set; }
        public string BoomboxStagingUrl { get; protected set; }
        public string HelpshiftLanguageTagOverride { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public string GetTestExcludes(int index)
        {
            return this.TestExcludes[index];
        }
    }
}