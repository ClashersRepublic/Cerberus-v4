namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicUnitStorageComponent : LogicComponent
    {
        private int _storageType;
        private int _maxCapacity;

        private LogicArrayList<LogicUnitSlot> _slots;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitStorageComponent" /> class.
        /// </summary>
        public LogicUnitStorageComponent(LogicGameObject gameObject, int capacity) : base(gameObject)
        {
            this._slots = new LogicArrayList<LogicUnitSlot>();
            this._maxCapacity = capacity;
            this.SetStorageType(gameObject);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._slots != null)
            {
                if (this._slots.Count != 0)
                {
                    do
                    {
                        LogicUnitSlot unitSlot = this._slots[0];

                        if (unitSlot != null)
                        {
                            this._slots[0].Destruct();
                        }

                        this._slots.Remove(0);
                    } while (this._slots.Count != 0);
                }

                this._slots = null;
            }
        }

        /// <summary>
        ///     Gets the storage type.
        /// </summary>
        public int GetStorageType()
        {
            return this._storageType;
        }

        /// <summary>
        ///     Sets the storage type.
        /// </summary>
        public void SetStorageType(LogicGameObject gameObject)
        {
            this._storageType = 0;

            if (gameObject.GetGameObjectType() == 0)
            {
                this._storageType = ((LogicBuilding) gameObject).GetBuildingData().ForgesSpells ? 1 : 0;
            }
        }

        /// <summary>
        ///     Gets the max capacity.
        /// </summary>
        public int GetMaxCapacity()
        {
            return this._maxCapacity;
        }

        /// <summary>
        ///     Sets the max capacity. 
        /// </summary>
        public void SetMaxCapacity(int capacity)
        {
            this._maxCapacity = capacity;
        }

        /// <summary>
        ///     Gets the unused capacity.
        /// </summary>
        public int GetUnusedCapacity()
        {
            return LogicMath.Max(this._maxCapacity - this.GetUsedCapacity(), 0);
        }

        /// <summary>
        ///     Gets the used capacity.
        /// </summary>
        public int GetUsedCapacity()
        {
            int usedCapacity = 0;

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitSlot unitSlot = this._slots[i];
                LogicCombatItemData combatItemData = (LogicCombatItemData) unitSlot.GetData();

                usedCapacity += combatItemData.GetHousingSpace() * unitSlot.GetCount();
            }

            return usedCapacity;
        }

        /// <summary>
        ///     Gets if the specified unit can be added.
        /// </summary>
        public bool CanAddUnit(LogicCombatItemData data)
        {
            LogicAvatar homeOwnerAvatar = this._parent.GetLevel().GetHomeOwnerAvatar();

            if (!homeOwnerAvatar.IsNpcAvatar())
            {
                if (this.GetComponentType() != 0)
                {
                    if (this._storageType == data.GetCombatItemType())
                    {
                        return this._maxCapacity >= data.GetHousingSpace() + this.GetUsedCapacity();
                    }
                }
                else
                {
                    LogicComponentManager componentManager = this._parent.GetLevel().GetComponentManager();

                    int totalUsedHousing = componentManager.GetTotalUsedHousing(this._storageType);
                    int totalMaxHousing = componentManager.GetTotalMaxHousing(this._storageType);

                    if (data.GetCombatItemType() == this._storageType)
                    {
                        if (this.GetUsedCapacity() < this._maxCapacity)
                        {
                            return totalMaxHousing >= totalUsedHousing;
                        }
                    }
                }

                return false;
            }

            return true;
        }

        /// <summary>
        ///     Adds the specified unit.
        /// </summary>
        public void AddUnit(LogicCombatItemData data)
        {
            this.AddUnitImpl(data, -1);
        }

        /// <summary>
        ///     Implementation to add unit.
        /// </summary>
        private void AddUnitImpl(LogicCombatItemData data, int upgLevel)
        {
            if (data != null)
            {
                if (this.CanAddUnit(data))
                {
                    int index = -1;

                    for (int i = 0; i < this._slots.Count; i++)
                    {
                        LogicUnitSlot slot = this._slots[i];

                        if (slot.GetData() == data && slot.GetLevel() == upgLevel)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        this._slots[index].SetCount(this._slots[index].GetCount() + 1);
                    }
                    else
                    {
                        this._slots.Add(new LogicUnitSlot(data, upgLevel, 1));
                    }
                }
                else
                {
                    Debugger.Warning("LogicUnitStorageComponent::addUnitImpl called and storage is full");
                }
            }
            else
            {
                Debugger.Warning("LogicUnitStorageComponent::addUnitImpl called and storage is full");
            }
        }

        /// <summary>
        ///     Removes all units.
        /// </summary>
        public void RemoveAllUnits()
        {
            if (this._slots.Count != 0)
            {
                do
                {
                    this._slots[0].Destruct();
                    this._slots.Remove(0);
                } while (this._slots.Count != 0);
            }
        }

        /// <summary>
        ///     Removes the number of specified units.
        /// </summary>
        public void RemoveUnits(LogicCombatItemData data, int count)
        {
            this.RemoveUnitsImpl(data, count, -1);
        }

        /// <summary>
        ///     Removes the number of specified units.
        /// </summary>
        public void RemoveUnits(LogicCombatItemData data, int count, int upgLevel)
        {
            this.RemoveUnitsImpl(data, count, upgLevel);
        }

        /// <summary>
        ///     Implementation to remove units.
        /// </summary>
        private void RemoveUnitsImpl(LogicCombatItemData data, int count, int upgLevel)
        {
            if (data != null)
            {
                int index = -1;

                for (int i = 0; i < this._slots.Count; i++)
                {
                    LogicUnitSlot slot = this._slots[i];

                    if (slot.GetData() == data && slot.GetLevel() == upgLevel)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    LogicUnitSlot slot = this._slots[index];

                    if (slot.GetCount() <= 0)
                    {
                        this._slots[index].Destruct();
                        this._slots.Remove(index);
                    }
                    else
                    {
                        slot.SetCount(slot.GetCount() - count);
                    }
                }
                else
                {
                    Debugger.Warning("LogicUnitStorageComponent::removeUnitsImpl No units with the given type found");
                }
            }
            else
            {
                Debugger.Warning("LogicUnitStorageComponent::removeUnits called with CharacterData NULL");
            }
        }

        /// <summary>
        ///     Gets the unit type count.
        /// </summary>
        public int GetUnitTypeCount()
        {
            return this._slots.Count;
        }

        /// <summary>
        ///     Gets the unit type.
        /// </summary>
        public LogicUnitSlot GetUnitType(int idx)
        {
            return this._slots[idx];
        }

        /// <summary>
        ///     Gets the unit count by data.
        /// </summary>
        public int GetUnitCountByData(LogicCombatItemData data)
        {
            int count = 0;

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitSlot unitSlot = this._slots[i];

                if (unitSlot.GetData() == data)
                {
                    count += unitSlot.GetCount();
                }
            }

            return count;
        }

        /// <summary>
        ///     Gets the unit count.
        /// </summary>
        public int GetUnitCount(int idx)
        {
            return this._slots[idx].GetCount();
        }

        /// <summary>
        ///     Gets the unit level.
        /// </summary>
        public int GetUnitLevel(int idx)
        {
            return this._slots[idx].GetLevel();
        }

        /// <summary>
        ///     Gets the unit type index.
        /// </summary>
        public int GetUnitTypeIndex(LogicCombatItemData data)
        {
            int index = -1;

            for (int i = 0; i < this._slots.Count; i++)
            {
                if (this._slots[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            LogicJSONArray unitArray = new LogicJSONArray();

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitSlot slot = this._slots[i];

                if (slot.GetData() != null && slot.GetCount() > 0)
                {
                    if (slot.GetLevel() != -1)
                    {
                        Debugger.Error("Invalid unit level.");
                    }

                    LogicJSONArray unitObject = new LogicJSONArray(2);
                    unitObject.Add(new LogicJSONNumber(slot.GetData().GetGlobalID()));
                    unitObject.Add(new LogicJSONNumber(slot.GetCount()));
                    unitArray.Add(unitObject);
                }
            }

            jsonObject.Put("units", unitArray);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONArray unitArray = jsonObject.GetJSONArray("units");

            if (unitArray != null)
            {
                if (this._slots.Count > 0)
                {
                    Debugger.Error("LogicUnitStorageComponent::load - Unit array size > 0!");
                }

                for (int i = 0, size = unitArray.Size(); i < size; i++)
                {
                    LogicJSONArray unitObject = unitArray.GetJSONArray(i);

                    if (unitObject != null)
                    {
                        LogicJSONNumber dataObject = unitObject.GetJSONNumber(0);
                        LogicJSONNumber countObject = unitObject.GetJSONNumber(1);

                        if (dataObject != null)
                        {
                            if (countObject != null)
                            {
                                LogicData data = LogicDataTables.GetDataById(dataObject.GetIntValue(), this._storageType != 0 ? 25 : 3);

                                if (data != null)
                                {
                                    this._slots.Add(new LogicUnitSlot(data, -1, countObject.GetIntValue()));
                                }
                                else
                                {
                                    Debugger.Error("LogicUnitStorageComponent::load - Character data is NULL!");
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets if this storage is empty.
        /// </summary>
        public bool IsEmpty()
        {
            for (int i = 0; i < this._slots.Count; i++)
            {
                if (this._slots[i].GetCount() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 0;
        }
    }
}