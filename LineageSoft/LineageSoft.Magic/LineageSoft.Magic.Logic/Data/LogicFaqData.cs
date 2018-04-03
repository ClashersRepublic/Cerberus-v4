namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicFaqData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicFaqData" /> class.
        /// </summary>
        public LogicFaqData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicFaqData.
        }

        public string TID { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}