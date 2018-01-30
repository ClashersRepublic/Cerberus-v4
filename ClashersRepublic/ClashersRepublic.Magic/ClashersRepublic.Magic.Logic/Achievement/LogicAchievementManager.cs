namespace ClashersRepublic.Magic.Logic.Achievement
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicAchievementManager
    {
        private LogicLevel _level;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAchievementManager"/> class.
        /// </summary>
        public LogicAchievementManager(LogicLevel level)
        {
            this._level = level;
        }

        /// <summary>
        ///     Refreshes status of all achievements.
        /// </summary>
        public void RefreshStatus()
        {
            
        }

        /// <summary>
        ///     Refreshes status of all achievements.
        /// </summary>
        public void RefreshAchievementProgress(LogicClientAvatar avatar, LogicAchievementData data, int value)
        {
            if (this._level.GetState() != 5)
            {
                int currentValue = avatar.GetAchievementProgress(data);
                int newValue = LogicMath.Min(value, 2000000000);

                if (currentValue < newValue)
                {
                    avatar.SetAchievementProgress(data, value);
                }
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            
        }
    }
}