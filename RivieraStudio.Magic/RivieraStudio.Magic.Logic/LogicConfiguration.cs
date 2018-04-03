namespace RivieraStudio.Magic.Logic
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicConfiguration
    {
        private LogicJSONObject _json;
        private LogicObstacleData _specialObstacle;
        private LogicArrayList<int> _milestoneScoreChangeForLosing;
        private LogicArrayList<int> _percentageScoreChangeForLosing;
        private LogicArrayList<int> _milestoneStrengthRangeForScore;
        private LogicArrayList<int> _percentageStrengthRangeForScore;

        private bool _battleWaitForDieDamage;
        private bool _battleWaitForProjectileDestruction;

        private int _maxTownHallLevel;
        private int _duelLootLimitCooldownInMinutes;
        private int _duelBonusLimitWinsPerDay;
        private int _duelBonusPercentWin;
        private int _duelBonusPercentLose;
        private int _duelBonusPercentDraw;
        private int _duelBonusMaxDiamondCostPercent;

        private string _giftPackExtension;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicConfiguration"/> class.
        /// </summary>
        public LogicConfiguration()
        {
            this._maxTownHallLevel = 8;
        }

        /// <summary>
        ///     Gets the configuration json.
        /// </summary>
        public LogicJSONObject GetJson()
        {
            return this._json;
        }

        /// <summary>
        ///     Gets if the battle wait for projectile destruction.
        /// </summary>
        public bool GetBatteWaitForProjectileDestruction()
        {
            return this._battleWaitForProjectileDestruction;
        }

        /// <summary>
        ///     Gets the max town hall level.
        /// </summary>
        public int GetMaxTownHallLevel()
        {
            return this._maxTownHallLevel;
        }

        /// <summary>
        ///     Gets the special obstacle data.
        /// </summary>
        public LogicObstacleData GetSpecialObstacleData()
        {
            return this._specialObstacle;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            this._json = jsonObject;

            if (jsonObject != null)
            {
                LogicJSONObject village1Object = jsonObject.GetJSONObject("Village1");
                Debugger.DoAssert(village1Object != null, "pVillage1 = NULL!");

                LogicJSONString specialObstacleObject = village1Object.GetJSONString("SpecialObstacle");

                if (specialObstacleObject != null)
                {
                    this._specialObstacle = LogicDataTables.GetObstacleByName(specialObstacleObject.GetStringValue());
                }

                LogicJSONObject village2Object = jsonObject.GetJSONObject("Village2");
                Debugger.DoAssert(village2Object != null, "pVillage2 = NULL!");

                this._maxTownHallLevel = LogicJSONHelper.GetJSONNumber(village2Object, "TownHallMaxLevel");

                LogicJSONArray scoreChangeForLosingArray = village2Object.GetJSONArray("ScoreChangeForLosing");
                Debugger.DoAssert(scoreChangeForLosingArray != null, "ScoreChangeForLosing array is null");

                this._milestoneScoreChangeForLosing = new LogicArrayList<int>(scoreChangeForLosingArray.Size());
                this._percentageScoreChangeForLosing = new LogicArrayList<int>(scoreChangeForLosingArray.Size());

                for (int i = 0; i < scoreChangeForLosingArray.Size(); i++)
                {
                    LogicJSONObject obj = scoreChangeForLosingArray.GetJSONObject(i);

                    if (obj != null)
                    {
                        LogicJSONNumber milestoneObject = obj.GetJSONNumber("Milestone");
                        LogicJSONNumber percentageObject = obj.GetJSONNumber("Percentage");

                        if (milestoneObject != null && percentageObject != null)
                        {
                            this._milestoneScoreChangeForLosing.Add(milestoneObject.GetIntValue());
                            this._percentageScoreChangeForLosing.Add(percentageObject.GetIntValue());
                        }
                    }
                }

                LogicJSONArray strengthRangeForScoreArray = village2Object.GetJSONArray("StrengthRangeForScore");
                Debugger.DoAssert(strengthRangeForScoreArray != null, "StrengthRangeForScore array is null");

                this._milestoneStrengthRangeForScore = new LogicArrayList<int>(strengthRangeForScoreArray.Size());
                this._percentageStrengthRangeForScore = new LogicArrayList<int>(strengthRangeForScoreArray.Size());

                for (int i = 0; i < strengthRangeForScoreArray.Size(); i++)
                {
                    LogicJSONObject obj = strengthRangeForScoreArray.GetJSONObject(i);

                    if (obj != null)
                    {
                        LogicJSONNumber milestoneObject = obj.GetJSONNumber("Milestone");
                        LogicJSONNumber percentageObject = obj.GetJSONNumber("Percentage");

                        if (milestoneObject != null && percentageObject != null)
                        {
                            this._milestoneStrengthRangeForScore.Add(milestoneObject.GetIntValue());
                            this._percentageStrengthRangeForScore.Add(percentageObject.GetIntValue());
                        }
                    }
                }

                LogicJSONObject killSwitchesObject = jsonObject.GetJSONObject("KillSwitches");
                Debugger.DoAssert(killSwitchesObject != null, "pKillSwitches = NULL!");

                this._battleWaitForProjectileDestruction = LogicJSONHelper.GetJSONBoolean(killSwitchesObject, "BattleWaitForProjectileDestruction");
                this._battleWaitForDieDamage = LogicJSONHelper.GetJSONBoolean(killSwitchesObject, "BattleWaitForDieDamage");

                LogicJSONObject globalsObject = jsonObject.GetJSONObject("Globals");
                Debugger.DoAssert(globalsObject != null, "pGlobals = NULL!");

                this._giftPackExtension = LogicJSONHelper.GetJSONString(globalsObject, "GiftPackExtension");

                this._duelLootLimitCooldownInMinutes = LogicJSONHelper.GetJSONNumber(globalsObject, "DuelLootLimitCooldownInMinutes");
                this._duelBonusLimitWinsPerDay = LogicJSONHelper.GetJSONNumber(globalsObject, "DuelBonusLimitWinsPerDay");
                this._duelBonusPercentWin = LogicJSONHelper.GetJSONNumber(globalsObject, "DuelBonusPercentWin");
                this._duelBonusPercentLose = LogicJSONHelper.GetJSONNumber(globalsObject, "DuelBonusPercentLose");
                this._duelBonusPercentDraw = LogicJSONHelper.GetJSONNumber(globalsObject, "DuelBonusPercentDraw");
                this._duelBonusMaxDiamondCostPercent = LogicJSONHelper.GetJSONNumber(globalsObject, "DuelBonusMaxDiamondCostPercent");
            }
            else
            {
                Debugger.Error("pConfiguration = NULL!");
            }
        }
    }
}