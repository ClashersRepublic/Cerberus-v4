namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicShieldData : LogicData
    {
        private int _diamondsCost;
        private int _cooldownSecs;
        private int _scoreLimit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicShieldData" /> class.
        /// </summary>
        public LogicShieldData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicShieldData.
        }
        
        public int TimeH { get; protected set; }
        public int GuardTimeH { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            this._diamondsCost = this.GetIntegerValue("Diamonds", 0);
            this._cooldownSecs = this.GetIntegerValue("CooldownS", 0);
            this._scoreLimit = this.GetIntegerValue("LockedAboveScore", 0);
        }

        /// <summary>
        ///     Gets the diamonds cost.
        /// </summary>
        public int GetDiamondsCost()
        {
            return this._diamondsCost;
        }

        /// <summary>
        ///     Gets the cooldown secs.
        /// </summary>
        public int GetCooldownSecs()
        {
            return this._cooldownSecs;
        }

        /// <summary>
        ///     Gets the score limit.
        /// </summary>
        public int GetScoreLimit()
        {
            return this._scoreLimit;
        }
    }
}