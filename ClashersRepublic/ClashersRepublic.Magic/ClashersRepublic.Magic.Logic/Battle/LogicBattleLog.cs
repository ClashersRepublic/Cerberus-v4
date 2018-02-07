namespace ClashersRepublic.Magic.Logic.Battle
{
    using ClashersRepublic.Magic.Logic.Level;

    public class LogicBattleLog
    {
        private LogicLevel _level;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBattleLog"/> class.
        /// </summary>
        public LogicBattleLog(LogicLevel level)
        {
            this._level = level;
        }

        /// <summary>
        ///     Gets a value indicating whether this battle is started.
        /// </summary>
        public bool GetBattleStarted()
        {
            return false;
        }
    }
}