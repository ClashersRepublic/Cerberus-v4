namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicMissionData : LogicData
    {
        private int _actionType;
        private int _buildBuildingCount;
        private int _buildBuildingLevel;
        private int _trainTroopCount;
        private int _villagers;
        private int _rewardResourceCount;
        private int _customData;
        private int _rewardXP;
        private int _rewardCharacterCount;
        private int _delay ;

        private bool _openAchievements;
        private bool _showMap;
        private bool _changeName;
        private bool _switchSides;
        private bool _showWarBase;
        private bool _openInfo;
        private bool _showDonate;
        private bool _warStates;
        private bool _forceCamera;

        private LogicNpcData _defendNpcData;
        private LogicNpcData _attackNpcData;
        private LogicBuildingData _buildBuildingData;
        private LogicResourceData _rewardCharacterData;
        private LogicResourceData _rewardResourceData;
        private LogicArrayList<LogicMissionData> _missionDependencies;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionData" /> class.
        /// </summary>
        public LogicMissionData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            this._actionType = -1;
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

            this._buildBuildingData = LogicDataTables.GetBuildingDataByName(this.GetValue("BuildBuilding", 0));

            if (this._buildBuildingData != null)
            {
                this._buildBuildingCount = this.GetIntegerValue("BuildBuildingCount", 0);
                this._buildBuildingLevel = this.GetIntegerValue("BuildBuildingLevel", 0) - 1;

                if (this._buildBuildingLevel != 0)
                {
                    this._actionType = 5;
                }

                if (this._buildBuildingCount < 0)
                {
                    Debugger.Error("missions.csv: BuildBuildingCount is invalid!");
                }
            }
            else
            {
                if (this._actionType == -1)
                {
                    this._openAchievements = this.GetBooleanValue("OpenAchievements", 0);

                    if (this._openAchievements)
                    {
                        this._actionType = 7;
                    }
                    else
                    {
                        this._defendNpcData = LogicDataTables.GetNpcByName(this.GetValue("DefendNPC", 0));

                        if (this._defendNpcData != null)
                        {
                            this._actionType = 1;
                        }
                        else
                        {
                            this._attackNpcData = LogicDataTables.GetNpcByName(this.GetValue("AttackNPC", 0));

                            if (this._attackNpcData != null)
                            {
                                this._actionType = 2;
                                this._showMap = this.GetBooleanValue("ShowMap", 0);
                            }
                            else
                            {
                                this._changeName = this.GetBooleanValue("ChangeName", 0);

                                if (this._changeName)
                                {
                                    this._actionType = 6;
                                }
                                else
                                {
                                    this._trainTroopCount = this.GetIntegerValue("TrainTroops", 0);

                                    if (this._trainTroopCount > 0)
                                    {
                                        this._actionType = 4;
                                    }
                                    else
                                    {
                                        this._switchSides = this.GetBooleanValue("SwitchSides", 0);

                                        if (this._switchSides)
                                        {
                                            this._actionType = 8;
                                        }
                                        else
                                        {
                                            this._showWarBase = this.GetBooleanValue("ShowWarBase", 0);

                                            if (this._showWarBase)
                                            {
                                                this._actionType = 9;
                                            }
                                            else
                                            {
                                                this._openInfo = this.GetBooleanValue("OpenInfo", 0);

                                                if (this._openInfo)
                                                {
                                                    this._actionType = 11;
                                                }
                                                else
                                                {
                                                    this._showDonate = this.GetBooleanValue("ShowDonate", 0);

                                                    if (this._showDonate)
                                                    {
                                                        this._actionType = 10;
                                                    }
                                                    else
                                                    {
                                                        this._showWarBase = this.GetBooleanValue("WarStates", 0);
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
    }
}