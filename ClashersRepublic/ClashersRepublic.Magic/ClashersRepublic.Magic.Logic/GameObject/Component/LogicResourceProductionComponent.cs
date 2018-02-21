namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

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
        ///     Restarts the timer.
        /// </summary>
        public void RestartTimer()
        {
            int totalTime = 0;

            if (this._productionPer100Hour != 0)
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
                        return (((this._productionPer100Hour >> 31) << 32) | this._productionPer100Hour) * (totalTime - remainingTime) / 360000;
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
        public void CollectResources(bool updateListener)
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
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
            int remainingSeconds = this._resourceTimer.GetRemainingSeconds(this._parent.GetLevel().GetLogicTime());
        }
    }
}