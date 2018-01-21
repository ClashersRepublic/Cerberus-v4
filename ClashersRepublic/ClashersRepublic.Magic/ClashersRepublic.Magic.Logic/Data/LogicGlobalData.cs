namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicGlobalData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGlobalData" /> class.
        /// </summary>
        public LogicGlobalData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicGlobalData.
        }

        public int NumberValue { get; protected set; }
        public bool BooleanValue { get; protected set; }
        public string TextValue { get; protected set; }
        protected int[] NumberArray { get; set; }
        protected int[] AltNumberArray { get; set; }
        public string StringArray { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public int GetNumberArray(int index)
        {
            return this.NumberArray[index];
        }

        public int GetAltNumberArray(int index)
        {
            return this.AltNumberArray[index];
        }
    }
}