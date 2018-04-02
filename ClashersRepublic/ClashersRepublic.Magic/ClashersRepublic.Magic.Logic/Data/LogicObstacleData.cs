namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Debug;

    public class LogicObstacleData : LogicData
    {
        private LogicResourceData _clearResourceData;
        private LogicResourceData _lootResourceData;
        private int _lootCount;
        private int _clearCost;
        private int _clearTimeSecs;
        private int _respawnWeight;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicObstacleData" /> class.
        /// </summary>
        public LogicObstacleData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicObstacleData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string SWF { get; protected set; }
        public string ExportName { get; protected set; }
        public string ExportNameBase { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool Passable { get; protected set; }
        public string ClearEffect { get; protected set; }
        public string PickUpEffect { get; protected set; }
        public bool IsTombstone { get; protected set; }
        public int TombGroup { get; protected set; }
        public int LootMultiplierForVersion2 { get; protected set; }
        public int AppearancePeriodHours { get; protected set; }
        public int MinRespawnTimeHours { get; protected set; }
        public string SpawnObstacle { get; protected set; }
        public int SpawnRadius { get; protected set; }
        public int SpawnIntervalSeconds { get; protected set; }
        public int SpawnCount { get; protected set; }
        public int MaxSpawned { get; protected set; }
        public int MaxLifetimeSpawns { get; protected set; }
        public int LootDefensePercentage { get; protected set; }
        public int RedMul { get; protected set; }
        public int GreenMul { get; protected set; }
        public int BlueMul { get; protected set; }
        public int RedAdd { get; protected set; }
        public int GreenAdd { get; protected set; }
        public int BlueAdd { get; protected set; }
        public bool LightsOn { get; protected set; }
        public int Village2RespawnCount { get; protected set; }
        public int VariationCount { get; protected set; }
        public bool TallGrass { get; protected set; }
        public bool TallGrassSpawnPoint { get; protected set; }
        public int LootHighlightPercentage { get; protected set; }
        public string HighlightExportName { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateVillageReferences();
            this._clearResourceData = LogicDataTables.GetResourceByName(this.GetValue("ClearResource", 0));

            if (this._clearResourceData == null)
            {
                Debugger.Error("Clear resource is not defined for obstacle: " + this.GetName());
            }

            this._clearCost = this.GetIntegerValue("ClearCost", 0);
            this._clearTimeSecs = this.GetIntegerValue("ClearTimeSeconds", 0);
            this._respawnWeight = this.GetIntegerValue("RespawnWeight", 0);
            
            string lootResourceName = this.GetValue("LootResource", 0);

            if (lootResourceName.Length <= 0)
            {
                this._respawnWeight = 0;
            }
            else
            {
                this._lootResourceData = LogicDataTables.GetResourceByName(lootResourceName);
                this._lootCount = this.GetIntegerValue("LootCount", 0);
            }
        }

        /// <summary>
        ///     Gets the respawn weight.
        /// </summary>
        public int GetRespawnWeight()
        {
            return this._respawnWeight;
        }

        /// <summary>
        ///     Gets the clear time.
        /// </summary>
        public int GetClearTime()
        {
            return this._clearTimeSecs;
        }

        /// <summary>
        ///     Gets the clear resource data.
        /// </summary>
        public LogicResourceData GetClearResourceData()
        {
            return this._clearResourceData;
        }

        /// <summary>
        ///     Gets the loot resource data.
        /// </summary>
        public LogicResourceData GetLootResourceData()
        {
            return this._lootResourceData;
        }

        /// <summary>
        ///     Gets the loot count.
        /// </summary>
        public int GetLootCount()
        {
            return this._lootCount;
        }

        /// <summary>
        ///     Gets the clear cost.
        /// </summary>
        public int GetClearCost()
        {
            return this._clearCost;
        }

        public bool IsLootCart()
        {
            return this.LootDefensePercentage > 0;
        }
    }
}