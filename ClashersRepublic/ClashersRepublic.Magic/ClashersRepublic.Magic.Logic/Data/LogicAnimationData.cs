namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicAnimationData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAnimationData" /> class.
        /// </summary>
        public LogicAnimationData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAnimationData.
        }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}