namespace LineageSoft.Magic.Logic.Util
{
    using LineageSoft.Magic.Logic.GameObject.Component;
    using LineageSoft.Magic.Titan.Debug;

    public class LogicMovementSystem
    {
        private LogicMovementComponent _parent;
        private LogicPathFinder _pathFinder;

        private int _speed;
        private int _x;
        private int _y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMovementSystem"/> class.
        /// </summary>
        public LogicMovementSystem(int speed, LogicMovementComponent movementComponent, LogicPathFinder pathFinder)
        {
            this._parent = movementComponent;
            this._pathFinder = pathFinder;

            if (this._parent != null && this._pathFinder != null)
            {
                Debugger.Error("LogicMovementSystem: both m_pParent and m_pPathFinder cant be used");
            }

            this._speed = 16 * speed / 1000;
        }

        /// <summary>
        ///     Sets the movement speed.
        /// </summary>
        public void SetSpeed(int speed)
        {
            this._speed = 16 * speed / 1000;
        }

        /// <summary>
        ///     Resets this instance.
        /// </summary>
        public void Reset(int x, int y)
        {
            this._x = x;
            this._y = y;

            if (this._parent != null)
            {
                this.ValidatePos();
            }
        }

        /// <summary>
        ///     Validates the position.
        /// </summary>
        public void ValidatePos()
        {
            if (this._parent != null)
            {
                if (!this._parent.IsHealerTrigger() &&
                    !this._parent.IsUnderground() &&
                    this._parent.GetJump() <= 0)
                {
                    this._parent.GetParent().GetLevel().GetTileMap().IsPassablePathFinder(this._x >> 8, this._y >> 8);
                }
            }
        }
    }
}