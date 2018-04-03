namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicBoomboxData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBoomboxData" /> class.
        /// </summary>
        public LogicBoomboxData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicBoomboxData.
        }

        public bool Enabled { get; protected set; }
        public bool EnabledLowMem { get; protected set; }
        public string DisabledDevices { get; protected set; }
        public bool PreLoading { get; protected set; }
        public bool PreLoadingLowMem { get; protected set; }
        protected string[] SupportedPlatforms { get; set; }
        public string SupportedPlatformsVersion { get; protected set; }
        protected string[] AllowedDomains { get; set; }
        protected string[] AllowedUrls { get; set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }

        public string GetSupportedPlatforms(int index)
        {
            return this.SupportedPlatforms[index];
        }

        public string GetAllowedDomains(int index)
        {
            return this.AllowedDomains[index];
        }

        public string GetAllowedUrls(int index)
        {
            return this.AllowedUrls[index];
        }
    }
}