namespace RivieraStudio.Magic.Logic.Mission
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicMissionManager
    {
        private LogicLevel _level;
        private LogicArrayList<LogicMission> _openMissions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionManager"/> class.
        /// </summary>
        public LogicMissionManager(LogicLevel level)
        {
            this._level = level;
            this._openMissions = new LogicArrayList<LogicMission>();
        }
        
        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._openMissions != null)
            {
                if (this._openMissions.Count != 0)
                {
                    do
                    {
                        this._openMissions[0].Destruct();
                        this._openMissions.Remove(0);
                    } while (this._openMissions.Count != 0);
                }

                this._openMissions = null;
            }

            this._level = null;
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            bool refresh = false;

            for (int i = 0; i < this._openMissions.Count; i++)
            {
                LogicMission mission = this._openMissions[i];

                if (mission != null)
                {
                    mission.RefreshProgress();

                    if (mission.IsFinished())
                    {
                        mission.Destruct();
                        this._openMissions.Remove(i--);
                        refresh = true;
                    }
                    else
                    {
                        mission.Tick();
                    }
                }
            }

            if (refresh)
            {
                this.RefreshOpenMissions();
            }
        }

        /// <summary>
        ///     Refreshes all open missions.
        /// </summary>
        public void RefreshOpenMissions()
        {
            if (this._level.GetState() != 4)
            {
                LogicClientAvatar playerAvatar = this._level.GetPlayerAvatar();
                LogicDataTable missionTable = LogicDataTables.GetTable(20);

                for (int i = 0; i < missionTable.GetItemCount(); i++)
                {
                    LogicMissionData missionData = (LogicMissionData) missionTable.GetItemAt(i);

                    if (missionData.IsOpenForAvatar(playerAvatar))
                    {
                        int index = -1;

                        for (int j = 0; j < this._openMissions.Count; j++)
                        {
                            if (this._openMissions[j].GetMissionData() == missionData)
                            {
                                index = j;
                                break;
                            }
                        }

                        if (index == -1)
                        {
                            LogicMission mission = new LogicMission(missionData, this._level);
                            mission.RefreshProgress();
                            this._openMissions.Add(mission);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the mission by data.
        /// </summary>
        public LogicMission GetMissionByData(LogicMissionData data)
        {
            for (int i = 0; i < this._openMissions.Count; i++)
            {
                LogicMission mission = this._openMissions[i];

                if (mission.GetMissionData() == data)
                {
                    return mission;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the mission by category.
        /// </summary>
        public LogicMission GetMissionByCategory(int category)
        {
            for (int i = 0; i < this._openMissions.Count; i++)
            {
                LogicMission mission = this._openMissions[i];

                if (mission.GetMissionData().GetMissionCategory() == category)
                {
                    return mission;
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets if the tutorial is finished.
        /// </summary>
        public bool IsTutorialFinished()
        {
            for (int i = 0; i < this._openMissions.Count; i++)
            {
                if (this._openMissions[i].GetMissionData().GetMissionCategory() == 0)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets if the village 2 tutorial is open.
        /// </summary>
        public bool IsVillage2TutorialOpen()
        {
            int openMissionCount = 0;

            for (int i = 0; i < this._openMissions.Count; i++)
            {
                if (this._openMissions[i].GetMissionData().GetMissionCategory() == 2)
                {
                    if (this._openMissions[i].IsOpenTutorialMission())
                    {
                        openMissionCount += 1;
                    }
                }
            }

            if (this._level.GetGameObjectManagerAt(0).GetShipyard() == null)
            {
                return false;
            }

            return this._level.GetGameObjectManagerAt(0).GetShipyard().IsConstructing() || openMissionCount > 0;
        }

        /// <summary>
        ///     Gets if the player has travel.
        /// </summary>
        public bool HasTravel(LogicAvatar playerAvatar)
        {
            LogicDataTable missionTable = LogicDataTables.GetTable(20);

            for (int i = 0; i < missionTable.GetItemCount(); i++)
            {
                LogicMissionData missionData = (LogicMissionData)missionTable.GetItemAt(i);

                if (missionData.GetMissionType() == 23)
                {
                    if (playerAvatar.IsMissionCompleted(missionData))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}