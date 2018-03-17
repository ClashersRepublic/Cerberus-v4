namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicVillageObjectData : LogicData
    {
        private int _upgradeLevelCount;
        private int[] _buildTime;
        private bool _shipyard;
        private bool _rowBoat;
        private bool _clanGate;

        private LogicResourceData _buildResourceData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicVillageObjectData" /> class.
        /// </summary>
        public LogicVillageObjectData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicVillageObjectData.
        }

        public bool Disabled { get; protected set; }
        protected string[] SWF { get; set; }
        protected string[] ExportName { get; set; }
        public int TileX100 { get; set; }
        public int TileY100 { get; set; }
        public int RequiredTH { get; protected set; }
        public bool AutomaticUpgrades { get; protected set; }
        public bool RequiresBuilder { get; protected set; }
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
            this._shipyard = string.Equals("Shipyard", this.GetName());

            if (!this._shipyard)
            {
                this._shipyard = string.Equals("Shipyard2", this.GetName());
            }

            this._rowBoat = string.Equals("Rowboat", this.GetName());

            if (!this._rowBoat)
            {
                this._rowBoat = string.Equals("Rowboat2", this.GetName());
            }

            this._clanGate = string.Equals("ClanGate", this.GetName());

            this._upgradeLevelCount = this._row.GetBiggestArraySize();
            this._buildTime = new int[this._row.GetBiggestArraySize()];

            for (int i = 0; i < this._upgradeLevelCount; i++)
            {
                this._buildTime[i] = 86400 * this.GetIntegerValue("BuildTimeD", i) +
                                     3600 * this.GetIntegerValue("BuildTimeH", i) +
                                     60 * this.GetIntegerValue("BuildTimeM", i) +
                                     this.GetIntegerValue("BuildTimeS", i);
            }

            this._buildResourceData = LogicDataTables.GetResourceByName(this.GetValue("BuildResource", 0));
        }

        public bool IsShipyard()
        {
            return this._shipyard;
        }

        public bool IsRowBoat()
        {
            return this._rowBoat;
        }

        public bool IsClanGate()
        {
            return this._clanGate;
        }

        public string GetSWF(int index)
        {
            return this.SWF[index];
        }

        public string GetExportName(int index)
        {
            return this.ExportName[index];
        }
        
        public int GetBuildTime(int index)
        {
            return this._buildTime[index];
        }

        public int GetTownHallLevel(int index)
        {
            return this.TownHallLevel[index];
        }

        public int GetUpgradeLevelCount()
        {
            return this._upgradeLevelCount;
        }

        public override int GetVillageType()
        {
            return this.VillageType;
        }
    }
}