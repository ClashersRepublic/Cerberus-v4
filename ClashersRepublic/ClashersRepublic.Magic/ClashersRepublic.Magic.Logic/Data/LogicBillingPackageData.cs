namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicBillingPackageData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBillingPackageData" /> class.
        /// </summary>
        public LogicBillingPackageData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicBillingPackageData.
        }

        public string TID { get; protected set; }
        public bool Disabled { get; protected set; }
        public bool ExistsApple { get; protected set; }
        public bool ExistsAndroid { get; protected set; }
        public int Diamonds { get; protected set; }
        public int USD { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string ShopItemExportName { get; protected set; }
        public string OfferItemExportName { get; protected set; }
        public int Order { get; protected set; }
        public bool RED { get; protected set; }
        public int RMB { get; protected set; }
        public bool KunlunOnly { get; protected set; }
        public int LenovoID { get; protected set; }
        public string TencentID { get; protected set; }
        public bool isOfferPackage { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}