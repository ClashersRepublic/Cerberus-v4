namespace ClashersRepublic.Magic.Logic.Time
{
    public class LogicTime
    {
        private int _tick;
        private int _fullTick;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTime"/> class.
        /// </summary>
        public LogicTime()
        {
            
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
        ///     Converts integer to time.
        /// </summary>
        public static implicit operator LogicTime(int tick)
        {
            return new LogicTime();
        }

        /// <summary>
        ///     Converts time to integer.
        /// </summary>
        public static implicit operator int(LogicTime time)
        {
            return time._tick;
        }
    }
}