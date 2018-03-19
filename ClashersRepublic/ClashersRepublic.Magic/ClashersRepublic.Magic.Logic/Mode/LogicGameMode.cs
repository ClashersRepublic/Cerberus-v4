namespace ClashersRepublic.Magic.Logic.Mode
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Calendar;
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicGameMode
    {
        private bool _battleOver;

        private int _state;
        private int _currentTimestamp;

        private LogicTimer _battleTimer;
        private LogicLevel _level;
        private LogicCommandManager _commandManager;
        private LogicGameListener _gameListener;
        private LogicCalendar _calendar;

        private LogicTimer _shieldTimer;
        private LogicTimer _guardTimer;
        private LogicTimer _maintenanceTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameMode" /> class.
        /// </summary>
        public LogicGameMode()
        {
            this._level = new LogicLevel(this);
            this._commandManager = new LogicCommandManager(this._level);
            this._calendar = new LogicCalendar();
            this._shieldTimer = new LogicTimer();
            this._guardTimer = new LogicTimer();
            this._maintenanceTimer = new LogicTimer();
            this._currentTimestamp = -1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._level != null)
            {
                this._level.Destruct();
                this._level = null;
            }

            if (this._commandManager != null)
            {
                this._commandManager.Destruct();
                this._commandManager = null;
            }

            if (this._calendar != null)
            {
                this._calendar.Destruct();
                this._calendar = null;
            }

            this._maintenanceTimer = null;
            this._battleTimer = null;
            this._shieldTimer = null;
            this._guardTimer = null;
        }

        /// <summary>
        ///     Calculates the checksum of this instance.
        /// </summary>
        public ChecksumHelper CalculateChecksum(int mode)
        {
            ChecksumHelper checksum = new ChecksumHelper();

            checksum.StartObject("LogicGameMode");

            checksum.WriteValue("subTick", this._level.GetLogicTime());
            checksum.WriteValue("m_currentTimestamp", this._currentTimestamp);

            if (this._level.GetHomeOwnerAvatar() != null)
            {
                checksum.StartObject("homeOwner");
                this._level.GetHomeOwnerAvatar().GetChecksum(checksum);
                checksum.EndObject();
            }

            if (this._level.GetVisitorAvatar() != null)
            {
                checksum.StartObject("visitor");
                this._level.GetVisitorAvatar().GetChecksum(checksum);
                checksum.EndObject();
            }

            this._level.GetGameObjectManager().GetChecksum(checksum, mode);

            if (this._calendar != null)
            {
                checksum.StartObject("calendar");
                this._calendar.GetChecksum(checksum);
                checksum.EndObject();
            }

            checksum.WriteValue("checksum", checksum);
            checksum.EndObject();

            return checksum;
        }

        /// <summary>
        ///     Gets the command manager instance.
        /// </summary>
        public LogicCommandManager GetCommandManager()
        {
            return this._commandManager;
        }

        /// <summary>
        ///     Gets the level instance.
        /// </summary>
        public LogicLevel GetLevel()
        {
            return this._level;
        }

        /// <summary>
        ///     Gets the current time.
        /// </summary>
        public int GetCurrentTime()
        {
            return (int) (16L * this._level.GetLogicTime() / 1000 + this._currentTimestamp);
        }

        /// <summary>
        ///     Gets the current timestamp.
        /// </summary>
        public int GetCurrentTimestamp()
        {
            return this._currentTimestamp;
        }

        /// <summary>
        ///     Sets the battle over.
        /// </summary>
        public void SetBattleOver()
        {
        }

        /// <summary>
        ///     Gets the game state.
        /// </summary>
        public int GetState()
        {
            return this._state;
        }

        /// <summary>
        ///     Gets the remaining shield time.
        /// </summary>
        public int GetShieldRemainingSeconds()
        {
            return this._shieldTimer.GetRemainingSeconds(this._level.GetLogicTime());
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void SaveToJSON(LogicJSONObject jsonObject)
        {
            this._level.SaveToJSON(jsonObject);
        }

        /// <summary>
        ///     Updates a one tick.
        /// </summary>
        public void UpdateOneSubTick()
        {
            LogicTime time = this._level.GetLogicTime();

            if (this._state != 2 || !this._battleOver)
            {
                this._commandManager.SubTick();
                this._level.SubTick();

                // if (this._replay != null)
                // {
                //     this._replay.SubTick();
                // }

                if (time.IsFullTick())
                {
                    this._level.Tick();
                }
            }

            if (this._state == 2 || this._state == 3 || this._state == 5)
            {
                if (this._battleTimer != null &&
                    this._battleTimer.GetRemainingSeconds(time) == 0 ||
                    this._level.GetBattleEndPending())
                {
                    this.SetBattleOver();
                }
            }

            time.IncreaseTick();
        }

        /// <summary>
        ///     Loads the home state.
        /// </summary>
        public void LoadHomeState(LogicClientHome home, LogicAvatar homeOwnerAvatar, int currentTimestamp, int secondsSinceLastSave)
        {
            if (home != null)
            {
                this._state = 1;
                this._currentTimestamp = currentTimestamp;
                this._level.SetHome(home, true);
                this._level.SetHomeOwnerAvatar(homeOwnerAvatar);
                this._level.FastForwardTime(secondsSinceLastSave);

                this._shieldTimer.StartTimer(home.GetShieldDurationSeconds(), this._level.GetLogicTime(), false, -1);
                this._guardTimer.StartTimer(home.GetGuardDurationSeconds(), this._level.GetLogicTime(), false, -1);
                this._maintenanceTimer.StartTimer(home.GetNextMaintenanceSeconds(), this._level.GetLogicTime(), false, -1);
            }
        }
    }
}