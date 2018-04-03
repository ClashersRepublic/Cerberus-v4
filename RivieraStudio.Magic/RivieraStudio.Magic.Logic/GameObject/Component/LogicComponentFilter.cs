namespace RivieraStudio.Magic.Logic.GameObject.Component
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

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public int GetComponentType()
        {
            return this._componentType;
        }

        /// <summary>
        ///     Gets a value indicating whether this filter is component filter.
        /// </summary>
        public override bool IsComponentFilter()
        {
            return true;
        }

        /// <summary>
        ///     Test the specified gameobject.
        /// </summary>
        public override bool TestGameObject(LogicGameObject gameObject)
        {
            LogicComponent component = gameObject.GetComponent(this._componentType);

            if (component != null && component.IsEnabled())
            {
                return base.TestGameObject(gameObject);
            }

            return false;
        }

        /// <summary>
        ///     Test the specified component.
        /// </summary>
        public bool TestComponent(LogicComponent component)
        {
            return this.TestGameObject(component.GetParent());
        }
    }
}