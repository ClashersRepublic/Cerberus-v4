namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicWarData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicWarData" /> class.
        /// </summary>
        public LogicWarData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicWarData.
        }

        public int TeamSize { get; protected set; }
        public int PreparationMinutes { get; protected set; }
        public int WarMinutes { get; protected set; }
        public bool DisableProduction { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}