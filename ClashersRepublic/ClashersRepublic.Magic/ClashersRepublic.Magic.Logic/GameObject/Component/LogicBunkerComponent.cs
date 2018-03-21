namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    public sealed class LogicBunkerComponent : LogicUnitStorageComponent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBunkerComponent" /> class.
        /// </summary>
        public LogicBunkerComponent(LogicGameObject gameObject, int capacity) : base(gameObject, capacity)
        {
            // LogicBunkerComponent.
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 7;
        }
    }
}