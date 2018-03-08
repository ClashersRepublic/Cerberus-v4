namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicBuilding : LogicGameObject
    {
        private int _upgLevel;
        private bool _locked;
        private bool _gearing;
        private bool _upgrading;

        private LogicTimer _constructionTimer;

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
            return this._locked;
        }

        /// <summary>
        ///     Gets the upgrade level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgLevel;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
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

                }
            }
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
                        this._locked = false;

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
                    if (!this._locked)
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

                    if (this._locked)
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