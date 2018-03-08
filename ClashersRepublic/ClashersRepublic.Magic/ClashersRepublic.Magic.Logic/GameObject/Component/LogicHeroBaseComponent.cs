namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    public sealed class LogicHeroBaseComponent : LogicComponent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHitpointComponent" /> class.
        /// </summary>
        public LogicHeroBaseComponent(LogicGameObject gameObject) : base(gameObject)
        {
            // LogicHeroBaseComponent.
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