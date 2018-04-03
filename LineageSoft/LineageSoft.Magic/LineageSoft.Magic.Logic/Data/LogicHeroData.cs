namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;

    public class LogicHeroData : LogicCharacterData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHeroData" /> class.
        /// </summary>
        public LogicHeroData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicHeroData.
        }

        protected int[] RequiredTownHallLevel { get; set; }
        public int CoolDownOverride { get; protected set; }
        public string SmallPicture { get; protected set; }
        public string SmallPictureSWF { get; protected set; }
        protected string[] RageProjectile { get; set; }
        public string AttackEffectAlt { get; protected set; }
        public string HitEffectActive { get; protected set; }
        public int ActivationTime { get; protected set; }
        public int ActiveDuration { get; protected set; }
        public int AbilityAttackCount { get; protected set; }
        protected string[] AnimationActivated { get; set; }
        public int MaxSearchRadiusForDefender { get; protected set; }
        protected int[] RegenerationTimeMinutes { get; set; }
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
        public bool NoAttackOverWalls { get; protected set; }
        public bool TargetGroups { get; protected set; }
        public bool FightWithGroups { get; protected set; }
        public int TargetGroupsRadius { get; protected set; }
        public int TargetGroupsRange { get; protected set; }
        public int TargetGroupsMinWeight { get; protected set; }
        public bool SmoothJump { get; protected set; }
        public int WakeUpSpeed { get; protected set; }
        public int WakeUpSpace { get; protected set; }
        public bool NoDefence { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
           base.CreateReferences();         
        }

        public int GetRequiredTownHallLevel(int index)
        {
            return this.RequiredTownHallLevel[index];
        }

        public string GetRageProjectile(int index)
        {
            return this.RageProjectile[index];
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

        public override int GetCombatItemType()
        {
            return 2;
        }
    }
}