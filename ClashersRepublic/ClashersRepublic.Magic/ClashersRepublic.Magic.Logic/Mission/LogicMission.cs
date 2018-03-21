namespace ClashersRepublic.Magic.Logic.Mission
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
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
                    this._requireProgress = data.GetBuildBuildingCount();
                    break;
                case 4:
                    this._requireProgress = data.GetTrainTroopCount();
                    break;
                case 18:
                    this._requireProgress = 2;
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
        ///     Gets the mission data.
        /// </summary>
        public LogicMissionData GetMissionData()
        {
            return this._data;
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
                    this._progress = 0;

                    if (this._level.GetState() == 1)
                    {
                        LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(8);

                        for (int i = 0; i < gameObjects.Count; i++)
                        {
                            LogicVillageObject villageObject = (LogicVillageObject) gameObjects[i];

                            if (villageObject.GetVillageObjectData() == this._data.GetFixVillageObjectData() &&
                                villageObject.GetUpgradeLevel() >= this._data.GetBuildBuildingLevel())
                            {
                                ++this._progress;
                            }
                        }
                    }

                    break;
                case 14:
                    this._progress = 0;

                    if (this._level.GetState() == 1 && this._level.GetVillageType() == 1)
                    {
                        ++this._progress;
                    }

                    break;
                case 15:
                    this._progress = 0;

                    if (this._level.GetState() == 1)
                    {
                        LogicArrayList<LogicGameObject> gameObjects = gameObjectManager.GetGameObjects(0);

                        for (int i = 0; i < gameObjects.Count; i++)
                        {
                            LogicBuilding building = (LogicBuilding) gameObjects[i];

                            if (building.GetBuildingData() == this._data.GetBuildBuildingData())
                            {
                                if (!building.IsLocked())
                                {
                                    ++this._progress;
                                }
                            }
                        }
                    }

                    break;
                case 17:
                    this._progress = 0;

                    if (this._level.GetState() == 1 && this._level.GetVillageType() == 1)
                    {
                        if (this._level.GetPlayerAvatar().GetUnitUpgradeLevel(this._data.GetCharacterData()) > 0)
                        {
                            ++this._progress;
                        }
                    }

                    break;
            }

            if (this._progress >= this._requireProgress)
            {
                this._progress = this._requireProgress;
                this.Finished();
            }
        }

        /// <summary>
        ///     Called when the change of state is confirmed.
        /// </summary>
        public void StateChangeConfirmed()
        {
            switch (this._data.GetMissionType())
            {
                case 1:
                    if (this._progress == 0)
                    {
                        this._level.GetGameMode().StartDefendState(LogicNpcAvatar.GetNpcAvatar(this._data.GetDefendNpcData()));
                        this._progress = 1;
                    }

                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 20:
                case 21:
                    this._progress = 1;
                    break;
            }
        }

        /// <summary>
        ///     Called when the mission is finished.
        /// </summary>
        public void Finished()
        {
            LogicClientAvatar playerAvatar = this._level.GetPlayerAvatar();

            if (!playerAvatar.IsMissionCompleted(this._data))
            {
                Debugger.Log("Mission " + this._data.GetName() + " finished");

                playerAvatar.SetMissionCompleted(this._data, true);
                playerAvatar.GetChangeListener().CommodityCountChanged(0, this._data.GetRewardResourceData(), 1);

                this.AddRewardUnits();

                LogicResourceData rewardResourceData = this._data.GetRewardResourceData();

                if (rewardResourceData != null)
                {
                    playerAvatar.AddMisisonResourceReward(rewardResourceData, this._data.GetRewardResourceCount());
                }

                int rewardXp = this._data.GetRewardXp();

                if (rewardXp > 0)
                {
                    playerAvatar.XpGainHelper(rewardXp);
                }
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            int missionType = this._data.GetMissionType();

            if (missionType <= 17)
            {
                if (missionType != 1)
                {
                    if (missionType == 2)
                    {
                        LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                        if (homeOwnerAvatar.IsNpcAvatar())
                        {
                            if (this._level.GetState() == 2)
                            {
                                this.Finished();
                                this._level.GetGameListener().ShowTroopPlacementTutorial(this._data.GetCustomData());
                            }
                        }
                    }
                }
                else
                {
                    if (this._level.GetState() == 1)
                    {
                        if (this._progress == 1)
                        {
                            this.Finished();
                        }
                    }
                }
            }
            else if (missionType == 19)
            {
                if (this._level.GetState() == 1)
                {
                    this._progress = 1;
                }
            }
            else if (missionType == 18)
            {
                if (this._progress == 0)
                {
                    LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                    if (homeOwnerAvatar.IsNpcAvatar())
                    {
                        if (this._level.GetState() == 2)
                        {
                            this._progress = 1;
                            this._level.GetGameListener().ShowTroopPlacementTutorial(this._data.GetCustomData());
                        }
                    }
                }
                else if (this._progress == 1)
                {
                    LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                    if (homeOwnerAvatar.IsNpcAvatar())
                    {
                        if (this._level.GetState() == 2)
                        {
                            if (this._level.GetBattleLog().GetBattleEnded())
                            {
                                this._progress = 2;
                                this.Finished();
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this mission is finished.
        /// </summary>
        public bool IsFinished()
        {
            return this._progress >= this._requireProgress;
        }

        /// <summary>
        ///     Adds the reward units.
        /// </summary>
        public void AddRewardUnits()
        {
            LogicCharacterData characterData = this._data.GetRewardCharacterData();

            if (characterData != null)
            {
                int characterCount = this._data.GetRewardCharacterCount();

                if (characterCount > 0)
                {
                    LogicClientAvatar playerAvatar = this._level.GetPlayerAvatar();
                    LogicComponentFilter filter = new LogicComponentFilter();

                    for (int i = 0; i < characterCount; i++)
                    {
                        filter.RemoveAllIgnoreObjects();

                        while (true)
                        {
                            LogicUnitStorageComponent component = (LogicUnitStorageComponent) this._level.GetComponentManagerAt(this._level.GetVillageType()).GetClosestComponent(0, 0, filter);

                            if (component != null)
                            {
                                if (component.CanAddUnit(characterData))
                                {
                                    playerAvatar.CommodityCountChangeHelper(0, characterData, 1);
                                    component.AddUnit(characterData);

                                    if (this._level.GetState() == 1 || this._level.GetState() == 3)
                                    {
                                        if (component.GetParentListener() != null)
                                        {
                                            component.GetParentListener().ExtraCharacterAdded(characterData, null);
                                        }
                                    }

                                    break;
                                }

                                filter.AddIgnoreObject(component.GetParent());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}