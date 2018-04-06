namespace RivieraStudio.Magic.Logic.Data
{
    using System;
    using RivieraStudio.Magic.Titan.CSV;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicAchievementData : LogicData
    {
        private bool _showValue;

        private int _uiGroup;
        private int _actionType;
        private int _diamondReward;
        private int _expReward;
        private int _actionCount;
        private int _level;
        private int _levelCount;

        private string _completedTID;
        private string _androidId;

        private LogicBuildingData _buildingData;
        private LogicResourceData _resourceData;
        private LogicCharacterData _characterData;
        private LogicArrayList<LogicAchievementData> _achievementLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAchievementData" /> class.
        /// </summary>
        public LogicAchievementData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicAchievementData.
        }
        
        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            this._villageType = this.GetIntegerValue("UIGroup", 0);
            this._diamondReward = this.GetIntegerValue("DiamondReward", 0);
            this._expReward = this.GetIntegerValue("ExpReward", 0);
            this._actionCount = this.GetIntegerValue("ActionCount", 0);
            this._level = this.GetIntegerValue("Level", 0);
            this._levelCount = this.GetIntegerValue("LevelCount", 0);

            this._completedTID = this.GetValue("CompetedTID", 0);
            this._showValue = this.GetBooleanValue("ShowValue", 0);
            this._androidId = this.GetValue("AndroidID", 0);

            if (this._actionCount == 0)
            {
                Debugger.Error("Achievement has invalid ActionCount 0");
            }

            string action = this.GetValue("Action", 0);

            switch (action)
            {
                case "npc_stars":
                    this._actionType = 0;
                    break;
                case "upgrade":
                    this._actionType = 1;
                    this._buildingData = LogicDataTables.GetBuildingByName(this.GetValue("ActionData", 0));

                    if (this._buildingData == null)
                    {
                        Debugger.Error("LogicAchievementData - Building data is NULL for upgrade achievement");
                    }

                    break;
                case "victory_points":
                    this._actionType = 2;
                    break;
                case "unit_unlock":
                    this._actionType = 3;
                    this._characterData = LogicDataTables.GetCharacterByName(this.GetValue("ActionData", 0));

                    if (this._characterData == null)
                    {
                        Debugger.Error("LogicCharacterData - Character data is NULL for unit_unlock achievement");
                    }

                    break;
                case "clear_obstacles":
                    this._actionType = 4;
                    break;
                case "donate_units":
                    this._actionType = 5;
                    break;
                case "loot":
                    this._actionType = 6;
                    this._resourceData = LogicDataTables.GetResourceByName(this.GetValue("ActionData", 0));

                    if (this._resourceData == null)
                    {
                        Debugger.Error("LogicAchievementData - Resource data is NULL for loot achievement");
                    }
                    
                    break;
                case "destroy":
                    this._actionType = 9;
                    this._buildingData = LogicDataTables.GetBuildingByName(this.GetValue("ActionData", 0));

                    if (this._buildingData == null)
                    {
                        Debugger.Error("LogicAchievementData - Building data is NULL for destroy achievement");
                    }

                    break;
                case "win_pvp_attack":
                    this._actionType = 10;
                    break;
                case "win_pvp_defense":
                    this._actionType = 11;
                    break;
                case "league":
                    this._actionType = 12;
                    break;
                case "war_stars":
                    this._actionType = 13;
                    break;
                case "war_loot":
                    this._actionType = 14;
                    break;
                case "donate_spells":
                    this._actionType = 15;
                    break;
                case "account_bound":
                    this._actionType = 16;
                    break;
                case "vs_battle_trophies":
                    this._actionType = 17;
                    break;
                case "gear_up":
                    this._actionType = 18;
                    break;
                case "repair_building":
                    this._actionType = 19;
                    this._buildingData = LogicDataTables.GetBuildingByName(this.GetValue("ActionData", 0));

                    if (this._buildingData == null)
                    {
                        Debugger.Error("LogicAchievementData - Building data is NULL for repair_building achievement");
                    }

                    break;
                default:
                    Debugger.Error(string.Format("Unknown Action in achievements {0}", action));
                    break;
            }

            this._achievementLevel = new LogicArrayList<LogicAchievementData>();

            String achievementName = this.GetName().Substring(0, this.GetName().Length - 1);
            LogicDataTable table = LogicDataTables.GetTable(22);

            for (int i = 0; i < table.GetItemCount(); i++)
            {
                LogicAchievementData achievementData = (LogicAchievementData) table.GetItemAt(i);

                if (achievementData.GetName().Contains(achievementName))
                {
                    if (achievementData.GetName().Substring(0, achievementData.GetName().Length - 1).Equals(achievementName))
                    {
                        this._achievementLevel.Add(achievementData);
                    }
                }
            }

            Debugger.DoAssert(this._achievementLevel.Count == this._levelCount, string.Format("Expected same amount of achievements named {0}X to be same as LevelCount={1} for {2}.", 
                                                                                              achievementName, 
                                                                                              this._levelCount, 
                                                                                              this.GetName()));
        }

        /// <summary>
        ///     Gets the action type.
        /// </summary>
        public int GetActionType()
        {
            return this._actionType;
        }

        /// <summary>
        ///     Gets the diamond reward.
        /// </summary>
        public int GetDiamondReward()
        {
            return this._diamondReward;
        }

        /// <summary>
        ///     Gets the exp reward.
        /// </summary>
        public int GetExpReward()
        {
            return this._expReward;
        }

        /// <summary>
        ///     Gets the action count.
        /// </summary>
        public int GetActionCount()
        {
            return this._actionCount;
        }

        /// <summary>
        ///     Gets the achievement level.
        /// </summary>
        public int GetLevel()
        {
            return this._level;
        }

        /// <summary>
        ///     Gets the completed tid.
        /// </summary>
        public string GetCompletedTID()
        {
            return this._completedTID;
        }

        /// <summary>
        ///     Gets the android id.
        /// </summary>
        public string GetAndroidID()
        {
            return this._androidId;
        }

        /// <summary>
        ///     Gets the achievement levels.
        /// </summary>
        public LogicArrayList<LogicAchievementData> GetAchievementLevels()
        {
            return this._achievementLevel;
        }

        /// <summary>
        ///     Gets the building data.
        /// </summary>
        public LogicBuildingData GetBuildingData()
        {
            return this._buildingData;
        }

        /// <summary>
        ///     Gets the resource data.
        /// </summary>
        public LogicResourceData GetResourceData()
        {
            return this._resourceData;
        }

        /// <summary>
        ///     Gets the character data.
        /// </summary>
        public LogicCharacterData GetCharacterData()
        {
            return this._characterData;
        }
    }
}