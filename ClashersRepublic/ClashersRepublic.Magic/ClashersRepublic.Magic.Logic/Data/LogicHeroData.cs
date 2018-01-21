namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicHeroData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHeroData" /> class.
        /// </summary>
        public LogicHeroData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicHeroData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string SWF { get; protected set; }
        public int Speed { get; protected set; }
        protected int[] Hitpoints { get; set; }
        protected int[] UpgradeTimeH { get; set; }
        public string UpgradeResource { get; protected set; }
        protected int[] UpgradeCost { get; set; }
        protected int[] RequiredTownHallLevel { get; set; }
        public int AttackRange { get; protected set; }
        public int AttackSpeed { get; protected set; }
        public int CoolDownOverride { get; protected set; }
        protected int[] DPS { get; set; }
        public int PreferedTargetDamageMod { get; protected set; }
        public int DamageRadius { get; protected set; }
        public string IconSWF { get; protected set; }
        public string IconExportName { get; protected set; }
        public string BigPicture { get; protected set; }
        public string BigPictureSWF { get; protected set; }
        public string SmallPicture { get; protected set; }
        public string SmallPictureSWF { get; protected set; }
        protected string[] Projectile { get; set; }
        protected string[] RageProjectile { get; set; }
        public string PreferedTargetBuilding { get; protected set; }
        public string DeployEffect { get; protected set; }
        public string AttackEffect { get; protected set; }
        public string AttackEffectAlt { get; protected set; }
        public string HitEffect { get; protected set; }
        public string HitEffectActive { get; protected set; }
        public bool IsFlying { get; protected set; }
        public bool AirTargets { get; protected set; }
        public bool GroundTargets { get; protected set; }
        public bool IsJumper { get; protected set; }
        public int AttackCount { get; protected set; }
        public string DieEffect { get; protected set; }
        protected string[] Animation { get; set; }
        public int ActivationTime { get; protected set; }
        public int ActiveDuration { get; protected set; }
        public int AbilityAttackCount { get; protected set; }
        protected string[] AnimationActivated { get; set; }
        public int MaxSearchRadiusForDefender { get; protected set; }
        public int HousingSpace { get; protected set; }
        public string SpecialAbilityEffect { get; protected set; }
        protected int[] RegenerationTimeMinutes { get; set; }
        public int TrainingTime { get; protected set; }
        public string TrainingResource { get; protected set; }
        public int TrainingCost { get; protected set; }
        public string CelebrateEffect { get; protected set; }
        public int SleepOffsetX { get; protected set; }
        public int SleepOffsetY { get; protected set; }
        public int PatrolRadius { get; protected set; }
        public string AbilityTriggerEffect { get; protected set; }
        public bool AbilityAffectsHero { get; protected set; }
        public string AbilityAffectsCharacter { get; protected set; }
        public int AbilityRadius { get; protected set; }
        protected int[] AbilityTime { get; set; }
        public bool AbilityOnce { get; protected set; }
        public int AbilityCooldown { get; protected set; }
        protected int[] AbilitySpeedBoost { get; set; }
        protected int[] AbilitySpeedBoost2 { get; set; }
        protected int[] AbilityDamageBoostPercent { get; set; }
        public string AbilitySummonTroop { get; protected set; }
        protected int[] AbilitySummonTroopCount { get; set; }
        public bool AbilityStealth { get; protected set; }
        protected int[] AbilityDamageBoostOffset { get; set; }
        protected int[] AbilityHealthIncrease { get; set; }
        public int AbilityShieldProjectileSpeed { get; protected set; }
        public int AbilityShieldProjectileDamageMod { get; protected set; }
        public string AbilityTID { get; protected set; }
        public string AbilityDescTID { get; protected set; }
        public string AbilityIcon { get; protected set; }
        public string AbilityBigPictureExportName { get; protected set; }
        public int AbilityDelay { get; protected set; }
        protected int[] StrengthWeight { get; set; }
        protected int[] StrengthWeight2 { get; set; }
        public int AlertRadius { get; protected set; }
        public int Scale { get; protected set; }
        public string AuraSpell { get; protected set; }
        protected int[] AuraSpellLevel { get; set; }
        public string AuraTID { get; protected set; }
        public string AuraDescTID { get; protected set; }
        public string AuraBigPictureExportName { get; protected set; }
        protected string[] AbilitySpell { get; set; }
        protected int[] AbilitySpellLevel { get; set; }
        public string RetributionSpell { get; protected set; }
        public int RetributionSpellLevel { get; protected set; }
        public int RetributionSpellTriggerHealth { get; protected set; }
        public bool HasAltMode { get; protected set; }
        public bool AltModeFlying { get; protected set; }
        protected string[] AltModeAnimation { get; set; }
        public string PreferedTargetBuildingClass { get; protected set; }
        public bool NoAttackOverWalls { get; protected set; }
        public bool TargetGroups { get; protected set; }
        public bool FightWithGroups { get; protected set; }
        public int TargetGroupsRadius { get; protected set; }
        public int TargetGroupsRange { get; protected set; }
        public int TargetGroupsMinWeight { get; protected set; }
        public bool SmoothJump { get; protected set; }
        public int WakeUpSpeed { get; protected set; }
        public int WakeUpSpace { get; protected set; }
        public int FriendlyGroupWeight { get; protected set; }
        public int EnemyGroupWeight { get; protected set; }
        public string AttackEffectShared { get; protected set; }
        public int TargetedEffectOffset { get; protected set; }
        public bool TriggersTraps { get; protected set; }
        public int VillageType { get; protected set; }
        public bool NoDefence { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            // LoadingFinished.
        }

        public int GetHitpoints(int index)
        {
            return this.Hitpoints[index];
        }

        public int GetUpgradeTimeH(int index)
        {
            return this.UpgradeTimeH[index];
        }

        public int GetUpgradeCost(int index)
        {
            return this.UpgradeCost[index];
        }

        public int GetRequiredTownHallLevel(int index)
        {
            return this.RequiredTownHallLevel[index];
        }

        public int GetDPS(int index)
        {
            return this.DPS[index];
        }

        public string GetProjectile(int index)
        {
            return this.Projectile[index];
        }

        public string GetRageProjectile(int index)
        {
            return this.RageProjectile[index];
        }

        public string GetAnimation(int index)
        {
            return this.Animation[index];
        }

        public string GetAnimationActivated(int index)
        {
            return this.AnimationActivated[index];
        }

        public int GetRegenerationTimeMinutes(int index)
        {
            return this.RegenerationTimeMinutes[index];
        }

        public int GetAbilityTime(int index)
        {
            return this.AbilityTime[index];
        }

        public int GetAbilitySpeedBoost(int index)
        {
            return this.AbilitySpeedBoost[index];
        }

        public int GetAbilitySpeedBoost2(int index)
        {
            return this.AbilitySpeedBoost2[index];
        }

        public int GetAbilityDamageBoostPercent(int index)
        {
            return this.AbilityDamageBoostPercent[index];
        }

        public int GetAbilitySummonTroopCount(int index)
        {
            return this.AbilitySummonTroopCount[index];
        }

        public int GetAbilityDamageBoostOffset(int index)
        {
            return this.AbilityDamageBoostOffset[index];
        }

        public int GetAbilityHealthIncrease(int index)
        {
            return this.AbilityHealthIncrease[index];
        }

        public int GetStrengthWeight(int index)
        {
            return this.StrengthWeight[index];
        }

        public int GetStrengthWeight2(int index)
        {
            return this.StrengthWeight2[index];
        }

        public int GetAuraSpellLevel(int index)
        {
            return this.AuraSpellLevel[index];
        }

        public string GetAbilitySpell(int index)
        {
            return this.AbilitySpell[index];
        }

        public int GetAbilitySpellLevel(int index)
        {
            return this.AbilitySpellLevel[index];
        }

        public string GetAltModeAnimation(int index)
        {
            return this.AltModeAnimation[index];
        }
    }
}