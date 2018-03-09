namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicBuilding : LogicGameObject
    {
        private int _gear;
        private int _upgLevel;
        private int _wallIndex;
        private int _wallX;
        private bool _wallPosition;
        private bool _isLocked;
        private bool _isHidden;
        private bool _gearing;
        private bool _upgrading;
        private bool _boostPaused;

        private LogicTimer _constructionTimer;
        private LogicTimer _boostCooldownTimer;
        private LogicTimer _boostTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuilding" /> class.
        /// </summary>
        public LogicBuilding(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicBuilding.
        }

        /// <summary>
        ///     Gets the <see cref="LogicBuildingData" /> instance.
        /// </summary>
        public LogicBuildingData GetBuildingData()
        {
            return (LogicBuildingData) this._data;
        }

        /// <summary>
        ///     Gets the <see cref="LogicHeroBaseComponent"/> instance.
        /// </summary>
        public LogicHeroBaseComponent GetHeroBaseComponent()
        {
            return null;
        }

        /// <summary>
        ///     Gets the remaining construction time.
        /// </summary>
        public int GetRemainingConstructionTime()
        {
            return this._constructionTimer != null ? this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime()) : 0;
        }

        /// <summary>
        ///     Gets the remaining boost cooldown time.
        /// </summary>
        public int GetRemainingBoostCooldownTime()
        {
            return this._boostCooldownTimer != null ? this._boostCooldownTimer.GetRemainingSeconds(this._level.GetLogicTime()) : 0;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicBuilding"/> instance is in construction.
        /// </summary>
        public bool IsConstructing()
        {
            return this._constructionTimer != null;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicBuilding"/> instance is in upgrading.
        /// </summary>
        public bool IsUpgrading()
        {
            return this._constructionTimer != null && this._upgrading;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicBuilding"/> instance is locked.
        /// </summary>
        public bool IsLocked()
        {
            return this._isLocked;
        }

        /// <summary>
        ///     Gets the <see cref="LogicBuilding"/> instance level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgLevel;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicBuilding"/> instance is wall.
        /// </summary>
        public override bool IsWall()
        {
            return this.GetBuildingData().IsWall();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            if (this._boostTimer != null)
            {
                this._boostTimer.Destruct();
                this._boostTimer = null;
            }
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (this._constructionTimer != null)
            {
                if (this._level.GetRemainingClockTowerBoostTime() > 0 && this.GetBuildingData().VillageType == 1)
                {
                    this._constructionTimer.SetFastForward(this._constructionTimer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
                }

                if (this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                {
                    this.FinishConstruction(false);
                }
            }

            if (this._boostTimer != null)
            {
                if (this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                {
                    this._boostTimer.Destruct();
                    this._boostTimer = null;

                    if (this.GetBuildingData().IsClockTower())
                    {
                        this._boostCooldownTimer = new LogicTimer();
                        this._boostCooldownTimer.StartTimer(LogicDataTables.GetGlobals().GetClockTowerBoostCooldownSecs(), this._level.GetLogicTime(), false, -1);

                        if (this._level.GetGameListener() != null)
                        {
                            // LogicGameListener.
                        }
                    }

                    if (this._listener != null)
                    {
                        this._listener.RefreshState();
                    }
                }
            }

            int state = this._level.GetState();

            if (state != 0)
            {
                this.GetHitpointComponent().EnableRegeneration(state == 1);
            }

            if (this._level.IsInCombatState())
            {
                if (this._isHidden)
                {
                    this.UpdateHidden();
                }
            }
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            if (this._upgLevel != 0 || this._constructionTimer != null || this._upgrading)
            {
                jsonObject.Put("lvl", new LogicJSONNumber(this._upgLevel));
            }
            else
            {
                jsonObject.Put("lvl", new LogicJSONNumber(-1));
            }

            if (this._gearing)
            {
                jsonObject.Put("gearing", new LogicJSONBoolean(true));
            }

            if (this._constructionTimer != null)
            {
                jsonObject.Put("const_t", new LogicJSONNumber(this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime())));

                if (this._constructionTimer.GetEndTimestamp() != -1)
                {
                    jsonObject.Put("const_t_end", new LogicJSONNumber(this._constructionTimer.GetEndTimestamp()));
                }

                if (this._constructionTimer.GetFastForward() > 0)
                {
                    jsonObject.Put("con_ff", new LogicJSONNumber(this._constructionTimer.GetFastForward()));
                }
            }

            if (this._isLocked)
            {
                jsonObject.Put("locked", new LogicJSONBoolean(true));
            }

            if (this._boostTimer != null)
            {
                jsonObject.Put("boost_t", new LogicJSONNumber(this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime())));
            }

            if (this._boostPaused)
            {
                jsonObject.Put("boost_pause", new LogicJSONBoolean(true));
            }

            if (this.GetRemainingBoostCooldownTime() > 0)
            {
                jsonObject.Put("bcd", new LogicJSONNumber(this.GetRemainingBoostCooldownTime()));
            }

            if (this._gear > 0)
            {
                jsonObject.Put("gear", new LogicJSONNumber(this._gear));
            }

            if (this.IsWall())
            {
                jsonObject.Put("wI", new LogicJSONNumber(this._wallIndex));

                if (this._wallPosition)
                {
                    jsonObject.Put("wP", new LogicJSONNumber(1));
                }

                jsonObject.Put("wX", new LogicJSONNumber(this._wallX));
            }

            base.Save(jsonObject);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber lvlObject = jsonObject.GetJSONNumber("lvl");

            if (lvlObject != null)
            {
                this._upgLevel = lvlObject.GetIntValue();
                int maxUpgLevel = this.GetBuildingData().GetUpgradeLevelCount();

                if (this._upgLevel >= maxUpgLevel)
                {
                    Debugger.Warning(string.Format("LogicBuilding::load() - Loaded upgrade level {0} is over max! (max = {1}) id {2} data id {3}", this._upgLevel, maxUpgLevel,
                        this._globalId, this._data.GetGlobalID()));
                    this._upgLevel = maxUpgLevel - 1;
                }
                else
                {
                    if (this._upgLevel < -1)
                    {
                        Debugger.Error("LogicBuilding::load() - Loaded an illegal upgrade level!");
                    }
                }
            }
            else
            {
                Debugger.Error("LogicBuilding::load - Upgrade level was not found!");
                this._upgLevel = 0;
            }

            this._level.GetWorkerManagerAt(1).DeallocateWorker(this);
            this._level.GetWorkerManagerAt(this._villageType).DeallocateWorker(this);

            LogicJSONNumber gearObject = jsonObject.GetJSONNumber("gear");

            if (gearObject != null)
            {
                this._gear = gearObject.GetIntValue();
            }

            LogicJSONBoolean lockedObject = jsonObject.GetJSONBoolean("locked");

            if (lockedObject != null)
            {
                this._isLocked = lockedObject.IsTrue();
            }

            LogicJSONNumber constTimeObject = jsonObject.GetJSONNumber("const_t");

            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            if (constTimeObject != null)
            {
                int secs = constTimeObject.GetIntValue();

                if (!LogicDataTables.GetGlobals().ClampBuildingTimes())
                {
                    if (this._upgLevel < this.GetBuildingData().GetUpgradeLevelCount() - 1)
                    {
                        secs = LogicMath.Min(secs, this.GetBuildingData().GetConstructionTime(this._upgLevel + 1, this._level, 0));
                    }
                }

                this._constructionTimer = new LogicTimer();
                this._constructionTimer.StartTimer(secs, this._level.GetLogicTime(), false, -1);

                LogicJSONNumber constTimeEndObject = jsonObject.GetJSONNumber("const_t_end");

                if (constTimeEndObject != null)
                {
                    this._constructionTimer.SetEndTimestamp(constTimeEndObject.GetIntValue());
                }

                LogicJSONNumber conFfObject = jsonObject.GetJSONNumber("con_ff");

                if (conFfObject != null)
                {
                    this._constructionTimer.SetFastForward(conFfObject.GetIntValue());
                }

                if (this._gearing)
                {
                    this._level.GetWorkerManagerAt(1).AllocateWorker(this);
                }
                else
                {
                    this._level.GetWorkerManagerAt(this._villageType).AllocateWorker(this);
                    this._upgrading = this._upgLevel != -1;
                }
            }

            LogicJSONNumber boostTimeObject = jsonObject.GetJSONNumber("boost_t");

            if (this._boostTimer != null)
            {
                this._boostTimer.Destruct();
                this._boostTimer = null;
            }

            if (boostTimeObject != null)
            {
                this._boostTimer = new LogicTimer();
                this._boostTimer.StartTimer(boostTimeObject.GetIntValue(), this._level.GetLogicTime(), false, -1);
            }

            LogicJSONNumber boostCooldownObject = jsonObject.GetJSONNumber("bcd");

            if (boostCooldownObject != null)
            {
                this._boostCooldownTimer = new LogicTimer();
                this._boostCooldownTimer.StartTimer(boostCooldownObject.GetIntValue(), this._level.GetLogicTime(), false, -1);
            }

            LogicJSONBoolean boostPauseObject = jsonObject.GetJSONBoolean("boost_pause");

            if (boostPauseObject != null)
            {
                this._boostPaused = boostPauseObject.IsTrue();
            }

            if (this._boostTimer == null)
            {
                if (LogicDataTables.GetGlobals().StopBoostPauseWhenBoostTimeZeroOnLoad())
                {
                    this._boostPaused = false;
                }
            }

            if (this.IsWall())
            {
                LogicJSONNumber wallIndexObject = jsonObject.GetJSONNumber("wI");

                if (wallIndexObject != null)
                {
                    this._wallIndex = wallIndexObject.GetIntValue();
                }

                LogicJSONNumber wallXObject = jsonObject.GetJSONNumber("wX");

                if (wallXObject != null)
                {
                    this._wallX = wallXObject.GetIntValue();
                }

                LogicJSONNumber wallPositionObject = jsonObject.GetJSONNumber("wP");

                if (wallPositionObject != null)
                {
                    this._wallPosition = wallPositionObject.GetIntValue() != 0;
                }
            }

            if (LogicDataTables.GetGlobals().FixMergeOldBarrackBoostPausing())
            {
                if (LogicDataTables.GetGlobals().UseNewTraining())
                {
                    if (this.GetBuildingData().GetUnitProduction(0) > 0)
                    {
                        if (this._boostTimer != null)
                        {
                            this._boostTimer.Destruct();
                            this._boostTimer = null;

                            if (this._boostCooldownTimer != null)
                            {
                                this._boostCooldownTimer.Destruct();
                                this._boostCooldownTimer = null;
                            }
                        }
                    }
                }
            }

            this.SetUpgradeLevel(LogicMath.Clamp(this._upgLevel, 0, this.GetBuildingData().GetUpgradeLevelCount()));
            base.Load(jsonObject);
        }

        /// <summary>
        ///     Updates the hidden state.
        /// </summary>
        public void UpdateHidden()
        {
            // TODO: Implement LogicBuilding::updateHidden();
        }

        /// <summary>
        ///     Finishes the construction of the <see cref="LogicBuilding"/>.
        /// </summary>
        public void FinishConstruction(bool notNeedHomeState)
        {
            int state = this._level.GetState();

            if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && notNeedHomeState)
            {
                if (this._level.GetHomeOwnerAvatar() != null)
                {
                    if (this._level.GetHomeOwnerAvatar().IsClientAvatar())
                    {
                        LogicClientAvatar homeOwnerAvatar = (LogicClientAvatar) this._level.GetHomeOwnerAvatar();

                        if (this._constructionTimer != null)
                        {
                            this._constructionTimer.Destruct();
                            this._constructionTimer = null;
                        }

                        this._level.GetWorkerManagerAt(this._gearing ? 1 : this.GetBuildingData().VillageType).DeallocateWorker(this);
                        this._isLocked = false;

                        if (this._gearing)
                        {
                        }
                        else
                        {
                            if (this._upgLevel > 0 || this._upgrading)
                            {
                                int newUpgLevel = this._upgLevel + 1;

                                if (this._upgLevel >= this.GetBuildingData().GetUpgradeLevelCount() - 1)
                                {
                                    Debugger.Warning("LogicBuilding - Trying to upgrade to level that doesn't exist! - " + this.GetBuildingData().GetName());
                                    newUpgLevel = this.GetBuildingData().GetUpgradeLevelCount() - 1;
                                }

                                int constructionTime = this.GetBuildingData().GetConstructionTime(this._upgLevel, this._level, 0);
                                int xpGain = LogicMath.Sqrt(constructionTime);
                                this.SetUpgradeLevel(newUpgLevel);

                                homeOwnerAvatar.XpGainHelper(xpGain);
                            }
                            else
                            {
                                int constructionTime = this.GetBuildingData().GetConstructionTime(this._upgLevel, this._level, 0);
                                int xpGain = LogicMath.Sqrt(constructionTime);
                                this.SetUpgradeLevel(this._upgLevel);

                                homeOwnerAvatar.XpGainHelper(xpGain);
                            }

                            if (this.GetComponent(10) != null)
                            {
                                // HERO
                            }
                        }

                        if (this.GetComponent(5) != null)
                        {
                            ((LogicResourceProductionComponent) this.GetComponent(5)).RestartTimer();
                        }

                        this._upgrading = false;

                        if (this._listener != null)
                        {
                            this._listener.RefreshState();
                        }

                        if (state == 1)
                        {
                            this._level.GetAchievementManager().RefreshStatus();
                        }

                        return;
                    }
                }

                Debugger.Warning("LogicBuilding::finishCostruction failed - Avatar is null or not client avatar");
            }
        }

        /// <summary>
        ///     Sets the building upgrade level.
        /// </summary>
        public void SetUpgradeLevel(int level)
        {
            LogicBuildingData buildingData = (LogicBuildingData) this._data;

            this._upgLevel = LogicMath.Clamp(level, 0, buildingData.GetUpgradeLevelCount() - 1);

            if (this._level.GetHomeOwnerAvatar() != null)
            {
                if (buildingData.IsAllianceCastle())
                {
                    if (!this._isLocked)
                    {
                        this._level.GetHomeOwnerAvatar().SetAllianceCastleLevel(this._upgLevel);
                    }
                }
                else if (buildingData.IsTownHall())
                {
                    this._level.GetHomeOwnerAvatar().SetTownHallLevel(this._upgLevel);
                }
                else if (buildingData.IsTownHall2())
                {
                    this._level.GetHomeOwnerAvatar().SetTownHallVillage2Level(this._upgLevel);
                }
            }

            if (this._upgLevel != 0 || this._constructionTimer == null)
            {
                if (this.GetHitpointComponent() != null)
                {
                    LogicHitpointComponent hitpointComponent = this.GetHitpointComponent();

                    if (this._isLocked)
                    {
                        hitpointComponent.SetMaxHitpoints(0);
                        hitpointComponent.SetHitpoints(0);
                        hitpointComponent.SetMaxRegenerationTime(100);
                    }
                    else
                    {
                        hitpointComponent.SetMaxHitpoints(buildingData.GetHitpoints(this._upgLevel));
                        hitpointComponent.SetHitpoints(buildingData.GetHitpoints(this._upgLevel));
                        hitpointComponent.SetMaxRegenerationTime(buildingData.GetRegenerationTime(this._upgLevel));
                    }
                }
            }

            if (this.GetComponent(5) != null)
            {
            }
        }
    }
}