﻿namespace RivieraStudio.Magic.Logic.Unit
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

        private bool _productionPause;
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
        ///     Gets if the boost is paused.
        /// </summary>
        public bool IsBoostPaused()
        {
            return this._productionPause;
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
            int index = -1;

            for (int i = 0; i < this._slots.Count; i++)
            {
                if (!this._slots[i].IsTerminate())
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                return (LogicCombatItemData) this._slots[index].GetData();
            }

            return null;
        }

        /// <summary>
        ///     Gets the currently trained count.
        /// </summary>
        public LogicUnitProductionSlot GetCurrentlyTrainedSlot()
        {
            if (this._slots.Count > 0)
            {
                LogicUnitProductionSlot slot = this._slots[0];

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
        public bool ProductionCompleted(bool unk)
        {
            Boolean success = false;

            if (!this._productionPause)
            {
                LogicComponentFilter filter = new LogicComponentFilter();

                filter.SetComponentType(0);

                while (true)
                {
                    LogicComponentManager componentManager = this._level.GetComponentManagerAt(this._villageType);
                    LogicUnitProductionSlot trainedSlot = this.GetCurrentlyTrainedSlot();

                    if (this._slots.Count <= 0)
                    {
                        if (unk)
                        {
                            return false;
                        }
                    }

                    if (trainedSlot == null)
                    {
                        return false;
                    }

                    LogicCombatItemData unitData = (LogicCombatItemData) trainedSlot.GetData();
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

                        break;
                    }

                    LogicUnitStorageComponent unitStorageComponent =
                        (LogicUnitStorageComponent) componentManager.GetClosestComponent(productionHouse.GetX(), productionHouse.GetY(), filter);

                    if (unitStorageComponent != null)
                    {
                        if (unitStorageComponent.CanAddUnit(unitData))
                        {
                            success = true;

                            this._level.GetHomeOwnerAvatar().CommodityCountChangeHelper(0, unitData, 1);
                            unitStorageComponent.AddUnit(unitData);

                            if (this._level.GetState() == 1 || this._level.GetState() == 3)
                            {
                                if (unitStorageComponent.GetParentListener() != null)
                                {
                                    // BLABLABLA
                                }
                            }

                            if (trainedSlot.IsTerminate())
                            {
                                this.RemoveUnit(unitData, -1);
                            }
                            else
                            {
                                this.StartProducingNextUnit();
                            }
                            
                            if (this._slots.Count > 0)
                            {
                                if (this._slots[0].IsTerminate())
                                {
                                    filter.Destruct();
                                }
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
                        }

                        break;
                    }
                    else
                    {
                        filter.AddIgnoreObject(productionHouse);
                    }
                }
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
                this._slots[0] != null &&
                this._slots[0].GetData() == data)
            {
                slot = this._slots[0];
            }
            else
            {
                if (this._slots.Count < 0)
                {
                    return false;
                }

                for (int i = 0; i < this._slots.Count; i++)
                {
                    if (this._slots[i].GetData() == data)
                    {
                        index = i;
                    }
                }

                if (index == -1)
                {
                    return false;
                }

                slot = this._slots[index];
            }

            int count = slot.GetCount();

            if (count > 0)
            {
                removed = true;

                if (count == 1)
                {
                    int terminateIndex = -1;

                    if (this._slots.Count > -1)
                    {
                        for (int i = 0; i < this._slots.Count; i++)
                        {
                            if (this._slots[i].IsTerminate())
                            {
                                terminateIndex = i;
                                break;
                            }
                        }
                    }

                    if (terminateIndex == index)
                    {
                        if (this._timer != null)
                        {
                            this._timer.Destruct();
                            this._timer = null;
                        }
                    }

                    this._slots[terminateIndex].Destruct();
                    this._slots.Remove(terminateIndex);
                }
            }

            if (this._slots.Count > 0)
            {
                LogicUnitProductionSlot productionSlot = null;

                for (int i = 0; i < this._slots.Count; i++)
                {
                    if (!this._slots[i].IsTerminate())
                    {
                        productionSlot = this._slots[i];
                        break;
                    }
                }

                if (productionSlot == null || this._timer != null)
                {
                    if (!removed)
                    {
                        return false;
                    }

                    this.StartProducingNextUnit();
                }
                else
                {
                    LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
                    LogicCombatItemData productionData = (LogicCombatItemData) productionSlot.GetData();

                    this._timer = new LogicTimer();
                    this._timer.StartTimer(productionData.GetTrainingTime(homeOwnerAvatar.GetUnitUpgradeLevel(productionData), this._level, 0), this._level.GetLogicTime(), false,
                        -1);

                    if (removed)
                    {
                        this.StartProducingNextUnit();
                    }
                }
            }
            else
            {
                if (!removed)
                {
                    return false;
                }

                this.StartProducingNextUnit();
            }

            return true;
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
                LogicUnitProductionSlot trainedSlot = null;

                int idx = -1;

                do
                {
                    LogicUnitProductionSlot tmp = this._slots[idx];

                    if (!tmp.IsTerminate())
                    {
                        trainedSlot = tmp;
                        break;
                    }
                } while (++idx < this._slots.Count);

                if (trainedSlot != null)
                {
                    if (trainedSlot.GetCount() == 1)
                    {
                        trainedSlot.SetTerminate(true);
                    }
                    else
                    {
                        trainedSlot.SetCount(trainedSlot.GetCount() - 1);

                        LogicUnitProductionSlot previousSlot = this._slots[LogicMath.Max(idx - 1, 0)];

                        if (previousSlot != null &&
                            previousSlot.IsTerminate() && 
                            previousSlot.GetData().GetGlobalID() == trainedSlot.GetData().GetGlobalID())
                        {
                            previousSlot.SetCount(previousSlot.GetCount() + 1);
                        }
                        else
                        {
                            this._slots.Add(idx, new LogicUnitProductionSlot(trainedSlot.GetData(), 1, true));
                        }
                    }
                }

                LogicCombatItemData nextProductionData = null;

                for (int i = 0; i < this._slots.Count; i++)
                {
                    LogicUnitProductionSlot tmp = this._slots[i];

                    if (!tmp.IsTerminate())
                    {
                        nextProductionData = (LogicCombatItemData) tmp.GetData();
                        break;
                    }
                }

                if (nextProductionData != null)
                {
                    this._timer = new LogicTimer();
                    this._timer.StartTimer(nextProductionData.GetTrainingTime(this._level.GetHomeOwnerAvatar().GetUnitUpgradeLevel(nextProductionData), this._level, 0),
                                           this._level.GetLogicTime(), false, -1);

                    return true;
                }
            }

            return false;
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
                    LogicCombatItemData currentProductionData = null;

                    for (int i = 0; i < this._slots.Count; i++)
                    {
                        LogicUnitProductionSlot tmp = this._slots[i];

                        if (!tmp.IsTerminate())
                        {
                            currentProductionData = (LogicCombatItemData) tmp.GetData();
                            break;
                        }
                    }
                    
                    this._slots.Add(index, new LogicUnitProductionSlot(data, 1, false));

                    if (currentProductionData != null)
                    {
                        int currentProductionIndex = -1;

                        for (int i = 0; i < this._slots.Count; i++)
                        {
                            if (!this._slots[i].IsTerminate())
                            {
                                currentProductionIndex = i;
                                break;
                            }    
                        }

                        if (currentProductionData == data || currentProductionIndex != index)
                        {
                            return index;
                        }
                    }

                    if (this._timer != null)
                    {
                        this._timer.Destruct();
                        this._timer = null;
                    }

                    LogicCombatItemData trainProductionData = null;

                    for (int i = 0; i < this._slots.Count; i++)
                    {
                        LogicUnitProductionSlot tmp = this._slots[i];

                        if (!tmp.IsTerminate())
                        {
                            trainProductionData = (LogicCombatItemData) tmp.GetData();
                            break;
                        }
                    }

                    this._timer = new LogicTimer();
                    this._timer.StartTimer((trainProductionData ?? currentProductionData).GetTrainingTime(this._level.GetLogicTime(), this._level, 0), this._level.GetLogicTime(), false, -1);

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
                if (data.GetCombatItemType() == this._unitProductionType)
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
                        Int32 totalUsedCapacity = this.GetTotalCount() + data.GetHousingSpace() + this._unitProductionType == 3
                            ? avatar.GetUnitsTotalCapacity()
                            : avatar.GetSpellsTotalCapacity();

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
                    this._nextProduction = LogicMath.Max(64, 0);
                }
            }

            if (this._boostTimer != null && this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
            {
                this._boostTimer.Destruct();
                this._boostTimer = null;
            }

            if (this._nextProduction == 0)
            {
                if (this.GetCurrentlyTrainedSlot() != null || unitTrained)
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
            // FastForwardTime.
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
                    Int32 idx = 0;
                    LogicUnitProductionSlot slot = null;

                    do
                    {
                        LogicUnitProductionSlot tmp = this._slots[idx];

                        if (!tmp.IsTerminate())
                        {
                            slot = tmp;
                            break;
                        }
                    } while (++idx != this._slots.Count);

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