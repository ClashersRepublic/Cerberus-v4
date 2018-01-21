namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicResourceData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicResourceData" /> class.
        /// </summary>
        public LogicResourceData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicResourceData.
        }

        public string TID { get; protected set; }
        public string SWF { get; protected set; }
        public string CollectEffect { get; protected set; }
        public string ResourceIconExportName { get; protected set; }
        public string StealEffect { get; protected set; }
        public int StealLimitMid { get; protected set; }
        public string StealEffectMid { get; protected set; }
        public int StealLimitBig { get; protected set; }
        public string StealEffectBig { get; protected set; }
        public bool PremiumCurrency { get; protected set; }
        public string HudInstanceName { get; protected set; }
        public string CapFullTID { get; protected set; }
        public int TextRed { get; protected set; }
        public int TextGreen { get; protected set; }
        public int TextBlue { get; protected set; }
        public string WarRefResource { get; protected set; }
        public string BundleIconExportName { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}