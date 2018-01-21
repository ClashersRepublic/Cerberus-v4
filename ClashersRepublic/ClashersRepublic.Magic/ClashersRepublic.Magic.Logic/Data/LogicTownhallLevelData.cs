namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicTownhallLevelData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTownhallLevelData" /> class.
        /// </summary>
        public LogicTownhallLevelData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicTownhallLevelData.
        }

        public int AttackCost { get; protected set; }
        public int ResourceStorageLootPercentage { get; protected set; }
        public int DarkElixirStorageLootPercentage { get; protected set; }
        public int ResourceStorageLootCap { get; protected set; }
        public int DarkElixirStorageLootCap { get; protected set; }
        public int WarPrizeResourceCap { get; protected set; }
        public int WarPrizeDarkElixirCap { get; protected set; }
        public int WarPrizeAllianceExpCap { get; protected set; }
        public int CartLootCapResource { get; protected set; }
        public int CartLootReengagementResource { get; protected set; }
        public int CartLootCapDarkElixir { get; protected set; }
        public int CartLootReengagementDarkElixir { get; protected set; }
        public int Barrack { get; protected set; }
        public int Cannon { get; protected set; }
        public int Cannon_gearup { get; protected set; }
        public int Wall { get; protected set; }
        public int Mortar { get; protected set; }
        public int Mortar_gearup { get; protected set; }
        public int Ejector { get; protected set; }
        public int Superbomb { get; protected set; }
        public int Mine { get; protected set; }
        public int Laboratory { get; protected set; }
        public int Bow { get; protected set; }
        public int Halloweenbomb { get; protected set; }
        public int Slowbomb { get; protected set; }
        public int AirTrap { get; protected set; }
        public int MegaAirTrap { get; protected set; }
        public int SantaTrap { get; protected set; }
        public int StrengthMaxTroopTypes { get; protected set; }
        public int StrengthMaxSpellTypes { get; protected set; }
        public int Totem { get; protected set; }
        public int Halloweenskels { get; protected set; }
        public int TreasuryGold { get; protected set; }
        public int TreasuryElixir { get; protected set; }
        public int TreasuryDarkElixir { get; protected set; }
        public int TreasuryWarGold { get; protected set; }
        public int TreasuryWarElixir { get; protected set; }
        public int TreasuryWarDarkElixir { get; protected set; }
        public int FriendlyCost { get; protected set; }
        public int PackElixir { get; protected set; }
        public int PackGold { get; protected set; }
        public int PackDarkElixir { get; protected set; }
        public int PackGold2 { get; protected set; }
        public int PackElixir2 { get; protected set; }
        public int FreezeBomb { get; protected set; }
        public int DuelPrizeResourceCap { get; protected set; }
        public int WallStraight { get; protected set; }
        public int Cannon2 { get; protected set; }
        public int Laboratory2 { get; protected set; }
        public int Barrack2 { get; protected set; }
        public int Pusher { get; protected set; }
        public int Crusher { get; protected set; }
        public int AirGroundTrap { get; protected set; }
        public int MegaAirGroundTrap { get; protected set; }
        public int AttackCostVillage2 { get; protected set; }
        public int ChangeTroopCost { get; protected set; }
        public int Flamer { get; protected set; }
        public int Ejector2 { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}