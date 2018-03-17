namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicMissionData : LogicData
    {
        private int _missionType;
        private int _missionCategory;
        private int _buildBuildingCount;
        private int _buildBuildingLevel;
        private int _trainTroopCount;
        private int _villagers;
        private int _rewardResourceCount;
        private int _customData;
        private int _rewardXP;
        private int _rewardCharacterCount;
        private int _villageType;
        private int _delay;

        private bool _openAchievements;
        private bool _showMap;
        private bool _changeName;
        private bool _switchSides;
        private bool _showWarBase;
        private bool _showStates;
        private bool _openInfo;
        private bool _showDonate;
        private bool _warStates;
        private bool _forceCamera;
        private bool _deprecated;
        private bool _firstStep;

        private string _action;
        private string _tutorialText;

        private LogicNpcData _defendNpcData;
        private LogicNpcData _attackNpcData;
        private LogicCharacterData _characterData;
        private LogicBuildingData _buildBuildingData;
        private LogicVillageObjectData _fixVillageObjectData;
        private LogicCharacterData _rewardCharacterData;
        private LogicResourceData _rewardResourceData;
        private LogicArrayList<LogicMissionData> _missionDependencies;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionData" /> class.
        /// </summary>
        public LogicMissionData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            this._missionType = -1;
        }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            for (int i = 0; i < this.GetArraySize("Dependencies"); i++)
            {
                LogicMissionData dependency = LogicDataTables.GetMissionByName(this.GetValue("Dependencies", i));

                if (dependency != null)
                {
                    this._missionDependencies.Add(dependency);
                }
            }

            this._action = this.GetValue("Action", 0);
            this._deprecated = this.GetBooleanValue("Deprecated", 0);
            this._missionCategory = this.GetIntegerValue("MissionCategory", 0);
            this._fixVillageObjectData = LogicDataTables.GetVillageObjectByName(this.GetValue("FixVillageObject", 0));

            if (this._fixVillageObjectData != null)
            {
                this._buildBuildingLevel = this.GetIntegerValue("BuildBuildingLevel", 0);
                this._missionType = 13;
            }

            if (string.Equals(this._action, "travel"))
            {
                this._missionType = 14;
            }
            else if (string.Equals(this._action, "upgrade2"))
            {
                this._characterData = LogicDataTables.GetCharacterByName(this.GetValue("Character", 0));
                this._missionType = 17;
            }
            else if (string.Equals(this._action, "duel"))
            {
                this._attackNpcData = LogicDataTables.GetNpcByName(this.GetValue("AttackNPC", 0));
                this._missionType = 18;
            }
            else if (string.Equals(this._action, "duel_end"))
            {
                this._attackNpcData = LogicDataTables.GetNpcByName(this.GetValue("AttackNPC", 0));
                this._missionType = 19;
            }
            else if (string.Equals(this._action, "duel_end2"))
            {
                this._missionType = 20;
            }
            else if (string.Equals(this._action, "show_builder_menu"))
            {
                this._missionType = 21;
            }

            this._buildBuildingData = LogicDataTables.GetBuildingByName(this.GetValue("BuildBuilding", 0));

            if (this._buildBuildingData != null)
            {
                this._buildBuildingCount = this.GetIntegerValue("BuildBuildingCount", 0);
                this._buildBuildingLevel = this.GetIntegerValue("BuildBuildingLevel", 0) - 1;
                this._missionType = string.Equals(this._action, "unlock") ? 15 : 5;

                if (this._buildBuildingCount < 0)
                {
                    Debugger.Error("missions.csv: BuildBuildingCount is invalid!");
                }
            }
            else
            {
                if (this._missionType == -1)
                {
                    this._openAchievements = this.GetBooleanValue("OpenAchievements", 0);

                    if (this._openAchievements)
                    {
                        this._missionType = 7;
                    }
                    else
                    {
                        this._defendNpcData = LogicDataTables.GetNpcByName(this.GetValue("DefendNPC", 0));

                        if (this._defendNpcData != null)
                        {
                            this._missionType = 1;
                        }
                        else
                        {
                            this._attackNpcData = LogicDataTables.GetNpcByName(this.GetValue("AttackNPC", 0));

                            if (this._attackNpcData != null)
                            {
                                this._missionType = 2;
                                this._showMap = this.GetBooleanValue("ShowMap", 0);
                            }
                            else
                            {
                                this._changeName = this.GetBooleanValue("ChangeName", 0);

                                if (this._changeName)
                                {
                                    this._missionType = 6;
                                }
                                else
                                {
                                    this._trainTroopCount = this.GetIntegerValue("TrainTroops", 0);

                                    if (this._trainTroopCount > 0)
                                    {
                                        this._missionType = 4;
                                    }
                                    else
                                    {
                                        this._switchSides = this.GetBooleanValue("SwitchSides", 0);

                                        if (this._switchSides)
                                        {
                                            this._missionType = 8;
                                        }
                                        else
                                        {
                                            this._showWarBase = this.GetBooleanValue("ShowWarBase", 0);

                                            if (this._showWarBase)
                                            {
                                                this._missionType = 9;
                                            }
                                            else
                                            {
                                                this._openInfo = this.GetBooleanValue("OpenInfo", 0);

                                                if (this._openInfo)
                                                {
                                                    this._missionType = 11;
                                                }
                                                else
                                                {
                                                    this._showDonate = this.GetBooleanValue("ShowDonate", 0);

                                                    if (this._showDonate)
                                                    {
                                                        this._missionType = 10;
                                                    }
                                                    else
                                                    {
                                                        this._showStates = this.GetBooleanValue("WarStates", 0);

                                                        if (this._showStates)
                                                        {
                                                            this._missionType = 12;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            this._villagers = this.GetIntegerValue("Villagers", 0);

            if (this._villagers > 0)
            {
                this._missionType = 16;
            }

            this._forceCamera = this.GetBooleanValue("ForceCamera", 0);

            if (this._missionType == -1)
            {
                Debugger.Error(string.Format("missions.csv: invalid mission ({0})", this.GetName()));
            }

            this._rewardResourceData = LogicDataTables.GetResourceByName(this.GetValue("RewardResource", 0));
            this._rewardResourceCount = this.GetIntegerValue("RewardResourceCount", 0);

            if (this._rewardResourceData != null)
            {
                if (this._rewardResourceCount != 0)
                {
                    if (this._rewardResourceCount < 0)
                    {
                        Debugger.Error("missions.csv: RewardResourceCount is negative!");

                        this._rewardResourceData = null;
                        this._rewardResourceCount = 0;
                    }
                }
                else
                {
                    this._rewardResourceData = null;
                }
            }
            else if (this._rewardResourceCount != 0)
            {
                Debugger.Warning("missions.csv: RewardResourceCount defined but RewardResource is not!");
                this._rewardResourceCount = 0;
            }

            this._customData = this.GetIntegerValue("CustomData", 0);
            this._rewardXP = this.GetIntegerValue("RewardXP", 0);

            if (this._rewardXP < 0)
            {
                Debugger.Warning("missions.csv: RewardXP is negative!");
                this._rewardXP = 0;
            }

            this._rewardCharacterData = LogicDataTables.GetCharacterByName(this.GetValue("RewardTroop", 0));
            this._rewardCharacterCount = this.GetIntegerValue("RewardTroopCount", 0);

            if (this._rewardCharacterData != null)
            {
                if (this._rewardCharacterCount != 0)
                {
                    if (this._rewardCharacterCount < 0)
                    {
                        Debugger.Error("missions.csv: RewardTroopCount is negative!");

                        this._rewardCharacterData = null;
                        this._rewardCharacterCount = 0;
                    }
                }
                else
                {
                    this._rewardCharacterData = null;
                }
            }
            else if (this._rewardCharacterCount != 0)
            {
                Debugger.Warning("missions.csv: RewardTroopCount defined but RewardTroop is not!");
                this._rewardCharacterCount = 0;
            }

            this._delay = this.GetIntegerValue("Delay", 0);
            this._villageType = this.GetIntegerValue("VillageType", 0);
            this._firstStep = this.GetBooleanValue("FirstStep", 0);
            this._tutorialText = this.GetValue("TutorialText", 0);

            if (this._tutorialText.Length > 0)
            {
                // BLABLABLA
            }
        }

        /// <summary>
        ///     Gets the mission type.
        /// </summary>
        public int GetMissionType()
        {
            return this._missionType;
        }

        /// <summary>
        ///     Gets the build building data.
        /// </summary>
        public LogicBuildingData GetBuildBuildingData()
        {
            return this._buildBuildingData;
        }

        /// <summary>
        ///     Gets the build building level.
        /// </summary>
        public int GetBuildBuildingLevel()
        {
            return this._buildBuildingLevel;
        }

        /// <summary>
        ///     Gets the train troop count.
        /// </summary>
        public int GetTrainTroopCount()
        {
            return this._trainTroopCount;
        }

        /// <summary>
        ///     Gets the mission category.
        /// </summary>
        public int GetMissionCategory()
        {
            return this._missionCategory;
        }

        /// <summary>
        ///     Gets the village type.
        /// </summary>
        public override int GetVillageType()
        {
            return this._villageType;
        }
    }
}