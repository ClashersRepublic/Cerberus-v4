namespace ClashersRepublic.Magic.Logic.Time
{
    using ClashersRepublic.Magic.Logic.Data;

    public class LogicTime
    {
        private int _tick;
        private int _fullTick;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTime"/> class.
        /// </summary>
        public LogicTime()
        {
            // LogicTime.
        }

        /// <summary>
        ///     Increases the tick.
        /// </summary>
        public void IncreaseTick()
        {
            ++this._tick;

            if ((this._tick & 3) == 0)
            {
                ++this._fullTick;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the tick is full.
        /// </summary>
        public bool IsFullTick()
        {
            return ((this._tick + 1) & 3) == 0;
        }

        /// <summary>
        ///     Gets the milliseconds in ticks.
        /// </summary>
        public static int GetMSInTicks(int time)
        {
            if (LogicDataTables.GetGlobalsInstance().MoreAccurateTime())
            {
                return 16 * time;
            }

            return (int) (1000L * time / 60);
        }

        /// <summary>
        ///     Gets the seconds in ticks.
        /// </summary>
        public static int GetSecondsInTicks(int time)
        {
            if (LogicDataTables.GetGlobalsInstance().MoreAccurateTime())
            {
                return (int) (1000L * time / 16);
            }

            return time / 60;
        }

        /// <summary>
        ///     Gets the cooldown seconds in ticks.
        /// </summary>
        public static int GetCooldownSecondsInTicks(int time)
        {
            if (LogicDataTables.GetGlobalsInstance().MoreAccurateTime())
            {
                return (int)(1000L * time / 64);
            }

            return time / 15;
        }

        /// <summary>
        ///     Gets the cooldown ticks in seconds.
        /// </summary>
        public static int GetCooldownTicksInSeconds(int time)
        {
            if (LogicDataTables.GetGlobalsInstance().MoreAccurateTime())
            {
                return (int) ((long) time << 6 / 1000);
            }

            return time / 15;
        }

        /// <summary>
        ///     Converts time to integer.
        /// </summary>
        public static implicit operator int(LogicTime time)
        {
            return time._tick;
        }

        /// <summary>
        ///     Converts ticks to time.
        /// </summary>
        public static implicit operator LogicTime(int tick)
        {
            return new LogicTime
            {
                _tick = tick
            };
        }
    }
}