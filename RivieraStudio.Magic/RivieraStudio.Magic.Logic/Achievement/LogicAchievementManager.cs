namespace RivieraStudio.Magic.Logic.Achievement
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicAchievementManager
    {
        private LogicLevel _level;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAchievementManager" /> class.
        /// </summary>
        public LogicAchievementManager(LogicLevel level)
        {
            this._level = level;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._level = null;
        }

        /// <summary>
        ///     Called when a obstacle is cleared.
        /// </summary>
        public void ObstacleCleared()
        {
            LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();
            LogicDataTable dataTable = LogicDataTables.GetTable(22);

            if (homeOwnerAvatar != null)
            {
                if (homeOwnerAvatar.IsClientAvatar())
                {
                    LogicClientAvatar clientAvatar = (LogicClientAvatar) homeOwnerAvatar;

                    for (int i = 0; i < dataTable.GetItemCount(); i++)
                    {
                        LogicAchievementData achievementData = (LogicAchievementData)dataTable.GetItemAt(i);

                        if (achievementData.GetActionType() == 4)
                        {
                            this.RefreshAchievementProgress(clientAvatar, achievementData, clientAvatar.GetAchievementProgress(achievementData) + 1);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Refreshes status of all achievements.
        /// </summary>
        public void RefreshStatus()
        {
            if (this._level.GetState() == 1)
            {
                LogicClientAvatar playerAvatar = this._level.GetPlayerAvatar();
                LogicDataTable dataTable = LogicDataTables.GetTable(22);

                for (int i = 0, progress = 0; i < dataTable.GetItemCount(); i++, progress = 0)
                {
                    LogicAchievementData achievementData = (LogicAchievementData) dataTable.GetItemAt(i);

                    switch (achievementData.GetActionType())
                    {
                        case 0:
                            progress = playerAvatar.GetTotalNpcStars();
                            break;
                        case 1:
                            progress = this._level.GetGameObjectManager().GetHighestBuildingLevel(achievementData.GetBuildingData()) + 1;
                            break;
                        case 2:
                            progress = playerAvatar.GetScore();
                            break;
                        case 3:
                            progress = achievementData.GetCharacterData().IsUnlockedForBarrackLevel(LogicMath.Max(this._level.GetComponentManagerAt(achievementData.GetVillageType()).GetMaxBarrackLevel(), 0)) ? 1 : 0;
                            break;
                        case 12:
                            progress = playerAvatar.GetLeagueType();
                            break;
                        case 16:
                            progress = playerAvatar.IsAccountBound() ? 1 : 0;
                            break;
                        case 17:
                            progress = playerAvatar.GetDuelScore();
                            break;
                        case 18:
                            progress = this._level.GetGameObjectManager().GetGearUpBuildingCount();
                            break;
                        case 19:
                            LogicArrayList<LogicAchievementData> achievementLevels = achievementData.GetAchievementLevels();

                            if (achievementLevels.Count <= 0)
                            {
                                break;
                            }

                            for (int j = 0; j < achievementLevels.Count; j++)
                            {
                                if (!this._level.IsBuildingUnlocked(achievementLevels[j].GetBuildingData()))
                                {
                                    goto refresh;
                                }
                            }

                            progress = 1;

                            break;
                    }

                    refresh:
                    this.RefreshAchievementProgress(playerAvatar, achievementData, progress);
                }
            }
        }

        /// <summary>
        ///     Refreshes status of all achievements.
        /// </summary>
        public void RefreshAchievementProgress(LogicClientAvatar avatar, LogicAchievementData data, int value)
        {
            if (this._level.GetState() != 5)
            {
                int currentValue = avatar.GetAchievementProgress(data);
                int newValue = LogicMath.Min(value, 2000000000);

                if (currentValue < newValue)
                {
                    avatar.SetAchievementProgress(data, value);
                    avatar.GetChangeListener().CommodityCountChanged(0, data, newValue);
                }

                int tmp = LogicMath.Min(newValue, data.GetActionCount());

                if (currentValue < tmp)
                {
                    LogicClientAvatar playerAvatar = this._level.GetPlayerAvatar();

                    if (playerAvatar == avatar)
                    {
                        if (tmp == data.GetActionCount())
                        {
                            this._level.GetGameListener().AchievementCompleted(data);
                        }
                        else
                        {
                            this._level.GetGameListener().AchievementProgress(data);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
        }
    }
}