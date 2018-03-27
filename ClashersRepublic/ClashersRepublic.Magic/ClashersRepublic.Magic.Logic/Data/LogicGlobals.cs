namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

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

        private int _startingDiamonds;
        private int _startingElixir;
        private int _startingElixir2;
        private int _startingGold;
        private int _startingGold2;
        private int _liveReplayFrequencySecs;
        private int _challengeBaseSaveCooldown;
        private int _allianceCreateCost;
        private int _clockTowerBoostMultiplier;
        private int _clockTowerBoostCooldownSecs;
        private int _clampLongTimeStampsToDays;
        private int _workerCostSecondBuildCost;
        private int _workerCostThirdBuildCost;
        private int _workerCostFourthBuildCost;
        private int _workerCostFifthBuildCost;
        private int _challengeBaseCooldownEnabledOnTh;

        private bool _useNewTraining;
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
        private bool _workerForZeroBuildingTime;
        private bool _adjustEndSubtickUseCurrentTime;
        
        private int[] _village2TroopHousingBuildCost;
        private int[] _village2TroopHousingBuildTimeSecs;

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

            this._startingDiamonds = this.GetIntValue("STARTING_DIAMONDS");
            this._startingGold = this.GetIntValue("STARTING_GOLD");
            this._startingElixir = this.GetIntValue("STARTING_ELIXIR");
            this._startingGold2 = this.GetIntValue("STARTING_GOLD2");
            this._startingElixir2 = this.GetIntValue("STARTING_ELIXIR2");
            this._liveReplayFrequencySecs = this.GetIntValue("LIVE_REPLAY_UPDATE_FREQUENCY_SECONDS");
            this._challengeBaseSaveCooldown = this.GetIntValue("CHALLENGE_BASE_SAVE_COOLDOWN");
            this._allianceCreateCost = this.GetIntValue("ALLIANCE_CREATE_COST");
            this._clockTowerBoostMultiplier = this.GetIntValue("CLOCK_TOWER_BOOST_MULTIPLIER");
            this._clockTowerBoostCooldownSecs = 60 * this.GetIntValue("CLOCK_TOWER_BOOST_COOLDOWN_MINS");
            this._clampLongTimeStampsToDays = this.GetIntValue("CLAMP_LONG_TIME_STAMPS_TO_DAYS");
            this._workerCostSecondBuildCost = this.GetIntValue("WORKER_COST_2ND");
            this._workerCostThirdBuildCost = this.GetIntValue("WORKER_COST_3RD");
            this._workerCostFourthBuildCost = this.GetIntValue("WORKER_COST_4TH");
            this._workerCostFifthBuildCost = this.GetIntValue("WORKER_COST_5TH");
            this._challengeBaseCooldownEnabledOnTh = this.GetIntValue("CHALLENGE_BASE_COOLDOWN_ENABLED_ON_TH");

            this._useNewPathFinder = this.GetBoolValue("USE_NEW_PATH_FINDER");
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

        public bool WorkerForZeroBuilTime()
        {
            return this._workerForZeroBuildingTime;
        }

        public bool AdjustEndSubtickUseCurrentTime()
        {
            return this._adjustEndSubtickUseCurrentTime;
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
        ///     Gets the speed up cost.
        /// </summary>
        public int GetSpeedUpCost(int time, int multiplier, bool isVillage2)
        {
            int speedUpDiamondCostPerMin;
            int speedUpDiamondCostPerHour;
            int speedUpDiamondCostPerDay;
            int speedUpDiamondCostPerWeek;

            if (isVillage2)
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