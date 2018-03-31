namespace ClashersRepublic.Magic.Logic.Battle
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicBattleLog
    {
        private LogicLevel _level;
        private LogicLong _attackerHomeId;
        private LogicLong _defenserHomeId;
        private LogicLong _attackerAllianceId;
        private LogicLong _defenserAllianceId;

        private int _attackerAllianceBadgeId;
        private int _defenserAllianceBadgeId;
        private int _attackerStars;
        private int _villageType;

        private string _attackerAllianceName;
        private string _defenserAllianceName;
        private string _attackerName;
        private string _defenderName;

        private bool _battleEnded;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBattleLog" /> class.
        /// </summary>
        public LogicBattleLog(LogicLevel level)
        {
            this._level = level;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._level = null;
            this._attackerAllianceName = null;
            this._defenserAllianceName = null;
            this._attackerName = null;
            this._defenderName = null;
            this._attackerHomeId = null;
            this._defenserHomeId = null;
            this._attackerAllianceId = null;
            this._defenserAllianceId = null;
        }

        /// <summary>
        ///     Gets the village type.
        /// </summary>
        public int GetVillageType()
        {
            return this._villageType;
        }

        /// <summary>
        ///     Sets the village type.
        /// </summary>
        public void SetVillageType(int value)
        {
            this._villageType = value;
        }

        /// <summary>
        ///     Gets a value indicating whether this battle is started.
        /// </summary>
        public bool GetBattleStarted()
        {
            return false;
        }

        /// <summary>
        ///     Gets a value indicating whether this battle is ended.
        /// </summary>
        public bool GetBattleEnded()
        {
            return this._battleEnded;
        }

        /// <summary>
        ///     Sets the attacker home id.
        /// </summary>
        public void SetAttackerHomeId(LogicLong homeId)
        {
            this._attackerHomeId = homeId;
        }

        /// <summary>
        ///     Sets the defenser home id.
        /// </summary>
        public void SetDefenserHomeId(LogicLong homeId)
        {
            this._defenserHomeId = homeId;
        }

        /// <summary>
        ///     Sets the attacker alliance id.
        /// </summary>
        public void SetAttackerAllianceId(LogicLong allianceId)
        {
            this._attackerAllianceId = allianceId;
        }

        /// <summary>
        ///     Sets the defenser alliance id.
        /// </summary>
        public void SetDefenserAllianceId(LogicLong allianceId)
        {
            this._defenserAllianceId = allianceId;
        }

        /// <summary>
        ///     Sets the attacker alliance badge.
        /// </summary>
        public void SetAttackerAllianceBadge(int badgeId)
        {
            this._attackerAllianceBadgeId = badgeId;
        }

        /// <summary>
        ///     Sets the defenser alliance badge.
        /// </summary>
        public void SetDefenserAllianceBadge(int badgeId)
        {
            this._defenserHomeId = badgeId;
        }

        /// <summary>
        ///     Sets the attacker alliance name.
        /// </summary>
        public void SetAttackerAllianceName(string name)
        {
            this._attackerAllianceName = name;
        }

        /// <summary>
        ///     Sets the defenser alliance name.
        /// </summary>
        public void SetDefenserAllianceName(string name)
        {
            this._defenserAllianceName = name;
        }

        /// <summary>
        ///     Sets the attacker stars.
        /// </summary>
        public void SetAttackerStars(int star)
        {
            this._attackerStars = star;
        }

        /// <summary>
        ///     Sets the attacker name.
        /// </summary>
        public void SetAttackerName(string name)
        {
            this._attackerName = name;
        }

        /// <summary>
        ///     Sets the defenser name.
        /// </summary>
        public void SetDefenserName(string name)
        {
            this._defenderName = name;
        }

        /// <summary>
        ///     Calculates the number of available resources.
        /// </summary>
        public void CalculateAvailableResources(LogicLevel level, int matchType)
        {
            // TODO: Implement LogicBattleLog::calculateAvailableResources.
        }
    }
}