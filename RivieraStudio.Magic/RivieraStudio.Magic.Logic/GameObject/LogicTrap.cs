namespace RivieraStudio.Magic.Logic.GameObject
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject.Component;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Time;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;

    public sealed class LogicTrap : LogicGameObject
    {
        private LogicTimer _constructionTimer;

        private bool _isLocked;
        private bool _upgrading;
        private bool _disarmed;
        private int _upgLevel;
        private int _numSpawns;
        private int _fadeTime;
        private int _spawnInitDelay;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicObstacle" /> class.
        /// </summary>
        public LogicTrap(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            this.AddComponent(new LogicLayoutComponent(this));

            LogicTrapData trapData = (LogicTrapData) data;

            this._numSpawns = trapData.GetNumSpawns(0);
            this._spawnInitDelay = trapData.GetSpawnInitialDelayMS() / 64;
        }

        /// <summary>
        ///     Gets the <see cref="LogicTrapData"/> instance.
        /// </summary>
        public LogicTrapData GetTrapData()
        {
            return (LogicTrapData) this._data;
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
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public override void GetChecksum(ChecksumHelper checksum, bool includeGameObjects)
        {
            checksum.StartObject("LogicTrap");

            base.GetChecksum(checksum, includeGameObjects);

            if (this._constructionTimer != null)
            {
                checksum.WriteValue("remainingMS", this._constructionTimer.GetRemainingMS(this._level.GetLogicTime()));
            }

            checksum.EndObject();
        }

        /// <summary>
        ///     Finishes the construction of the <see cref="LogicBuilding"/>.
        /// </summary>
        public void FinishConstruction(bool ignoreState)
        {
            int state = this._level.GetState();

            if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
            {
                LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                if (homeOwnerAvatar != null && homeOwnerAvatar.IsClientAvatar())
                {
                    LogicTrapData data = this.GetTrapData();

                    if (this._constructionTimer != null)
                    {
                        this._constructionTimer.Destruct();
                        this._constructionTimer = null;
                    }

                    this._level.GetWorkerManagerAt(this._data.GetVillageType()).DeallocateWorker(this);

                    if (this._upgLevel != 0 || this._upgrading)
                    {
                        if (this._upgLevel >= data.GetUpgradeLevelCount() - 1)
                        {
                            Debugger.Warning("LogicTrap - Trying to upgrade to level that doesn't exist! - " + data.GetName());
                            this._upgLevel = data.GetUpgradeLevelCount() - 1;
                        }
                        else
                        {
                            this._upgLevel += 1;
                        }
                    }

                    if (!ignoreState && !this._disarmed)
                    {
                        if (this.GetListener() != null)
                        {
                            // Listener.
                        }
                    }

                    this.XpGainHelper(LogicGamePlayUtil.TimeToExp(data.GetBuildTime(this._upgLevel)), homeOwnerAvatar, ignoreState);

                    if (this._disarmed)
                    {
                        // Listener.
                    }

                    this._fadeTime = 0;
                    this._disarmed = false;
                    this._upgrading = false;

                    if (this._listener != null)
                    {
                        this._listener.RefreshState();
                    }

                    if (state == 1)
                    {
                        this._level.GetAchievementManager().RefreshStatus();
                    }
                }
                else
                {
                    Debugger.Warning("LogicTrap::finishCostruction failed - Avatar is null or not client avatar");
                }
            }
        }

        /// <summary>
        ///     Gets if the trap is fading out.
        /// </summary>
        public bool IsFadingOut()
        {
            return this._fadeTime > 0;
        }

        /// <summary>
        ///     Sets the upgrade level.
        /// </summary>
        public void SetUpgradeLevel(int upgLevel)
        {
            LogicTrapData data = this.GetTrapData();

            this._upgLevel = LogicMath.Min(upgLevel, data.GetUpgradeLevelCount());
            this._numSpawns = data.GetNumSpawns(upgLevel);
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
                    if (this._data.GetVillageType() == 1)
                    {
                        this._constructionTimer.FastForward(this._constructionTimer.GetFastForward() + 60 * LogicMath.Min(secs, maxClockTowerFastForward) * (LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1));
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
        ///     Gets if the trap is passable.
        /// </summary>
        public override bool IsPassable()
        {
            return true;
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (this._constructionTimer != null)
            {
                if (this._level.GetRemainingClockTowerBoostTime() > 0 &&
                    this._data.GetVillageType() == 1)
                {
                    this._constructionTimer.SetFastForward(this._constructionTimer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
                }

                if (this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                {
                    this.FinishConstruction(false);
                }
            }

            if (this._disarmed)
            {
                if (this._fadeTime >= 0)
                {
                    this._fadeTime = LogicMath.Max(this._fadeTime + 64, 1000);
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

            if (this._constructionTimer != null)
            {
                jsonObject.Put("const_t", new LogicJSONNumber(this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime())));

                if (this._constructionTimer.GetEndTimestamp() != -1)
                {
                    jsonObject.Put("const_t_end", new LogicJSONNumber(this._constructionTimer.GetEndTimestamp()));
                }

                if (this._constructionTimer.GetFastForward() != -1)
                {
                    jsonObject.Put("con_ff", new LogicJSONNumber(this._constructionTimer.GetFastForward()));
                }
            }

            if (this._disarmed)
            {
                jsonObject.Put("needs_repair", new LogicJSONBoolean(true));
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
                int maxLvl = this.GetTrapData().GetUpgradeLevelCount();

                if (this._upgLevel >= maxLvl)
                {
                    Debugger.Warning(string.Format("LogicVillageObject::load() - Loaded upgrade level {0} is over max! (max = {1}) id {2} data id {3}",
                        lvlObject.GetIntValue(),
                        maxLvl,
                        this._globalId,
                        this._data.GetGlobalID()));
                    this._upgLevel = maxLvl - 1;
                }
                else
                {
                    if (this._upgLevel < -1)
                    {
                        Debugger.Error("LogicVillageObject::load() - Loaded an illegal upgrade level!");
                    }
                }
            }
            else
            {
                Debugger.Error("LogicVillageObject::load - Upgrade level was not found!");
            }

            this._level.GetWorkerManagerAt(this._villageType).DeallocateWorker(this);

            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            LogicJSONNumber constTimeObject = jsonObject.GetJSONNumber("const_t");

            if (constTimeObject != null)
            {
                int constTime = constTimeObject.GetIntValue();

                if (!LogicDataTables.GetGlobals().ClampBuildingTimes())
                {
                    if (this._upgLevel < this.GetTrapData().GetUpgradeLevelCount() - 1)
                    {
                        constTime = LogicMath.Min(constTime, this.GetTrapData().GetBuildTime(this._upgLevel + 1));
                    }
                }

                this._constructionTimer = new LogicTimer();
                this._constructionTimer.StartTimer(constTime, this._level.GetLogicTime(), false, -1);

                LogicJSONNumber constTimeEndObject = jsonObject.GetJSONNumber("const_t_end");

                if (constTimeEndObject != null)
                {
                    this._constructionTimer.SetEndTimestamp(constTimeEndObject.GetIntValue());
                }

                LogicJSONNumber conffObject = jsonObject.GetJSONNumber("con_ff");

                if (conffObject != null)
                {
                    this._constructionTimer.SetFastForward(conffObject.GetIntValue());
                }

                this._level.GetWorkerManagerAt(this._villageType).AllocateWorker(this);
                this._upgrading = this._upgLevel != -1;
            }

            LogicJSONBoolean disarmed = jsonObject.GetJSONBoolean("needs_repair");

            if (disarmed != null)
            {
                this._disarmed = disarmed.IsTrue();
            }

            base.Load(jsonObject);
        }

        /// <summary>
        ///     Called when the loading of this <see cref="LogicObstacle"/> instance is finished.
        /// </summary>
        public override void LoadingFinished()
        {
            base.LoadingFinished();

            if (LogicDataTables.GetGlobals().ClampBuildingTimes())
            {
                if (this._constructionTimer != null)
                {
                    if (this._upgLevel < this.GetTrapData().GetUpgradeLevelCount() - 1)
                    {
                        int remainingSecs = this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime());
                        int totalSecs = this.GetTrapData().GetBuildTime(this._upgLevel + 1);

                        if (remainingSecs > totalSecs)
                        {
                            this._constructionTimer.StartTimer(totalSecs, this._level.GetLogicTime(), true, this._level.GetGameMode().GetCurrentTimestamp());
                        }
                    }
                }
            }

            if (this._listener != null)
            {
                this._listener.LoadedFromJSON();
            }
        }

        /// <summary>
        ///     Gets the <see cref="LogicGameObject"/> type.
        /// </summary>
        public override int GetGameObjectType()
        {
            return 4;
        }

        /// <summary>
        ///     Gets the width of gameobject in tiles.
        /// </summary>
        public override int GetWidthInTiles()
        {
            return this.GetTrapData().GetWidth();
        }

        /// <summary>
        ///     Gets the height of gameobject in tiles.
        /// </summary>
        public override int GetHeightInTiles()
        {
            return this.GetTrapData().GetHeight();
        }

        /// <summary>
        ///     Gets the upgrade level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgLevel;
        }

        /// <summary>
        ///     Gets if the village object is constructing.
        /// </summary>
        public bool IsConstructing()
        {
            return this._constructionTimer != null;
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
        ///     Gets the remaining construction time in ms.
        /// </summary>
        public int GetRemainingConstructionTimeMS()
        {
            if (this._constructionTimer != null)
            {
                return this._constructionTimer.GetRemainingMS(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Starts the upgrading of the trap.
        /// </summary>
        public void StartUpgrading()
        {
            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            LogicTrapData data = this.GetTrapData();

            this._upgrading = true;
            int buildTime = data.GetBuildTime(this._upgLevel + 1);

            if (buildTime <= 0)
            {
                this.FinishConstruction(false);
            }
            else
            {
                this._level.GetWorkerManagerAt(data.GetVillageType()).AllocateWorker(this);

                this._constructionTimer = new LogicTimer();
                this._constructionTimer.StartTimer(buildTime, this._level.GetLogicTime(), true, this._level.GetGameMode().GetCurrentTimestamp());
            }
        }

        /// <summary>
        ///     Cancels the construction of trap.
        /// </summary>
        public void CancelConstruction()
        {
            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

            if (homeOwnerAvatar != null && homeOwnerAvatar.IsClientAvatar())
            {
                if (this._constructionTimer != null)
                {
                    this._constructionTimer.Destruct();
                    this._constructionTimer = null;

                    int upgLevel = this._upgLevel;

                    if (this._upgrading)
                    {
                        this.SetUpgradeLevel(this._upgLevel);
                        upgLevel += 1;
                    }

                    LogicTrapData data = this.GetTrapData();
                    LogicResourceData buildResourceData = data.GetBuildResource();
                    Int32 buildCost = data.GetBuildCost(upgLevel);
                    Int32 refundedCount = LogicMath.Max(LogicDataTables.GetGlobals().GetBuildCancelMultiplier() * buildCost / 100, 0);
                    Debugger.Print("LogicTrap::cancelConstruction refunds: " + refundedCount);

                    homeOwnerAvatar.CommodityCountChangeHelper(0, buildResourceData, refundedCount);

                    this._level.GetWorkerManagerAt(this._data.GetVillageType()).DeallocateWorker(this);

                    if (upgLevel != 0)
                    {
                        if (this._listener != null)
                        {
                            this._listener.RefreshState();
                        }
                    }
                    else
                    {
                        this.GetGameObjectManager().RemoveGameObject(this);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the required townhall level for upgrade.
        /// </summary>
        public int GetRequiredTownHallLevelForUpgrade()
        {
            return this.GetTrapData().GetRequiredTownHallLevel(LogicMath.Min(this._upgLevel + 1, this.GetTrapData().GetUpgradeLevelCount()));
        }

        /// <summary>
        ///     Gets if this <see cref="LogicTrap"/> can be upgraded.
        /// </summary>
        public bool CanUpgrade(bool canCallListener)
        {
            if (this._constructionTimer == null && !this.IsMaxUpgradeLevel())
            {
                if (this._level.GetTownHallLevel(this._villageType) >= this.GetRequiredTownHallLevelForUpgrade())
                {
                    return true;
                }

                if (canCallListener)
                {
                    this._level.GetGameListener().TownHallLevelTooLow(this.GetRequiredTownHallLevelForUpgrade());
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicTrap"/> is maxed.
        /// </summary>
        public bool IsMaxUpgradeLevel()
        {
            LogicTrapData trapData = this.GetTrapData();

            if (trapData.GetVillageType() != 1 || this.GetRequiredTownHallLevelForUpgrade() < this._level.GetGameMode().GetConfiguration().GetMaxTownHallLevel() - 1)
            {
                return this._upgLevel >= trapData.GetUpgradeLevelCount() - 1;
            }

            return true;
        }

        /// <summary>
        ///     Speed up the trap construction.
        /// </summary>
        public bool SpeedUpConstruction()
        {
            if (this._constructionTimer != null)
            {
                int remainingSecs = this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime());
                int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(remainingSecs, 0, this._villageType);
                
                if (this._level.GetPlayerAvatar().HasEnoughDiamonds(speedUpCost, true, this._level))
                {
                    Debugger.Print("LogicTrap::speedUpConstruction speedup cost: " + speedUpCost);

                    this._level.GetPlayerAvatar().UseDiamonds(speedUpCost);
                    this.FinishConstruction(false);

                    return true;
                }
            }

            return false;
        }
    }
}