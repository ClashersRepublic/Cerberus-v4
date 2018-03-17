namespace ClashersRepublic.Magic.Logic.Mission
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicMission
    {
        private LogicLevel _level;
        private LogicMissionData _data;

        private int _progress;
        private int _requireProgress;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMission"/> class.
        /// </summary>
        public LogicMission(LogicMissionData data, LogicLevel level)
        {
            if (data == null)
            {
                Debugger.Error("LogicMission::constructor - pData is NULL!");
            }

            this._data = data;
            this._level = level;
            this._requireProgress = 1;

            switch (data.GetMissionType())
            {
                case 1:
                    this._requireProgress = 2;
                    break;
                case 0:
                case 5:
                    this._requireProgress = data.GetBuildBuildingLevel();
                    break;
                case 4:
                    this._requireProgress = data.GetTrainTroopCount();
                    break;
            }

            if (data.GetMissionCategory() == 1)
            {
                // MMh.
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._data = null;
            this._level = null;
            this._progress = 0;
            this._requireProgress = 0;
        }

        /// <summary>
        ///     Gets the mission type.
        /// </summary>
        public int GetMissionType()
        {
            return this._data.GetMissionType();
        }

        /// <summary>
        ///     Refreshes the mission progress.
        /// </summary>
        public void RefreshProgress()
        {
            LogicGameObjectManager gameObjectManager = this._level.GetGameObjectManager();

            switch (this._data.GetMissionType())
            {
                case 0:
                case 5:
                    this._progress = 0;

                    if (this._level.GetState() == 1)
                    {
                        LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(0);

                        for (int i = 0; i < gameObjects.Count; i++)
                        {
                            LogicBuilding building = (LogicBuilding) gameObjects[i];

                            if (building.GetBuildingData() == this._data.GetBuildBuildingData())
                            {
                                if (!building.IsConstructing() || building.IsUpgrading())
                                {
                                    if (building.GetUpgradeLevel() >= this._data.GetBuildBuildingLevel())
                                    {
                                        ++this._progress;
                                    }
                                }
                            }
                        }
                    }

                    break;

                case 4:
                    this._progress = this._level.GetPlayerAvatar().GetUnitsTotalCapacity();
                    break;
                case 6:
                    this._progress = this._level.GetPlayerAvatar().GetNameSetByUser() ? 1 : 0;
                    break;
                case 13:
                    if (this._level.GetState() == 1)
                    {
                        LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(8);

                    }

                    break;
            }
        }
    }
}