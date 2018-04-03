namespace LineageSoft.Magic.Logic.GameObject.Component
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Time;
    using LineageSoft.Magic.Logic.Util;
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Util;

    public sealed class LogicVillage2UnitComponent : LogicComponent
    {
        private LogicDataSlot _unit;
        private LogicTimer _trainingTimer;

        private bool _noBarrack;
        private int _productionType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicVillage2UnitComponent" /> class.
        /// </summary>
        public LogicVillage2UnitComponent(LogicGameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        ///     Train the specified unit.
        /// </summary>
        public void TrainUnit(LogicCombatItemData combatItemData)
        {
            if (this._unit != null)
            {
                this._unit.Destruct();
                this._unit = null;
            }

            if (this._trainingTimer != null)
            {
                this._trainingTimer.Destruct();
                this._trainingTimer = null;
            }

            this._unit = new LogicDataSlot(combatItemData, 0);
            this._trainingTimer = new LogicTimer();
            this._trainingTimer.StartTimer(this.GetTrainingTime(combatItemData), this._parent.GetLevel().GetLogicTime(), false, -1);
        }

        /// <summary>
        ///     Sets the unit.
        /// </summary>
        public void SetUnit(LogicCombatItemData combatItemData, int count)
        {
            if (this._unit != null)
            {
                this._unit.Destruct();
                this._unit = null;
            }

            this._unit = new LogicDataSlot(combatItemData, count);
        }
        
        /// <summary>
        ///     Gets the training time.
        /// </summary>
        public int GetTrainingTime(LogicCombatItemData data)
        {
            return data.GetTrainingTime(this._parent.GetLevel().GetHomeOwnerAvatar().GetUnitUpgradeLevel(data), this._parent.GetLevel(), 0);
        }

        /// <summary>
        ///     Gets the total training time.
        /// </summary>
        public int GetTotalSecs()
        {
            if (this._unit != null)
            {
                return this.GetTrainingTime((LogicCombatItemData) this._unit.GetData());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the max unit count.
        /// </summary>
        public int GetMaxCapacity()
        {
            LogicAvatar homeOwnerAvatar = this._parent.GetLevel().GetHomeOwnerAvatar();
            LogicCharacterData characterData = (LogicCharacterData) this._unit.GetData();

            if (homeOwnerAvatar != null)
            {
                return characterData.GetUnitsInCamp(homeOwnerAvatar.GetUnitUpgradeLevel(characterData));
            }

            Debugger.Error("AVATAR = NULL");

            return 0;
        }

        /// <summary>
        ///     Gets if this component is empty.
        /// </summary>
        public bool IsEmpty()
        {
            if (this._unit != null)
            {
                return this._unit.GetCount() <= 0;
            }

            return true;
        }

        /// <summary>
        ///     Gets the remaining secs before the end of unit production.
        /// </summary>
        public int GetRemainingSecs()
        {
            if(this._trainingTimer != null)
            {
                int remainingSecs = this._trainingTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime());
                int trainingTime = this._unit != null ? this.GetTrainingTime((LogicCombatItemData) this._unit.GetData()) : 0;

                return LogicMath.Min(remainingSecs, trainingTime);
            }

            return 0;
        }

        /// <summary>
        ///     Called when the training of the unit is finished.
        /// </summary>
        public void TrainingFinished()
        {
            this._unit.SetCount(this.GetMaxCapacity());

            if (this._trainingTimer != null)
            {
                this._trainingTimer.Destruct();
                this._trainingTimer = null;
            }

            LogicAvatar homeOwnerAvatar = this._parent.GetLevel().GetHomeOwnerAvatar();
            LogicCombatItemData combatItemData =  (LogicCombatItemData) this._unit.GetData();

            int unitCount = this._parent.GetLevel().GetMissionManager().IsVillage2TutorialOpen()
                ? this._unit.GetCount() - homeOwnerAvatar.GetUnitCountVillage2(combatItemData)
                : this._unit.GetCount();

            homeOwnerAvatar.CommodityCountChangeHelper(7, this._unit.GetData(), unitCount);

            if (this._parent.GetLevel().GetGameListener() != null)
            {
                // ?
            }

            int state = this._parent.GetLevel().GetState();

            if (state == 1)
            {
                if (this._parent.GetListener() != null)
                {
                    // ?
                }
            }
        }

        /// <summary>
        ///     Gets the remaining ms before the end of unit production.
        /// </summary>
        public int GetRemainingMS()
        {
            if (this._trainingTimer != null)
            {
                return this._trainingTimer.GetRemainingMS(this._parent.GetLevel().GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 15;
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public override void Tick()
        {
            LogicArrayList<LogicComponent> components = this._parent.GetComponentManager().GetComponents(15);
            int barrackCount = this._parent.GetGameObjectManager().GetBarrackCount();
            int barrackFoundCount = 0;

            if (barrackCount > 0)
            {
                int idx = 0;

                do
                {
                    LogicBuilding building = (LogicBuilding) this._parent.GetGameObjectManager().GetBarrack(idx);

                    if (building != null && !building.IsConstructing())
                    {
                        barrackFoundCount += 1;
                    }
                } while (++idx != barrackCount);
            }

            this._noBarrack = barrackFoundCount == 0;

            for (int i = 0; i < components.Count; i++)
            {
                LogicVillage2UnitComponent component = (LogicVillage2UnitComponent) components[i];

                if (barrackFoundCount != 0)
                {
                    if (component == this)
                    {
                        break;
                    }
                }

                if (component != null)
                {
                    if (component._unit != null)
                    {
                        if (component._unit.GetData() != null && component._unit.GetCount() == 0)
                        {
                            if (component.GetRemainingSecs() > 0)
                            {
                                if (this._trainingTimer != null)
                                {
                                    this._trainingTimer.StartTimer(this._trainingTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime()),
                                                                   this._parent.GetLevel().GetLogicTime(), false, -1);
                                }

                                return;
                            }
                        }
                    }
                }
            }

            if (this._trainingTimer != null)
            {
                if (this._parent.GetLevel().GetRemainingClockTowerBoostTime() > 0)
                {
                    if (this._parent.GetData().GetDataType() == 0)
                    {
                        if (this._parent.GetData().GetVillageType() == 1)
                        {
                            this._trainingTimer.FastForwardSubticks(4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
                        }
                    }
                }
            }

            if (this.GetParent() != null)
            {
                LogicGameObject gameObject = this.GetParent();

                if (gameObject.GetListener() != null)
                {

                }
            }

            if (this._trainingTimer != null)
            {
                if (this._trainingTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime()) <= 0)
                {
                    if (this._unit != null)
                    {
                        this.TrainingFinished();
                    }
                }
            }
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONObject unitProductionObject = jsonObject.GetJSONObject("up2");

            if (unitProductionObject != null)
            {
                LogicJSONNumber timerObject = jsonObject.GetJSONNumber("t");

                if (timerObject != null)
                {
                    int time = timerObject.GetIntValue();

                    if (this._trainingTimer != null)
                    {
                        this._trainingTimer.Destruct();
                        this._trainingTimer = null;
                    }

                    this._trainingTimer = new LogicTimer();
                    this._trainingTimer.StartTimer(time, this._parent.GetLevel().GetLogicTime(), false, -1);
                }

                LogicJSONArray unitArray = jsonObject.GetJSONArray("unit");

                if (unitArray != null)
                {
                    LogicJSONNumber dataObject = unitArray.GetJSONNumber(0);
                    LogicJSONNumber cntObject = unitArray.GetJSONNumber(0);

                    if (dataObject != null)
                    {
                        if (cntObject != null)
                        {
                            LogicData data = LogicDataTables.GetDataById(dataObject.GetIntValue(), this._productionType != 0 ? 25 : 3);

                            if (data != null)
                            {
                                this._unit = new LogicDataSlot(data, cntObject.GetIntValue());
                            }
                            else
                            {
                                Debugger.Error("LogicVillage2UnitComponent::load - Character data is NULL!");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            LogicJSONObject unitProductionObject = new LogicJSONObject();

            if (this._unit != null && this._unit.GetData() != null)
            {
                LogicJSONArray unitArray = new LogicJSONArray();
                unitArray.Add(new LogicJSONNumber(this._unit.GetData().GetGlobalID()));
                unitArray.Add(new LogicJSONNumber(this._unit.GetCount()));
                unitProductionObject.Put("unit", unitArray);
            }

            if (this._trainingTimer != null)
            {
                unitProductionObject.Put("t", new LogicJSONNumber(this._trainingTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime())));
            }

            jsonObject.Put("up2", unitProductionObject);
        }
    }
}