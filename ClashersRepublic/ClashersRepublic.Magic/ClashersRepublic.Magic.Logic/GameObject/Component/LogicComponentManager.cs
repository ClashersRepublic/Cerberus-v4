namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicComponentManager
    {
        private readonly LogicLevel _level;
        private readonly LogicArrayList<LogicComponent>[] _components;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicComponentManager" /> class.
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
        ///     Calculates the loot.
        /// </summary>
        public void CalculateLoot(bool unk)
        {
            // TODO: Implement LogicComponentManager::calculateLoot();
        }

        /// <summary>
        ///     Devides the avatar resources to storages.
        /// </summary>
        public void DevideAvatarResourcesToStorages()
        {
            // TODO: Implement LogicComponentManager::divideAvatarResourcesToStorages();
        }

        /// <summary>
        ///     Gets all components to specified index.
        /// </summary>
        public LogicArrayList<LogicComponent> GetComponents(int componentType)
        {
            return this._components[componentType];
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
        ///     Gets the closest component.
        /// </summary>
        public LogicComponent GetClosestComponent(int x, int y, LogicComponentFilter filter)
        {
            LogicArrayList<LogicComponent> components = this._components[filter.GetComponentType()];
            LogicComponent minDistanceComponent = null;

            if (components.Count > 0)
            {
                int idx = 0;
                int minDistance = 0;

                do
                {
                    LogicComponent component = components[idx];

                    if (filter.TestComponent(component))
                    {
                        int distance = component.GetParent().GetPosition().GetDistanceSquaredTo(x, y);

                        if (distance < minDistance || minDistanceComponent == null)
                        {
                            minDistance = distance;
                            minDistanceComponent = component;
                        }
                    }
                } while (++idx != components.Count);
            }

            return minDistanceComponent;
        }

        /// <summary>
        ///     Gets the total used housing.
        /// </summary>
        public int GetTotalUsedHousing(int unitType)
        {
            LogicArrayList<LogicComponent> components = this._components[0];

            if (components.Count > 0)
            {
                int housing = 0;
                int idx = 0;

                do
                {
                    LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent) components[idx];

                    if (unitStorageComponent.GetStorageType() == unitType)
                    {
                        housing += unitStorageComponent.GetUsedCapacity();
                    } 
                } while (++idx != components.Count);

                return housing;
            }

            return 0;
        }

        /// <summary>
        ///     Gets the total used housing.
        /// </summary>
        public int GetTotalMaxHousing(int unitType)
        {
            LogicArrayList<LogicComponent> components = this._components[0];

            if (components.Count > 0)
            {
                int housing = 0;
                int idx = 0;

                do
                {
                    LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent)components[idx];

                    if (unitStorageComponent.GetStorageType() == unitType)
                    {
                        housing += unitStorageComponent.GetMaxCapacity();
                    }
                } while (++idx != components.Count);

                return housing;
            }

            return 0;
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