namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    public class LogicComponentFilter : LogicGameObjectFilter
    {
        private int _componentType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicComponentFilter"/> class.
        /// </summary>
        public LogicComponentFilter()
        {
            // LogicComponentFilter.
        }

        /// <summary>
        ///     Sets the component type.
        /// </summary>
        public void SetComponentType(int type)
        {
            this._componentType = type;
        }
    }
}