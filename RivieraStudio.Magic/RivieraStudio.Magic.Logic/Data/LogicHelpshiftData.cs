namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicHelpshiftData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHelpshiftData" /> class.
        /// </summary>
        public LogicHelpshiftData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicHelpshiftData.
        }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // LoadingFinished.
        }
    }
}