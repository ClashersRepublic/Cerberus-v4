namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicDecoData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDecoData" /> class.
        /// </summary>
        public LogicDecoData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicDecoData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string SWF { get; protected set; }
        public string ExportName { get; protected set; }
        public string ExportNameConstruction { get; protected set; }
        public string BuildResource { get; protected set; }
        public int BuildCost { get; protected set; }
        public int RequiredExpLevel { get; protected set; }
        public int MaxCount { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public string Icon { get; protected set; }
        public int BaseGfx { get; protected set; }
        public string ExportNameBase { get; protected set; }
        public bool IsRed { get; protected set; }
        public bool NotInShop { get; protected set; }
        public int VillageType { get; protected set; }
        public int RedMul { get; protected set; }
        public int GreenMul { get; protected set; }
        public int BlueMul { get; protected set; }
        public int RedAdd { get; protected set; }
        public int GreenAdd { get; protected set; }
        public int BlueAdd { get; protected set; }
        public bool LightsOn { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}