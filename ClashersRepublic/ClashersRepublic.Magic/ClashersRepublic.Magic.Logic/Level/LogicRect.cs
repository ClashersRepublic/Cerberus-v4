namespace ClashersRepublic.Magic.Logic.Level
{
    public sealed class LogicRect
    {
        private readonly int _startX;
        private readonly int _startY;
        private readonly int _endX;
        private readonly int _endY;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicRect" /> class.
        /// </summary>
        public LogicRect(int startX, int startY, int endX, int endY)
        {
            this._startX = startX;
            this._startY = startY;
            this._endX = endX;
            this._endY = endY;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Gets a value indicating whether the specified position is in inside of rectangle.
        /// </summary>
        public bool InInside(int x, int y)
        {
            if (this._startX <= x)
            {
                if (this._startY <= y)
                {
                    return this._endX > x && this._endY > y;
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified rectangle is in inside of rectangle.
        /// </summary>
        public bool InInside(LogicRect rect)
        {
            if (this._startX <= rect._startX)
            {
                if (this._startY <= rect._startY)
                {
                    return this._endX > rect._endX && this._endY > rect._endY;
                }
            }

            return false;
        }
    }
}