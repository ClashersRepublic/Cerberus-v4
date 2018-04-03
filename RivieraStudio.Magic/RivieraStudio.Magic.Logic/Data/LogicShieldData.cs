namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicShieldData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicShieldData" /> class.
        /// </summary>
        public LogicShieldData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicShieldData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public int TimeH { get; protected set; }
        public int GuardTimeH { get; protected set; }
        public int Diamonds { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public int CooldownS { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }
    }
}