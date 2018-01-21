namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicLeague2Data : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLeague2Data" /> class.
        /// </summary>
        public LogicLeague2Data(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicLeague2Data.
        }

        public int TrophyLimitLow { get; protected set; }
        public int TrophyLimitHigh { get; protected set; }
        public int GoldReward { get; protected set; }
        public int ElixirReward { get; protected set; }
        public int BonusGold { get; protected set; }
        public int BonusElixir { get; protected set; }
        public int SeasonTrophyReset { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}