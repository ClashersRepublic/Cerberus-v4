namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    public sealed class LogicWarResourceStorageComponent : LogicResourceStorageComponent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHitpointComponent" /> class.
        /// </summary>
        public LogicWarResourceStorageComponent(LogicGameObject gameObject) : base(gameObject)
        {
            // LogicWarResourceStorageComponent.
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 11;
        }
    }
}