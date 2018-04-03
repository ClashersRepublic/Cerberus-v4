namespace RivieraStudio.Magic.Logic.Time
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Math;

    public class LogicTimer
    {
        private int _remainingTime;
        private int _endTimestamp;
        private int _fastForward;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTimer"/> class.
        /// </summary>
        public LogicTimer()
        {
            this._endTimestamp = -1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._remainingTime = 0;
            this._endTimestamp = -1;
            this._fastForward = 0;
        }

        /// <summary>
        ///     Gets the number of remaining seconds.
        /// </summary>
        public int GetRemainingSeconds(LogicTime time)
        {
            int remaining = this._remainingTime - time - this._fastForward;

            if (LogicDataTables.GetGlobals().MoreAccurateTime())
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
                    return LogicMath.Max((remaining + 59) / 60, 1);
                }
            }

            return 0;
        }

        /// <summary>
        ///     Gets the number of remaining milliseconds.
        /// </summary>
        public int GetRemainingMS(LogicTime time)
        {
            int remaining = this._remainingTime - time - this._fastForward;

            if (LogicDataTables.GetGlobals().MoreAccurateTime())
            {
                return 16 * remaining;
            }

            int ms = 1000 * (remaining / 60);

            if (ms % 60 != 0)
            {
                ms += (2133 * ms) >> 7;
            }

            return ms;
        }

        /// <summary>
        ///     Starts the timer.
        /// </summary>
        public void StartTimer(int totalSecs, LogicTime time, bool setEndTimestamp, int currentTimestamp)
        {
            int totalTicks = 0;

            if (LogicDataTables.GetGlobals().MoreAccurateTime())
            {
                totalTicks = (int) (1000L * totalSecs / 16);
            }
            else
            {
                totalTicks = 60 * totalSecs;
            }

            this._remainingTime = totalTicks + time;

            if (currentTimestamp != -1 && setEndTimestamp)
            {
                this._endTimestamp = currentTimestamp + totalSecs;
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForward(int totalSecs)
        {
            int totalTicks = 0;

            if (LogicDataTables.GetGlobals().MoreAccurateTime())
            {
                totalTicks = (int) (1000L * totalSecs / 16);
            }
            else
            {
                totalTicks = 60 * totalSecs;
            }

            this._remainingTime -= totalTicks;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardSubticks(int tick)
        {
            if (tick > 0)
            {
                this._remainingTime -= tick;
            }
        }

        /// <summary>
        ///     Adjusts the end subtick of timer.
        /// </summary>
        public void AdjustEndSubtick(LogicLevel level)
        {
            if(this._endTimestamp != -1)
            {
                int currentTime = LogicDataTables.GetGlobals().AdjustEndSubtickUseCurrentTime() ? level.GetGameMode().GetActiveTimestamp() : level.GetGameMode().GetCurrentTimestamp();

                if (currentTime != -1)
                {
                    int passedTime = this._endTimestamp - currentTime;
                    int clamp = LogicDataTables.GetGlobals().GetClampLongTimeStampsToDays();

                    if (clamp * 86400 > 0)
                    {
                        if (passedTime > 86400 * clamp)
                        {
                            passedTime = 86400 * clamp;
                        }
                        else if (passedTime < -86400 * clamp)
                        {
                            passedTime = -86400 * clamp;
                        }
                    }

                    this._remainingTime = level.GetLogicTime() + (LogicDataTables.GetGlobals().MoreAccurateTime() ? (int) (1000L * passedTime / 16) : 60 * passedTime);
                }
            }
        }

        /// <summary>
        ///     Gets the end timestamp time.
        /// </summary>
        public int GetEndTimestamp()
        {
            return this._endTimestamp;
        }

        /// <summary>
        ///     Sets the end timestamp.
        /// </summary>
        public void SetEndTimestamp(int endTimestamp)
        {
            this._endTimestamp = endTimestamp;
        }

        /// <summary>
        ///     Gets the fast forward value.
        /// </summary>
        public int GetFastForward()
        {
            return this._fastForward;
        }

        /// <summary>
        ///     Sets the fast forward value.
        /// </summary>
        public void SetFastForward(int value)
        {
            this._fastForward = value;
        }
    }
}