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
                        return false;
                    }

                    LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent) componentManager.GetClosestComponent(productionHouse.GetX(), productionHouse.GetY(), filter);

                    if (unitStorageComponent != null)
                    {
                        if (unitStorageComponent.CanAddUnit(unitData))
                        {
                            this._level.GetHomeOwnerAvatar().CommodityCountChangeHelper(0, unitData, 1);
                            unitStorageComponent.AddUnit(unitData);

                            if (this._level.GetState() == 1 || this._level.GetState() == 3)
                            {
                                if (unitStorageComponent.GetParentListener() != null)
                                {
                                    // BLABLABLA
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
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
                            LogicCombatItemData combatItemData = (LogicCombatItemData)slot.GetData();
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

            root.Put("unit_prod", root);
        }
    }
}