namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicBuilding : LogicGameObject
    {
        private bool _isUpgrading;
        private bool _isConstructing;
        private bool _isLocked;
        private bool _isHidden;

        private int _upgradeLevel;
        private int _direction;

        private LogicTimer _boostTimer;
        private LogicTimer _constructionTimer;
        private LogicBuildingData _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuilding"/> class.
        /// </summary>
        public LogicBuilding(LogicData data, LogicLevel level) : base(data, level)
        {
            this._level = level;
            this._data = (LogicBuildingData) data;

            LogicHitpointComponent hitpointComponent = new LogicHitpointComponent(this, this._data.GetHitpoints(0), 1);
            hitpointComponent.SetMaxRegenerationTime(this._data.GetRegenerationTime(0));
            this.AddComponent(hitpointComponent);
        }

        /// <summary>
        ///     Gets the building data instance.
        /// </summary>
        public LogicBuildingData GetBuildingData()
        {
            return this._data;
        }

        /// <summary>
        ///     Gets a value indicating whether the building is upgrading.
        /// </summary>
        public bool IsUpgrading()
        {
            return this._isConstructing && this._isUpgrading;
        }

        /// <summary>
        ///     Gets a value indicating whether the building is constructing.
        /// </summary>
        public bool IsConstructing()
        {
            return this._isConstructing;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber upgradeLevelObject = jsonObject.GetJSONNumber("lvl");
            LogicJSONNumber constructionTimeObject = jsonObject.GetJSONNumber("const_t");
            LogicJSONBoolean lockedObject = jsonObject.GetJSONBoolean("locked");
            LogicJSONNumber boostTimeObject = jsonObject.GetJSONNumber("boost_t");
            
            if (upgradeLevelObject != null)
            {
                this._upgradeLevel = upgradeLevelObject.GetIntValue();

                if (this._upgradeLevel <= this._data.GetUpgradeLevelCount() - 1)
                {
                    if (this._upgradeLevel < -1)
                    {
                        Debugger.Error("LogicBuilding::load() - Loaded an illegal upgrade level!");
                    }
                }
                else
                {
                    Debugger.Warning("LogicBuilding::load() - Loaded upgrade level " + this._upgradeLevel + " is over max! (max = " + this._data.GetUpgradeLevelCount() + ")");
                    this._upgradeLevel = this._data.GetUpgradeLevelCount();
                }
            }
            else
            {
                Debugger.Error("LogicBuilding::load - Upgrade level was not found!");
                this._upgradeLevel = 0;
            }

            this._level.GetWorkerManagerAt(this._villageType).DeallocateWorker(this);
            this._isUpgrading = false;

            if (constructionTimeObject != null)
            {
                if (this._constructionTimer == null)
                {
                    this._constructionTimer = new LogicTimer();
                }

                this._constructionTimer.StartTimer(LogicMath.Min(constructionTimeObject.GetIntValue(), this._data.GetConstructionTime(this._upgradeLevel + 1)), this._level.GetLogicTime());
                this._level.GetWorkerManagerAt(this._villageType).AllocateWorker(this);
                this._isUpgrading = this._upgradeLevel != -1;
            }
            else
            {
                this._constructionTimer = null;
            }

            this._isLocked = lockedObject != null && lockedObject.IsTrue();
            
            if (boostTimeObject != null)
            {
                if (this._boostTimer == null)
                {
                    this._boostTimer = new LogicTimer();
                }

                this._boostTimer.StartTimer(boostTimeObject.GetIntValue(), this._level.GetLogicTime());
            }

            this.SetUpgradeLevel(LogicMath.Clamp(this._upgradeLevel, 0, this._data.GetUpgradeLevelCount() - 1));

            base.Load(jsonObject);
        }

        /// <summary>
        ///     Sets the upgrade level.
        /// </summary>
        public void SetUpgradeLevel(int upgradeLevel)
        {
            this._upgradeLevel = LogicMath.Clamp(upgradeLevel, 0, this._data.GetUpgradeLevelCount() - 1);

            if (this._level.GetHomeOwnerAvatar() != null)
            {
                if (this._data.IsAllianceCastle())
                {
                    if (!this._isLocked)
                    {
                        this._level.GetHomeOwnerAvatar().SetAllianceCastleLevel(this._upgradeLevel);
                    } 
                }
                else
                {
                    if (this._data.IsTownHall())
                    {
                        this._level.GetHomeOwnerAvatar().SetTownHallLevel(this._upgradeLevel);
                    }
                    else if (this._data.IsTownHall2())
                    {
                        this._level.GetHomeOwnerAvatar().SetTownHallVillage2Level(this._upgradeLevel);
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the building is at the max upgrade level.
        /// </summary>
        public bool IsMaxUpgradeLevel()
        {
            return this._upgradeLevel >= this._data.GetUpgradeLevelCount() - 1;
        }

        /// <summary>
        ///     Gets a value indicating whether the building is hidden.
        /// </summary>
        public bool IsHidden()
        {
            return this._isHidden;
        }

        /// <summary>
        ///     Gets the direction of building.
        /// </summary>
        public int GetDirection()
        {
            return this._direction / 1000;
        }

        /// <summary>
        ///     Gets the required town hall level for upgrade.
        /// </summary>
        public int GetRequiredTownHallLevelForUpgrade()
        {
            return this._data.GetRequiredTownHallLevel(LogicMath.Min(this._upgradeLevel + 1, this._data.GetUpgradeLevelCount() - 1));
        }

        /// <summary>
        ///     Gets the boost cost.
        /// </summary>
        public int GetBoostCost()
        {
            return this._data.GetBoostCost(this._upgradeLevel);
        }

        /// <summary>
        ///     Gets the remaining boost time.
        /// </summary>
        public int GetRemainingBoostTime()
        {
            if (this._boostTimer != null)
            {
                return this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Updates the hidden state.
        /// </summary>
        public void UpdateHidden()
        {
            // TODO Implement LogicBuilding::updateHidden().
        }

        /// <summary>
        ///     Gets the upgrade level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgradeLevel;
        }

        /// <summary>
        ///     Gets the total construction time.
        /// </summary>
        public int GetTotalConstructionTime()
        {
            if (!this._isConstructing || !this._isUpgrading)
            {
                return this._data.GetConstructionTime(this._upgradeLevel);
            }
            
            return this._data.GetConstructionTime(this._upgradeLevel + 1);
        }

        /// <summary>
        ///     Gets the sell resource.
        /// </summary>
        public LogicResourceData GetSellResource()
        {
            return this._data.GetBuildResource(this._upgradeLevel);
        }

        /// <summary>
        ///     Gets the type of gameobject.
        /// </summary>
        public override int GetGameObjectType()
        {
            return 0;
        }

        /// <summary>
        ///     Gets a value indicating whether the building can be sell.
        /// </summary>
        public override bool CanSell()
        {
            return false;
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public override void GetChecksum(ChecksumHelper checksum)
        {
            checksum.StartObject("LogicBuilding");
            base.GetChecksum(checksum);
            checksum.EndObject();
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (this._constructionTimer != null)
            {
                if (this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                {
                    // TODO Implement LogicBuilding::finishConstruction().
                }
            }

            if (this._boostTimer != null)
            {
                if (this._boostTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                {
                    this._boostTimer = null;
                }
            }
            
            this.GetHitpointComponent().EnableRegeneration(this._level.GetState() == 1);

            if (this._level.IsInCombatState())
            {
                if (this._isHidden)
                {
                    this.UpdateHidden();
                }
            }
        }

        /// <summary>
        ///     Gets the heigh in tiles of building.
        /// </summary>
        public override int GetHeighInTiles()
        {
            return this._data.GetHeight();
        }

        /// <summary>
        ///     Gets the width in tiles of building.
        /// </summary>
        public override int GetWidthInTiles()
        {
            return this._data.GetWidth();
        }
    }
}