namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicObstacleData : LogicData
    {
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
        public int ClearTimeSeconds { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public bool Passable { get; protected set; }
        public string ClearResource { get; protected set; }
        public int ClearCost { get; protected set; }
        public string LootResource { get; protected set; }
        public int LootCount { get; protected set; }
        public string ClearEffect { get; protected set; }
        public string PickUpEffect { get; protected set; }
        public int RespawnWeight { get; protected set; }
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
        public int VillageType { get; protected set; }
        public int Village2RespawnCount { get; protected set; }
        public int VariationCount { get; protected set; }
        public bool TallGrass { get; protected set; }
        public bool TallGrassSpawnPoint { get; protected set; }
        public int LootHighlightPercentage { get; protected set; }
        public string HighlightExportName { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        /// <summary>
        ///     Gets the village type.
        /// </summary>
        public override int GetVillageType()
        {
            return this.VillageType;
        }

        public bool IsLootCart()
        {
            return this.LootDefensePercentage > 0;
        }
    }
}