namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicObstacle : LogicGameObject
    {
        private LogicTimer _clearTimer;
        private int _lootMultiplyVersion;
        private int _fadeTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicObstacle" /> class.
        /// </summary>
        public LogicObstacle(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicObstacle.
        }

        /// <summary>
        ///     Gets the <see cref="LogicObstacleData"/> instance.
        /// </summary>
        public LogicObstacleData GetObstacleData()
        {
            return (LogicObstacleData) this._data;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._clearTimer != null)
            {
                this._clearTimer.Destruct();
                this._clearTimer = null;
            }

            this._fadeTime = 0;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacleData"/> is passable.
        /// </summary>
        public override bool IsPassable()
        {
            return this.GetObstacleData().Passable;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
            base.FastForwardTime(time);

            if (this._clearTimer != null)
            {
                int remainingSeconds = this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime());

                if (remainingSeconds <= time)
                {
                    if (LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
                    {
                        this._clearTimer.StartTimer(0, this._level.GetLogicTime(), false, -1);
                    }
                    else
                    {
                        this.ClearingFinished(true);
                    }
                }
                else
                {
                    this._clearTimer.StartTimer(remainingSeconds - time, this._level.GetLogicTime(), false, -1);
                }
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (this._clearTimer != null)
            {
                if (this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime()) > 0 && this._level.GetRemainingClockTowerBoostTime() > 0 && this.GetObstacleData().GetVillageType() == 1)
                {
                    this._clearTimer.SetFastForward(this._clearTimer.GetFastForward() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
                }
            }

            if (this._fadeTime < 1)
            {
                if (this._clearTimer != null)
                {
                    if (this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                    {
                        this.ClearingFinished(false);
                    }
                }
            }
            else
            {
                int tmp = 2000;

                if (this.GetObstacleData().LootDefensePercentage > 1)
                {
                    tmp = 4000;
                }

                if (this.GetObstacleData().TallGrass)
                {
                    tmp = 1000;
                }

                this._fadeTime = LogicMath.Min(this._fadeTime + 64, tmp);
            }
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> instance should be destroy.
        /// </summary>
        public override bool ShouldDestruct()
        {
            int tmp = 2000;

            if (this.GetObstacleData().LootDefensePercentage > 1)
            {
                tmp = 4000;
            }

            if (this.GetObstacleData().TallGrass)
            {
                tmp = 1000;
            }

            return this._fadeTime >= tmp;
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            base.Save(jsonObject);

            if (this._clearTimer != null)
            {
                jsonObject.Put("clear_t", new LogicJSONNumber(this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime())));
                jsonObject.Put("clear_ff", new LogicJSONNumber(this._clearTimer.GetFastForward()));
            }

            if (this._lootMultiplyVersion != 1)
            {
                jsonObject.Put("lmv", new LogicJSONNumber(this._lootMultiplyVersion));
            }
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            base.Load(jsonObject);

            LogicJSONNumber clearTimeObject = jsonObject.GetJSONNumber("clear_t");

            if (clearTimeObject != null)
            {
                if (this._clearTimer != null)
                {
                    this._clearTimer.Destruct();
                    this._clearTimer = null;
                }

                this._clearTimer = new LogicTimer();
                this._clearTimer.StartTimer(clearTimeObject.GetIntValue(), this._level.GetLogicTime(), false, -1);
                this._level.GetWorkerManagerAt(this._villageType).AllocateWorker(this);
            }

            LogicJSONNumber clearFastForwardObject = jsonObject.GetJSONNumber("clear_ff");

            if (clearFastForwardObject != null)
            {
                if (this._clearTimer != null)
                {
                    this._clearTimer.SetFastForward(clearFastForwardObject.GetIntValue());
                }
            }

            LogicJSONNumber lootMultiplyVersionObject = jsonObject.GetJSONNumber("loot_multiply_ver");

            if (lootMultiplyVersionObject == null)
            {
                lootMultiplyVersionObject = jsonObject.GetJSONNumber("lmv");

                if (lootMultiplyVersionObject == null)
                {
                    this._lootMultiplyVersion = 1;
                    return;
                }
            }

            this._lootMultiplyVersion = lootMultiplyVersionObject.GetIntValue();
        }

        /// <summary>
        ///     Called when the loading of this <see cref="LogicObstacle"/> instance is finished.
        /// </summary>
        public override void LoadingFinished()
        {
            base.LoadingFinished();

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
            return 3;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> is unbuildable.
        /// </summary>
        public override bool IsUnbuildable()
        {
            return this.GetObstacleData().IsTombstone;
        }

        /// <summary>
        ///     Gets the width in tiles.
        /// </summary>
        public override int GetWidthInTiles()
        {
            return this.GetObstacleData().Width;
        }

        /// <summary>
        ///     Gets the height in tiles.
        /// </summary>
        public override int GetHeightInTiles()
        {
            return this.GetObstacleData().Height;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> is a tombstone.
        /// </summary>
        public bool IsTombstone()
        {
            return this.GetObstacleData().IsTombstone;
        }

        /// <summary>
        ///     Gets the group of tomb.
        /// </summary>
        public int GetTombGroup()
        {
            return this.GetObstacleData().TombGroup;
        }

        /// <summary>
        ///     Gets the fade time.
        /// </summary>
        public int GetFadeTime()
        {
            return this._fadeTime;
        }

        /// <summary>
        ///     Gets the loot multiply version.
        /// </summary>
        public int GetLootMultiplyVersion()
        {
            return this._lootMultiplyVersion;
        }

        /// <summary>
        ///     Sets the loot multiply version.
        /// </summary>
        public void SetLootMultiplyVersion(int version)
        {
            this._lootMultiplyVersion = version;
        }

        /// <summary>
        ///     Gets the remaining clearing time.
        /// </summary>
        public int GetRemainingClearingTime()
        {
            if (this._clearTimer != null)
            {
                return this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the remaining clearing time in ms.
        /// </summary>
        public int GetRemainingClearingTimeMS()
        {
            if (this._clearTimer != null)
            {
                return this._clearTimer.GetRemainingMS(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> instance can start clearing.
        /// </summary>
        public bool CanStartClearing()
        {
            return this._clearTimer == null && this._fadeTime == 0;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> instance is clearing on going.
        /// </summary>
        public bool IsClearingOnGoing()
        {
            return this._clearTimer != null;
        }

        /// <summary>
        ///     Starts the clearing of obstacle.
        /// </summary>
        public void StartClearing()
        {
            if (this._clearTimer == null && this._fadeTime == 0)
            {
                if (this.GetObstacleData().GetClearTime() != 0)
                {
                    this._level.GetWorkerManagerAt(this._villageType).AllocateWorker(this);
                    this._clearTimer = new LogicTimer();
                    this._clearTimer.StartTimer(this.GetObstacleData().GetClearTime(), this._level.GetLogicTime(), false, -1);

                    if (this._listener != null)
                    {
                        // Listener.
                    }
                }
                else
                {
                    this.ClearingFinished(false);
                }
            }
        }

        /// <summary>
        ///     Called when the clearing of this <see cref="LogicObstacle"/> instance is finished.
        /// </summary>
        public void ClearingFinished(bool ignoreState)
        {
            int state = this._level.GetState();

            if (state == 1 || !LogicDataTables.GetGlobals().CompleteConstructionOnlyHome() && ignoreState)
            {
                if (this._level.GetHomeOwnerAvatar().IsClientAvatar())
                {
                    LogicClientAvatar homeOwnerAvatar = (LogicClientAvatar) this._level.GetHomeOwnerAvatar();
                    LogicObstacleData obstacleData = this.GetObstacleData();
                    LogicResourceData lootResourceData = obstacleData.GetLootResourceData();
                    int lootCount = obstacleData.GetLootCount();

                    if (obstacleData.IsLootCart())
                    {
                        LogicComponent component = this.GetComponent(14);
                        LogicDataTable resourceTable = LogicDataTables.GetTable(2);

                        if (component != null && resourceTable.GetItemCount() > 0)
                        {
                            for (int i = 0; i < resourceTable.GetItemCount(); i++)
                            {
                                // TODO: Implement LootCart.
                            }
                        }
                    }

                    if (!obstacleData.IsTombstone && !obstacleData.IsLootCart())
                    {
                        this._level.GetAchievementManager().ObstacleCleared();
                    }

                    this.XpGainHelper(LogicGamePlayUtil.TimeToExp(obstacleData.GetClearTime()), homeOwnerAvatar, ignoreState || state == 1);

                    if (lootResourceData != null && lootCount > 0)
                    {
                        if (homeOwnerAvatar != null)
                        {
                            if (lootResourceData.PremiumCurrency)
                            {
                                int lootMultipler = 1;

                                if (this._lootMultiplyVersion >= 2)
                                {
                                    lootMultipler = this._lootMultiplyVersion;
                                }

                                int diamondsCount = 0;

                                if (obstacleData.GetName().Equals("Bonus Gembox"))
                                {
                                    diamondsCount = lootCount * lootMultipler;
                                }
                                else
                                {
                                    diamondsCount = this._level.GetGameObjectManagerAt(this._villageType).IncreaseObstacleClearCounter(lootMultipler);
                                }

                                if (diamondsCount > 0)
                                {
                                    homeOwnerAvatar.SetDiamonds(homeOwnerAvatar.GetDiamonds() + diamondsCount);
                                    homeOwnerAvatar.SetFreeDiamonds(homeOwnerAvatar.GetFreeDiamonds() + diamondsCount);
                                    homeOwnerAvatar.GetChangeListener().FreeDiamondsAdded(diamondsCount);
                                }
                            }
                            else
                            {
                                int gainCount = LogicMath.Min(homeOwnerAvatar.GetUnusedResourceCap(lootResourceData), lootCount);

                                if (gainCount > 0)
                                {
                                    homeOwnerAvatar.CommodityCountChangeHelper(0, lootResourceData, gainCount);
                                }
                            }
                        }
                        else
                        {
                            Debugger.Error("LogicObstacle::clearingFinished - Home owner avatar is NULL!");
                        }
                    }

                    if (obstacleData.GetVillageType() == this._level.GetVillageType())
                    {
                        // ?
                    }

                    if (this._clearTimer != null)
                    {
                        this._clearTimer.Destruct();
                        this._clearTimer = null;
                    }

                    this._fadeTime = 1;
                }
            }
        }

        /// <summary>
        ///     Speeds up the clearing of the obstacle.
        /// </summary>
        public bool SpeedUpClearing()
        {
            if (this._clearTimer != null)
            {
                int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime()), 0, this._villageType);

                if (this._level.GetPlayerAvatar().HasEnoughDiamonds(speedUpCost, true, this._level))
                {
                    this._level.GetPlayerAvatar().UseDiamonds(speedUpCost);
                    this.ClearingFinished(false);
                    return true;
                }
            }

            return false;
        }
    }
}