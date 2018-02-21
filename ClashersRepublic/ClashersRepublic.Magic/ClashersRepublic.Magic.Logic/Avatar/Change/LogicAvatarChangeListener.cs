﻿namespace ClashersRepublic.Magic.Logic.Avatar.Change
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicAvatarChangeListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAvatarChangeListener"/> class.
        /// </summary>
        public LogicAvatarChangeListener()
        {
            // LogicAvatarChangeListener.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        public virtual void FreeDiamondsAdded(int count) { }
        public virtual void DiamondPurchaseMade(int count) { }
        public virtual void CommodityCountChanged(int commodityType, LogicData data, int count) { }
        public virtual void ExpPointsGained(int count) { }
        public virtual void ExpLevelGained(int count) { }
        public virtual void AllianceJoined(LogicLong allianceId, string allianceName, int allianceBadgeId, int allianceExpLevel) { }
        public virtual void AllianceLeft() { }
        public virtual void SetAllianceCastleLevel(int count) { }
        public virtual void SetTownHallLevel(int count) { }
    }
}