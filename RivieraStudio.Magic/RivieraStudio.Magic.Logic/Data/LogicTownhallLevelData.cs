namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicTownhallLevelData : LogicData
    {
        private LogicArrayList<int> _buildingCaps;
        private LogicArrayList<int> _buildingGearupCaps;
        private LogicArrayList<int> _trapCaps;
        private LogicArrayList<int> _treasuryCaps;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTownhallLevelData" /> class.
        /// </summary>
        public LogicTownhallLevelData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicTownhallLevelData.
        }

        protected int AttackCost { get; set; }
        protected int ResourceStorageLootPercentage { get; set; }
        protected int DarkElixirStorageLootPercentage { get; set; }
        protected int ResourceStorageLootCap { get; set; }
        protected int DarkElixirStorageLootCap { get; set; }
        protected int WarPrizeResourceCap { get; set; }
        protected int WarPrizeDarkElixirCap { get; set; }
        protected int WarPrizeAllianceExpCap { get; set; }
        protected int CartLootCapResource { get; set; }
        protected int CartLootReengagementResource { get; set; }
        protected int CartLootCapDarkElixir { get; set; }
        protected int CartLootReengagementDarkElixir { get; set; }
        protected int StrengthMaxTroopTypes { get; set; }
        protected int StrengthMaxSpellTypes { get; set; }
        protected int FriendlyCost { get; set; }
        protected int PackElixir { get; set; }
        protected int PackGold { get; set; }
        protected int PackDarkElixir { get; set; }
        protected int PackGold2 { get; set; }
        protected int PackElixir2 { get; set; }
        protected int DuelPrizeResourceCap { get; set; }
        protected int AttackCostVillage2 { get; set; }
        protected int ChangeTroopCost { get; set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            this._buildingCaps = new LogicArrayList<int>();
            this._buildingGearupCaps = new LogicArrayList<int>();
            this._trapCaps = new LogicArrayList<int>();
            this._treasuryCaps = new LogicArrayList<int>();

            LogicTownhallLevelData previousItem = null;

            if (this.GetInstanceID() > 0)
            {
                previousItem = (LogicTownhallLevelData) this._table.GetItemAt(this.GetInstanceID() - 1);
            }

            LogicDataTable buildingTable = LogicDataTables.GetTable(0);

            for (int i = 0; i < buildingTable.GetItemCount(); i++)
            {
                LogicData item = buildingTable.GetItemAt(i);

                int cap = this.GetIntegerValue(item.GetName(), 0);
                int gearup = this.GetIntegerValue(item.GetName() + "_gearup", 0);

                if (previousItem != null)
                {
                    if (cap == 0)
                    {
                        cap = previousItem._buildingCaps[i];
                    }

                    if (gearup == 0)
                    {
                        gearup = previousItem._buildingGearupCaps[i];
                    }
                }

                this._buildingCaps.Add(cap);
                this._buildingGearupCaps.Add(gearup);
            }

            LogicDataTable trapTable = LogicDataTables.GetTable(11);

            for (int i = 0; i < trapTable.GetItemCount(); i++)
            {
                int cap = this.GetIntegerValue(trapTable.GetItemAt(i).GetName(), 0);

                if (previousItem != null)
                {
                    if (cap == 0)
                    {
                        cap = previousItem._buildingCaps[i];
                    }
                }

                this._trapCaps.Add(cap);
            }

            LogicDataTable resourceTable = LogicDataTables.GetTable(2);

            for (int i = 0; i < resourceTable.GetItemCount(); i++)
            {
                this._treasuryCaps.Add(this.GetIntegerValue("Treasury" + resourceTable.GetItemAt(i).GetName(), 0));
            }

            if (this.DarkElixirStorageLootPercentage > 100 || this.DarkElixirStorageLootPercentage < 0 || this.ResourceStorageLootPercentage > 100 || this.ResourceStorageLootPercentage < 0)
            {
                Debugger.Error("townhall_levels.csv: Invalid loot percentage!");
            }
        }

        /// <summary>
        ///     Gets the storage loot percentage.
        /// </summary>
        public int GetStorageLootPercentage(LogicResourceData data)
        {
            if (LogicDataTables.GetDarkElixirData() == data)
            {
                return this.DarkElixirStorageLootPercentage;
            }

            return this.ResourceStorageLootPercentage;
        }

        /// <summary>
        ///     Gets the storage loot cap.
        /// </summary>
        public int GetStorageLootCap(LogicResourceData data)
        {
            if (data == null || data.PremiumCurrency)
            {
                return 0;
            }

            if (LogicDataTables.GetDarkElixirData() == data)
            {
                return this.DarkElixirStorageLootCap;
            }

            return this.ResourceStorageLootCap;
        }

        /// <summary>
        ///     Gets the cart loot cap.
        /// </summary>
        public int GetCartLootCap(LogicResourceData data)
        {
            if (data == null || data.PremiumCurrency)
            {
                return 0;
            }

            if (LogicDataTables.GetDarkElixirData() == data)
            {
                return this.CartLootCapDarkElixir;
            }

            return this.CartLootCapResource;
        }

        /// <summary>
        ///     Gets the unlocked building count.
        /// </summary>
        public int GetUnlockedBuildingCount(LogicBuildingData data)
        {
            return this._buildingCaps[data.GetInstanceID()];
        }

        /// <summary>
        ///     Gets the unlocked building gearup count.
        /// </summary>
        public int GetUnlockedBuildingGearupCount(LogicBuildingData data)
        {
            return this._buildingGearupCaps[data.GetInstanceID()];
        }

        /// <summary>
        ///     Gets the unlocked trap count.
        /// </summary>
        public int GetUnlockedTrapCount(LogicTrapData data)
        {
            return this._trapCaps[data.GetInstanceID()];
        }

        /// <summary>
        ///     Gets the treasury caps.
        /// </summary>
        public LogicArrayList<int> GetTreasuryCaps()
        {
            return this._treasuryCaps;
        }
    }
}