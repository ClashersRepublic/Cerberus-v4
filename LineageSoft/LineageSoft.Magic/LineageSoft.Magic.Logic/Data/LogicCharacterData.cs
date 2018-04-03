namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicCharacterData : LogicCombatItemData
    {
        private bool _healerTrigger;
        private bool _isUnderground;

        private int _speed;
        private int _unlockedBarrackLevel;
        private int[] _upgradeLevelByThownHall;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCharacterData" /> class.
        /// </summary>
        public LogicCharacterData(CSVRow row, LogicDataTable table) : base(row, table)
        {
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string SWF { get; protected set; }
        protected int[] Hitpoints { get; set; }
        protected int[] AttackRange { get; set; }
        protected int[] AttackSpeed { get; set; }
        protected int[] DPS { get; set; }
        public int PreferedTargetDamageMod { get; protected set; }
        protected int[] DamageRadius { get; set; }
        public bool AreaDamageIgnoresWalls { get; protected set; }
        public bool SelfAsAoeCenter { get; protected set; }
        public string IconSWF { get; protected set; }
        public string BigPictureSWF { get; protected set; }
        protected string[] Projectile { get; set; }
        public string AltProjectile { get; protected set; }
        public string PreferedTargetBuilding { get; protected set; }
        public string PreferedTargetBuildingClass { get; protected set; }
        public bool PreferredTargetNoTargeting { get; protected set; }
        public string DeployEffect { get; protected set; }
        protected string[] AttackEffect { get; set; }
        public string AttackEffect2 { get; protected set; }
        protected string[] HitEffect { get; set; }
        protected string[] HitEffect2 { get; set; }
        public bool IsFlying { get; protected set; }
        public bool AirTargets { get; protected set; }
        public bool GroundTargets { get; protected set; }
        public int AttackCount { get; protected set; }
        protected string[] DieEffect { get; set; }
        public string DieEffect2 { get; protected set; }
        protected string[] Animation { get; set; }
        public bool IsJumper { get; protected set; }
        public int MovementOffsetAmount { get; protected set; }
        public int MovementOffsetSpeed { get; protected set; }
        public string TombStone { get; protected set; }
        protected int[] DieDamage { get; set; }
        public int DieDamageRadius { get; protected set; }
        protected string[] DieDamageEffect { get; set; }
        public int DieDamageDelay { get; protected set; }
        public string SecondaryTroop { get; protected set; }
        public bool IsSecondaryTroop { get; protected set; }
        protected int[] SecondaryTroopCnt { get; set; }
        protected int[] SecondarySpawnDist { get; set; }
        public bool RandomizeSecSpawnDist { get; protected set; }
        public bool PickNewTargetAfterPushback { get; protected set; }
        public int PushbackSpeed { get; protected set; }
        public string SummonTroop { get; protected set; }
        protected int[] SummonTroopCount { get; set; }
        protected int[] SummonCooldown { get; set; }
        public string SummonEffect { get; protected set; }
        protected int[] SummonLimit { get; set; }
        public int SpawnIdle { get; protected set; }
        public bool SpawnOnAttack { get; protected set; }
        protected int[] StrengthWeight { get; set; }
        public string ChildTroop { get; protected set; }
        public int ChildTroopCount { get; protected set; }
        public int SpeedDecreasePerChildTroopLost { get; protected set; }
        public int ChildTroop0_X { get; protected set; }
        public int ChildTroop0_Y { get; protected set; }
        public int ChildTroop1_X { get; protected set; }
        public int ChildTroop1_Y { get; protected set; }
        public int ChildTroop2_X { get; protected set; }
        public int ChildTroop2_Y { get; protected set; }
        public bool AttackMultipleBuildings { get; protected set; }
        public bool IncreasingDamage { get; protected set; }
        public int DPSLv2 { get; protected set; }
        public int DPSLv3 { get; protected set; }
        public int Lv2SwitchTime { get; protected set; }
        public int Lv3SwitchTime { get; protected set; }
        public string AttackEffectLv2 { get; protected set; }
        public string AttackEffectLv3 { get; protected set; }
        public string AttackEffectLv4 { get; protected set; }
        public string TransitionEffectLv2 { get; protected set; }
        public string TransitionEffectLv3 { get; protected set; }
        public string TransitionEffectLv4 { get; protected set; }
        public int HitEffectOffset { get; protected set; }
        public int TargetedEffectOffset { get; protected set; }
        public int SecondarySpawnOffset { get; protected set; }
        public string CustomDefenderIcon { get; protected set; }
        public int SpecialMovementMod { get; protected set; }
        public int InvisibilityRadius { get; protected set; }
        public int HealthReductionPerSecond { get; protected set; }
        public int AutoMergeDistance { get; protected set; }
        public int AutoMergeGroupSize { get; protected set; }
        public int ProjectileBounces { get; protected set; }
        public int FriendlyGroupWeight { get; protected set; }
        public int EnemyGroupWeight { get; protected set; }
        public int NewTargetAttackDelay { get; protected set; }
        public bool TriggersTraps { get; protected set; }
        public int ChainShootingDistance { get; protected set; }
        public string PreAttackEffect { get; protected set; }
        public string MoveStartsEffect { get; protected set; }
        public string MoveTrailEffect { get; protected set; }
        public string BecomesTargetableEffect { get; protected set; }
        public bool BoostedIfAlone { get; protected set; }
        public int BoostRadius { get; protected set; }
        public int BoostDmgPerfect { get; protected set; }
        public int BoostAttackSpeed { get; protected set; }
        public string HideEffect { get; protected set; }
        protected int[] UnitsInCamp { get; set; }
        protected int[] SpecialAbilityLevel { get; set; }
        public string SpecialAbilityName { get; protected set; }
        public string SpecialAbilityInfo { get; protected set; }
        public string SpecialAbilityType { get; protected set; }
        protected int[] SpecialAbilityAttribute { get; set; }
        protected int[] SpecialAbilityAttribute2 { get; set; }
        protected int[] SpecialAbilityAttribute3 { get; set; }
        public string SpecialAbilitySpell { get; protected set; }
        public string SpecialAbilityEffect { get; protected set; }
        public bool DisableDonate { get; protected set; }
        public bool ScaleByTH { get; protected set; }
        public int LoseHpPerTick { get; protected set; }
        public int LoseHpInterval { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateReferences();
            this._speed = this.GetIntegerValue("Speed", 0);
            this._unlockedBarrackLevel = this.GetIntegerValue("BarrackLevel", 0) - 1;
            this._healerTrigger = this.GetBooleanValue("HealerTrigger", 0);
            this._isUnderground = this.GetBooleanValue("IsUnderground", 0);
        }

        /// <summary>
        ///     Gets the required barrack level.
        /// </summary>
        public override int GetRequiredProductionLevel()
        {
            return this._unlockedBarrackLevel;
        }

        /// <summary>
        ///     Gets if the character is underground.
        /// </summary>
        public bool IsUnderground()
        {
            return this._isUnderground;
        }

        /// <summary>
        ///     Gets if the character is a healer trigger.
        /// </summary>
        public bool IsHealerTrigger()
        {
            return this._healerTrigger;
        }

        public int GetHitpoints(int index)
        {
            return this.Hitpoints[index];
        }

        public int GetAttackRange(int index)
        {
            return this.AttackRange[index];
        }

        public int GetAttackSpeed(int index)
        {
            return this.AttackSpeed[index];
        }

        /// <summary>
        ///     Gets the character speed.
        /// </summary>
        public int GetSpeed()
        {
            return this._speed;
        }

        public int GetDPS(int index)
        {
            return this.DPS[index];
        }
        
        public int GetDamageRadius(int index)
        {
            return this.DamageRadius[index];
        }

        public string GetProjectile(int index)
        {
            return this.Projectile[index];
        }

        public string GetAttackEffect(int index)
        {
            return this.AttackEffect[index];
        }

        public string GetHitEffect(int index)
        {
            return this.HitEffect[index];
        }

        public string GetHitEffect2(int index)
        {
            return this.HitEffect2[index];
        }

        public string GetDieEffect(int index)
        {
            return this.DieEffect[index];
        }

        public string GetAnimation(int index)
        {
            return this.Animation[index];
        }

        public int GetDieDamage(int index)
        {
            return this.DieDamage[index];
        }

        public string GetDieDamageEffect(int index)
        {
            return this.DieDamageEffect[index];
        }

        public int GetSecondaryTroopCnt(int index)
        {
            return this.SecondaryTroopCnt[index];
        }

        public int GetSecondarySpawnDist(int index)
        {
            return this.SecondarySpawnDist[index];
        }

        public int GetSummonTroopCount(int index)
        {
            return this.SummonTroopCount[index];
        }

        public int GetSummonCooldown(int index)
        {
            return this.SummonCooldown[index];
        }

        public int GetSummonLimit(int index)
        {
            return this.SummonLimit[index];
        }

        public int GetStrengthWeight(int index)
        {
            return this.StrengthWeight[index];
        }

        public int GetUnitsInCamp(int index)
        {
            return this.UnitsInCamp[index];
        }

        public int GetSpecialAbilityLevel(int index)
        {
            return this.SpecialAbilityLevel[index];
        }

        public int GetSpecialAbilityAttribute(int index)
        {
            return this.SpecialAbilityAttribute[index];
        }

        public int GetSpecialAbilityAttribute2(int index)
        {
            return this.SpecialAbilityAttribute2[index];
        }

        public int GetSpecialAbilityAttribute3(int index)
        {
            return this.SpecialAbilityAttribute3[index];
        }

        public bool IsUnlockedForBarrackLevel(int barrackLevel)
        {
            return this._unlockedBarrackLevel != -1 && this._unlockedBarrackLevel <= barrackLevel;
        }

        public override int GetCombatItemType()
        {
            return 0;
        }
    }
}