namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Titan.CSV;

    public class LogicTrapData : LogicData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTrapData" /> class.
        /// </summary>
        public LogicTrapData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicTrapData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string SWF { get; protected set; }
        protected string[] ExportName { get; set; }
        protected string[] ExportNameAir { get; set; }
        protected string[] ExportNameBuildAnim { get; set; }
        protected string[] ExportNameBuildAnimAir { get; set; }
        protected string[] ExportNameBroken { get; set; }
        protected string[] ExportNameBrokenAir { get; set; }
        public string BigPicture { get; protected set; }
        public string BigPictureSWF { get; protected set; }
        public string EffectBroken { get; protected set; }
        protected int[] Damage { get; set; }
        protected int[] DamageRadius { get; set; }
        public int TriggerRadius { get; protected set; }
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public string Effect { get; protected set; }
        public string Effect2 { get; protected set; }
        public string DamageEffect { get; protected set; }
        public bool Passable { get; protected set; }
        public string BuildResource { get; protected set; }
        protected int[] BuildTimeD { get; set; }
        protected int[] BuildTimeH { get; set; }
        protected int[] BuildTimeM { get; set; }
        protected int[] BuildCost { get; set; }
        protected int[] RearmCost { get; set; }
        protected int[] TownHallLevel { get; set; }
        public bool EjectVictims { get; protected set; }
        public int MinTriggerHousingLimit { get; protected set; }
        protected int[] EjectHousingLimit { get; set; }
        protected string[] ExportNameTriggered { get; set; }
        protected string[] ExportNameTriggeredAir { get; set; }
        public int ActionFrame { get; protected set; }
        public string PickUpEffect { get; protected set; }
        public string PlacingEffect { get; protected set; }
        public string AppearEffect { get; protected set; }
        public string ToggleAttackModeEffect { get; protected set; }
        public int DurationMS { get; protected set; }
        public int SpeedMod { get; protected set; }
        public int DamageMod { get; protected set; }
        public bool AirTrigger { get; protected set; }
        public bool GroundTrigger { get; protected set; }
        public bool HealerTrigger { get; protected set; }
        public int HitDelayMS { get; protected set; }
        public int HitCnt { get; protected set; }
        protected string[] Projectile { get; set; }
        public string Spell { get; protected set; }
        protected int[] StrengthWeight { get; set; }
        public int PreferredTargetDamageMod { get; protected set; }
        public string PreferredTarget { get; protected set; }
        public string SpawnedCharGround { get; protected set; }
        public string SpawnedCharAir { get; protected set; }
        protected int[] NumSpawns { get; set; }
        public int SpawnInitialDelayMs { get; protected set; }
        public int TimeBetweenSpawnsMs { get; protected set; }
        public bool Disabled { get; protected set; }
        public int ThrowDistance { get; protected set; }
        protected int[] Pushback { get; set; }
        protected bool[] DoNotScalePushByDamage { get; set; }
        public bool EnabledByCalendar { get; protected set; }
        public int DirectionCount { get; protected set; }
        public bool HasAltMode { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateVillageReferences();
        }

        public string GetExportName(int index)
        {
            return this.ExportName[index];
        }

        public string GetExportNameAir(int index)
        {
            return this.ExportNameAir[index];
        }

        public string GetExportNameBuildAnim(int index)
        {
            return this.ExportNameBuildAnim[index];
        }

        public string GetExportNameBuildAnimAir(int index)
        {
            return this.ExportNameBuildAnimAir[index];
        }

        public string GetExportNameBroken(int index)
        {
            return this.ExportNameBroken[index];
        }

        public string GetExportNameBrokenAir(int index)
        {
            return this.ExportNameBrokenAir[index];
        }

        public int GetDamage(int index)
        {
            return this.Damage[index];
        }

        public int GetDamageRadius(int index)
        {
            return this.DamageRadius[index];
        }

        public int GetBuildTimeD(int index)
        {
            return this.BuildTimeD[index];
        }

        public int GetBuildTimeH(int index)
        {
            return this.BuildTimeH[index];
        }

        public int GetBuildTimeM(int index)
        {
            return this.BuildTimeM[index];
        }

        public int GetBuildCost(int index)
        {
            return this.BuildCost[index];
        }

        public int GetRearmCost(int index)
        {
            return this.RearmCost[index];
        }

        public int GetTownHallLevel(int index)
        {
            return this.TownHallLevel[index];
        }

        public int GetEjectHousingLimit(int index)
        {
            return this.EjectHousingLimit[index];
        }

        public string GetExportNameTriggered(int index)
        {
            return this.ExportNameTriggered[index];
        }

        public string GetExportNameTriggeredAir(int index)
        {
            return this.ExportNameTriggeredAir[index];
        }

        public string GetProjectile(int index)
        {
            return this.Projectile[index];
        }

        public int GetStrengthWeight(int index)
        {
            return this.StrengthWeight[index];
        }

        public int GetNumSpawns(int index)
        {
            return this.NumSpawns[index];
        }

        public int GetPushback(int index)
        {
            return this.Pushback[index];
        }

        public bool GetDoNotScalePushByDamage(int index)
        {
            return this.DoNotScalePushByDamage[index];
        }
    }
}