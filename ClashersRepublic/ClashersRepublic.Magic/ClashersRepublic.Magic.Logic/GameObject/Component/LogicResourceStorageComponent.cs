namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public sealed class LogicResourceStorageComponent : LogicComponent
    {
        private LogicArrayList<int> _resourceCount;
        private LogicArrayList<int> _maxResourceCount;
        private LogicArrayList<int> _maxPercentageResourceCount;
        private LogicArrayList<int> _availableLootResourceCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicResourceStorageComponent" /> class.
        /// </summary>
        public LogicResourceStorageComponent(LogicGameObject gameObject) : base(gameObject)
        {
            int resourceDataCount = LogicDataTables.GetTable(2).GetItemCount();

            this._maxResourceCount = new LogicArrayList<int>(resourceDataCount);
            this._availableLootResourceCount = new LogicArrayList<int>(resourceDataCount);
            this._resourceCount = new LogicArrayList<int>(resourceDataCount);
            this._maxPercentageResourceCount = new LogicArrayList<int>(resourceDataCount);

            for (int i = 0; i < resourceDataCount; i++)
            {
                this._resourceCount.Add(0);
                this._maxResourceCount.Add(0);
                this._availableLootResourceCount.Add(0);

                this._maxPercentageResourceCount.Add(100);
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._maxResourceCount = null;
            this._availableLootResourceCount = null;
            this._resourceCount = null;
        }

        /// <summary>
        ///     Gets the number of stealable resources.
        /// </summary>
        public int GetStealableResourceCount(int idx)
        {
            return LogicMath.Min(this._resourceCount[idx], this._availableLootResourceCount[idx]);
        }

        /// <summary>
        ///     Called when the gameobject has received damages.
        /// </summary>
        public void ResourcesStolen(int damage, int health)
        {
            // TODO: Implement LogicResourceStorageComponent::resourceStolen(damage,health).
        }

        /// <summary>
        ///     Sets the resource count.
        /// </summary>
        public void SetCount(int idx, int count)
        {
            this._resourceCount[idx] = LogicMath.Clamp(count, 0, this.GetMax(idx));

            if (this._parent.GetListener() != null)
            {
                this._parent.GetListener().RefreshResourceCount();
            }
        }

        /// <summary>
        ///     Gets the max of resources.
        /// </summary>
        public int GetMax(int idx)
        {
            return this._maxResourceCount[idx];
        }

        /// <summary>
        ///     Gets the resource count.
        /// </summary>
        public int GetResourceCount(int idx)
        {
            return this._resourceCount[idx];
        }

        /// <summary>
        ///     Gets the recommended resource max.
        /// </summary>
        public int GetRecommendedMax(int idx)
        {
            int max = this.GetMax(idx);

            if (this._parent.GetHitpointComponent() != null)
            {
                LogicHitpointComponent hitpointComponent = this._parent.GetHitpointComponent();

                if (hitpointComponent.InternalGetHp() <= 10000)
                {
                    if (hitpointComponent.InternalGetHp() <= 1000)
                    {
                        return max * hitpointComponent.InternalGetHp() / hitpointComponent.InternalGetMaxHp();
                    }

                    return 100 * (max * (hitpointComponent.InternalGetHp() / 100) / hitpointComponent.InternalGetMaxHp());
                }

                return 1000 * (max * (hitpointComponent.InternalGetHp() / 1000) / hitpointComponent.InternalGetMaxHp());
            }

            return max;
        }

        /// <summary>
        ///     Sets the max array.
        /// </summary>
        public void SetMaxArray(LogicArrayList<int> max)
        {
            for (int i = 0; i < max.Count; i++)
            {
                this._maxResourceCount[i] = max[i];
            }

            this._parent.GetLevel().RefreshResourceCaps();
        }

        /// <summary>
        ///     Sets the max percentage array.
        /// </summary>
        public void SetMaxPercentageArray(LogicArrayList<int> max)
        {
            for (int i = 0; i < max.Count; i++)
            {
                this._maxPercentageResourceCount[i] = max[i];
            }
        }

        /// <summary>
        ///     Gets the component type of this instance.
        /// </summary>
        public override int GetComponentType()
        {
            return 6;
        }
    }
}