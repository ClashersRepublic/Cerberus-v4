namespace RivieraStudio.Magic.Logic.Data
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Math;

    public class LogicGlobals
    {
        private int _speedUpDiamondCostPerMin;
        private int _speedUpDiamondCostPerHour;
        private int _speedUpDiamondCostPerDay;
        private int _speedUpDiamondCostPerWeek;
        private int _speedUpDiamondCostPerMinVillage2;
        private int _speedUpDiamondCostPerHourVillage2;
        private int _speedUpDiamondCostPerDayVillage2;
        private int _speedUpDiamondCostPerWeekVillage2;

        private int _resourceDiamondCost100;
        private int _resourceDiamondCost1000;
        private int _resourceDiamondCost10000;
        private int _resourceDiamondCost100000;
        private int _resourceDiamondCost1000000;
        private int _resourceDiamondCost10000000;
        private int _village2ResourceDiamondCost100;
        private int _village2ResourceDiamondCost1000;
        private int _village2ResourceDiamondCost10000;
        private int _village2resourceDiamondCost100000;
        private int _village2resourceDiamondCost1000000;
        private int _village2ResourceDiamondCost10000000;
        private int _darkElixirDiamondCost1;
        private int _darkElixirDiamondCost10;
        private int _darkElixirDiamondCost100;
        private int _darkElixirDiamondCost1000;
        private int _darkElixirDiamondCost10000;
        private int _darkElixirDiamondCost100000;

        private int _startingDiamonds;
        private int _startingElixir;
        private int _startingElixir2;
        private int _startingGold;
        private int _startingGold2;
        private int _liveReplayFrequencySecs;
        private int _challengeBaseSaveCooldown;
        private int _allianceCreateCost;
        private int _clockTowerBoostCooldownSecs;
        private int _clampLongTimeStampsToDays;
        private int _workerCostSecondBuildCost;
        private int _workerCostThirdBuildCost;
        private int _workerCostFourthBuildCost;
        private int _workerCostFifthBuildCost;
        private int _challengeBaseCooldownEnabledOnTh;
        private int _obstacleRespawnSecs;
        private int _obstacleMaxCount;
        private int _resourceProductionLootPercentage;
        private int _darkElixirProductionLootPercentage;
        private int _village2MinTownHallLevelForDestructObstacle;
        private int _attackVillage2PreparationLengthSecs;
        private int _attackPreparationLengthSecs;
        private int _attackLengthSecs;
        private int _village2StartUnitLevel;
        private int _resourceProductionBoostSecs;
        private int _barracksBoostSecs;
        private int _spellFactoryBoostSecs;

        private int _clockTowerBoostMultiplier;
        private int _resourceProductionBoostMultiplier;
        private int _spellTrainingCostMultiplier;
        private int _spellSpeedUpCostMultiplier;
        private int _heroHealthSpeedUpCostMultipler;
        private int _troopRequestSpeedUpCostMultiplier;
        private int _troopTrainingCostMultiplier;
        private int _speedUpBoostCooldownCostMultiplier;
        private int _barracksBoostNewMultiplier;
        private int _barracksBoostMultiplier;
        private int _spellFactoryBoostNewMultiplier;
        private int _spellFactoryBoostMultiplier;
        private int _clockTowerSpeedUpMultiplier;
        private int _buildCancelMultiplier;

        private bool _useNewTraining;
        private bool _useTroopWalksOutFromTraining;
        private bool _useVillageObjects;
        private bool _moreAccurateTime;
        private bool _dragInTraining;
        private bool _dragInTrainingFix;
        private bool _dragInTrainingFix2;
        private bool _useNewPathFinder;
        private bool _liveReplayEnabled;
        private bool _revertBrokenWarLayouts;
        private bool _removeRevengeWhenBattleIsLoaded;
        private bool _completeConstructionOnlyHome;
        private bool _useNewSpeedUpCalculation;
        private bool _clampBuildingTimes;
        private bool _stopBoostPauseWhenBoostTimeZeroOnLoad;
        private bool _fixMergeOldBarrackBoostPausing;
        private bool _saveVillageObjects;
        private bool _startInLastUsedVillage;
        private bool _workerForZeroBuildingTime;
        private bool _adjustEndSubtickUseCurrentTime;
        private bool _collectAllResourcesAtOnce;
        private bool _useSwapBuildings;
        private bool _treasurySizeBasedOnTawnHall;
        private bool _useTeslaTriggerCommand;
        private bool _useTrapTriggerCommand;
        private bool _validateTroopUpgradeLevels;
        private bool _allowCancelBuildingConstruction;

        private int[] _village2TroopHousingBuildCost;
        private int[] _village2TroopHousingBuildTimeSecs;
        private int[] _lootMultiplierByTownHallDifference;
        private int[] _barrackReduceTrainingDivisor;
        private int[] _darkBarrackReduceTrainingDivisor;
        private int[] _clockTowerBoostSecs;

        private LogicResourceData _allianceCreateResourceData;

        /// <summary>
        ///     Creates references.
        /// </summary>
        public void CreateReferences()
        {
            this._speedUpDiamondCostPerMin = this.GetIntValue("SPEED_UP_DIAMOND_COST_1_MIN");
            this._speedUpDiamondCostPerHour = this.GetIntValue("SPEED_UP_DIAMOND_COST_1_HOUR");
            this._speedUpDiamondCostPerDay = this.GetIntValue("SPEED_UP_DIAMOND_COST_24_HOURS");
            this._speedUpDiamondCostPerWeek = this.GetIntValue("SPEED_UP_DIAMOND_COST_1_WEEK");
            this._speedUpDiamondCostPerMinVillage2 = this.GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_MIN");
            this._speedUpDiamondCostPerHourVillage2 = this.GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_HOUR");
            this._speedUpDiamondCostPerDayVillage2 = this.GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_24_HOURS");
            this._speedUpDiamondCostPerWeekVillage2 = this.GetIntValue("VILLAGE2_SPEED_UP_DIAMOND_COST_1_WEEK");

            this._resourceDiamondCost100 = this.GetIntValue("RESOURCE_DIAMOND_COST_100");
            this._resourceDiamondCost1000 = this.GetIntValue("RESOURCE_DIAMOND_COST_1000");
            this._resourceDiamondCost10000 = this.GetIntValue("RESOURCE_DIAMOND_COST_10000");
            this._resourceDiamondCost100000 = this.GetIntValue("RESOURCE_DIAMOND_COST_100000");
            this._resourceDiamondCost1000000 = this.GetIntValue("RESOURCE_DIAMOND_COST_1000000");
            this._resourceDiamondCost10000000 = this.GetIntValue("RESOURCE_DIAMOND_COST_10000000");
            this._village2ResourceDiamondCost100 = this.GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_100");
            this._village2ResourceDiamondCost1000 = this.GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_1000");
            this._village2ResourceDiamondCost10000 = this.GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_10000");
            this._village2resourceDiamondCost100000 = this.GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_100000");
            this._village2resourceDiamondCost1000000 = this.GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_1000000");
            this._village2ResourceDiamondCost10000000 = this.GetIntValue("VILLAGE2_RESOURCE_DIAMOND_COST_10000000");
            this._darkElixirDiamondCost1 = this.GetIntValue("DARK_ELIXIR_DIAMOND_COST_1");
            this._darkElixirDiamondCost10 = this.GetIntValue("DARK_ELIXIR_DIAMOND_COST_10");
            this._darkElixirDiamondCost100 = this.GetIntValue("DARK_ELIXIR_DIAMOND_COST_100");
            this._darkElixirDiamondCost1000 = this.GetIntValue("DARK_ELIXIR_DIAMOND_COST_1000");
            this._darkElixirDiamondCost10000 = this.GetIntValue("DARK_ELIXIR_DIAMOND_COST_10000");
            this._darkElixirDiamondCost100000 = this.GetIntValue("DARK_ELIXIR_DIAMOND_COST_100000");

            this._startingDiamonds = this.GetIntValue("STARTING_DIAMONDS");
            this._startingGold = this.GetIntValue("STARTING_GOLD");
            this._startingElixir = this.GetIntValue("STARTING_ELIXIR");
            this._startingGold2 = this.GetIntValue("STARTING_GOLD2");
            this._startingElixir2 = this.GetIntValue("STARTING_ELIXIR2");
            this._liveReplayFrequencySecs = this.GetIntValue("LIVE_REPLAY_UPDATE_FREQUENCY_SECONDS");
            this._challengeBaseSaveCooldown = this.GetIntValue("CHALLENGE_BASE_SAVE_COOLDOWN");
            this._allianceCreateCost = this.GetIntValue("ALLIANCE_CREATE_COST");
            this._clockTowerBoostCooldownSecs = 60 * this.GetIntValue("CLOCK_TOWER_BOOST_COOLDOWN_MINS");
            this._clampLongTimeStampsToDays = this.GetIntValue("CLAMP_LONG_TIME_STAMPS_TO_DAYS");
            this._workerCostSecondBuildCost = this.GetIntValue("WORKER_COST_2ND");
            this._workerCostThirdBuildCost = this.GetIntValue("WORKER_COST_3RD");
            this._workerCostFourthBuildCost = this.GetIntValue("WORKER_COST_4TH");
            this._workerCostFifthBuildCost = this.GetIntValue("WORKER_COST_5TH");
            this._challengeBaseCooldownEnabledOnTh = this.GetIntValue("CHALLENGE_BASE_COOLDOWN_ENABLED_ON_TH");
            this._obstacleRespawnSecs = this.GetIntValue("OBSTACLE_RESPAWN_SECONDS");
            this._obstacleMaxCount = this.GetIntValue("OBSTACLE_COUNT_MAX");
            this._resourceProductionLootPercentage = this.GetIntValue("RESOURCE_PRODUCTION_LOOT_PERCENTAGE");
            this._darkElixirProductionLootPercentage = this.GetIntValue("RESOURCE_PRODUCTION_LOOT_PERCENTAGE_DARK_ELIXIR");
            this._village2MinTownHallLevelForDestructObstacle = this.GetIntValue("VILLAGE2_DO_NOT_ALLOW_CLEAR_OBSTACLE_TH");
            this._attackVillage2PreparationLengthSecs = this.GetIntValue("ATTACK_PREPARATION_LENGTH_VILLAGE2_SEC");
            this._attackPreparationLengthSecs = this.GetIntValue("ATTACK_PREPARATION_LENGTH_SEC");
            this._attackLengthSecs = this.GetIntValue("ATTACK_LENGTH_SEC");
            this._village2StartUnitLevel = this.GetIntValue("VILLAGE2_START_UNIT_LEVEL");
            this._resourceProductionBoostSecs = 60 * this.GetIntValue("RESOURCE_PRODUCTION_BOOST_MINS");
            this._barracksBoostSecs = 60 * this.GetIntValue("BARRACKS_BOOST_MINS");
            this._spellFactoryBoostSecs = 60 * this.GetIntValue("SPELL_FACTORY_BOOST_MINS");

            this._clockTowerBoostMultiplier = this.GetIntValue("CLOCK_TOWER_BOOST_MULTIPLIER");
            this._resourceProductionBoostMultiplier = this.GetIntValue("RESOURCE_PRODUCTION_BOOST_MULTIPLIER");
            this._spellTrainingCostMultiplier = this.GetIntValue("SPELL_TRAINING_COST_MULTIPLIER");
            this._spellSpeedUpCostMultiplier = this.GetIntValue("SPELL_SPEED_UP_COST_MULTIPLIER");
            this._heroHealthSpeedUpCostMultipler = this.GetIntValue("HERO_HEALTH_SPEED_UP_COST_MULTIPLIER");
            this._troopRequestSpeedUpCostMultiplier = this.GetIntValue("TROOP_REQUEST_SPEED_UP_COST_MULTIPLIER");
            this._troopTrainingCostMultiplier = this.GetIntValue("TROOP_TRAINING_COST_MULTIPLIER");
            this._speedUpBoostCooldownCostMultiplier = this.GetIntValue("SPEEDUP_BOOST_COOLDOWN_COST_MULTIPLIER");
            this._clockTowerSpeedUpMultiplier = this.GetIntValue("CHALLENGE_BASE_COOLDOWN_ENABLED_ON_TH");
            this._barracksBoostMultiplier = this.GetIntValue("BARRACKS_BOOST_MULTIPLIER");
            this._barracksBoostNewMultiplier = this.GetIntValue("BARRACKS_BOOST_MULTIPLIER_NEW");
            this._spellFactoryBoostNewMultiplier = this.GetIntValue("SPELL_FACTORY_BOOST_MULTIPLIER_NEW");
            this._spellFactoryBoostMultiplier = this.GetIntValue("SPELL_FACTORY_BOOST_MULTIPLIER");
            this._buildCancelMultiplier = this.GetIntValue("BUILD_CANCEL_MULTIPLIER");

            this._useNewPathFinder = this.GetBoolValue("USE_NEW_PATH_FINDER");
            this._useTroopWalksOutFromTraining = this.GetBoolValue("USE_TROOP_WALKS_OUT_FROM_TRAINING");
            this._useVillageObjects = this.GetBoolValue("USE_VILLAGE_OBJECTS");
            this._moreAccurateTime = this.GetBoolValue("MORE_ACCURATE_TIME");
            this._useNewTraining = this.GetBoolValue("USE_NEW_TRAINING");
            this._dragInTraining = this.GetBoolValue("DRAG_IN_TRAINING");
            this._dragInTrainingFix = this.GetBoolValue("DRAG_IN_TRAINING_FIX");
            this._dragInTrainingFix2 = this.GetBoolValue("DRAG_IN_TRAINING_FIX2");
            this._revertBrokenWarLayouts = this.GetBoolValue("REVERT_BROKEN_WAR_LAYOUTS");
            this._liveReplayEnabled = this.GetBoolValue("LIVE_REPLAY_ENABLED");
            this._removeRevengeWhenBattleIsLoaded = this.GetBoolValue("REMOVE_REVENGE_WHEN_BATTLE_IS_LOADED");
            this._completeConstructionOnlyHome = this.GetBoolValue("COMPLETE_CONSTRUCTIONS_ONLY_HOME");
            this._useNewSpeedUpCalculation = this.GetBoolValue("USE_NEW_SPEEDUP_CALCULATION");
            this._clampBuildingTimes = this.GetBoolValue("CLAMP_BUILDING_TIMES");
            this._stopBoostPauseWhenBoostTimeZeroOnLoad = this.GetBoolValue("STOP_BOOST_PAUSE_WHEN_BOOST_TIME_ZERO_ON_LOAD");
            this._fixMergeOldBarrackBoostPausing = this.GetBoolValue("FIX_MERGE_OLD_BARRACK_BOOST_PAUSING");
            this._saveVillageObjects = this.GetBoolValue("SAVE_VILLAGE_OBJECTS");
            this._workerForZeroBuildingTime = this.GetBoolValue("WORKER_FOR_ZERO_BUILD_TIME");
            this._adjustEndSubtickUseCurrentTime = this.GetBoolValue("ADJUST_END_SUBTICK_USE_CURRENT_TIME");
            this._collectAllResourcesAtOnce = this.GetBoolValue("COLLECT_ALL_RESOURCES_AT_ONCE");
            this._useSwapBuildings = this.GetBoolValue("USE_SWAP_BUILDINGS");
            this._treasurySizeBasedOnTawnHall = this.GetBoolValue("TREASURY_SIZE_BASED_ON_TH");
            this._startInLastUsedVillage = this.GetBoolValue("START_IN_LAST_USED_VILLAGE");
            this._useTeslaTriggerCommand = this.GetBoolValue("USE_TESLA_TRIGGER_CMD");
            this._useTrapTriggerCommand = this.GetBoolValue("USE_TRAP_TRIGGER_CMD");
            this._validateTroopUpgradeLevels = this.GetBoolValue("VALIDATE_TROOP_UPGRADE_LEVELS");
            this._allowCancelBuildingConstruction = this.GetBoolValue("ALLOW_CANCEL_BUILDING_CONSTRUCTION");

            this._allianceCreateResourceData = LogicDataTables.GetResourceByName(this.GetGlobalData("ALLIANCE_CREATE_RESOURCE").TextValue);

            LogicGlobalData village2TroopHousingBuildCostData = this.GetGlobalData("TROOP_HOUSING_V2_COST");

            this._village2TroopHousingBuildCost = new int[village2TroopHousingBuildCostData.GetNumberArraySize()];

            for (int i = 0; i < this._village2TroopHousingBuildCost.Length; i++)
            {
                this._village2TroopHousingBuildCost[i] = village2TroopHousingBuildCostData.GetNumberArray(i);
            }

            LogicGlobalData village2TroopHousingBuildTimeSecsData = this.GetGlobalData("TROOP_HOUSING_V2_BUILD_TIME_SECONDS");

            this._village2TroopHousingBuildTimeSecs = new int[village2TroopHousingBuildTimeSecsData.GetNumberArraySize()];

            for (int i = 0; i < this._village2TroopHousingBuildTimeSecs.Length; i++)
            {
                this._village2TroopHousingBuildTimeSecs[i] = village2TroopHousingBuildTimeSecsData.GetNumberArray(i);
            }

            LogicGlobalData lootMultiplierByTownHallDifferenceObject = this.GetGlobalData("LOOT_MULTIPLIER_BY_TH_DIFF");

            this._lootMultiplierByTownHallDifference = new int[lootMultiplierByTownHallDifferenceObject.GetNumberArraySize()];

            for (int i = 0; i < this._lootMultiplierByTownHallDifference.Length; i++)
            {
                this._lootMultiplierByTownHallDifference[i] = lootMultiplierByTownHallDifferenceObject.GetNumberArray(i);
            }

            LogicGlobalData barrackReduceTrainingDivisorObject = this.GetGlobalData("BARRACK_REDUCE_TRAINING_DIVISOR");

            this._barrackReduceTrainingDivisor = new int[barrackReduceTrainingDivisorObject.GetNumberArraySize()];

            for (int i = 0; i < this._barrackReduceTrainingDivisor.Length; i++)
            {
                this._barrackReduceTrainingDivisor[i] = barrackReduceTrainingDivisorObject.GetNumberArray(i);
            }

            LogicGlobalData darkBarrackReduceTrainingDivisorObject = this.GetGlobalData("DARK_BARRACK_REDUCE_TRAINING_DIVISOR");

            this._darkBarrackReduceTrainingDivisor = new int[darkBarrackReduceTrainingDivisorObject.GetNumberArraySize()];

            for (int i = 0; i < this._darkBarrackReduceTrainingDivisor.Length; i++)
            {
                this._darkBarrackReduceTrainingDivisor[i] = darkBarrackReduceTrainingDivisorObject.GetNumberArray(i);
            }

            LogicGlobalData clockTowerBoostObject = this.GetGlobalData("CLOCK_TOWER_BOOST_MINS");

            this._clockTowerBoostSecs = new int[clockTowerBoostObject.GetNumberArraySize()];

            for (int i = 0; i < this._clockTowerBoostSecs.Length; i++)
            {
                this._clockTowerBoostSecs[i] = clockTowerBoostObject.GetNumberArray(i);
            }
        }

        /// <summary>
        ///     Gets the data instance by the name.
        /// </summary>
        private LogicGlobalData GetGlobalData(string name)
        {
            return LogicDataTables.GetGlobalByName(name);
        }

        /// <summary>
        ///     Gets the boolean value of specified data name.
        /// </summary>
        private bool GetBoolValue(string name)
        {
            return this.GetGlobalData(name).BooleanValue;
        }

        /// <summary>
        ///     Gets the integer value of specified data name.
        /// </summary>
        private int GetIntValue(string name)
        {
            return this.GetGlobalData(name).NumberValue;
        }

        /// <summary>
        ///     Gets the number of starting diamonds.
        /// </summary>
        public int GetStartingDiamonds()
        {
            return this._startingDiamonds;
        }

        /// <summary>
        ///     Gets the number of starting gold.
        /// </summary>
        public int GetStartingGold()
        {
            return this._startingGold;
        }

        /// <summary>
        ///     Gets the number of starting elixir.
        /// </summary>
        public int GetStartingElixir()
        {
            return this._startingElixir;
        }

        /// <summary>
        ///     Gets the number of starting gold2.
        /// </summary>
        public int GetStartingGold2()
        {
            return this._startingGold2;
        }

        /// <summary>
        ///     Gets the number of starting elixir2.
        /// </summary>
        public int GetStartingElixir2()
        {
            return this._startingElixir2;
        }

        public int GetLiveReplayUpdateFrequencySecs()
        {
            return this._liveReplayFrequencySecs;
        }

        public int GetChallengeBaseSaveCooldown()
        {
            return this._challengeBaseSaveCooldown;
        }

        /// <summary>
        ///     Gets the alliance create cost.
        /// </summary>
        public int GetAllianceCreateCost()
        {
            return this._allianceCreateCost;
        }

        /// <summary>
        ///     Gets the clock tower boost multiplier.
        /// </summary>
        public int GetClockTowerBoostMultiplier()
        {
            return this._clockTowerBoostMultiplier;
        }

        /// <summary>
        ///     Gets the resource production boost multiplier.
        /// </summary>
        public int GetResourceProductionBoostMultiplier()
        {
            return this._resourceProductionBoostMultiplier;
        }

        /// <summary>
        ///     Gets the resource production boost time.
        /// </summary>
        public int GetResourceProductionBoostSecs()
        {
            return this._resourceProductionBoostSecs;
        }

        /// <summary>
        ///     Gets the spell factory boost multiplier.
        /// </summary>
        public int GetSpellFactoryBoostMultiplier()
        {
            return this._spellFactoryBoostMultiplier;
        }

        /// <summary>
        ///     Gets the spell factory boost new multiplier.
        /// </summary>
        public int GetSpellFactoryBoostNewMultiplier()
        {
            return this._spellFactoryBoostNewMultiplier;
        }

        /// <summary>
        ///     Gets the spell factory boost time.
        /// </summary>
        public int GetSpellFactoryBoostSecs()
        {
            return this._spellFactoryBoostSecs;
        }

        /// <summary>
        ///     Gets the barrack boost new multiplier.
        /// </summary>
        public int GetBarracksBoostNewMultiplier()
        {
            return this._barracksBoostNewMultiplier;
        }

        /// <summary>
        ///     Gets the barrack boost multiplier.
        /// </summary>
        public int GetBarracksBoostMultiplier()
        {
            return this._barracksBoostMultiplier;
        }

        /// <summary>
        ///     Gets the build cancel multiplier.
        /// </summary>
        public int GetBuildCancelMultiplier()
        {
            return this._buildCancelMultiplier;
        }

        /// <summary>
        ///     Gets the barrack boost time.
        /// </summary>
        public int GetBarracksBoostSecs()
        {
            return this._barracksBoostSecs;
        }

        /// <summary>
        ///     Gets the clock tower boost cooldown mins.
        /// </summary>
        public int GetClockTowerBoostCooldownSecs()
        {
            return this._clockTowerBoostCooldownSecs;
        }

        public int GetClampLongTimeStampsToDays()
        {
            return this._clampLongTimeStampsToDays;
        }

        /// <summary>
        ///     Gets the obstacle respawn time.
        /// </summary>
        public int GetObstacleRespawnSecs()
        {
            return this._obstacleRespawnSecs;
        }

        /// <summary>
        ///     Gets the obstacle max count.
        /// </summary>
        public int GetObstacleMaxCount()
        {
            return this._obstacleMaxCount;
        }

        /// <summary>
        ///     Gets the resource production loot percentage.
        /// </summary>
        public int GetResourceProductionLootPercentage(LogicResourceData data)
        {
            if (LogicDataTables.GetDarkElixirData() == data)
            {
                return this._darkElixirProductionLootPercentage;
            }

            return this._resourceProductionLootPercentage;
        }

        /// <summary>
        ///     Gets the loot multiplier by townhall difference.
        /// </summary>
        public int GetLootMultiplierByTownHallDiff(int townHallLevel1, int townHallLevel2)
        {
            return this._lootMultiplierByTownHallDifference[LogicMath.Clamp(townHallLevel1 + 4 - townHallLevel2, 0, this._lootMultiplierByTownHallDifference.Length - 1)];
        }

        /// <summary>
        ///     Gets the reduce barrack training divisor.
        /// </summary>
        public int[] GetBarrackReduceTrainingDevisor()
        {
            return this._barrackReduceTrainingDivisor;
        }

        /// <summary>
        ///     Gets the reduce dark barrack training divisor.
        /// </summary>
        public int[] GetDarkBarrackReduceTrainingDevisor()
        {
            return this._darkBarrackReduceTrainingDivisor;
        }

        /// <summary>
        ///     Gets the worker cost.
        /// </summary>
        public int GetWorkerCost(LogicLevel level)
        {
            int totalWorkers = level.GetWorkerManagerAt(level.GetVillageType()).GetTotalWorkers() + level.GetUnplacedObjectCount(LogicDataTables.GetWorkerData());

            switch (totalWorkers)
            {
                case 1: return this._workerCostSecondBuildCost;
                case 2: return this._workerCostThirdBuildCost;
                case 3: return this._workerCostFourthBuildCost;
                case 4: return this._workerCostFifthBuildCost;
                default: return this._workerCostFifthBuildCost;
            }
        }

        /// <summary>
        ///     Gets the townhall for the challenge base cooldown.
        /// </summary>
        public int GetChallengeBaseCooldownTownHall()
        {
            return this._challengeBaseCooldownEnabledOnTh;
        }

        /// <summary>
        ///     Gets the multiplier of the spell training cost.
        /// </summary>
        public int GetSpellTrainingCostMultiplier()
        {
            return this._spellTrainingCostMultiplier;
        }

        /// <summary>
        ///     Gets the multiplier of the speed up of spell.
        /// </summary>
        public int GetSpellSpeedUpCostMultiplier()
        {
            return this._spellSpeedUpCostMultiplier;
        }

        /// <summary>
        ///     Gets the multiplier of the speed up of hero health.
        /// </summary>
        public int GetHeroHealthSpeedUpCostMultipler()
        {
            return this._heroHealthSpeedUpCostMultipler;
        }

        /// <summary>
        ///     Gets the multiplier of the speed up of troop request.
        /// </summary>
        public int GetTroopRequestSpeedUpCostMultiplier()
        {
            return this._troopRequestSpeedUpCostMultiplier;
        }

        /// <summary>
        ///     Gets the multiplier of troop training.
        /// </summary>
        public int GetTroopTrainingCostMultiplier()
        {
            return this._troopTrainingCostMultiplier;
        }

        /// <summary>
        ///     Gets the multiplier of the speed up of boost cooldown.
        /// </summary>
        public int GetSpeedUpBoostCooldownCostMultiplier()
        {
            return this._speedUpBoostCooldownCostMultiplier;
        }

        /// <summary>
        ///     Gets the multiplier of the speed up of clock tower.
        /// </summary>
        public int GetClockTowerSpeedUpMultiplier()
        {
            return this._clockTowerSpeedUpMultiplier;
        }

        /// <summary>
        ///     Gets the minimum village 2 town hall level for destruct obstacle.
        /// </summary>
        public int GetMinVillage2TownHallLevelForDestructObstacle()
        {
            return this._village2MinTownHallLevelForDestructObstacle;
        }

        /// <summary>
        ///     Gets the length of attack preparation.
        /// </summary>
        public int GetAttackPreparationLengthSecs()
        {
            return this._attackPreparationLengthSecs;
        }

        /// <summary>
        ///     Gets the length of attack village 2 preparation.
        /// </summary>
        public int GetAttackVillage2PreparationLengthSecs()
        {
            return this._attackVillage2PreparationLengthSecs;
        }

        /// <summary>
        ///     Gets the attack length.
        /// </summary>
        public int GetAttackLengthSecs()
        {
            return this._attackLengthSecs;
        }

        /// <summary>
        ///     Gets the village 2 start unit level.
        /// </summary>
        public int GetVillage2StartUnitLevel()
        {
            return this._village2StartUnitLevel;
        }

        /// <summary>
        ///     Gets a value indicating whether the time is more accurate.
        /// </summary>
        public bool MoreAccurateTime()
        {
            return this._moreAccurateTime;
        }

        /// <summary>
        ///     Gets a value indicating whether the new training method is used.
        /// </summary>
        public bool UseNewTraining()
        {
            return this._useNewTraining;
        }

        public bool UseTroopWalksOutFromTraining()
        {
            return this._useTroopWalksOutFromTraining;
        }

        public bool UseVillageObjects()
        {
            return this._useVillageObjects;
        }

        public bool UseDragInTraining()
        {
            return this._dragInTraining;
        }

        public bool UseDragInTrainingFix()
        {
            return this._dragInTrainingFix;
        }

        public bool UseDragInTrainingFix2()
        {
            return this._dragInTrainingFix2;
        }

        public bool RevertBrokenWarLayouts()
        {
            return this._revertBrokenWarLayouts;
        }

        public bool LiveReplayEnabled()
        {
            return this._liveReplayEnabled;
        }

        public bool RemoveRevengeWhenBattleIsLoaded()
        {
            return this._removeRevengeWhenBattleIsLoaded;
        }

        public bool UseNewPathFinder()
        {
            return this._useNewPathFinder;
        }

        public bool CompleteConstructionOnlyHome()
        {
            return this._completeConstructionOnlyHome;
        }

        public bool UseNewSpeedUpCalculation()
        {
            return this._useNewSpeedUpCalculation;
        }

        public bool ClampBuildingTimes()
        {
            return this._clampBuildingTimes;
        }

        public bool StopBoostPauseWhenBoostTimeZeroOnLoad()
        {
            return this._stopBoostPauseWhenBoostTimeZeroOnLoad;
        }

        public bool FixMergeOldBarrackBoostPausing()
        {
            return this._fixMergeOldBarrackBoostPausing;
        }

        public bool SaveVillageObjects()
        {
            return this._saveVillageObjects;
        }

        public bool StartInLastUsedVillage()
        {
            return this._startInLastUsedVillage;
        }

        public bool WorkerForZeroBuilTime()
        {
            return this._workerForZeroBuildingTime;
        }

        public bool AdjustEndSubtickUseCurrentTime()
        {
            return this._adjustEndSubtickUseCurrentTime;
        }

        public bool CollectAllResourcesAtOnce()
        {
            return this._collectAllResourcesAtOnce;
        }

        public bool UseSwapBuildings()
        {
            return this._useSwapBuildings;
        }

        public bool TreasurySizeBasedOnTownHall()
        {
            return this._treasurySizeBasedOnTawnHall;
        }

        public bool UseTeslaTriggerCommand()
        {
            return this._useTeslaTriggerCommand;
        }

        public bool UseTrapTriggerCommand()
        {
            return this._useTrapTriggerCommand;
        }

        public bool ValidateTroopUpgradeLevels()
        {
            return this._validateTroopUpgradeLevels;
        }

        public bool AllowCancelBuildingConstruction()
        {
            return this._allowCancelBuildingConstruction;
        }

        /// <summary>
        ///     Gets the alliance create <see cref="LogicResourceData" /> data.
        /// </summary>
        public LogicResourceData GetAllianceCreateResourceData()
        {
            return this._allianceCreateResourceData;
        }

        /// <summary>
        ///     Gets the troop housing build cost village 2.
        /// </summary>
        public int GetTroopHousingBuildCostVillage2(LogicLevel level)
        {
            LogicBuildingData data = LogicDataTables.GetBuildingByName("Troop Housing2");

            if (data != null)
            {
                return this._village2TroopHousingBuildCost[LogicMath.Clamp(level.GetGameObjectManagerAt(1).GetGameObjectCountByData(data),
                    0,
                    this._village2TroopHousingBuildCost.Length - 1)];
            }
            else
            {
                Debugger.Error("Could not find Troop Housing2 data");
            }

            return 0;
        }

        /// <summary>
        ///     Gets the troop housing build time village 2.
        /// </summary>
        public int GetTroopHousingBuildTimeVillage2(LogicLevel level, int ignoreBuildingCnt)
        {
            LogicBuildingData data = LogicDataTables.GetBuildingByName("Troop Housing2");

            if (data != null)
            {
                return this._village2TroopHousingBuildTimeSecs[LogicMath.Clamp(level.GetGameObjectManagerAt(1).GetGameObjectCountByData(data) - ignoreBuildingCnt,
                    0,
                    this._village2TroopHousingBuildTimeSecs.Length - 1)];
            }
            else
            {
                Debugger.Error("Could not find Troop Housing2 data");
            }

            return 0;
        }

        /// <summary>
        ///     Gets the clock tower boost time.
        /// </summary>
        public int GetClockTowerBoostSecs(int upgLevel)
        {
            if (this._clockTowerBoostSecs.Length > upgLevel)
            {
                return this._clockTowerBoostSecs[upgLevel];
            }

            return this._clockTowerBoostSecs[this._clockTowerBoostSecs.Length - 1];
        }

        /// <summary>
        ///     Gets the resource diamond cost.
        /// </summary>
        public int GetResourceDiamondCost(int count, LogicResourceData data)
        {
            if (LogicDataTables.GetDarkElixirData() != data)
            {
                int resourceDiamondCost100;
                int resourceDiamondCost1000;
                int resourceDiamondCost10000;
                int resourceDiamondCost100000;
                int resourceDiamondCost1000000;
                int resourceDiamondCost10000000;

                if (data.GetVillageType() == 1)
                {
                    resourceDiamondCost100 = this._village2ResourceDiamondCost100;
                    resourceDiamondCost1000 = this._village2ResourceDiamondCost1000;
                    resourceDiamondCost10000 = this._village2ResourceDiamondCost10000;
                    resourceDiamondCost100000 = this._village2resourceDiamondCost100000;
                    resourceDiamondCost1000000 = this._village2resourceDiamondCost1000000;
                    resourceDiamondCost10000000 = this._village2ResourceDiamondCost10000000;
                }
                else
                {
                    resourceDiamondCost100 = this._resourceDiamondCost100;
                    resourceDiamondCost1000 = this._resourceDiamondCost1000;
                    resourceDiamondCost10000 = this._resourceDiamondCost10000;
                    resourceDiamondCost100000 = this._resourceDiamondCost100000;
                    resourceDiamondCost1000000 = this._resourceDiamondCost1000000;
                    resourceDiamondCost10000000 = this._resourceDiamondCost10000000;
                }

                if (count >= 1)
                {
                    if (count >= 100)
                    {
                        if (count >= 1000)
                        {
                            if (count >= 10000)
                            {
                                if (count >= 100000)
                                {
                                    if (count >= 1000000)
                                    {
                                        return resourceDiamondCost1000000 + ((resourceDiamondCost10000000 - resourceDiamondCost1000000) * (count / 1000 - 1000) + 4500) / 9000;
                                    }

                                    return resourceDiamondCost100000 + ((resourceDiamondCost1000000 - resourceDiamondCost100000) * (count / 100 - 1000) + 4500) / 9000;
                                }

                                return resourceDiamondCost10000 + ((resourceDiamondCost100000 - resourceDiamondCost10000) * (count / 10 - 1000) + 4500) / 9000;
                            }

                            return resourceDiamondCost1000 + ((resourceDiamondCost10000 - resourceDiamondCost1000) * (count - 1000) + 4500) / 9000;
                        }

                        return resourceDiamondCost100 + ((resourceDiamondCost1000 - resourceDiamondCost100) * (count - 100) + 450) / 900;
                    }

                    return resourceDiamondCost100;
                }

                return 0;
            }
            else
            {
                return this.GetDarkElixirDiamondCost(count);
            }
        }

        /// <summary>
        ///     Gets the dark elixir diamond cost.
        /// </summary>
        public int GetDarkElixirDiamondCost(int count)
        {
            if (count >= 1)
            {
                if (count >= 10)
                {
                    if (count >= 100)
                    {
                        if (count >= 1000)
                        {
                            if (count >= 10000)
                            {
                                return this._darkElixirDiamondCost10000 + ((this._darkElixirDiamondCost100000 - this._darkElixirDiamondCost10000) * (count - 10000) + 45000) / 90000;
                            }

                            return this._darkElixirDiamondCost1000 + ((this._darkElixirDiamondCost10000 - this._darkElixirDiamondCost1000) * (count - 1000) + 4500) / 9000;
                        }

                        return this._darkElixirDiamondCost100 + ((this._darkElixirDiamondCost1000 - this._darkElixirDiamondCost100) * (count - 100) + 450) / 900;
                    }

                    return this._darkElixirDiamondCost10 + ((this._darkElixirDiamondCost100 - this._darkElixirDiamondCost10) * (count - 10) + 45) / 90;
                }

                return this._darkElixirDiamondCost1 + ((this._darkElixirDiamondCost10 - this._darkElixirDiamondCost1) * (count - 1) + 4) / 9;
            }

            return 0;
        }

        /// <summary>
        ///     Gets the speed up cost.
        /// </summary>
        public int GetSpeedUpCost(int time, int multiplier, int villageType)
        {
            int speedUpDiamondCostPerMin;
            int speedUpDiamondCostPerHour;
            int speedUpDiamondCostPerDay;
            int speedUpDiamondCostPerWeek;

            if (villageType == 1)
            {
                speedUpDiamondCostPerMin = this._speedUpDiamondCostPerMinVillage2;
                speedUpDiamondCostPerHour = this._speedUpDiamondCostPerHourVillage2;
                speedUpDiamondCostPerDay = this._speedUpDiamondCostPerDayVillage2;
                speedUpDiamondCostPerWeek = this._speedUpDiamondCostPerWeekVillage2;
            }
            else
            {
                speedUpDiamondCostPerMin = this._speedUpDiamondCostPerMin;
                speedUpDiamondCostPerHour = this._speedUpDiamondCostPerHour;
                speedUpDiamondCostPerDay = this._speedUpDiamondCostPerDay;
                speedUpDiamondCostPerWeek = this._speedUpDiamondCostPerWeek;
            }

            int multiplier1 = multiplier;
            int multiplier2 = multiplier;
            int cost;

            if (this._useNewSpeedUpCalculation)
            {
                multiplier1 = 100;
            }
            else
            {
                multiplier2 = 100;
            }

            if (time >= 60)
            {
                if (time >= 3600)
                {
                    if (time >= 86400)
                    {
                        int tmp = (speedUpDiamondCostPerWeek - speedUpDiamondCostPerDay) * (time - 86400);

                        cost = multiplier2 * speedUpDiamondCostPerDay / 100 + tmp / 100 * multiplier2 / 518400;

                        if (cost < 0 || tmp / 100 > 0x7FFFFFFF / multiplier2)
                        {
                            cost = multiplier2 * (speedUpDiamondCostPerDay + tmp / 518400) / 100;
                        }
                    }
                    else
                    {
                        cost = multiplier2 * speedUpDiamondCostPerHour / 100 + (speedUpDiamondCostPerDay - speedUpDiamondCostPerHour) * (time - 3600) / 100 * multiplier2 / 82800;
                    }
                }
                else
                {
                    cost = multiplier2 * speedUpDiamondCostPerMin / 100 + (speedUpDiamondCostPerHour - speedUpDiamondCostPerMin) * (time - 60) * multiplier2 / 354000;
                }
            }
            else
            {
                cost = multiplier2 * speedUpDiamondCostPerMin * time / 6000;
            }

            return LogicMath.Max(cost * multiplier1 / 100, 1);
        }
    }
}