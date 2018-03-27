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
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Util;

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
            LogicBuildingData buildingData = this.GetBuildingData();

            this._isLocked = buildingData.IsLocked();

            if (buildingData.GetHitpoints(0) > 0)
            {
                LogicHitpointComponent hitpointComponent = new LogicHitpointComponent(this, buildingData.GetHitpoints(0), 1);
                hitpointComponent.SetMaxRegenerationTime(buildingData.GetRegenerationTime(0));
                this.AddComponent(hitpointComponent);
            }

            if (buildingData.GetHeroData() != null)
            {
                LogicHeroBaseComponent heroBaseComponent = new LogicHeroBaseComponent(this, buildingData.GetHeroData());
                this.AddComponent(heroBaseComponent);

                if (buildingData.ShareHeroCombatData)
                {
                    // ???
                }
            }

            if (buildingData.UpgradesUnits)
            {
                LogicUnitUpgradeComponent unitUpgradeComponent = new LogicUnitUpgradeComponent(this);
                unitUpgradeComponent.SetEnabled(false);
                this.AddComponent(unitUpgradeComponent);
            }

            if (buildingData.Village2Housing > 0)
            {
                LogicVillage2UnitComponent village2UnitComponent = new LogicVillage2UnitComponent(this);
                village2UnitComponent.SetEnabled(false);
                this.AddComponent(village2UnitComponent);
            }

            if (buildingData.GetUnitProduction(0) > 0)
            {
                LogicUnitProductionComponent unitProductionComponent = new LogicUnitProductionComponent(this);
                unitProductionComponent.SetEnabled(false);
                this.AddComponent(unitProductionComponent);
            }

            if(buildingData.GetUnitStorageCapacity(0) > 0)
            {
                if (buildingData.IsAllianceCastle())
                {
                    this.AddComponent(new LogicBunkerComponent(this, 0));
                }
                else
                {
                    this.AddComponent(new LogicUnitStorageComponent(this, 0));
                }
            }

            if (buildingData.GetProduceResource() != null)
            {
                LogicResourceProductionComponent resourceProductionComponent = new LogicResourceProductionComponent(this, buildingData.GetProduceResource());
                resourceProductionComponent.SetEnabled(false);
                this.AddComponent(resourceProductionComponent);
            }

            if (buildingData.StoresResources())
            {
                if (buildingData.IsAllianceCastle())
                {
                    // TODO: Implement LogicBunkerResourceStorageComponent.
                }
                else
                {
                    this.AddComponent(new LogicResourceStorageComponent(this));
                }
            }
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
            if (this._constructionTimer != null)
            {
                return this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the remaining boost cooldown time.
        /// </summary>
        public int GetRemainingBoostCooldownTime()
        {
            if (this._boostCooldownTimer != null)
            {
                return this._boostCooldownTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the gear level.
        /// </summary>
        public int GetGearLevel()
        {
            return this._gear;
        }

        /// <summary>
        ///     Gets the remaining boost time.
        /// </summary>
        public override int GetRemainingBoostTime()
        {
            if (this._boostTimer != null)
            {
                return this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
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
        ///     Gets if the boost of this <see cref="LogicBuilding"/> instance is paused.
        /// </summary>
        public bool IsBoostPaused()
        {
            return this._boostPaused;
        }

        /// <summary>
        ///     Gets the <see cref="LogicBuilding"/> instance level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgLevel;
        }

        /// <summary>
        ///     Gets the wall index.
        /// </summary>
        public int GetWallIndex()
        {
            return this._wallIndex;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicBuilding"/> instance is wall.
        /// </summary>
        public override bool IsWall()
        {
            return this.GetBuildingData().IsWall();
        }

        /// <summary>
        ///     Gets the width in tiles.
        /// </summary>
        public override int GetWidthInTiles()
        {
            return this.GetBuildingData().GetWidth();
        }

        /// <summary>
        ///     Gets the height in tiles.
        /// </summary>
        public override int GetHeightInTiles()
        {
            return this.GetBuildingData().GetHeight();
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
			
			if (this._boostCooldownTimer != null)
            {
                this._boostCooldownTimer.Destruct();
                this._boostCooldownTimer = null;
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
            if (this._upgLevel != 0 || this._constructionTimer == null || this._upgrading)
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
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int secs)
        {
            if (this._constructionTimer != null)
            {
                if (this._constructionTimer.GetEndTimestamp() == -1)
                {
                    int remainingTime = this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime());

                    if (remainingTime > secs)
                    {
                        base.FastForwardTime(secs);
                        this._constructionTimer.StartTimer(remainingTime - secs, this._level.GetLogicTime(), false, -1);
                    }
                    else
                    {
                        secs -= remainingTime;
                        goto finishConstruction;
                    }
                }
                else
                {
                    this._constructionTimer.AdjustEndSubtick(this._level);

                    if (this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime()) == 0)
                    {
                        goto finishConstruction;
                    }

                    base.FastForwardTime(secs);
                }

                int maxClockTowerFastForward = this._level.GetUpdatedClockTowerBoostTime();

                if (maxClockTowerFastForward > 0 && !this._level.IsClockTowerBoostPaused())
                {
                    if (this._data.GetVillageType() == this._villageType)
                    {
                        this._constructionTimer.FastForward(this._constructionTimer.GetFastForward() + 60 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() * LogicMath.Min(secs, maxClockTowerFastForward) - 1);
                    }
                }

                return;

                finishConstruction:

                if (LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
                {
                    base.FastForwardTime(secs);
                    this._constructionTimer.StartTimer(0, this._level.GetLogicTime(), false, -1);
                }
                else
                {
                    base.FastForwardTime(0);
                    this.FinishConstruction(true);
                    base.FastForwardTime(secs);
                }
            }
            else
            {
                base.FastForwardTime(secs);
            }
        }

        /// <summary>
        ///     Updates the hidden state.
        /// </summary>
        public void UpdateHidden()
        {
            // TODO: Implement LogicBuilding::updateHidden();
        }

        /// <summary>
        ///     Gets if this building is maxed.
        /// </summary>
        public bool IsMaxUpgradeLevel()
        {
            LogicBuildingData buildingData = this.GetBuildingData();

            if (buildingData.IsTownHallVillage2())
            {
                return this._upgLevel >= this._level.GetGameMode().GetConfiguration().GetMaxTownHallLevel() - 1;
            }

            if (buildingData.GetVillageType() != 1 || this.GetRequiredTownHallLevelForUpgrade() < this._level.GetGameMode().GetConfiguration().GetMaxTownHallLevel() - 1)
            {
                return this._upgLevel >= buildingData.GetUpgradeLevelCount() - 1;
            }

            return true;
        }

        /// <summary>
        ///     Gets the required townhall level for upgrade.
        /// </summary>
        public int GetRequiredTownHallLevelForUpgrade()
        {
            return this.GetBuildingData().GetRequiredTownHallLevel(LogicMath.Min(this._upgLevel + 1, this.GetBuildingData().GetUpgradeLevelCount()));
        }

        /// <summary>
        ///     Gets if this <see cref="LogicBuilding"/> can be upgraded.
        /// </summary>
        public bool CanUpgrade(bool canCallListener)
        {
            if (this._constructionTimer == null && !this.IsMaxUpgradeLevel())
            {
                if (this._level.GetTownHallLevel(this._villageType) >= this.GetRequiredTownHallLevelForUpgrade())
                {
                    return true;
                }

                this._level.GetGameListener().TownHallLevelTooLow(this.GetRequiredTownHallLevelForUpgrade());
            }

            return false;
        }

        /// <summary>
        ///     Speed up the construction.
        /// </summary>
        public bool SpeedUpConstruction()
        {
            if (this._constructionTimer != null)
            {
                int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(this._constructionTimer?.GetRemainingSeconds(this._level.GetLogicTime()) ?? 0, 0, this._villageType);

                if (this._level.GetPlayerAvatar().HasEnoughDiamonds(speedUpCost, true, this._level))
                {
                    Debugger.Log("LogicBuilding::speedUpConstruction speedup cost: " + speedUpCost);

                    this._level.GetPlayerAvatar().UseDiamonds(speedUpCost);
                    this.FinishConstruction(true);

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Starts the construction of the <see cref="LogicBuilding"/>.
        /// </summary>
        public void StartConstructing(bool ignoreState)
        {
            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            int constructionTime = this.GetBuildingData().GetConstructionTime(this._upgLevel, this._level, 1);

            if (constructionTime <= 0)
            {
                this.FinishConstruction(ignoreState);
            }
            else
            {
                this._constructionTimer = new LogicTimer();
                this._constructionTimer.StartTimer(constructionTime, this._level.GetLogicTime(), true, this._level.GetGameMode().GetCurrentTime());

                this._level.GetWorkerManagerAt(this.GetBuildingData().GetVillageType()).AllocateWorker(this);
            }

            if (this._villageType == 1 && this._isLocked)
            {
                // this._level.GetGameListener.???
            }
        }

        /// <summary>
        ///     Starts the construction of the <see cref="LogicBuilding"/>.
        /// </summary>
        public void StartUpgrading(bool ignoreState, bool gearup)
        {
            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            int constructionTime;

            if (this._gearing)
            {
                constructionTime = this.GetBuildingData().GetGearUpTime(this._upgLevel);
                this._gearing = true;
            }
            else
            {
                constructionTime = this.GetBuildingData().GetConstructionTime(this._upgLevel + 1, this._level, 0);
                this._upgrading = true;
            }

            if (constructionTime <= 0)
            {
                this.FinishConstruction(ignoreState);
            }
            else
            {
                this._level.GetWorkerManagerAt(this._gearing ? 1 : this.GetBuildingData().GetVillageType()).AllocateWorker(this);

                if (this.GetComponent(5) != null)
                {
                    ((LogicResourceProductionComponent) this.GetComponent(5)).CollectResources(false);
                }

                this.EnableComponent(5, false);
                this.EnableComponent(3, false);
                this.EnableComponent(9, false);

                this._constructionTimer = new LogicTimer();
                this._constructionTimer.StartTimer(constructionTime, this._level.GetLogicTime(), true, this._level.GetGameMode().GetCurrentTime());
            }
        }

        /// <summary>
        ///     Finishes the construction of the <see cref="LogicBuilding"/>.
        /// </summary>
        public void FinishConstruction(bool ignoreState)
        {
            int state = this._level.GetState();

            if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
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
                            if (this._upgLevel != 0 || this._upgrading)
                            {
                                int newUpgLevel = this._upgLevel + 1;

                                if (this._upgLevel >= this.GetBuildingData().GetUpgradeLevelCount() - 1)
                                {
                                    Debugger.Warning("LogicBuilding - Trying to upgrade to level that doesn't exist! - " + this.GetBuildingData().GetName());
                                    newUpgLevel = this.GetBuildingData().GetUpgradeLevelCount() - 1;
                                }

                                int constructionTime = this.GetBuildingData().GetConstructionTime(newUpgLevel, this._level, 0);
                                int xpGain = LogicMath.Sqrt(constructionTime);
                                this.SetUpgradeLevel(newUpgLevel);

                                homeOwnerAvatar.XpGainHelper(xpGain);
                            }
                            else
                            {
                                int constructionTime = this.GetBuildingData().GetConstructionTime(0, this._level, 0);
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
                else if (buildingData.IsTownHallVillage2())
                {
                    this._level.GetHomeOwnerAvatar().SetTownHallVillage2Level(this._upgLevel);
                }
            }

            if (this._upgLevel != 0 || this._upgrading || this._constructionTimer == null)
            {
                bool enable = this._constructionTimer == null;

                this.EnableComponent(3, enable);
                this.EnableComponent(9, enable);
                this.EnableComponent(1, enable);
                this.EnableComponent(15, enable);
                this.EnableComponent(5, enable);

                if (this.GetComponent(0) != null)
                {
                    LogicUnitStorageComponent unitStorageComponent = (LogicUnitStorageComponent) this.GetComponent(0);
                    unitStorageComponent.SetMaxCapacity(buildingData.GetUnitStorageCapacity(this._upgLevel));
                }

                if (this.GetComponent(7) != null)
                {
                    LogicBunkerComponent bunkerComponent = (LogicBunkerComponent) this.GetComponent(7);
                    bunkerComponent.SetMaxCapacity(buildingData.GetUnitStorageCapacity(this._upgLevel));
                }

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
                
                if (this.GetComponent(5) != null)
                {
                    LogicResourceProductionComponent resourceProductionComponent = (LogicResourceProductionComponent) this.GetComponent(5);
                    resourceProductionComponent.SetProduction(buildingData.GetResourcePer100Hours(this._upgLevel), buildingData.GetResourceMax(this._upgLevel));
                }

                if (this.GetComponent(6) != null)
                {
                    LogicResourceStorageComponent resourceStorageComponent = (LogicResourceStorageComponent) this.GetComponent(6);
                    resourceStorageComponent.SetMaxArray(buildingData.GetMaxStoredResourceCounts(this._upgLevel));
                    resourceStorageComponent.SetMaxPercentageArray(buildingData.GetMaxPercentageStoredResourceCounts(this._upgLevel));
                }
            }
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public override void GetChecksum(ChecksumHelper checksum)
        {
            checksum.StartObject("LogicBuilding");

            base.GetChecksum(checksum);

            if (this.GetComponent(6) != null)
            {
                this.GetComponent(6).GetChecksum(checksum);
            }

            if (this.GetComponent(5) != null)
            {
                this.GetComponent(5).GetChecksum(checksum);
            }

            checksum.EndObject();
        }
    }
}