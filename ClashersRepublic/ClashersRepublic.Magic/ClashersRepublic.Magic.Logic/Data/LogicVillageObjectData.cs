namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicVillageObjectData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicVillageObjectData" /> class.
        /// </summary>
        public LogicVillageObjectData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicVillageObjectData.
        }

        public bool Disabled { get; protected set; }
        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        protected string[] SWF { get; set; }
        protected string[] ExportName { get; set; }
        protected int[] TileX100 { get; set; }
        protected int[] TileY100 { get; set; }
        public int RequiredTH { get; protected set; }
        public bool AutomaticUpgrades { get; protected set; }
        protected int[] BuildTimeD { get; set; }
        protected int[] BuildTimeH { get; set; }
        protected int[] BuildTimeM { get; set; }
        protected int[] BuildTimeS { get; set; }
        public bool RequiresBuilder { get; protected set; }
        public string BuildResource { get; protected set; }
        public int BuildCost { get; protected set; }
        protected int[] TownHallLevel { get; set; }
        public string PickUpEffect { get; protected set; }
        public string Animations { get; protected set; }
        public int AnimX { get; protected set; }
        public int AnimY { get; protected set; }
        public int AnimID { get; protected set; }
        public int AnimDir { get; protected set; }
        public int AnimVisibilityOdds { get; protected set; }
        public bool HasInfoScreen { get; protected set; }
        public int VillageType { get; protected set; }
        public int UnitHousing { get; protected set; }
        public bool HousesUnits { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public string GetSWF(int index)
        {
            return this.SWF[index];
        }

        public string GetExportName(int index)
        {
            return this.ExportName[index];
        }

        public int GetTileX100(int index)
        {
            return this.TileX100[index];
        }

        public int GetTileY100(int index)
        {
            return this.TileY100[index];
        }

        public int GetBuildTimeD(int index)
        {
            return this.BuildTimeD[index];
        }

        public int GetBuildTimeH(int index)
        {
            return this.BuildTimeH[index];
        }

        public int GetBuildTimeM(int index)
        {
            return this.BuildTimeM[index];
        }

        public int GetBuildTimeS(int index)
        {
            return this.BuildTimeS[index];
        }

        public int GetTownHallLevel(int index)
        {
            return this.TownHallLevel[index];
        }
    }
}