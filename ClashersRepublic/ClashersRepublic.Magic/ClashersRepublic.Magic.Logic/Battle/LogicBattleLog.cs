namespace ClashersRepublic.Magic.Logic.Battle
{
    using ClashersRepublic.Magic.Logic.Level;

    public class LogicBattleLog
    {
        private LogicLevel _level;
        private bool _battleEnded;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBattleLog" /> class.
        /// </summary>
        public LogicBattleLog(LogicLevel level)
        {
            this._level = level;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Gets a value indicating whether this battle is started.
        /// </summary>
        public bool GetBattleStarted()
        {
            return false;
        }

        /// <summary>
        ///     Gets a value indicating whether this battle is ended.
        /// </summary>
        public bool GetBattleEnded()
        {
            return this._battleEnded;
        }
    }
}