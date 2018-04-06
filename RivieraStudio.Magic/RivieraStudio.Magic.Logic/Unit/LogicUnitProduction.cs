namespace RivieraStudio.Magic.Logic.Unit
{
    using System;

    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.GameObject.Component;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Time;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicUnitProduction
    {
        private LogicLevel _level;
        private LogicTimer _timer;
        private LogicTimer _boostTimer;
        private LogicArrayList<LogicUnitProductionSlot> _slots;

        private int _villageType;
        private int _unitProductionType;
        private int _nextProduction;

        private bool _locked;
        private bool _boostPause;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitProduction" /> class.
        /// </summary>
        public LogicUnitProduction(LogicLevel level, int unitProductionType, int villageType)
        {
            this._level = level;
            this._villageType = villageType;
            this._unitProductionType = unitProductionType;

            this._slots = new LogicArrayList<LogicUnitProductionSlot>();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._slots.Count != 0)
            {
                do
                {
                    this._slots[0].Destruct();
                    this._slots.Remove(0);
                } while (this._slots.Count != 0);
            }

            if (this._timer != null)
            {
                this._timer.Destruct();
                this._timer = null;
            }

            if (this._boostTimer != null)
            {
                this._boostTimer.Destruct();
                this._boostTimer = null;
            }

            this._level = null;
        }

        /// <summary>
        ///     Gets the unit production type.
        /// </summary>
        public int GetUnitProductionType()
        {
            return this._unitProductionType;
        }

        /// <summary>
        ///     Gets if the unit production is locked.
        /// </summary>
        public bool IsLocked()
        {
            return this._locked;
        }

        /// <summary>
        ///     SEts if the unit production is locked.
        /// </summary>
        public void SetLocked(bool state)
        {
            this._locked = state;
        }

        /// <summary>
        ///     Gets if the boost is paused.
        /// </summary>
        public bool IsBoostPaused()
        {
            return this._boostPause;
        }

        /// <summary>
        ///     Gets the remaining boost time in secs.
        /// </summary>
        public int GetRemainingBoostTimeSecs()
        {
            if (this._boostTimer != null)
            {
                return this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the remaining seconds.
        /// </summary>
        public int GetRemainingSeconds()
        {
            if (this._timer != null)
            {
                return this._timer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the remaining ms.
        /// </summary>
        public int GetRemainingMS()
        {
            if (this._timer != null)
            {
                return this._timer.GetRemainingMS(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the total production seconds.
        /// </summary>
        public int GetTotalSeconds()
        {
            LogicUnitProductionSlot slot = null;

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitProductionSlot tmp = this._slots[i];

                if (!tmp.IsTerminate())
                {
                    slot = tmp;
                    break;
                }
            }

            if (slot != null)
            {
                LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
                LogicCombatItemData data = (LogicCombatItemData) slot.GetData();

                return data.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(data), this._level, 0);
            }

            return 0;
        }

        /// <summary>
        ///     Gets the total remaining seconds.
        /// </summary>
        public int GetTotalRemainingSeconds()
        {
            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
            LogicComponentManager componentManager = this._level.GetComponentManagerAt(this._villageType);
            Int32 totalMaxHousing = componentManager.GetTotalMaxHousing(this._unitProductionType != 3 ? 1 : 0);
            Int32 totalUsedCapacity = this._unitProductionType == 3 ? homeOwnerAvatar.GetUnitsTotalCapacity() : homeOwnerAvatar.GetSpellsTotalCapacity();
            Int32 freeCapacity = totalMaxHousing - totalUsedCapacity;
            Int32 remainingSecs = 0;

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitProductionSlot slot = this._slots[i];
                LogicCombatItemData data = (LogicCombatItemData)slot.GetData();
                Int32 housingSpace = data.GetHousingSpace();
                Int32 count = slot.GetCount();

                if (count > 0)
                {
                    if (i == 0)
                    {
                        if (!slot.IsTerminate() && freeCapacity - housingSpace >= 0)
                        {
                            if (this._timer != null)
                            {
                                remainingSecs += this._timer.GetRemainingSeconds(this._level.GetLogicTime());
                            }
                        }

                        freeCapacity -= housingSpace;
                        count -= 1;
                    }

                    for (int j = 0; j < count; j++)
                    {
                        if (!slot.IsTerminate() && freeCapacity - housingSpace >= 0)
                        {
                            remainingSecs += data.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(data), this._level, 0);
                        }

                        freeCapacity -= housingSpace;
                    }
                }
            }

            return remainingSecs;
        }

        /// <summary>
        ///     Gets if the tutorial capacity is full.
        /// </summary>
        public bool IsTutorialCapacityFull()
        {
            return this._level.GetHomeOwnerAvatar().GetUnitsTotalCapacity() + this.GetTotalCount() >= 20;
        }

        /// <summary>
        ///     Gets the max train count.
        /// </summary>
        public int GetMaxTrainCount()
        {
            return this._level.GetComponentManagerAt(this._villageType).GetTotalMaxHousing(this._unitProductionType != 3 ? 1 : 0) * 2;
        }

        /// <summary>
        ///     Gets the tutorial max capacity.
        /// </summary>
        public int GetTutorialMax()
        {
            return 20;
        }

        /// <summary>
        ///     Get sthe tutorial unit count.
        /// </summary>
        public int GetTutorialCount()
        {
            return this._level.GetHomeOwnerAvatar().GetUnitsTotalCapacity() + this.GetTotalCount();
        }

        /// <summary>
        ///     Gets the slot count.
        /// </summary>
        public int GetSlotCount()
        {
            return this._slots.Count;
        }

        /// <summary>
        ///     Gets the training count.
        /// </summary>
        public int GetTrainingCount(int idx)
        {
            return this._slots[idx].GetCount();
        }

        /// <summary>
        ///     Gets the currently trained unit.
        /// </summary>
        public LogicCombatItemData GetCurrentlyTrainedUnit()
        {
            for (int i = 0; i < this._slots.Count; i++)
            {
                if (!this._slots[i].IsTerminate())
                {
                    return (LogicCombatItemData) this._slots[i].GetData();
                }
            }
            
            return null;
        }

        /// <summary>
        ///     Gets the currently trained slot idx.
        /// </summary>
        public int GetCurrentlyTrainedIndex()
        {
            for (int i = 0; i < this._slots.Count; i++)
            {
                if (!this._slots[i].IsTerminate())
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Gets the currently trained count.
        /// </summary>
        public LogicUnitProductionSlot GetCurrentlyTrainedSlot()
        {
            for (int i = 0; i < this._slots.Count; i++)
            {
                if (!this._slots[i].IsTerminate())
                {
                    return this._slots[i];
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets a slot waiting for space.
        /// </summary>
        public LogicUnitProductionSlot GetWaitingForSpaceSlot()
        {
            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitProductionSlot slot = this._slots[i];

                if (slot.IsTerminate() ||
                    this._timer != null && this._timer.GetRemainingSeconds(this._level.GetLogicTime()) == 0)
                {
                    return slot;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the boost multiplier.
        /// </summary>
        public int GetBoostMultiplier()
        {
            if (this._unitProductionType == 25)
            {
                if (LogicDataTables.GetGlobals().UseNewTraining())
                {
                    return LogicDataTables.GetGlobals().GetSpellFactoryBoostNewMultiplier();
                }

                return LogicDataTables.GetGlobals().GetSpellFactoryBoostMultiplier();
            }
            else
            {
                if (this._unitProductionType != 3)
                {
                    return 1;
                }

                if (LogicDataTables.GetGlobals().UseNewTraining())
                {
                    return LogicDataTables.GetGlobals().GetBarracksBoostNewMultiplier();
                }

                return LogicDataTables.GetGlobals().GetBarracksBoostMultiplier();
            }
        }

        /// <summary>
        ///     Called when the current production is completed.
        /// </summary>
        public bool ProductionCompleted(bool speedUp)
        {
            Boolean success = false;

            if (!this._locked)
            {
                LogicComponentFilter filter = new LogicComponentFilter();

                filter.SetComponentType(0);

                while (true)
                {
                    LogicComponentManager componentManager = this._level.GetComponentManagerAt(this._villageType);
                    LogicUnitProductionSlot waitingSlot = this.GetWaitingForSpaceSlot();

                    if (speedUp)
                    {
                        if (this._slots.Count <= 0)
                        {
                            return false;
                        }

                        waitingSlot = this._slots[0];
                    }
                    
                    if (waitingSlot == null)
                    {
                        return false;
                    }

                    LogicCombatItemData unitData = (LogicCombatItemData) waitingSlot.GetData();
                    LogicBuildingData buildingProductionData = unitData.GetProductionHouseData();
                    LogicGameObjectManager gameObjectManager = this._level.GetGameObjectManagerAt(this._villageType);
                    LogicBuilding productionHouse = gameObjectManager.GetHighestBuilding(buildingProductionData);

                    if (LogicDataTables.GetGlobals().UseTroopWalksOutFromTraining())
                    {
                        int gameObjectCount = gameObjectManager.GetNumGameObjects();

                        for (int i = 0; i < gameObjectCount; i++)
                        {
                            LogicGameObject gameObject = gameObjectManager.GetGameObjectByIndex(i);

                            if (gameObject != null)
                            {
                                if (gameObject.GetGameObjectType() == 0)
                                {
                                    LogicBuilding building = (LogicBuilding) gameObject;
                                    LogicUnitProductionComponent unitProductionComponent = building.GetUnitProductionComponent();

                                    if (unitProductionComponent != null)
                                    {
                                        if (unitProductionComponent.GetProductionType() == unitData.GetCombatItemType())
                                        {
                                            if (building.GetBuildingData().ProducesUnitsOfType == unitData.GetUnitOfType() &&
                                                !building.IsUpgrading() &&
                                                !building.IsConstructing())
                                            {
                                                if (unitData.IsUnlockedForProductionHouseLevel(building.GetUpgradeLevel()))
                                                {
                                                    if (productionHouse != null)
                                                    {
                                                        int seed = this._level.GetPlayerAvatar().GetExpPoints();

                                                        if (building.Rand(seed) % 1000 > 750)
                                                        {
                                                            productionHouse = building;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        productionHouse = building;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (productionHouse == null)
                    {
                        if (this._timer != null)
                        {
                            if (this._timer.GetRemainingSeconds(this._level.GetLogicTime()) == 0)
                            {
                                success = this.TrainingFinished();
                            }
                        }

                        filter.Destruct();

                        break;
                    }

                    LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent) componentManager.GetClosestComponent(productionHouse.GetX(), productionHouse.GetY(), filter);

                    if (unitStorageComponent != null)
                    {
                        if (unitStorageComponent.CanAddUnit(unitData))
                        {
                            Debugger.Print("LogicUnitProduction::productionCompleted");
                            
                            this._level.GetHomeOwnerAvatar().CommodityCountChangeHelper(0, unitData, 1);
                            unitStorageComponent.AddUnit(unitData);

                            if (this._level.GetState() == 1 || this._level.GetState() == 3)
                            {
                                if (unitStorageComponent.GetParentListener() != null)
                                {
                                    // BLABLABLA
                                }
                            }

                            if (waitingSlot.IsTerminate())
                            {
                                this.RemoveUnit(unitData, -1);
                            }
                            else
                            {
                                this.StartProducingNextUnit();
                            }

                            success = this._slots.Count <= 0 || !this._slots[0].IsTerminate() || this._slots[0].GetCount() <= 0 || this._slots[0].GetData() == null;

                            if (success)
                            {
                                filter.Destruct();
                                break;
                            }
                        }
                        else
                        {
                            filter.AddIgnoreObject(unitStorageComponent.GetParent());
                        }
                    }
                    else
                    {
                        if (this._timer != null)
                        {
                            if (this._timer.GetRemainingSeconds(this._level.GetLogicTime()) == 0)
                            {
                                success = this.TrainingFinished();
                            }
                        }

                        break;
                    }
                }

                this._nextProduction = success ? 0 : 2000;
            }

            return success;
        }

        /// <summary>
        ///     Removes the specified unit.
        /// </summary>
        public bool RemoveUnit(LogicCombatItemData data, int index)
        {
            LogicUnitProductionSlot slot = null;
            Boolean removed = false;

            if (index > -1 &&
                this._slots.Count > index &&
                this._slots[0].GetData() == data)
            {
                slot = this._slots[0];
            }
            else
            {
                index = this.GetCurrentlyTrainedIndex();

                if (index == -1)
                {
                    return false;
                }

                slot = this._slots[index];
            }

            int count = slot.GetCount();
            
            if (slot.GetCount() > 0)
            {
                removed = true;
                slot.SetCount(count - 1);

                if (count == 1)
                {
                    int prodIdx = this.GetCurrentlyTrainedIndex();
                    
                    if (prodIdx == index)
                    {
                        if (this._timer != null)
                        {
                            this._timer.Destruct();
                            this._timer = null;
                        }
                    }

                    this._slots[index].Destruct();
                    this._slots.Remove(index);

                    Debugger.Print("LogicUnitProduction::removeUnit unit removed");
                }
            }

            if (this._slots.Count > 0)
            {
                LogicUnitProductionSlot productionSlot = this.GetCurrentlyTrainedSlot();

                if (productionSlot == null || this._timer != null)
                {
                    if (!removed)
                    {
                        return false;
                    }

                    this.MergeSlots();
                }
                else
                {
                    Debugger.Print("LogicUnitProduction::removeUnit start next production");

                    LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
                    LogicCombatItemData productionData = (LogicCombatItemData) productionSlot.GetData();

                    this._timer = new LogicTimer();
                    this._timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), this._level, 0), this._level.GetLogicTime(), false,
                        -1);

                    if (removed)
                    {
                        this.MergeSlots();
                    }
                }
            }
            else
            {
                if (!removed)
                {
                    return false;
                }

                this.MergeSlots();
            }

            return true;
        }

        /// <summary>
        ///     Speed up the unit production.
        /// </summary>
        public void SpeedUp()
        {
            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
            LogicComponentManager componentManager = this._level.GetComponentManagerAt(this._villageType);
            Int32 totalMaxHousing = componentManager.GetTotalMaxHousing(this._unitProductionType != 3 ? 1 : 0);
            Int32 totalUsedCapacity = this._unitProductionType == 3 ? homeOwnerAvatar.GetUnitsTotalCapacity() : homeOwnerAvatar.GetSpellsTotalCapacity();
            Int32 freeCapacity = totalMaxHousing - totalUsedCapacity;

            while(this._slots.Count > 0)
            {
                LogicUnitProductionSlot slot = this._slots[0];
                LogicCombatItemData data = (LogicCombatItemData) slot.GetData();
                Int32 count = slot.GetCount();

                if (count > 0)
                {
                    do
                    {
                        freeCapacity -= data.GetHousingSpace();

                        if (freeCapacity < 0)
                        {
                            return;
                        }

                        this.ProductionCompleted(true);
                    } while (--count > 0);
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Gets the unit at specified index.
        /// </summary>
        public LogicUnitProductionSlot GetUnit(int index)
        {
            if (index > -1 && this._slots.Count > index)
            {
                return this._slots[index];
            }

            return null;
        }

        /// <summary>
        ///     Tries to merge slots.
        /// </summary>
        public void MergeSlots()
        {
            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

            if (this._slots.Count > 0)
            {
                if (this._slots.Count > 1)
                {
                    for (int i = 1; i < this._slots.Count; i++)
                    {
                        LogicUnitProductionSlot slot1 = this._slots[i];
                        LogicUnitProductionSlot slot2 = this._slots[i - 1];

                        if (slot1.GetData() == slot2.GetData())
                        {
                            if (slot1.IsTerminate() == slot2.IsTerminate())
                            {
                                this._slots.Remove(i--);
                                slot2.SetCount(slot2.GetCount() + slot1.GetCount());
                                slot1.Destruct();
                                slot1 = null;
                            }
                        }
                    }
                }
            }

            LogicComponentManager componentManager = this._level.GetComponentManagerAt(this._villageType);
            Int32 totalCapcity = componentManager.GetTotalMaxHousing(this._unitProductionType != 3 ? 1 : 0);
            Int32 usedCapacity = this._unitProductionType == 25 ? homeOwnerAvatar.GetSpellsTotalCapacity() : homeOwnerAvatar.GetUnitsTotalCapacity();
            Int32 freeCapacity = totalCapcity - usedCapacity;

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitProductionSlot slot = this._slots[i];
                LogicCombatItemData data = (LogicCombatItemData) slot.GetData();

                int housingSpace = data.GetHousingSpace() * slot.GetCount();

                if (freeCapacity < housingSpace)
                {
                    if (slot.GetCount() > 0 && housingSpace / data.GetHousingSpace() > 0)
                    {
                        if (slot.GetCount() > housingSpace / data.GetHousingSpace())
                        {
                            this._slots.Add(i + 1, new LogicUnitProductionSlot(data, slot.GetCount() - housingSpace / data.GetHousingSpace(), slot.IsTerminate()));
                        }
                    }

                    break;
                }

                freeCapacity -= housingSpace;
            }
        }

        /// <summary>
        ///     Starts the production of next unit.
        /// </summary>
        public void StartProducingNextUnit()
        {
            if (this._timer != null)
            {
                this._timer.Destruct();
                this._timer = null;
            }

            if (this._slots.Count > 0)
            {
                LogicUnitProductionSlot nextProductionSlot = null;

                for (int i = 0; i < this._slots.Count; i++)
                {
                    LogicUnitProductionSlot tmp = this._slots[i];

                    if (!tmp.IsTerminate())
                    {
                        nextProductionSlot = tmp;
                        break;
                    }
                }

                if (nextProductionSlot != null)
                {
                    this.RemoveUnit((LogicCombatItemData) nextProductionSlot.GetData(), -1);
                }
            }
        }

        /// <summary>
        ///     Called when the training of unit is ended.
        /// </summary>
        public bool TrainingFinished()
        {
            if (this._timer != null)
            {
                this._timer.Destruct();
                this._timer = null;
            }

            if (this._slots.Count > 0)
            {
                LogicUnitProductionSlot prodSlot = this.GetCurrentlyTrainedSlot();
                Int32 prodIdx = this.GetCurrentlyTrainedIndex();

                if (prodSlot != null)
                {
                    if (prodSlot.GetCount() == 1)
                    {
                        prodSlot.SetTerminate(true);
                    }
                    else
                    {
                        prodSlot.SetCount(prodSlot.GetCount() - 1);

                        LogicUnitProductionSlot previousSlot = this._slots[LogicMath.Max(prodIdx - 1, 0)];

                        if (previousSlot != null &&
                            previousSlot.IsTerminate() &&
                            previousSlot.GetData().GetGlobalID() == prodSlot.GetData().GetGlobalID())
                        {
                            previousSlot.SetCount(previousSlot.GetCount() + 1);
                        }
                        else
                        {
                            this._slots.Add(prodIdx, new LogicUnitProductionSlot(prodSlot.GetData(), 1, true));
                        }
                    }
                }

                LogicCombatItemData nextProductionData = this.GetCurrentlyTrainedUnit();

                if (nextProductionData != null)
                {
                    this._timer = new LogicTimer();
                    this._timer.StartTimer(nextProductionData.GetTrainingTime(this._level.GetHomeOwnerAvatar().GetUnitUpgradeLevel(nextProductionData), this._level, 0),
                        this._level.GetLogicTime(), false, -1);

                    this.MergeSlots();

                    return true;
                }
            }

            this.MergeSlots();

            return false;
        }

        /// <summary>
        ///     Drags the slot to the specified index.
        /// </summary>
        public bool DragSlot(int slotIdx, int dragIdx)
        {
            this._locked = false;

            if (slotIdx > -1 && slotIdx < this._slots.Count)
            {
                LogicCombatItemData productionData = this.GetCurrentlyTrainedUnit();
                LogicUnitProductionSlot slot = this._slots[slotIdx];

                this._slots.Remove(slotIdx);

                if (slot != null)
                {
                    if (slotIdx <= dragIdx)
                    {
                        dragIdx -= 1;
                    }

                    if (dragIdx >= 0 && dragIdx <= this._slots.Count)
                    {
                        this._slots.Add(dragIdx, slot);
                        this.MergeSlots();

                        LogicCombatItemData prodData = this.GetCurrentlyTrainedUnit();
                        Int32 prodIdx = this.GetCurrentlyTrainedIndex();

                        if (productionData != prodData && (dragIdx >= prodIdx || prodIdx == slotIdx || prodIdx == dragIdx + 1))
                        {
                            if (this._timer != null)
                            {
                                this._timer.Destruct();
                                this._timer = null;
                            }

                            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                            this._timer = new LogicTimer();
                            this._timer.StartTimer(prodData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(prodData), this._level, 0),
                                                   this._level.GetLogicTime(), false, -1);
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Adds the specified unit to queue.
        /// </summary>
        public int AddUnitToQueue(LogicCombatItemData data)
        {
            if (data != null)
            {
                if (this.CanAddUnitToQueue(data, false))
                {
                    LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                    for (int i = this._slots.Count - 1; i >= 0; i--)
                    {
                        LogicUnitProductionSlot tmp = this._slots[i];

                        if (tmp != null)
                        {
                            if (tmp.GetData() == data)
                            {
                                tmp.SetCount(tmp.GetCount() + 1);
                                this.MergeSlots();

                                return i;
                            }
                        }
                    }
                    
                    this._slots.Add(new LogicUnitProductionSlot(data, 1, false));
                    this.MergeSlots();

                    if (this._slots.Count > 0)
                    {
                        LogicCombatItemData productionData = this.GetCurrentlyTrainedUnit();

                        if (productionData != null && this._timer == null)
                        {
                            this._timer = new LogicTimer();
                            this._timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), this._level, 0),
                                                   this._level.GetLogicTime(), false, -1);
                        }
                    }
                }
            }
            else
            {
                Debugger.Error("LogicUnitProduction - Trying to add NULL character!");
            }

            return -1;
        }

        /// <summary>
        ///     Adds the specified unit to queue.
        /// </summary>
        public int AddUnitToQueue(LogicCombatItemData data, int index, bool ignoreCapacity)
        {
            if (data != null)
            {
                if (this.CanAddUnitToQueue(data, ignoreCapacity))
                {
                    LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
                    LogicCombatItemData productionData = this.GetCurrentlyTrainedUnit();

                    this._slots.Add(index, new LogicUnitProductionSlot(data, 1, false));
                    this.MergeSlots();

                    if (productionData != null)
                    {
                        if (this.GetCurrentlyTrainedUnit() == data || this.GetCurrentlyTrainedIndex() != index)
                        {
                            return index;
                        }
                    }
                    else
                    {
                        productionData = this.GetCurrentlyTrainedUnit();
                    }

                    if (this._timer != null)
                    {
                        this._timer.Destruct();
                        this._timer = null;
                    }

                    this._timer = new LogicTimer();
                    this._timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), this._level, 0),
                        this._level.GetLogicTime(), false, -1);
                    
                    return index;
                }
            }
            else
            {
                Debugger.Error("LogicUnitProduction - Trying to add NULL character!");
            }

            return -1;
        }

        /// <summary>
        ///     Gets if the specified unit can be add to queue.
        /// </summary>
        public bool CanAddUnitToQueue(LogicCombatItemData data, bool ignoreCapacity)
        {
            if (data != null)
            {
                if (data.GetDataType() == this._unitProductionType)
                {
                    LogicGameObjectManager gameObjectManager = this._level.GetGameObjectManagerAt(0);
                    LogicBuilding productionHouse = gameObjectManager.GetHighestBuilding(data.GetProductionHouseData());

                    if (productionHouse != null)
                    {
                        if (!data.IsUnlockedForProductionHouseLevel(productionHouse.GetUpgradeLevel()))
                        {
                            return false;
                        }

                        if (data.GetUnitOfType() != productionHouse.GetBuildingData().ProducesUnitsOfType)
                        {
                            return false;
                        }
                    }

                    if (this._level.GetMissionManager().IsTutorialFinished() || this._level.GetHomeOwnerAvatar().GetUnitsTotalCapacity() + this.GetTotalCount() < 20)
                    {
                        if (ignoreCapacity)
                        {
                            return true;
                        }

                        LogicAvatar avatar = this._level.GetHomeOwnerAvatar();
                        LogicComponentManager componentManager = this._level.GetComponentManagerAt(this._villageType);
                        Int32 totalMaxHousing = componentManager.GetTotalMaxHousing(this._unitProductionType != 3 ? 1 : 0) * 2;
                        Int32 totalUsedCapacity = this.GetTotalCount() + data.GetHousingSpace() + (this._unitProductionType == 3
                            ? avatar.GetUnitsTotalCapacity()
                            : avatar.GetSpellsTotalCapacity());

                        return totalMaxHousing >= totalUsedCapacity;
                    }
                }
                else
                {
                    Debugger.Error("Trying to add wrong unit type to UnitProduction");
                }
            }
            else
            {
                Debugger.Error("Trying to add NULL troop to UnitProduction");
            }

            return false;
        }

        /// <summary>
        ///     Gets the total unit count.
        /// </summary>
        public int GetTotalCount()
        {
            int count = 0;

            for (int i = 0; i < this._slots.Count; i++)
            {
                LogicUnitProductionSlot slot = this._slots[i];
                LogicCombatItemData data = (LogicCombatItemData) slot.GetData();

                count += data.GetHousingSpace() * slot.GetCount();
            }

            return count;
        }

        /// <summary>
        ///     Ticks for update this instance. Called before Tick method.
        /// </summary>
        public void SubTick()
        {
            if (this._boostTimer != null && this._boostPause)
            {
                this._boostTimer.StartTimer(this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime()), this._level.GetLogicTime(), false, -1);
            }
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public void Tick()
        {
            bool unitTrained = false;

            if (this._timer != null)
            {
                if (this.GetRemainingBoostTimeSecs() > 0)
                {
                    if (!this.IsBoostPaused())
                    {
                        this._timer.FastForwardSubticks(4 * this.GetBoostMultiplier() - 4);
                    }
                }

                unitTrained = this._timer.GetRemainingSeconds(this._level.GetLogicTime()) == 0;
            }

            if (this._nextProduction > 0)
            {
                if (!unitTrained)
                {
                    this._nextProduction = LogicMath.Max(this._nextProduction - 64, 0);
                }
            }

            if (this._boostTimer != null && this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
            {
                this._boostTimer.Destruct();
                this._boostTimer = null;
            }

            if (this._nextProduction == 0)
            {
                if (this.GetWaitingForSpaceSlot() != null || unitTrained)
                {
                    this.ProductionCompleted(false);
                }
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardTime(int secs)
        {
            if (this._boostTimer != null && !this._boostPause)
            {
                int remainingSecs = this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime());

                if (remainingSecs <= secs)
                {
                    this._boostTimer.Destruct();
                    this._boostTimer = null;
                }
                else
                {
                    this._boostTimer.StartTimer(remainingSecs - secs, this._level.GetLogicTime(), false, -1);
                }
            }
            
            if (this.GetRemainingBoostTimeSecs() > 0)
            {
                if (this.GetBoostMultiplier() >= 2 && !this.IsBoostPaused())
                {
                    secs = LogicMath.Min(secs, this.GetRemainingSeconds()) * (this.GetBoostMultiplier() - 1) + secs;
                }

                if (this._timer != null)
                {
                    if (!this.IsBoostPaused())
                    {
                        this._timer.FastForwardSubticks(4 * this.GetBoostMultiplier() - 4);
                    }
                }
            }

            do
            {
                if (secs <= 0)
                {
                    break;
                }

                LogicUnitProductionSlot productionSlot = this.GetCurrentlyTrainedSlot();

                if (productionSlot == null)
                {
                    break;
                }

                if (this._timer == null)
                {
                    LogicCombatItemData productionData = (LogicCombatItemData) productionSlot.GetData();

                    this._timer = new LogicTimer();
                    this._timer.StartTimer(productionData.GetTrainingTime(this._level.GetHomeOwnerAvatar().GetUnitUpgradeLevel(productionData), this._level, 0),
                        this._level.GetLogicTime(), false, -1);

                    Debugger.Print("LogicUnitProduction::fastForwardTime null timer, restart: " + this._timer.GetRemainingSeconds(this._level.GetLogicTime()));
                }

                int remainingSecs = this._timer.GetRemainingSeconds(this._level.GetLogicTime());

                if (secs < remainingSecs)
                {
                    this._timer.StartTimer(remainingSecs - secs, this._level.GetLogicTime(), false, -1);
                    break;
                }

                secs -= remainingSecs;
                this._timer.StartTimer(0, this._level.GetLogicTime(), false, -1);
            } while (this.ProductionCompleted(false));

            if (this._timer != null)
            {
                Debugger.Print("LogicUnitProduction::fastForwardTime time: " + this._timer.GetRemainingSeconds(this._level.GetLogicTime()));
            }
        }

        /// <summary>
        ///     Loads this instance to json.
        /// </summary>
        public void Load(LogicJSONObject root)
        {
            if (this._timer != null)
            {
                this._timer.Destruct();
                this._timer = null;
            }

            if (this._boostTimer != null)
            {
                this._boostTimer.Destruct();
                this._boostTimer = null;
            }

            if (this._slots.Count != 0)
            {
                do
                {
                    this._slots[0].Destruct();
                    this._slots.Remove(0);
                } while (this._slots.Count != 0);
            }

            LogicJSONObject jsonObject = root.GetJSONObject("unit_prod");

            if (jsonObject != null)
            {
                LogicJSONArray slotArray = jsonObject.GetJSONArray("slots");

                if (slotArray != null)
                {
                    for (int i = 0; i < slotArray.Size(); i++)
                    {
                        LogicJSONObject slotObject = slotArray.GetJSONObject(i);

                        if (slotObject != null)
                        {
                            LogicJSONNumber dataObject = slotObject.GetJSONNumber("id");

                            if (dataObject != null)
                            {
                                LogicData data = LogicDataTables.GetDataById(dataObject.GetIntValue());

                                if (data != null)
                                {
                                    LogicJSONNumber countObject = slotObject.GetJSONNumber("cnt");
                                    LogicJSONBoolean termineObject = slotObject.GetJSONBoolean("t");

                                    if (countObject != null)
                                    {
                                        if (countObject.GetIntValue() > 0)
                                        {
                                            LogicUnitProductionSlot slot = new LogicUnitProductionSlot(data, countObject.GetIntValue(), false);

                                            if (termineObject != null)
                                            {
                                                slot.SetTerminate(termineObject.IsTrue());
                                            }

                                            this._slots.Add(slot);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (this._slots.Count > 0)
                {
                    LogicUnitProductionSlot slot = this.GetCurrentlyTrainedSlot();

                    if (slot != null)
                    {
                        LogicJSONNumber timeObject = jsonObject.GetJSONNumber("t");

                        if (timeObject != null)
                        {
                            this._timer = new LogicTimer();
                            this._timer.StartTimer(timeObject.GetIntValue(), this._level.GetLogicTime(), false, -1);
                        }
                        else
                        {
                            LogicCombatItemData combatItemData = (LogicCombatItemData) slot.GetData();
                            LogicAvatar avatar = this._level.GetHomeOwnerAvatar();
                            Int32 upgradeLevel = 0;

                            if (avatar != null)
                            {
                                upgradeLevel = avatar.GetUnitUpgradeLevel(combatItemData);
                            }

                            this._timer = new LogicTimer();
                            this._timer.StartTimer(combatItemData.GetTrainingTime(upgradeLevel, this._level, 0), this._level.GetLogicTime(), false, -1);
                            
                            Debugger.Print("LogicUnitProduction::load null timer, restart: " + this._timer.GetRemainingSeconds(this._level.GetLogicTime()));
                        }
                    }
                }

                LogicJSONNumber boostTimeObject = jsonObject.GetJSONNumber("boost_t");

                if (boostTimeObject != null)
                {
                    this._boostTimer = new LogicTimer();
                    this._boostTimer.StartTimer(boostTimeObject.GetIntValue(), this._level.GetLogicTime(), false, -1);
                }

                LogicJSONBoolean boostPauseObject = jsonObject.GetJSONBoolean("boost_pause");

                if (boostPauseObject != null)
                {
                    this._boostPause = boostPauseObject.IsTrue();
                }
            }
            else
            {
                Debugger.Warning("LogicUnitProduction::load - Component wasn't found from the JSON");
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject root)
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            if (this._timer != null)
            {
                jsonObject.Put("t", new LogicJSONNumber(this._timer.GetRemainingSeconds(this._level.GetLogicTime())));
            }

            if (this._slots.Count > 0)
            {
                LogicJSONArray slotArray = new LogicJSONArray();

                for (int i = 0; i < this._slots.Count; i++)
                {
                    LogicUnitProductionSlot slot = this._slots[i];

                    if (slot != null)
                    {
                        LogicJSONObject slotObject = new LogicJSONObject();

                        slotObject.Put("id", new LogicJSONNumber(slot.GetData().GetGlobalID()));
                        slotObject.Put("cnt", new LogicJSONNumber(slot.GetCount()));

                        if (slot.IsTerminate())
                        {
                            slotObject.Put("t", new LogicJSONBoolean(true));
                        }

                        slotArray.Add(slotObject);
                    }
                }

                jsonObject.Put("slots", slotArray);
            }

            if (this._boostTimer != null)
            {
                jsonObject.Put("boost_t", new LogicJSONNumber(this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime())));
            }

            if (this._boostPause)
            {
                jsonObject.Put("boost_pause", new LogicJSONBoolean(true));
            }

            root.Put("unit_prod", jsonObject);
        }
    }
}