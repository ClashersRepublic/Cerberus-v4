namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicCreditData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCreditData" /> class.
        /// </summary>
        public LogicCreditData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicCreditData.
        }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}