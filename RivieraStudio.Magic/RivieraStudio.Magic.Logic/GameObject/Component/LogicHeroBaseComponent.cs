namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using RivieraStudio.Magic.Logic.Data;

    public sealed class LogicHeroBaseComponent : LogicComponent
    {
        private LogicHeroData _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHitpointComponent" /> class.
        /// </summary>
        public LogicHeroBaseComponent(LogicGameObject gameObject, LogicHeroData data) : base(gameObject)
        {
            this._data = data;
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 10;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicHeroBaseComponent"/> is in upgrading.
        /// </summary>
        public bool IsUpgrading()
        {
            return false;
        }

        /// <summary>
        ///     Gets the remaining upgrade seconds.
        /// </summary>
        public int GetRemainingUpgradeSeconds()
        {
            return 0;
        }
    }
}