namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicComponentManager
    {
        private readonly LogicLevel _level;
        private readonly LogicArrayList<LogicComponent>[] _components;
        private readonly LogicArrayList<LogicDataSlot> _units;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicComponentManager" /> class.
        /// </summary>
        public LogicComponentManager(LogicLevel level)
        {
            this._level = level;
            this._units = new LogicArrayList<LogicDataSlot>();
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
        ///     Validates all troop upgrade levels.
        /// </summary>
        public void ValidateTroopUpgradeLevels()
        {
            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

            if (homeOwnerAvatar != null)
            {
                if (homeOwnerAvatar.IsClientAvatar())
                {
                    int[] laboratoryLevels = new int[2];

                    for (int i = 0; i < 2; i++)
                    {
                        LogicBuilding laboratory = this._level.GetGameObjectManagerAt(i).GetLaboratory();

                        if (laboratory != null)
                        {
                            laboratoryLevels[i] = laboratory.GetUpgradeLevel();
                        }
                    }

                    LogicDataTable characterTable = LogicDataTables.GetTable(3);

                    if (characterTable.GetItemCount() > 0)
                    {
                        int idx = 0;

                        do
                        {
                            LogicCharacterData characterData = (LogicCharacterData) characterTable.GetItemAt(idx);

                            int upgradeLevel = homeOwnerAvatar.GetUnitUpgradeLevel(characterData);
                            int villageType = characterData.GetVillageType();
                            int newUpgradeLevel = upgradeLevel;

                            if (upgradeLevel >= characterData.GetUpgradeLevelCount())
                            {
                                newUpgradeLevel = characterData.GetUpgradeLevelCount() - 1;
                            }

                            int laboratoryLevel = laboratoryLevels[villageType];
                            int requireLaboratoryLevel;

                            do
                            {
                                requireLaboratoryLevel = characterData.GetRequiredLaboratoryLevel(newUpgradeLevel--);
                            } while (newUpgradeLevel >= 0 && requireLaboratoryLevel > laboratoryLevel);

                            newUpgradeLevel += 1;

                            if (upgradeLevel > newUpgradeLevel)
                            {
                                homeOwnerAvatar.SetUnitUpgradeLevel(characterData, newUpgradeLevel);
                                homeOwnerAvatar.GetChangeListener().CommodityCountChanged(1, characterData, newUpgradeLevel);
                            }

                        } while (++idx != characterTable.GetItemCount());
                    }

                    LogicDataTable spellTable = LogicDataTables.GetTable(25);

                    if (spellTable.GetItemCount() > 0)
                    {
                        int idx = 0;

                        do
                        {
                            LogicSpellData spellData = (LogicSpellData) spellTable.GetItemAt(idx);

                            int upgradeLevel = homeOwnerAvatar.GetUnitUpgradeLevel(spellData);
                            int villageType = spellData.GetVillageType();
                            int newUpgradeLevel = upgradeLevel;

                            if (upgradeLevel >= spellData.GetUpgradeLevelCount())
                            {
                                newUpgradeLevel = spellData.GetUpgradeLevelCount() - 1;
                            }

                            int laboratoryLevel = laboratoryLevels[villageType];
                            int requireLaboratoryLevel;

                            do
                            {
                                requireLaboratoryLevel = spellData.GetRequiredLaboratoryLevel(newUpgradeLevel--);
                            } while (newUpgradeLevel >= 0 && requireLaboratoryLevel > laboratoryLevel);

                            newUpgradeLevel += 1;

                            if (upgradeLevel > newUpgradeLevel)
                            {
                                homeOwnerAvatar.SetUnitUpgradeLevel(spellData, newUpgradeLevel);
                                homeOwnerAvatar.GetChangeListener().CommodityCountChanged(1, spellData, newUpgradeLevel);
                            }

                        } while (++idx != spellTable.GetItemCount());
                    }
                }
            }
        }

        /// <summary>
        ///     Calculates the loot.
        /// </summary>
        public void CalculateLoot(bool includeStorage)
        {
            if (this._components[6].Count > 0)
            {
                if (includeStorage)
                {
                    LogicArrayList<LogicComponent> components = this._components[6];

                    for (int i = 0; i < components.Count; i++)
                    {
                        ((LogicResourceStorageComponent) components[i]).RecalculateAvailableLoot();
                    }
                }
            }

            if (this._components[5].Count > 0)
            {
                LogicArrayList<LogicComponent> components = this._components[5];

                for (int i = 0; i < components.Count; i++)
                {
                    ((LogicResourceProductionComponent) components[i]).RecalculateAvailableLoot();
                }
            }

            if (this._components[11].Count > 0)
            {
                LogicArrayList<LogicComponent> components = this._components[11];
                Debugger.DoAssert(components.Count < 2, "Too many war storage components");

                for (int i = 0; i < components.Count; i++)
                {
                }
            }
        }

        /// <summary>
        ///     Divides the avatar resources to storages.
        /// </summary>
        public void DivideAvatarResourcesToStorages()
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
        ///     Gets the max barrack level.
        /// </summary>
        public int GetMaxBarrackLevel()
        {
            LogicArrayList<LogicComponent> components = this._components[3];

            if (components.Count > 0)
            {
                int maxUpgLevel = -1;
                int idx = 0;

                do
                {
                    LogicUnitProductionComponent component = (LogicUnitProductionComponent) components[idx];

                    if (component.GetProductionType() == 0)
                    {
                        if (component.GetParent().GetGameObjectType() == 0)
                        {
                            LogicBuilding parent = (LogicBuilding) component.GetParent();

                            if (parent.GetBuildingData().GetProducesUnitsOfType() == 1)
                            {
                                maxUpgLevel = LogicMath.Max(parent.GetUpgradeLevel(), maxUpgLevel);
                            }
                        }
                    }
                } while (++idx != components.Count);

                return maxUpgLevel;
            }

            return -1;
        }
        
        /// <summary>
        ///     Gets the max dark barrack level.
        /// </summary>
        public int GetMaxDarkBarrackLevel()
        {
            LogicArrayList<LogicComponent> components = this._components[3];

            if (components.Count > 0)
            {
                int maxUpgLevel = -1;
                int idx = 0;

                do
                {
                    LogicUnitProductionComponent component = (LogicUnitProductionComponent)components[idx];

                    if (component.GetProductionType() == 0)
                    {
                        if (component.GetParent().GetGameObjectType() == 0)
                        {
                            LogicBuilding parent = (LogicBuilding)component.GetParent();

                            if (parent.GetBuildingData().GetProducesUnitsOfType() == 2 && (!parent.IsConstructing() || parent.IsUpgrading()))
                            {
                                maxUpgLevel = LogicMath.Max(parent.GetUpgradeLevel(), maxUpgLevel);
                            }
                        }
                    }
                } while (++idx != components.Count);

                return maxUpgLevel;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the max spell forge level.
        /// </summary>
        public int GetMaxSpellForgeLevel()
        {
            LogicArrayList<LogicComponent> components = this._components[3];

            if (components.Count > 0)
            {
                int maxUpgLevel = -1;
                int idx = 0;

                do
                {
                    LogicUnitProductionComponent component = (LogicUnitProductionComponent)components[idx];

                    if (component.GetProductionType() != 0)
                    {
                        if (component.GetParent().GetGameObjectType() == 0)
                        {
                            LogicBuilding parent = (LogicBuilding)component.GetParent();

                            if (parent.GetBuildingData().GetProducesUnitsOfType() == 1 && (!parent.IsConstructing() || parent.IsUpgrading()))
                            {
                                maxUpgLevel = LogicMath.Max(parent.GetUpgradeLevel(), maxUpgLevel);
                            }
                        }
                    }
                } while (++idx != components.Count);

                return maxUpgLevel;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the max spell forge level.
        /// </summary>
        public int GetMaxMiniSpellForgeLevel()
        {
            LogicArrayList<LogicComponent> components = this._components[3];

            if (components.Count > 0)
            {
                int maxUpgLevel = -1;
                int idx = 0;

                do
                {
                    LogicUnitProductionComponent component = (LogicUnitProductionComponent)components[idx];

                    if (component.GetProductionType() != 0)
                    {
                        if (component.GetParent().GetGameObjectType() == 0)
                        {
                            LogicBuilding parent = (LogicBuilding)component.GetParent();

                            if (parent.GetBuildingData().GetProducesUnitsOfType() == 2 && (!parent.IsConstructing() || parent.IsUpgrading()))
                            {
                                maxUpgLevel = LogicMath.Max(parent.GetUpgradeLevel(), maxUpgLevel);
                            }
                        }
                    }
                } while (++idx != components.Count);

                return maxUpgLevel;
            }

            return -1;
        }

        /// <summary>
        ///     Divides the avatar units to storages.
        /// </summary>
        public void DivideAvatarUnitsToStorages(int villageType)
        {
            if (this._level.GetHomeOwnerAvatar() != null)
            {
                if (villageType == 1)
                {
                    // TODO: Implement divideAvatarUnitsToStorages(vType) for village type 1.
                }
                else
                {
                    if (this._units.Count != 0)
                    {
                        do
                        {
                            this._units[0].Destruct();
                            this._units.Remove(0);
                        } while (this._units.Count != 0);
                    }

                    LogicArrayList<LogicComponent> components = this._components[0];

                    for (int i = 0; i < components.Count; i++)
                    {
                        LogicUnitStorageComponent storageComponent = (LogicUnitStorageComponent) components[i];

                        for (int j = 0; j < storageComponent.GetUnitTypeCount(); j++)
                        {
                            LogicCombatItemData unitType = storageComponent.GetUnitType(j);
                            Int32 unitCount = storageComponent.GetUnitCount(j);
                            Int32 index = -1;

                            for (int k = 0; k < this._units.Count; k++)
                            {
                                LogicDataSlot tmp = this._units[k];

                                if (tmp.GetData() == unitType)
                                {
                                    index = k;
                                    break;
                                }
                            }

                            if (index != -1)
                            {
                                this._units[index].SetCount(this._units[index].GetCount() - unitCount);
                            }
                            else
                            {
                                this._units.Add(new LogicDataSlot(unitType, -unitCount));
                            }
                        }
                    }

                    LogicArrayList<LogicDataSlot> units = this._level.GetHomeOwnerAvatar().GetUnits();

                    for (int i = 0; i < units.Count; i++)
                    {
                        LogicDataSlot slot = units[i];
                        Int32 index = -1;

                        for (int j = 0; j < this._units.Count; j++)
                        {
                            LogicDataSlot tmp = this._units[j];

                            if (tmp.GetData() == slot.GetData())
                            {
                                index = j;
                                break;
                            }
                        }

                        if (index != -1)
                        {
                            this._units[index].SetCount(this._units[index].GetCount() + slot.GetCount());
                        }
                        else
                        {
                            this._units.Add(new LogicDataSlot(slot.GetData(), slot.GetCount()));
                        }
                    }

                    LogicArrayList<LogicDataSlot> spells = this._level.GetHomeOwnerAvatar().GetSpells();

                    for (int i = 0; i < spells.Count; i++)
                    {
                        LogicDataSlot slot = spells[i];
                        Int32 index = -1;

                        for (int j = 0; j < this._units.Count; j++)
                        {
                            LogicDataSlot tmp = this._units[j];

                            if (tmp.GetData() == slot.GetData())
                            {
                                index = j;
                                break;
                            }
                        }

                        if (index != -1)
                        {
                            this._units[index].SetCount(this._units[index].GetCount() + slot.GetCount());
                        }
                        else
                        {
                            this._units.Add(new LogicDataSlot(slot.GetData(), slot.GetCount()));
                        }
                    }

                    for (int i = 0; i < this._units.Count; i++)
                    {
                        LogicDataSlot slot = this._units[i];
                        LogicCombatItemData data = (LogicCombatItemData) slot.GetData();
                        Int32 unitCount = slot.GetCount();

                        if (unitCount != 0)
                        {
                            for (int j = 0; j < components.Count; j++)
                            {
                                LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent) components[j];

                                if (unitCount >= 0)
                                {
                                    while (unitStorageComponent.CanAddUnit(data))
                                    {
                                        unitStorageComponent.AddUnit(data);

                                        if (--unitCount <= 0)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    int idx = unitStorageComponent.GetUnitTypeIndex(data);

                                    if (idx != -1)
                                    {
                                        int count = unitStorageComponent.GetUnitCount(idx);

                                        if (count < -unitCount)
                                        {
                                            unitStorageComponent.RemoveUnits(data, count);
                                            unitCount += count;
                                        }
                                        else
                                        {
                                            unitStorageComponent.RemoveUnits(data, -unitCount);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
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