namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicDecoData : LogicData
    {
        private bool _inShop;
        private LogicResourceData _buildResourceData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDecoData" /> class.
        /// </summary>
        public LogicDecoData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicDecoData.
        }

        protected string SWF { get; set; }
        protected string ExportName { get; set; }
        protected string ExportNameConstruction { get; set; }
        protected int BuildCost { get; set; }
        protected int RequiredExpLevel { get; set; }
        protected int MaxCount { get; set; }
        protected int Width { get; set; }
        protected int Height { get; set; }
        protected string Icon { get; set; }
        protected int BaseGfx { get; set; }
        protected string ExportNameBase { get; set; }
        protected bool IsRed { get; set; }
        protected int RedMul { get; set; }
        protected int GreenMul { get; set; }
        protected int BlueMul { get; set; }
        protected int RedAdd { get; set; }
        protected int GreenAdd { get; set; }
        protected int BlueAdd { get; set; }
        protected bool LightsOn { get; set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateVillageReferences();
            
            this._inShop = !this.GetBooleanValue("NotInShop", 0);
            this._buildResourceData = LogicDataTables.GetResourceByName(this.GetValue("BuildingResource", 0));
        }

        /// <summary>
        ///     Gets a value indicating whether this deco is in shop.
        /// </summary>
        public bool IsInShop()
        {
            return this._inShop;
        }

        /// <summary>
        ///     Gets max count of this deco.
        /// </summary>
        public int GetMaxCount()
        {
            return this.MaxCount;
        }

        /// <summary>
        ///     Gets the required exp level.
        /// </summary>
        public int GetRequiredExpLevel()
        {
            return this.RequiredExpLevel;
        }

        /// <summary>
        ///     Gets the sell price.
        /// </summary>
        public int GetSellPrice()
        {
            return this.BuildCost / 10;
        }
    }
}