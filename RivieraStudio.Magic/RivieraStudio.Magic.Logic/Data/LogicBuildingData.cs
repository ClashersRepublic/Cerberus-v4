namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.CSV;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicBuildingData : LogicData
    {
        private LogicBuildingClassData _buildingClass;
        private LogicResourceData[] _buildResourceData;
        private LogicResourceData[] _altBuildResourceData;
        private LogicResourceData _gearUpResourceData;
        private LogicResourceData _produceResourceData;
        private LogicHeroData _heroData;
        private LogicArrayList<int>[] _storedResourceCounts;
        private LogicArrayList<int>[] _percentageStoredResourceCounts;

        private int[] _constructionTimes;
        private int[] _townHallLevel;
        private int[] _townHallVillage2Level;
        private int[] _wallBlockX;
        private int[] _wallBlockY;
        private int[] _gearUpTime;
        private int[] _gearUpCost;
        private int[] _boostCost;
        private int _upgradeLevelCount;
        private bool _isClockTower;
        private bool _isFlamer;
        private bool _isBarrackVillage2;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuildingData" /> class.
        /// </summary>
        public LogicBuildingData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicBuildingData.
        }

        public string TID { get; protected set; }
        public string InfoTID { get; protected set; }
        public string BuildingClass { get; protected set; }
        public string SecondaryTargetingClass { get; protected set; }
        public string ShopBuildingClass { get; protected set; }
        public string SWF { get; protected set; }
        protected string[] ExportName { get; set; }
        public string ExportNameNpc { get; protected set; }
        public string ExportNameConstruction { get; protected set; }
        public string ExportNameLocked { get; protected set; }
        protected int[] BuildCost { get; set; }
        protected int Width { get; set; }
        protected int Height { get; set; }
        public string Icon { get; protected set; }
        protected string[] ExportNameBuildAnim { get; set; }
        protected string[] ExportNameUpgradeAnim { get; set; }
        public bool LootOnDestruction { get; protected set; }
        public bool Bunker { get; protected set; }
        public int Village2Housing { get; protected set; }
        protected int[] HousingSpace { get; set; }
        protected int[] HousingSpaceAlt { get; set; }
        protected int[] ResourcePer100Hours { get; set; }
        protected int[] ResourceMax { get; set; }
        protected int[] ResourceIconLimit { get; set; }
        protected int[] UnitProduction { get; set; }
        public bool UpgradesUnits { get; protected set; }
        public int ProducesUnitsOfType { get; set; }
        public bool FreeBoost { get; protected set; }
        protected int[] Hitpoints { get; set; }
        protected int[] RegenTime { get; set; }
        public int AttackRange { get; protected set; }
        public bool AltAttackMode { get; protected set; }
        public int AltAttackRange { get; protected set; }
        public int PrepareSpeed { get; protected set; }
        public int AttackSpeed { get; protected set; }
        public int AltAttackSpeed { get; protected set; }
        public int CoolDownOverride { get; protected set; }
        protected int[] DPS { get; set; }
        protected int[] AltDPS { get; set; }
        protected int[] Damage { get; set; }
        public string PreferredTarget { get; protected set; }
        public int PreferredTargetDamageMod { get; protected set; }
        public bool RandomHitPosition { get; protected set; }
        protected string[] DestroyEffect { get; set; }
        protected string[] DestroyDamageEffect { get; set; }
        protected string[] AttackEffect { get; set; }
        protected string[] AttackEffect2 { get; set; }
        public int ChainAttackDistance { get; protected set; }
        public string AttackEffectAlt { get; protected set; }
        protected string[] HitEffect { get; set; }
        protected string[] Projectile { get; set; }
        protected string[] AltProjectile { get; set; }
        protected string[] ExportNameDamaged { get; set; }
        public int BuildingW { get; protected set; }
        public int BuildingH { get; protected set; }
        public int BaseGfx { get; protected set; }
        protected string[] ExportNameBase { get; set; }
        public bool AirTargets { get; protected set; }
        public bool GroundTargets { get; protected set; }
        public bool AltAirTargets { get; protected set; }
        public bool AltGroundTargets { get; protected set; }
        public bool AltMultiTargets { get; protected set; }
        public int AmmoCount { get; protected set; }
        public string AmmoResource { get; protected set; }
        protected int[] AmmoCost { get; set; }
        public int MinAttackRange { get; protected set; }
        public int DamageRadius { get; protected set; }
        public int PushBack { get; protected set; }
        protected bool[] WallCornerPieces { get; set; }
        public string LoadAmmoEffect { get; protected set; }
        public string NoAmmoEffect { get; protected set; }
        public string ToggleAttackModeEffect { get; protected set; }
        public string PickUpEffect { get; protected set; }
        public string PlacingEffect { get; protected set; }
        public bool CanNotSellLast { get; protected set; }
        protected string[] DefenderCharacter { get; set; }
        protected int[] DefenderCount { get; set; }
        protected int[] DefenderZ { get; set; }
        protected int[] AltDefenderZ { get; set; }
        protected int[] DestructionXP { get; set; }
        public bool Locked { get; protected set; }
        public int StartingHomeCount { get; protected set; }
        public bool Hidden { get; protected set; }
        public string AOESpell { get; protected set; }
        public string AOESpellAlternate { get; protected set; }
        public int TriggerRadius { get; protected set; }
        protected string[] ExportNameTriggered { get; set; }
        public string AppearEffect { get; protected set; }
        public bool ForgesSpells { get; protected set; }
        public bool ForgesMiniSpells { get; protected set; }
        public bool IsHeroBarrack { get; protected set; }
        public bool IncreasingDamage { get; protected set; }
        protected int[] DPSLv2 { get; set; }
        protected int[] DPSLv3 { get; set; }
        protected int[] DPSMulti { get; set; }
        protected int[] Lv2SwitchTime { get; set; }
        protected int[] Lv3SwitchTime { get; set; }
        protected string[] AttackEffectLv2 { get; set; }
        protected string[] AttackEffectLv3 { get; set; }
        public string TransitionEffectLv2 { get; protected set; }
        public string TransitionEffectLv3 { get; protected set; }
        protected int[] AltNumMultiTargets { get; set; }
        public bool PreventsHealing { get; protected set; }
        protected int[] StrengthWeight { get; set; }
        public int AlternatePickNewTargetDelay { get; protected set; }
        public int SpeedMod { get; protected set; }
        public int StatusEffectTime { get; protected set; }
        protected int[] ShockwavePushStrength { get; set; }
        public int ShockwaveArcLength { get; protected set; }
        public int ShockwaveExpandRadius { get; protected set; }
        public int TargetingConeAngle { get; protected set; }
        public int AimRotateStep { get; protected set; }
        public bool PenetratingProjectile { get; protected set; }
        public int PenetratingRadius { get; protected set; }
        public int PenetratingExtraRange { get; protected set; }
        public int TurnSpeed { get; protected set; }
        public bool NeedsAim { get; protected set; }
        public bool TargetGroups { get; protected set; }
        public int TargetGroupsRadius { get; protected set; }
        public string HitSpell { get; protected set; }
        public int HitSpellLevel { get; protected set; }
        public string ExportNameBeamStart { get; protected set; }
        public string ExportNameBeamEnd { get; protected set; }
        public int Damage2 { get; protected set; }
        public int Damage2Radius { get; protected set; }
        public int Damage2Delay { get; protected set; }
        public int Damage2Min { get; protected set; }
        public int Damage2FalloffStart { get; protected set; }
        public int Damage2FalloffEnd { get; protected set; }
        public string HitEffect2 { get; protected set; }
        public int WakeUpSpeed { get; protected set; }
        public int WakeUpSpace { get; protected set; }
        public string PreAttackEffect { get; protected set; }
        public bool ShareHeroCombatData { get; protected set; }
        public int BurstCount { get; protected set; }
        public int BurstDelay { get; protected set; }
        public int AltBurstCount { get; protected set; }
        public int AltBurstDelay { get; protected set; }
        public int DummyProjectileCount { get; protected set; }
        protected int[] DieDamage { get; set; }
        public int DieDamageRadius { get; protected set; }
        public string DieDamageEffect { get; protected set; }
        public int DieDamageDelay { get; protected set; }
        public bool IsRed { get; protected set; }
        public int RedMul { get; protected set; }
        public int GreenMul { get; protected set; }
        public int BlueMul { get; protected set; }
        public int RedAdd { get; protected set; }
        public int GreenAdd { get; protected set; }
        public int BlueAdd { get; protected set; }
        protected int[] DefenceTroopCount { get; set; }
        public string DefenceTroopCharacter { get; protected set; }
        public string DefenceTroopCharacter2 { get; protected set; }
        protected int[] DefenceTroopLevel { get; set; }
        protected int[] AmountCanBeUpgraded { get; set; }
        public bool SelfAsAoeCenter { get; protected set; }
        public int NewTargetAttackDelay { get; protected set; }
        public string GearUpBuilding { get; protected set; }
        public int GearUpLevelRequirement { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            base.CreateVillageReferences();

            this._upgradeLevelCount = this._row.GetBiggestArraySize();
            this._buildingClass = LogicDataTables.GetBuildingClassByName(this.BuildingClass);

            if (this._buildingClass == null)
            {
                Debugger.Error("Building class is not defined for " + this.GetName());
            }

            int longestArraySize = this._row.GetBiggestArraySize();

            this._buildResourceData = new LogicResourceData[longestArraySize];
            this._altBuildResourceData = new LogicResourceData[longestArraySize];
            this._storedResourceCounts = new LogicArrayList<int>[longestArraySize];
            this._percentageStoredResourceCounts = new LogicArrayList<int>[longestArraySize];
            this._townHallLevel = new int[longestArraySize];
            this._townHallVillage2Level = new int[longestArraySize];
            this._constructionTimes = new int[longestArraySize];
            this._gearUpTime = new int[longestArraySize];
            this._gearUpCost = new int[longestArraySize];
            this._boostCost = new int[longestArraySize];

            for (int i = 0; i < longestArraySize; i++)
            {
                this._gearUpCost[i] = this.GetClampedIntegerValue("GearUpCost", i);
                this._gearUpTime[i] = this.GetClampedIntegerValue("GearUpTime", i);
                this._boostCost[i] = this.GetClampedIntegerValue("BoostCost", i);
                this._buildResourceData[i] = LogicDataTables.GetResourceByName(this.GetClampedValue("BuildResource", i));
                this._altBuildResourceData[i] = LogicDataTables.GetResourceByName(this.GetClampedValue("AltBuildResource", i));
                this._townHallLevel[i] = LogicMath.Max(this.GetClampedIntegerValue("TownHallLevel", i) - 1, 0);
                this._townHallVillage2Level[i] = LogicMath.Max(this.GetClampedIntegerValue("TownHallLevel2", i) - 1, 0);
                this._storedResourceCounts[i] = new LogicArrayList<int>();
                this._percentageStoredResourceCounts[i] = new LogicArrayList<int>();

                LogicDataTable table = LogicDataTables.GetTable(2);

                for (int j = 0; j < table.GetItemCount(); j++)
                {
                    this._storedResourceCounts[i].Add(this.GetIntegerValue("MaxStored" + table.GetItemAt(j).GetName(), i));
                    this._percentageStoredResourceCounts[i].Add(this.GetIntegerValue("PercentageStored" + table.GetItemAt(j).GetName(), i));
                }

                this._constructionTimes[i] = 86400 * this.GetIntegerValue("BuildTimeD", i) +
                                             3600 * this.GetIntegerValue("BuildTimeH", i) +
                                             60 * this.GetIntegerValue("BuildTimeM", i) +
                                             this.GetIntegerValue("BuildTimeS", i);
            }

            this._produceResourceData = LogicDataTables.GetResourceByName(this.GetValue("ProducesResource", 0));
            this._gearUpResourceData = LogicDataTables.GetResourceByName(this.GetClampedValue("GearUpResource", 0));

            string heroType = this.GetValue("HeroType", 0);

            if (!string.IsNullOrEmpty(heroType))
            {
                this._heroData = LogicDataTables.GetHeroByName(heroType);
            }

            string wallBlockX = this.GetValue("WallBlockX", 0);

            if (wallBlockX.Length > 0)
            {
                this.LoadWallBlock(wallBlockX, out this._wallBlockX);
                this.LoadWallBlock(this.GetValue("WallBlockY", 0), out this._wallBlockY);
            }

            this._isClockTower = this.GetName().Equals("Clock Tower");
            this._isFlamer = this.GetName().Equals("Flamer");
            this._isBarrackVillage2 = this.GetName().Equals("Barrack2");
        }

        /// <summary>
        ///     Loads the wall block.
        /// </summary>
        public void LoadWallBlock(string value, out int[] wallBlock)
        {
            string[] tmp = value.Split(',');
            wallBlock = new int[tmp.Length];

            for (int i = 0; i < tmp.Length; i++)
            {
                wallBlock[i] = int.Parse(tmp[i]);
            }
        }

        /// <summary>
        ///     Gets the building class.
        /// </summary>
        public LogicBuildingClassData GetBuildingClass()
        {
            return this._buildingClass;
        }

        public int GetUpgradeLevelCount()
        {
            return this._upgradeLevelCount;
        }

        public int GetConstructionTime(int upgLevel, LogicLevel level, int ignoreBuildingCnt)
        {
            if (this.Village2Housing < 1)
            {
                return this._constructionTimes[upgLevel];
            }

            return LogicDataTables.GetGlobals().GetTroopHousingBuildTimeVillage2(level, ignoreBuildingCnt);
        }

        public bool IsTownHall()
        {
            return this._buildingClass.IsTownHall();
        }

        public bool IsTownHallVillage2()
        {
            return this._buildingClass.IsTownHall2();
        }

        public bool IsWorkerBuilding()
        {
            return this._buildingClass.IsWorker();
        }

        public bool IsWall()
        {
            return this._buildingClass.IsWall();
        }

        public bool IsAllianceCastle()
        {
            return this.Bunker;
        }

        public bool IsLaboratory()
        {
            return this.UpgradesUnits;
        }

        public bool IsLocked()
        {
            return this.Locked;
        }

        public bool IsClockTower()
        {
            return this._isClockTower;
        }

        public bool IsFlamer()
        {
            return this._isFlamer;
        }

        public bool IsBarrackVillage2()
        {
            return this._isBarrackVillage2;
        }

        public int GetUnitStorageCapacity(int level)
        {
            return this.HousingSpace[level];
        }

        public int GetAltUnitStorageCapacity(int level)
        {
            return this.HousingSpaceAlt[level];
        }

        /// <summary>
        ///     Gets the gear up resource.
        /// </summary>
        public LogicResourceData GetGearUpResource()
        {
            return this._gearUpResourceData;
        }

        public LogicResourceData GetBuildResource(int idx)
        {
            return this._buildResourceData[idx];
        }

        public LogicResourceData GetAltBuildResource(int idx)
        {
            return this._altBuildResourceData[idx];
        }

        public LogicResourceData GetProduceResource()
        {
            return this._produceResourceData;
        }

        public LogicHeroData GetHeroData()
        {
            return this._heroData;
        }

        public string GetExportName(int index)
        {
            return this.ExportName[index];
        }

        public int GetBuildCost(int index, LogicLevel level)
        {
            if (this.Village2Housing <= 0)
            {
                if (this._buildingClass.IsWorker())
                {
                    return LogicDataTables.GetGlobals().GetWorkerCost(level);
                }

                return this.BuildCost[index];
            }

            return LogicDataTables.GetGlobals().GetTroopHousingBuildCostVillage2(level);
        }

        public int GetRequiredTownHallLevel(int index)
        {
            return this._townHallLevel[index];
        }

        public int GetTownHallLevel2(int index)
        {
            return this._townHallVillage2Level[index];
        }

        public int GetWidth()
        {
            return this.Width;
        }

        public int GetHeight()
        {
            return this.Height;
        }

        public string GetExportNameBuildAnim(int index)
        {
            return this.ExportNameBuildAnim[index];
        }

        public string GetExportNameUpgradeAnim(int index)
        {
            return this.ExportNameUpgradeAnim[index];
        }

        public bool StoresResources()
        {
            LogicArrayList<int> storeCount = this._storedResourceCounts[0];

            for (int i = 0; i < storeCount.Count; i++)
            {
                if (storeCount[i] > 0)
                {
                    return true;
                }
            }

            return false;
        }

        public LogicArrayList<int> GetMaxStoredResourceCounts(int idx)
        {
            return this._storedResourceCounts[idx];
        }

        public LogicArrayList<int> GetMaxPercentageStoredResourceCounts(int idx)
        {
            return this._percentageStoredResourceCounts[idx];
        }

        public int GetResourcePer100Hours(int index)
        {
            return this.ResourcePer100Hours[index];
        }

        public int GetResourceMax(int index)
        {
            return this.ResourceMax[index];
        }

        public int GetResourceIconLimit(int index)
        {
            return this.ResourceIconLimit[index];
        }

        public int GetUnitProduction(int index)
        {
            return this.UnitProduction[index];
        }

        public int GetProducesUnitsOfType()
        {
            return this.ProducesUnitsOfType;
        }

        public int GetBoostCost(int index)
        {
            return this._boostCost[index];
        }

        public int GetHitpoints(int index)
        {
            return this.Hitpoints[index];
        }

        public int GetRegenerationTime(int index)
        {
            return this.RegenTime[index];
        }

        public int GetDPS(int index)
        {
            return this.DPS[index];
        }

        public int GetAltDPS(int index)
        {
            return this.AltDPS[index];
        }

        public int GetDamage(int index)
        {
            return this.Damage[index];
        }

        public string GetDestroyEffect(int index)
        {
            return this.DestroyEffect[index];
        }

        public string GetDestroyDamageEffect(int index)
        {
            return this.DestroyDamageEffect[index];
        }

        public string GetAttackEffect(int index)
        {
            return this.AttackEffect[index];
        }

        public string GetAttackEffect2(int index)
        {
            return this.AttackEffect2[index];
        }

        public string GetHitEffect(int index)
        {
            return this.HitEffect[index];
        }

        public string GetProjectile(int index)
        {
            return this.Projectile[index];
        }

        public string GetAltProjectile(int index)
        {
            return this.AltProjectile[index];
        }

        public string GetExportNameDamaged(int index)
        {
            return this.ExportNameDamaged[index];
        }

        public string GetExportNameBase(int index)
        {
            return this.ExportNameBase[index];
        }

        public int GetAmmoCost(int index)
        {
            return this.AmmoCost[index];
        }

        public bool GetWallCornerPieces(int index)
        {
            return this.WallCornerPieces[index];
        }

        public string GetDefenderCharacter(int index)
        {
            return this.DefenderCharacter[index];
        }

        public int GetDefenderCount(int index)
        {
            return this.DefenderCount[index];
        }

        public int GetDefenderZ(int index)
        {
            return this.DefenderZ[index];
        }

        public int GetAltDefenderZ(int index)
        {
            return this.AltDefenderZ[index];
        }

        public int GetDestructionXP(int index)
        {
            return this.DestructionXP[index];
        }

        public string GetExportNameTriggered(int index)
        {
            return this.ExportNameTriggered[index];
        }

        public int GetDPSLv2(int index)
        {
            return this.DPSLv2[index];
        }

        public int GetDPSLv3(int index)
        {
            return this.DPSLv3[index];
        }

        public int GetDPSMulti(int index)
        {
            return this.DPSMulti[index];
        }

        public int GetLv2SwitchTime(int index)
        {
            return this.Lv2SwitchTime[index];
        }

        public int GetLv3SwitchTime(int index)
        {
            return this.Lv3SwitchTime[index];
        }

        public string GetAttackEffectLv2(int index)
        {
            return this.AttackEffectLv2[index];
        }

        public string GetAttackEffectLv3(int index)
        {
            return this.AttackEffectLv3[index];
        }

        public int GetAltNumMultiTargets(int index)
        {
            return this.AltNumMultiTargets[index];
        }

        public int GetStrengthWeight(int index)
        {
            return this.StrengthWeight[index];
        }

        public int GetShockwavePushStrength(int index)
        {
            return this.ShockwavePushStrength[index];
        }

        public int GetDieDamage(int index)
        {
            return this.DieDamage[index];
        }

        public int GetDefenceTroopCount(int index)
        {
            return this.DefenceTroopCount[index];
        }

        public int GetDefenceTroopLevel(int index)
        {
            return this.DefenceTroopLevel[index];
        }

        public int GetAmountCanBeUpgraded(int index)
        {
            return this.AmountCanBeUpgraded[index];
        }

        public int GetGearUpCost(int index)
        {
            return this._gearUpCost[index];
        }

        public int GetGearUpTime(int index)
        {
            return this._gearUpTime[index];
        }

        public int GetWallBlockX(int index)
        {
            return this._wallBlockX[index];
        }

        public int GetWallBlockY(int index)
        {
            return this._wallBlockY[index];
        }

        public int GetWallBlockCount()
        {
            return this._wallBlockX.Length;
        }
    }
}