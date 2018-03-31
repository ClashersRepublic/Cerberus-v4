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
        private int _shieldTime;
        private int _guardTime;
        private int _maintenanceTime;
        private int _elapsedSecs;

        private LogicTimer _battleTimer;
        private LogicLevel _level;
        private LogicCommandManager _commandManager;
        private LogicGameListener _gameListener;
        private LogicCalendar _calendar;
        private LogicConfiguration _configuration;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameMode" /> class.
        /// </summary>
        public LogicGameMode()
        {
            this._level = new LogicLevel(this);
            this._commandManager = new LogicCommandManager(this._level);
            this._calendar = new LogicCalendar();
            this._configuration = new LogicConfiguration();
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
            
            if (this._battleTimer != null)
            {
                this._battleTimer.Destruct();
                this._battleTimer = null;
            }

            this._configuration = null;
        }

        /// <summary>
        ///     Calculates the checksum of this instance.
        /// </summary>
        public ChecksumHelper CalculateChecksum(bool includeGameObjects)
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

            this._level.GetGameObjectManager().GetChecksum(checksum, includeGameObjects);

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
        public int GetActiveTimestamp()
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
        ///     Gets the elapsed seconds.
        /// </summary>
        public int GetElapsedSeconds()
        {
            return this._elapsedSecs;
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
            return LogicMath.Max(LogicTime.GetTicksInSeconds(this._shieldTime - this._level.GetLogicTime()), 0);
        }

        /// <summary>
        ///     Gets the remaining guard time.
        /// </summary>
        public int GetGuardRemainingSeconds()
        {
            return LogicMath.Max(LogicTime.GetTicksInSeconds(this._guardTime - this._level.GetLogicTime()), 0);
        }

        /// <summary>
        ///     Gets the remaining guard time.
        /// </summary>
        public int GetMaintenanceRemainingSeconds()
        {
            return LogicMath.Max(LogicTime.GetTicksInSeconds(this._maintenanceTime - this._level.GetLogicTime()), 0);
        }
        
        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void SaveToJSON(LogicJSONObject jsonObject)
        {
            this._level.SaveToJSON(jsonObject);
        }

        /// <summary>
        ///     Sub ticks this instance.
        /// </summary>
        public void SubTick()
        {
            if (this._state == 1)
            {
                this._calendar.SetCalendarData(this.GetActiveTimestamp(), this._level.GetHomeOwnerAvatar(), this._level);
            }

            this._commandManager.SubTick();
            this._level.SubTick();
        }

        /// <summary>
        ///     Updates a one tick.
        /// </summary>
        public void UpdateOneSubTick()
        {
            LogicTime time = this._level.GetLogicTime();

            if (this._state != 2 || !this._battleOver)
            {
                this.SubTick();

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
        public void LoadHomeState(LogicClientHome home, LogicAvatar homeOwnerAvatar, int currentTimestamp, int secondsSinceLastSave, int elapsedSecs)
        {
            if (home != null)
            {
                this._state = 1;

                if (LogicDataTables.GetGlobals().StartInLastUsedVillage())
                {
                    int lastUsedVillage = homeOwnerAvatar.GetVillageToGoTo();

                    if (!this._level.GetMissionManager().HasTravel(homeOwnerAvatar))
                    {
                        lastUsedVillage = 0;
                    }

                    if (lastUsedVillage < 0)
                    {
                        Debugger.Warning("VillageToGoTo<0");
                    }
                    else
                    {
                        if (lastUsedVillage > 1)
                        {
                            Debugger.Warning("VillageToGoTo too big");
                        }
                        else
                        {
                            this._level.SetVillageType(lastUsedVillage);
                        }
                    }
                }

                this._elapsedSecs = elapsedSecs;
                this._currentTimestamp = currentTimestamp;
                this._configuration.Load((LogicJSONObject) LogicJSONParser.Parse(home.GetGlobalJSON()));
                this._calendar.Load(home.GetCalendarJSON(), currentTimestamp);

                if (this._battleTimer != null)
                {
                    this._battleTimer.Destruct();
                    this._battleTimer = null;
                }

                this._level.SetHome(home, true);
                this._level.SetHomeOwnerAvatar(homeOwnerAvatar);
                this._level.FastForwardTime(secondsSinceLastSave);

                homeOwnerAvatar.SetLevel(this._level);

                this._level.LoadingFinished();

                this._shieldTime = LogicTime.GetSecondsInTicks(home.GetShieldDurationSeconds());
                this._guardTime = LogicTime.GetSecondsInTicks(home.GetGuardDurationSeconds());
                this._maintenanceTime = LogicTime.GetSecondsInTicks(home.GetNextMaintenanceSeconds());

                if (LogicDataTables.GetGlobals().UseVillageObjects())
                {
                    this._level.LoadVillageObjects();
                }
            }
        }

        /// <summary>
        ///     Loads the npc attack state.
        /// </summary>
        public void LoadNpcAttackState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave, int elapsedSecs)
        {
            if (this._state == 1)
            {
                Debugger.Error("loadAttackState called from invalid state");
            }
            else
            {
                this._state = 2;
                this._elapsedSecs = elapsedSecs;
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
        public void LoadNpcDuelState(LogicClientHome home, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave, int seed)
        {
        }
    }
}