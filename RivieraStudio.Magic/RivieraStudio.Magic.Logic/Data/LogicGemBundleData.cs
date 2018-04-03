namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicGemBundleData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGemBundleData" /> class.
        /// </summary>
        public LogicGemBundleData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicGemBundleData.
        }

        public int LinkedPackageID { get; protected set; }
        public string BillingPackage { get; protected set; }
        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public bool Disabled { get; protected set; }
        public bool ExistsApple { get; protected set; }
        public bool ExistsAndroid { get; protected set; }
        public bool ExistsKunlun { get; protected set; }
        public bool ExistsBazaar { get; protected set; }
        public bool ExistsTencent { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string ShopItemExportName { get; protected set; }
        public string ShopItemInfoExportName { get; protected set; }
        public string ShopItemBG { get; protected set; }
        public int TownhallLimitMin { get; protected set; }
        public int TownhallLimitMax { get; protected set; }
        public int VillageType { get; protected set; }
        protected string[] Buildings { get; set; }
        protected int[] GemCost { get; set; }
        protected string[] BuildingType { get; set; }
        public int BuildingNumber { get; protected set; }
        protected int[] BuildingLevel { get; set; }
        public string UnlocksTroop { get; protected set; }
        public string TroopType { get; protected set; }
        public int GiftGems { get; protected set; }
        public int GiftUsers { get; protected set; }
        protected string[] Resources { get; set; }
        protected int[] ResourceAmounts { get; set; }
        public bool ResourceAmountFromThCSV { get; protected set; }
        public int THResourceMultiplier { get; protected set; }
        public bool RED { get; protected set; }
        protected int[] Priority { get; set; }
        public bool FrontPageItem { get; protected set; }
        public bool TreasureItem { get; protected set; }
        public string ReplacesBillingPackage { get; protected set; }
        public string ValueTID { get; protected set; }
        public int ValueForUI { get; protected set; }
        public int AvailableTimeMinutes { get; protected set; }
        public int CooldownAfterPurchaseMinutes { get; protected set; }
        public int TimesCanBePurchased { get; protected set; }
        public int ShopFrontPageCooldownAfterPurchaseMin { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }

        public string GetBuildings(int index)
        {
            return this.Buildings[index];
        }

        public int GetGemCost(int index)
        {
            return this.GemCost[index];
        }

        public string GetBuildingType(int index)
        {
            return this.BuildingType[index];
        }

        public int GetBuildingLevel(int index)
        {
            return this.BuildingLevel[index];
        }

        public string GetResources(int index)
        {
            return this.Resources[index];
        }

        public int GetResourceAmounts(int index)
        {
            return this.ResourceAmounts[index];
        }

        public int GetPriority(int index)
        {
            return this.Priority[index];
        }
    }
}