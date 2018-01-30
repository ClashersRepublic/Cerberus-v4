namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicComponentManager
    {
        private LogicLevel _level;
        private LogicArrayList<LogicComponent>[] _components;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicComponentManager"/> class.
        /// </summary>
        public LogicComponentManager(LogicLevel level)
        {
            this._level = level;
            this._components = new LogicArrayList<LogicComponent>[22];

            for (int i = 0; i < 22; i++)
            {
                this._components[i] = new LogicArrayList<LogicComponent>();
            }
        }

        /// <summary>
        ///     Adds the specified component to this instance.
        /// </summary>
        public void AddComponent(LogicComponent component)
        {
            this._components[component.GetComponentType()].Add(component);
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public void Tick()
        {

        }

        /// <summary>
        ///     Ticks for update this instance. Called before Tick method.
        /// </summary>
        public void SubTick()
        {

        }
    }
}