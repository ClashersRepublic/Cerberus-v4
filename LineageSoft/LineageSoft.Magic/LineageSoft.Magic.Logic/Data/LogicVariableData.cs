namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicVariableData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicVariableData" /> class.
        /// </summary>
        public LogicVariableData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicVariableData.
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