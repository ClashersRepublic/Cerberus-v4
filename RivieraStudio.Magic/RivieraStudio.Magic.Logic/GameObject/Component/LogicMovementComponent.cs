namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using RivieraStudio.Magic.Logic.Util;

    public sealed class LogicMovementComponent : LogicComponent
    {
        private int _jump;

        private bool _isUnderground;
        private bool _healerTrigger;
        private LogicMovementSystem _movementSystem;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHitpointComponent" /> class.
        /// </summary>
        public LogicMovementComponent(LogicGameObject gameObject, int speed, bool healerTrigger, bool isUnderground) : base(gameObject)
        {
            this._healerTrigger = healerTrigger;
            this._isUnderground = isUnderground;

            this._movementSystem = new LogicMovementSystem(speed, this, null);
        }

        /// <summary>
        ///     Gets if the character is a healer trigger.
        /// </summary>
        public bool IsHealerTrigger()
        {
            return this._healerTrigger;
        }

        /// <summary>
        ///     Gets if the character is underground.
        /// </summary>
        public bool IsUnderground()
        {
            return this._isUnderground;
        }

        /// <summary>
        ///     Gets the jump length.
        /// </summary>
        public int GetJump()
        {
            return this._jump;
        }

        /// <summary>
        ///     Gets the movement system instance.
        /// </summary>
        public LogicMovementSystem GetMovementSystem()
        {
            return this._movementSystem;
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 4;
        }
    }
}