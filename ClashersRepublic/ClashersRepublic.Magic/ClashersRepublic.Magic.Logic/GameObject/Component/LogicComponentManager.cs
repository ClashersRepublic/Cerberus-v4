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
            this._components = new LogicArrayList<LogicComponent>[17];

            for (int i = 0; i < 17; i++)
            {
                this._components[i] = new LogicArrayList<LogicComponent>(32);
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
        ///     Removes the specified component.
        /// </summary>
        public void RemoveComponent(LogicComponent component)
        {
            LogicArrayList<LogicComponent> components = this._components[component.GetComponentType()];

            int index = -1;

            for (int i = 0; i < components.Count; i++)
            {
                LogicComponent tmp = components[i];

                if (tmp.Equals(component))
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                components.Remove(index);
            }
        }

        /// <summary>
        ///     Removes all references for the specified gameobject.
        /// </summary>
        public void RemoveGameObjectReferences(LogicGameObject gameObject)
        {
            for (int i = 0; i < 17; i++)
            {
                LogicArrayList<LogicComponent> components = this._components[0];

                for (int j = 0; j < components.Count; j++)
                {
                    components[j].RemoveGameObjectReferences(gameObject);
                }
            }
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public void Tick()
        {
            bool isInCombatState = this._level.IsInCombatState();

            LogicArrayList<LogicComponent> components = this._components[0];

            for (int i = 0; i < components.Count; i++)
            {
                LogicComponent component = components[i];

                if (component.IsEnabled())
                {
                    component.Tick();
                }
            }

            for (int i = isInCombatState ? 1 : 2; i < 17; i++)
            {
                components = this._components[i];

                for (int j = 0; j < components.Count; j++)
                {
                    LogicComponent component = components[j];

                    if (component.IsEnabled())
                    {
                        component.Tick();
                    }
                }
            }
        }

        /// <summary>
        ///     Ticks for update this instance. Called before Tick method.
        /// </summary>
        public void SubTick()
        {

        }
    }
}