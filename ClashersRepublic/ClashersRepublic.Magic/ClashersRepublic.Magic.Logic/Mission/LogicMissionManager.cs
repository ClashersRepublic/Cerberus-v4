namespace ClashersRepublic.Magic.Logic.Mission
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicMissionManager
    {
        private LogicLevel _level;
        private LogicArrayList<LogicMission> _openMissions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionManager"/> class.
        /// </summary>
        public LogicMissionManager()
        {
            // LogicMissionManager.
        }

        /// <summary>
        ///     Initializes the instance.
        /// </summary>
        public void Init(LogicLevel level)
        {
            this._level = level;
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
                    }
                    else
                    {
                        mission.Tick();
                    }
                }
            }

            if (this._openMissions.Count > 0)
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

                    if(missionData.Is)
                }
            }
        }
    }
}