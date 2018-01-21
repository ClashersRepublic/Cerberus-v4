namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicAllianceLevelData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAllianceLevelData" /> class.
        /// </summary>
        public LogicAllianceLevelData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAllianceLevelData.
        }

        public int ExpPoints { get; protected set; }
        public bool IsVisible { get; protected set; }
        public int TroopRequestCooldown { get; protected set; }
        public int TroopDonationLimit { get; protected set; }
        public int TroopDonationRefund { get; protected set; }
        public int TroopDonationUpgrade { get; protected set; }
        public int WarLootCapacityPercent { get; protected set; }
        public int WarLootMultiplierPercent { get; protected set; }
        public int BadgeLevel { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}