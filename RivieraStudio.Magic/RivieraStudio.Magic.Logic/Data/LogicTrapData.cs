namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Math;

    public class LogicTrapData : LogicData
    {
        private bool _enableByCalendar;
        private bool _hasAltMode;
        private bool _ejectVictims;
        private bool _doNotScalePushByDamage;
        private bool _airTrigger;
        private bool _groundTrigger;
        private bool _healerTrigger;

        private int _upgradeLevelCount;
        private int _preferredTargetDamageMod;
        private int _minTriggerHousingLimit;
        private int _timeBetweenSpawnsMS;
        private int _spawnInitialDelayMS;
        private int _throwDistance;
        private int _triggerRadius;
        private int _directionCount;
        private int _actionFrame;
        private int _pushback;
        private int _speedMod;
        private int _damageMod;
        private int _durationMS;
        private int _hitDelayMS;
        private int _hitCount;

        private int[] _constructionTimes;
        private int[] _townHallLevel;
        private int[] _buildCost;
        private int[] _rearmCost;
        private int[] _strenghtWeight;
        private int[] _damage;
        private int[] _damageRadius;
        private int[] _ejectHousingLimit;
        private int[] _numSpawns;

        private int _width;
        private int _height;

        private LogicSpellData _spell;
        private LogicEffectData _effectData;
        private LogicEffectData _effect2Data;
        private LogicEffectData _effectBrokenData;
        private LogicEffectData _damageEffectData;
        private LogicEffectData _pickUpEffectData;
        private LogicEffectData _placingEffectData;
        private LogicEffectData _appearEffectData;
        private LogicEffectData _toggleAttackModeEffectData;
        private LogicCharacterData _preferredTargetData;
        private LogicCharacterData _spawnedCharGroundData;
        private LogicCharacterData _spawnedCharAirData;
        private LogicResourceData _buildResourceData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTrapData" /> class.
        /// </summary>
        public LogicTrapData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicTrapData.
        }
        
        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateVillageReferences();

            this._width = this.GetIntegerValue("Width", 0);
            this._height = this.GetIntegerValue("Height", 0);

            this._upgradeLevelCount = this._row.GetBiggestArraySize();

            this._buildCost = new int[this._upgradeLevelCount];
            this._rearmCost = new int[this._upgradeLevelCount];
            this._townHallLevel = new int[this._upgradeLevelCount];
            this._strenghtWeight = new int[this._upgradeLevelCount];
            this._damage = new int[this._upgradeLevelCount];
            this._damageRadius = new int[this._upgradeLevelCount];
            this._ejectHousingLimit = new int[this._upgradeLevelCount];
            this._numSpawns = new int[this._upgradeLevelCount];
            this._constructionTimes = new int[this._upgradeLevelCount];

            for (int i = 0; i < this._upgradeLevelCount; i++)
            {
                this._buildCost[i] = this.GetClampedIntegerValue("BuildCost", i);
                this._rearmCost[i] = this.GetClampedIntegerValue("RearmCost", i);
                this._townHallLevel[i] = LogicMath.Max(this.GetClampedIntegerValue("TownHallLevel", i) - 1, 0);
                this._strenghtWeight[i] = this.GetClampedIntegerValue("StrengthWeight", i);
                this._damage[i] = LogicMath.Min(this.GetClampedIntegerValue("Damage", i), 1000);
                this._damageRadius[i] = (this.GetClampedIntegerValue("DamageRadius", i) << 9) / 100;
                this._ejectHousingLimit[i] = this.GetIntegerValue("EjectHousingLimit", i);
                this._numSpawns[i] = this.GetClampedIntegerValue("NumSpawns", i);
                this._constructionTimes[i] = 86400 * this.GetIntegerValue("BuildTimeD", i) +
                                             3600 * this.GetIntegerValue("BuildTimeH", i) +
                                             60 * this.GetIntegerValue("BuildTimeM", i) +
                                             this.GetIntegerValue("BuildTimeS", i);
            }

            this._preferredTargetData = LogicDataTables.GetCharacterByName(this.GetValue("PreferredTarget", 0));
            this._preferredTargetDamageMod = this.GetIntegerValue("PreferredTargetDamageMod", 0);

            if (this._preferredTargetDamageMod == 0)
            {
                this._preferredTargetDamageMod = 100;
            }

            this._buildResourceData = LogicDataTables.GetResourceByName(this.GetValue("BuildResource", 0));

            if (this._buildResourceData == null)
            {
                Debugger.Error("build resource is not defined for trap: " + this.GetName());
            }

            this._ejectVictims = this.GetBooleanValue("EjectVictims", 0);
            this._actionFrame = 1000 * this.GetIntegerValue("ActionFrame", 0) / 24;
            this._pushback = this.GetIntegerValue("Pushback", 0);
            this._doNotScalePushByDamage = this.GetBooleanValue("DoNotScalePushByDamage", 0);
            this._effectData = LogicDataTables.GetEffectByName(this.GetValue("Effect", 0));
            this._effect2Data = LogicDataTables.GetEffectByName(this.GetValue("Effect2", 0));
            this._effectBrokenData = LogicDataTables.GetEffectByName(this.GetValue("EffectBroken", 0));
            this._damageEffectData = LogicDataTables.GetEffectByName(this.GetValue("DamageEffect", 0));
            this._pickUpEffectData = LogicDataTables.GetEffectByName(this.GetValue("PickUpEffect", 0));
            this._placingEffectData = LogicDataTables.GetEffectByName(this.GetValue("PlacingEffect", 0));
            this._appearEffectData = LogicDataTables.GetEffectByName(this.GetValue("AppearEffect", 0));
            this._toggleAttackModeEffectData = LogicDataTables.GetEffectByName(this.GetValue("ToggleAttackModeEffect", 0));
            this._triggerRadius = (this.GetIntegerValue("TriggerRadius", 0) << 9) / 100;
            this._directionCount = this.GetIntegerValue("DirectionCount", 0);
            this._spell = LogicDataTables.GetSpellByName(this.GetValue("Spell", 0));
            this._airTrigger = this.GetBooleanValue("AirTrigger", 0);
            this._groundTrigger = this.GetBooleanValue("GroundTrigger", 0);
            this._healerTrigger = this.GetBooleanValue("HealerTrigger", 0);
            this._speedMod = this.GetIntegerValue("SpeedMod", 0);
            this._damageMod = this.GetIntegerValue("DamageMod", 0);
            this._durationMS = this.GetIntegerValue("DurationMS", 0);
            this._hitDelayMS = this.GetIntegerValue("HitDelayMS", 0);
            this._hitCount = this.GetIntegerValue("HitCnt", 0);
            this._minTriggerHousingLimit = this.GetIntegerValue("MinTriggerHousingLimit", 0);
            this._spawnedCharGroundData = LogicDataTables.GetCharacterByName(this.GetValue("SpawnedCharGround", 0));
            this._spawnedCharAirData = LogicDataTables.GetCharacterByName(this.GetValue("SpawnedCharAir", 0));
            this._timeBetweenSpawnsMS = this.GetIntegerValue("TimeBetweenSpawnsMs", 0);
            this._spawnInitialDelayMS = this.GetIntegerValue("SpawnInitialDelayMs", 0);
            this._throwDistance = this.GetIntegerValue("ThrowDistance", 0);
            this._hasAltMode = this.GetBooleanValue("HasAltMode", 0);
            this._enableByCalendar = this.GetBooleanValue("EnabledByCalendar", 0);

            if (this._enableByCalendar)
            {
                if (this._upgradeLevelCount > 1)
                {
                    Debugger.Error("Temporary traps should not have upgrade levels!");
                }
            }
        }

        /// <summary>
        ///     Gets the width.
        /// </summary>
        public int GetWidth()
        {
            return this._width;
        }

        /// <summary>
        ///     Gets the height.
        /// </summary>
        public int GetHeight()
        {
            return this._height;
        }

        /// <summary>
        ///     Gets the upgrade level count.
        /// </summary>
        public int GetUpgradeLevelCount()
        {
            return this._upgradeLevelCount;
        }

        /// <summary>
        ///     Gets the spawn initial delay in ms.
        /// </summary>
        public int GetSpawnInitialDelayMS()
        {
            return this._spawnInitialDelayMS;
        }

        /// <summary>
        ///     Gets the number of spawns.
        /// </summary>
        public int GetNumSpawns(int upgLevel)
        {
            return this._numSpawns[upgLevel];
        }

        /// <summary>
        ///     Gets the construction time.
        /// </summary>
        public int GetBuildTime(int upgLevel)
        {
            return this._constructionTimes[upgLevel];
        }

        /// <summary>
        ///     Gets the build resource.
        /// </summary>
        public LogicResourceData GetBuildResource()
        {
            return this._buildResourceData;
        }

        /// <summary>
        ///     Gets the build cost.
        /// </summary>
        public int GetBuildCost(int upgLevel)
        {
            return this._buildCost[upgLevel];
        }

        /// <summary>
        ///     Gets the required town hall level.
        /// </summary>
        public int GetRequiredTownHallLevel(int upgLevel)
        {
            return this._townHallLevel[upgLevel];
        }
    }
}