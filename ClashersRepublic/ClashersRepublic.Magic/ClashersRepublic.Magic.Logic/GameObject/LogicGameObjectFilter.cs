namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicGameObjectFilter
    {
        private LogicArrayList<LogicGameObject> _ignoreGameObjects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObjectFilter"/> class.
        /// </summary>
        public LogicGameObjectFilter()
        {
            this._ignoreGameObjects = new LogicArrayList<LogicGameObject>();
        }

        /// <summary>
        ///     Test the specified gameobject.
        /// </summary>
        public virtual void TestGameObject(LogicGameObject gameObject)
        {
            if(this._ignoreGameObjects == null || )
        }

        /// <summary>
        ///     Gets if this filter is a component filter.
        /// </summary>
        public virtual bool IsComponentFilter()
        {
            return false;
        }
    }
}