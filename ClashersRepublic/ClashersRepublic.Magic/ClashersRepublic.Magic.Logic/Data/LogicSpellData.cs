namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicSpellData : LogicCombatItemData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSpellData" /> class.
        /// </summary>
        public LogicSpellData(CSVRow row, LogicDataTable table) : base(row, table)
        {
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        protected int[] SpellForgeLevel { get; set; }
        protected int[] DeployTimeMS { get; set; }
        protected int[] ChargingTimeMS { get; set; }
        protected int[] HitTimeMS { get; set; }
        protected int[] BoostTimeMS { get; set; }
        protected int[] SpeedBoost { get; set; }
        protected int[] SpeedBoost2 { get; set; }
        protected int[] JumpHousingLimit { get; set; }
        protected int[] JumpBoostMS { get; set; }
        protected int[] DamageBoostPercent { get; set; }
        protected int[] BuildingDamageBoostPercent { get; set; }
        protected int[] Damage { get; set; }
        public int TroopDamagePermil { get; protected set; }
        protected int[] BuildingDamagePermil { get; set; }
        public int ExecuteHealthPermil { get; protected set; }
        public int DamagePermilMin { get; protected set; }
        public int PreferredDamagePermilMin { get; protected set; }
        protected int[] Radius { get; set; }
        protected int[] NumberOfHits { get; set; }
        protected int[] RandomRadius { get; set; }
        protected int[] TimeBetweenHitsMS { get; set; }
        public string IconSWF { get; protected set; }
        protected string[] PreDeployEffect { get; set; }
        protected string[] DeployEffect { get; set; }
        public int DeployEffect2Delay { get; protected set; }
        protected string[] DeployEffect2 { get; set; }
        public string ChargingEffect { get; protected set; }
        public string HitEffect { get; protected set; }
        public bool RandomRadiusAffectsOnlyGfx { get; protected set; }
        protected int[] FreezeTimeMS { get; set; }
        public string SpawnObstacle { get; protected set; }
        public int NumObstacles { get; protected set; }
        protected int[] StrengthWeight { get; set; }
        public bool TroopsOnly { get; protected set; }
        public string TargetInfoString { get; protected set; }
        public string PreferredTarget { get; protected set; }
        public int PreferredTargetDamageMod { get; protected set; }
        public bool BoostDefenders { get; protected set; }
        public int HeroDamageMultiplier { get; protected set; }
        public int ShieldProjectileSpeed { get; protected set; }
        public int ShieldProjectileDamageMod { get; protected set; }
        protected int[] ExtraHealthPermil { get; set; }
        protected int[] ExtraHealthMin { get; set; }
        protected int[] ExtraHealthMax { get; set; }
        protected int[] PoisonDPS { get; set; }
        public bool PoisonIncreaseSlowly { get; protected set; }
        protected int[] AttackSpeedBoost { get; set; }
        public bool BoostLinkedToPoison { get; protected set; }
        public bool PoisonAffectAir { get; protected set; }
        public bool ScaleDeployEffects { get; protected set; }
        protected int[] InvulnerabilityTime { get; set; }
        public int MaxUnitsHit { get; protected set; }
        public string EnemyDeployEffect { get; protected set; }
        public bool SnapToGrid { get; protected set; }
        protected int[] DuplicateHousing { get; set; }
        public int DuplicateLifetime { get; protected set; }
        public string SummonTroop { get; protected set; }
        protected int[] UnitsToSpawn { get; set; }
        public int ShrinkReduceSpeedRatio { get; protected set; }
        public int ShrinkHitpointsRatio { get; protected set; }
        protected int[] SpawnDuration { get; set; }
        protected int[] SpawnFirstGroupSize { get; set; }
        public int DamageTHPercent { get; protected set; }
        public bool ScaleByTH { get; protected set; }
        public int PauseCombatComponentsMs { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateReferences();
        }

        public int GetSpellForgeLevel(int index)
        {
            return this.SpellForgeLevel[index];
        }

        public int GetDeployTimeMS(int index)
        {
            return this.DeployTimeMS[index];
        }

        public int GetChargingTimeMS(int index)
        {
            return this.ChargingTimeMS[index];
        }

        public int GetHitTimeMS(int index)
        {
            return this.HitTimeMS[index];
        }

        public int GetBoostTimeMS(int index)
        {
            return this.BoostTimeMS[index];
        }

        public int GetSpeedBoost(int index)
        {
            return this.SpeedBoost[index];
        }

        public int GetSpeedBoost2(int index)
        {
            return this.SpeedBoost2[index];
        }

        public int GetJumpHousingLimit(int index)
        {
            return this.JumpHousingLimit[index];
        }

        public int GetJumpBoostMS(int index)
        {
            return this.JumpBoostMS[index];
        }

        public int GetDamageBoostPercent(int index)
        {
            return this.DamageBoostPercent[index];
        }

        public int GetBuildingDamageBoostPercent(int index)
        {
            return this.BuildingDamageBoostPercent[index];
        }

        public int GetDamage(int index)
        {
            return this.Damage[index];
        }

        public int GetBuildingDamagePermil(int index)
        {
            return this.BuildingDamagePermil[index];
        }

        public int GetRadius(int index)
        {
            return this.Radius[index];
        }

        public int GetNumberOfHits(int index)
        {
            return this.NumberOfHits[index];
        }

        public int GetRandomRadius(int index)
        {
            return this.RandomRadius[index];
        }

        public int GetTimeBetweenHitsMS(int index)
        {
            return this.TimeBetweenHitsMS[index];
        }

        public string GetPreDeployEffect(int index)
        {
            return this.PreDeployEffect[index];
        }

        public string GetDeployEffect(int index)
        {
            return this.DeployEffect[index];
        }

        public string GetDeployEffect2(int index)
        {
            return this.DeployEffect2[index];
        }

        public int GetFreezeTimeMS(int index)
        {
            return this.FreezeTimeMS[index];
        }

        public int GetStrengthWeight(int index)
        {
            return this.StrengthWeight[index];
        }

        public int GetExtraHealthPermil(int index)
        {
            return this.ExtraHealthPermil[index];
        }

        public int GetExtraHealthMin(int index)
        {
            return this.ExtraHealthMin[index];
        }

        public int GetExtraHealthMax(int index)
        {
            return this.ExtraHealthMax[index];
        }

        public int GetPoisonDPS(int index)
        {
            return this.PoisonDPS[index];
        }

        public int GetAttackSpeedBoost(int index)
        {
            return this.AttackSpeedBoost[index];
        }

        public int GetInvulnerabilityTime(int index)
        {
            return this.InvulnerabilityTime[index];
        }

        public int GetDuplicateHousing(int index)
        {
            return this.DuplicateHousing[index];
        }

        public int GetUnitsToSpawn(int index)
        {
            return this.UnitsToSpawn[index];
        }

        public int GetSpawnDuration(int index)
        {
            return this.SpawnDuration[index];
        }

        public int GetSpawnFirstGroupSize(int index)
        {
            return this.SpawnFirstGroupSize[index];
        }

        public override int GetCombatItemType()
        {
            return 1;
        }
    }
}