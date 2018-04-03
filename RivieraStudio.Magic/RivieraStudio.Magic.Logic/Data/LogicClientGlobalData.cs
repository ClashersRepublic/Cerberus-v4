namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicClientGlobalData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientGlobalData" /> class.
        /// </summary>
        public LogicClientGlobalData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicClientGlobalData.
        }

        public int NumberValue { get; protected set; }
        public bool BooleanValue { get; protected set; }
        public string TextValue { get; protected set; }
        protected int[] NumberArray { get; set; }
        protected string[] StringArray { get; set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }

        public int GetNumberArray(int index)
        {
            return this.NumberArray[index];
        }

        public string GetStringArray(int index)
        {
            return this.StringArray[index];
        }
    }
}