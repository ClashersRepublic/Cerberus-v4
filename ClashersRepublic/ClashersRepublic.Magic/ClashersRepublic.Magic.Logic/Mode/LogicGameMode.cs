namespace ClashersRepublic.Magic.Logic.Mode
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Calendar;
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

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
        private LogicConfiguration _configuration;

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
            this._configuration = new LogicConfiguration();
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

            if (this._maintenanceTimer != null)
            {
                this._maintenanceTimer.Destruct();
                this._maintenanceTimer = null;
            }

            if (this._battleTimer != null)
            {
                this._battleTimer.Destruct();
                this._battleTimer = null;
            }

            if (this._shieldTimer != null)
            {
                this._shieldTimer.Destruct();
                this._shieldTimer = null;
            }

            if (this._guardTimer != null)
            {
                this._guardTimer.Destruct();
                this._guardTimer = null;
            }

            this._configuration = null;
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
        ///     Gets the configuration instance.
        /// </summary>
        public LogicConfiguration GetConfiguration()
        {
            return this._configuration;
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
        ///     Starts the defend state.
        /// </summary>
        public void StartDefendState(LogicAvatar avatar)
        {
            if (this._state == 1)
            {
                this._state = 3;
                this._battleOver = false;
                this._level.DefenseStateStarted(avatar);
            }
            else
            {
                Debugger.Error("startDefendState called from invalid state");
            }
        }

        /// <summary>
        ///     Ends the defend state.
        /// </summary>
        public void EndDefendState()
        {
            if (this._state == 3)
            {
                this._state = 1;
                this._level.DefenseStateEnded();
            }
            else
            {
                Debugger.Error("endDefendState called from invalid state");
            }
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
                this._calendar.Load(home.GetCalendarJSON(), currentTimestamp);

                if (this._battleTimer != null)
                {
                    this._battleTimer.Destruct();
                    this._battleTimer = null;
                }

                this._level.SetHome(home, true);
                this._level.SetHomeOwnerAvatar(homeOwnerAvatar);
                this._level.FastForwardTime(secondsSinceLastSave);
                this._level.LoadingFinished();

                this._shieldTimer.StartTimer(home.GetShieldDurationSeconds(), this._level.GetLogicTime(), false, -1);
                this._guardTimer.StartTimer(home.GetGuardDurationSeconds(), this._level.GetLogicTime(), false, -1);
                this._maintenanceTimer.StartTimer(home.GetNextMaintenanceSeconds(), this._level.GetLogicTime(), false, -1);
            }
        }

        /// <summary>
        ///     Loads the npc attack state.
        /// </summary>
        public void LoadNpcAttackState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave)
        {
            if (this._state == 1)
            {
                Debugger.Error("loadAttackState called from invalid state");
            }
            else
            {
                this._state = 2;
                this._currentTimestamp = currentTimestamp;
                this._calendar.Load(home.GetCalendarJSON(), currentTimestamp);

                if (this._battleTimer != null)
                {
                    this._battleTimer.Destruct();
                    this._battleTimer = null;
                }

                if (homeOwnerAvatar.IsNpcAvatar())
                {
                    LogicNpcAvatar npcAvatar = (LogicNpcAvatar) homeOwnerAvatar;
                    LogicNpcData npcData = npcAvatar.GetNpcData();

                    homeOwnerAvatar.SetResourceCount(LogicDataTables.GetGoldData(), LogicMath.Max(npcData.Gold - visitorAvatar.GetLootedNpcGold(npcData), 0));
                    homeOwnerAvatar.SetResourceCount(LogicDataTables.GetElixirData(), LogicMath.Max(npcData.Elixir - visitorAvatar.GetLootedNpcElixir(npcData), 0));

                    this._level.SetMatchType(2, 0);
                    this._level.SetHome(home, false);
                    this._level.SetHomeOwnerAvatar(homeOwnerAvatar);
                    this._level.SetVisitorAvatar(visitorAvatar);
                    this._level.FastForwardTime(secondsSinceLastSave);
                    this._level.LoadingFinished();
                }
                else
                {
                    Debugger.Error("loadNpcAttackState called and home owner is not npc avatar");
                }
            }
        }

        /// <summary>
        ///     Loads the npc duel state.
        /// </summary>
        public void LoadNpcDuelState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave)
        {
        }
    }
}