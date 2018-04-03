namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicLeagueData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLeagueData" /> class.
        /// </summary>
        public LogicLeagueData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicLeagueData.
        }

        public string TID { get; protected set; }
        public string TIDShort { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string LeagueBannerIcon { get; protected set; }
        public string LeagueBannerIconNum { get; protected set; }
        public string LeagueBannerIconHUD { get; protected set; }
        public int GoldReward { get; protected set; }
        public int ElixirReward { get; protected set; }
        public int DarkElixirReward { get; protected set; }
        public bool UseStarBonus { get; protected set; }
        public int GoldRewardStarBonus { get; protected set; }
        public int ElixirRewardStarBonus { get; protected set; }
        public int DarkElixirRewardStarBonus { get; protected set; }
        public int PlacementLimitLow { get; protected set; }
        public int PlacementLimitHigh { get; protected set; }
        public int DemoteLimit { get; protected set; }
        public int PromoteLimit { get; protected set; }
        protected int[] BucketPlacementRangeLow { get; set; }
        protected int[] BucketPlacementRangeHigh { get; set; }
        protected int[] BucketPlacementSoftLimit { get; set; }
        protected int[] BucketPlacementHardLimit { get; set; }
        public bool IgnoredByServer { get; protected set; }
        public bool DemoteEnabled { get; protected set; }
        public bool PromoteEnabled { get; protected set; }
        public int AllocateAmount { get; protected set; }
        public int SaverCount { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }

        public int GetBucketPlacementRangeLow(int index)
        {
            return this.BucketPlacementRangeLow[index];
        }

        public int GetBucketPlacementRangeHigh(int index)
        {
            return this.BucketPlacementRangeHigh[index];
        }

        public int GetBucketPlacementSoftLimit(int index)
        {
            return this.BucketPlacementSoftLimit[index];
        }

        public int GetBucketPlacementHardLimit(int index)
        {
            return this.BucketPlacementHardLimit[index];
        }
    }
}