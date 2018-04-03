namespace LineageSoft.Magic.Logic.GameObject.Component
{
    public sealed class LogicUnitUpgradeComponent : LogicComponent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitUpgradeComponent" /> class.
        /// </summary>
        public LogicUnitUpgradeComponent(LogicGameObject gameObject) : base(gameObject)
        {
            // LogicUnitUpgradeComponent.
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 9;
        }
    }
}