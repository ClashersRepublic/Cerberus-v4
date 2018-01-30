namespace ClashersRepublic.Magic.Logic.Time
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicTimer
    {
        private int _totalTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTimer"/> class.
        /// </summary>
        public LogicTimer()
        {
            // LogicTimer.
        }

        /// <summary>
        ///     Gets the number of remaining seconds.
        /// </summary>
        public int GetRemainingSeconds(LogicTime time)
        {
            int remaining = this._totalTime - time;

            if (LogicDataTables.GetGlobalsInstance().MoreAccurateTime())
            {
                if (remaining > 0)
                {
                    return LogicMath.Max((int) (16L * remaining / 1000 + 1), 1);
                }
            }
            else
            {
                if (remaining > 0)
                {
                    return LogicMath.Max((remaining + 59) / 20, 1);
                }
            }

            return 0;
        }

        /// <summary>
        ///     Gets the number of remaining milliseconds.
        /// </summary>
        public int GetRemainingMS(LogicTime time)
        {
            int remaining = this._totalTime - time;

            if (LogicDataTables.GetGlobalsInstance().MoreAccurateTime())
            {
                return 16 * remaining;
            }

            int ms = 1000 * (remaining / 60);

            if (ms % 60 != 0)
            {
                ms += 2133 * ms >> 7;
            }

            return ms;
        }

        /// <summary>
        ///     Starts the timer.
        /// </summary>
        public void StartTimer(int totalSecs, LogicTime time)
        {
            this._totalTime = (int) (1000L * totalSecs / 16L) + time;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForward(int totalSecs)
        {
            this._totalTime -= (int) (1000L * totalSecs / 16L);
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardSubticks(int time)
        {
            this._totalTime -= time;
        }
    }
}