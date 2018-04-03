namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicNewsData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNewsData" /> class.
        /// </summary>
        public LogicNewsData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicNewsData.
        }

        public int ID { get; protected set; }
        public bool Enabled { get; protected set; }
        public bool EnabledIOS { get; protected set; }
        public bool EnabledAndroid { get; protected set; }
        public bool EnabledKunlun { get; protected set; }
        public bool EnabledTencent { get; protected set; }
        public bool EnabledLowEnd { get; protected set; }
        public bool EnabledHighEnd { get; protected set; }
        public string Type { get; protected set; }
        public bool ShowAsNew { get; protected set; }
        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string ButtonTID { get; protected set; }
        public string ActionType { get; protected set; }
        public string ActionParameter1 { get; protected set; }
        public string ActionParameter2 { get; protected set; }
        public string NativeAndroidURL { get; protected set; }
        protected string[] IncludedLanguages { get; set; }
        protected string[] ExcludedLanguages { get; set; }
        public string IncludedCountries { get; protected set; }
        public string ExcludedCountries { get; protected set; }
        protected string[] IncludedLoginCountries { get; set; }
        public string ExcludedLoginCountries { get; protected set; }
        public bool CenterText { get; protected set; }
        public bool LoadResources { get; protected set; }
        public bool LoadInLowEnd { get; protected set; }
        public string ItemSWF { get; protected set; }
        public string ItemExportName { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public int IconFrame { get; protected set; }
        public bool AnimateIcon { get; protected set; }
        public bool CenterIcon { get; protected set; }
        public int MinTownHall { get; protected set; }
        public int MaxTownHall { get; protected set; }
        public int MinLevel { get; protected set; }
        public int MaxLevel { get; protected set; }
        public int MaxDiamonds { get; protected set; }
        public bool ClickToDismiss { get; protected set; }
        public bool NotifyAlways { get; protected set; }
        public int NotifyMinLevel { get; protected set; }
        public int AvatarIdModulo { get; protected set; }
        public int ModuloMin { get; protected set; }
        public int ModuloMax { get; protected set; }
        public bool Collapsed { get; protected set; }
        public string MinOS { get; protected set; }
        public string MaxOS { get; protected set; }
        public string ButtonTID2 { get; protected set; }
        public string Action2Type { get; protected set; }
        public string Action2Parameter1 { get; protected set; }
        public string Action2Parameter2 { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }

        public string GetIncludedLanguages(int index)
        {
            return this.IncludedLanguages[index];
        }

        public string GetExcludedLanguages(int index)
        {
            return this.ExcludedLanguages[index];
        }

        public string GetIncludedLoginCountries(int index)
        {
            return this.IncludedLoginCountries[index];
        }
    }
}