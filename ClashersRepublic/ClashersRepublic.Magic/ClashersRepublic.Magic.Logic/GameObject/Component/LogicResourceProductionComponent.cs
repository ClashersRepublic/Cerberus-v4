namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using System;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Json;

    public sealed class LogicResourceProductionComponent : LogicComponent
    {
        private readonly LogicResourceData _resourceData;
        private readonly LogicTimer _resourceTimer;

        private int _availableLoot;
        private int _maxResources;
        private int _productionPer100Hour;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicResourceProductionComponent" /> class.
        /// </summary>
        public LogicResourceProductionComponent(LogicGameObject gameObject, LogicResourceData data) : base(gameObject)
        {
            this._resourceTimer = new LogicTimer();
            this._resourceData = data;
        }

        /// <summary>
        ///     Gets the component type of this instance.
        /// </summary>
        public override int GetComponentType()
        {
            return 5;
        }

        /// <summary>
        ///     Gets the resource data.
        /// </summary>
        public LogicResourceData GetResourceData()
        {
            return this._resourceData;
        }

        /// <summary>
        ///     Restarts the timer.
        /// </summary>
        public void RestartTimer()
        {
            int totalTime = 0;

            if (this._productionPer100Hour >= 1)
            {
                totalTime = (int) (360000L * this._maxResources / this._productionPer100Hour);
            }

            this._resourceTimer.StartTimer(totalTime, this._parent.GetLevel().GetLogicTime(), false, -1);
        }

        /// <summary>
        ///     Sets the production.
        /// </summary>
        public void SetProduction(int productionPer100Hour, int maxResources)
        {
            this._productionPer100Hour = productionPer100Hour;
            this._maxResources = maxResources;

            this.RestartTimer();
        }

        /// <summary>
        ///     Gets the stealable resource count.
        /// </summary>
        public int GetStealableResourceCount()
        {
            return LogicMath.Min(this.GetResourceCount(), this._maxResources);
        }

        /// <summary>
        ///     Gets the resource count.
        /// </summary>
        public int GetResourceCount()
        {
            if (this._productionPer100Hour != 0)
            {
                int totalTime = (int) (360000L * this._maxResources / this._productionPer100Hour);

                if (totalTime != 0)
                {
                    int remainingTime = this._resourceTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime());

                    if (remainingTime != 0)
                    {
                        long productionPer100Hour = this._productionPer100Hour;
                        long maxTime = 0;

                        if (this._productionPer100Hour > 0)
                        {
                            maxTime = 360000L * this._maxResources / this._productionPer100Hour;
                        }

                        return (int) (productionPer100Hour * ((int) maxTime - remainingTime) / 360000L);
                    }

                    return this._maxResources;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Decreases the number of specified resources.
        /// </summary>
        public void DecreaseResources(int count)
        {
            int resourceCount = this.GetResourceCount();
            int removeCount = LogicMath.Min(count, resourceCount);

            if (this._productionPer100Hour != 0)
            {
                int totalTime = (int) (360000L * this._maxResources / this._productionPer100Hour);
                int skipTime = (int) (360000L * (resourceCount - removeCount) / this._productionPer100Hour);

                this._resourceTimer.StartTimer(totalTime - skipTime, this._parent.GetLevel().GetLogicTime(), false, -1);
            }
        }

        /// <summary>
        ///     Collects available resources.
        /// </summary>
        public int CollectResources(bool updateListener)
        {
            if (this._parent.GetLevel().GetHomeOwnerAvatar() != null)
            {
                int resourceCount = this.GetResourceCount();

                if (this._parent.GetLevel().GetHomeOwnerAvatar().IsNpcAvatar())
                {
                    Debugger.Error("LogicResourceProductionComponent::collectResources() called for Npc avatar");
                }
                else
                {
                    LogicClientAvatar clientAvatar = (LogicClientAvatar) this._parent.GetLevel().GetHomeOwnerAvatar();

                    if (resourceCount != 0)
                    {
                        if (this._resourceData.PremiumCurrency)
                        {
                            this.DecreaseResources(resourceCount);

                            clientAvatar.SetDiamonds(clientAvatar.GetDiamonds() + resourceCount);
                            clientAvatar.SetFreeDiamonds(clientAvatar.GetFreeDiamonds() + resourceCount);
                        }
                        else
                        {
                            int unusedResourceCap = clientAvatar.GetUnusedResourceCap(this._resourceData);

                            if (unusedResourceCap != 0)
                            {
                                if (resourceCount > unusedResourceCap)
                                {
                                    resourceCount = unusedResourceCap;
                                }

                                this.DecreaseResources(resourceCount);

                                clientAvatar.CommodityCountChangeHelper(0, this._resourceData, resourceCount);
                            }
                            else
                            {
                                resourceCount = 0;
                            }
                        }

                        return resourceCount;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
            int remainingSeconds = this._resourceTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime());
            int boostTime = this._parent.GetRemainingBoostTime();

            if (boostTime > 0 && LogicDataTables.GetGlobals().GetResourceProductionBoostMultiplier() > 1 && !this._parent.GetBoostPaused())
            {
                time += (LogicDataTables.GetGlobals().GetResourceProductionBoostMultiplier() - 1) * LogicMath.Min(time, boostTime);
            }

            int clockBoostTime = this._parent.GetLevel().GetRemainingClockTowerBoostTime();

            if (clockBoostTime > 0 && !this._parent.GetLevel().IsClockTowerBoostPaused())
            {
                if (this._parent.GetData().GetVillageType() == 1)
                {
                    time += (LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 1) * LogicMath.Min(time, clockBoostTime);
                }
            }

            this._resourceTimer.StartTimer(LogicMath.Max(remainingSeconds - time, 0), this._parent.GetLevel().GetLogicTime(), false, -1);
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public override void GetChecksum(ChecksumHelper checksum)
        {
            checksum.StartObject("LogicResourceProductionComponent");
            checksum.WriteValue("resourceTimer", this._resourceTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime()));
            checksum.WriteValue("m_availableLoot", this._availableLoot);
            checksum.WriteValue("m_maxResources", this._maxResources);
            checksum.WriteValue("m_productionPer100Hour", this._productionPer100Hour);
            checksum.EndObject();
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber resourceTimeObject = jsonObject.GetJSONNumber("res_time");
            Int32 time = this._productionPer100Hour > 0 ? (int)(360000L * this._maxResources / this._productionPer100Hour) : 0;

            if (resourceTimeObject != null)
            {
                time = LogicMath.Min(resourceTimeObject.GetIntValue(), time);
            }

            this._resourceTimer.StartTimer(time, this._parent.GetLevel().GetLogicTime(), false, -1);
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
           jsonObject.Put("res_time", new LogicJSONNumber(this._resourceTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime())));
        }

        /// <summary>
        ///     Recalculates the available loot.
        /// </summary>
        public void RecalculateAvailableLoot()
        {
            LogicAvatar homeOwnerAvatar = this._parent.GetLevel().GetHomeOwnerAvatar();

            if (!homeOwnerAvatar.IsNpcAvatar())
            {
                int matchType = this._parent.GetLevel().GetMatchType();

                if (matchType < 10)
                {
                    if (matchType == 0 ||
                        matchType == 2 ||
                        matchType >= 4 && matchType <= 6)
                    {
                        int resourceProductionLootPercentage = LogicDataTables.GetGlobals().GetResourceProductionLootPercentage(this._resourceData);

                        if (homeOwnerAvatar.IsClientAvatar())
                        {
                            if (this._parent.GetLevel().GetVisitorAvatar() != null)
                            {
                                LogicAvatar visitorAvatar = this._parent.GetLevel().GetVisitorAvatar();

                                if (visitorAvatar.IsClientAvatar())
                                {
                                    resourceProductionLootPercentage = resourceProductionLootPercentage *
                                                                       LogicDataTables.GetGlobals().GetLootMultiplierByTownHallDiff(visitorAvatar.GetTownHallLevel(),
                                                                                                                                    homeOwnerAvatar.GetTownHallLevel()) / 100;
                                }
                            }
                        }

                        if (resourceProductionLootPercentage > 100)
                        {
                            resourceProductionLootPercentage = 100;
                        }

                        this._availableLoot = this.GetResourceCount() * resourceProductionLootPercentage / 100;
                    }
                    else
                    {
                        this._availableLoot = 0;
                    }
                }
                else
                {
                    this._availableLoot = 0;
                }
            }
            else
            {
                this._availableLoot = 0;
            }
        }
    }
}