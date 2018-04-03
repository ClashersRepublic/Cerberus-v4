namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicCalendarEventFunctionData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCalendarEventFunctionData" /> class.
        /// </summary>
        public LogicCalendarEventFunctionData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicCalendarEventFunctionData.
        }

        protected string[] ParameterType { get; set; }
        protected string[] ParameterName { get; set; }
        protected string[] Description { get; set; }
        protected int[] MinValue { get; set; }
        protected int[] MaxValue { get; set; }
        public bool TargetingSupported { get; protected set; }
        public string Category { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            // CreateReferences.
        }

        public string GetParameterType(int index)
        {
            return this.ParameterType[index];
        }

        public string GetParameterName(int index)
        {
            return this.ParameterName[index];
        }

        public string GetDescription(int index)
        {
            return this.Description[index];
        }

        public int GetMinValue(int index)
        {
            return this.MinValue[index];
        }

        public int GetMaxValue(int index)
        {
            return this.MaxValue[index];
        }
    }
}