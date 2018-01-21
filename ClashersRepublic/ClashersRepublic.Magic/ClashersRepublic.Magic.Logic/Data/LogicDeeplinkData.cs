namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicDeeplinkData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDeeplinkData" /> class.
        /// </summary>
        public LogicDeeplinkData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicDeeplinkData.
        }

        protected string[] ParameterType { get; set; }
        protected string[] ParameterName { get; set; }
        protected string[] Description { get; set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
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
    }
}