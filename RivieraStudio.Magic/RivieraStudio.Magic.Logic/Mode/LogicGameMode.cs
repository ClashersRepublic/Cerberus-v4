﻿namespace RivieraStudio.Magic.Logic.Mode
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Battle;
    using RivieraStudio.Magic.Logic.Calendar;
    using RivieraStudio.Magic.Logic.Command;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Home;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Time;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;

    public class LogicGameMode
    {
        private bool _battleOver;

        private int _state;
        private int _currentTimestamp;
        private int _shieldTime;
        private int _guardTime;
        private int _maintenanceTime;
        private int _startGuardTime;
        private int _secondsSinceLastMaintenance;
        private int _skipPrepationSecs;

        private LogicTimer _battleTimer;
        private LogicLevel _level;
        private LogicCommandManager _commandManager;
        private LogicGameListener _gameListener;
        private LogicCalendar _calendar;
        private LogicConfiguration _configuration;
        private LogicReplay _replay;

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

            if (this._replay != null)
            {
                this._replay.Destruct();
                this._replay = null;
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
        ///     Gets the calendar instance.
        /// </summary>
        public LogicCalendar GetCalendar()
        {
            return this._calendar;
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
        ///     Gets the seconds since last maintenance.
        /// </summary>
        public int GetSecondsSinceLastMaintenance()
        {
            return this._secondsSinceLastMaintenance;
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
        ///     Sets the shield remaining seconds.
        /// </summary>
        public void SetShieldRemainingSeconds(int secs)
        {
            this._shieldTime = LogicTime.GetSecondsInTicks(secs) + this._level.GetLogicTime();

            int logicTime = this._level.GetLogicTime();
            int startGuardTime = logicTime;

            if (this._shieldTime >= logicTime)
            {
                startGuardTime = this._shieldTime;
            }

            this._startGuardTime = startGuardTime;
        }

        /// <summary>
        ///     Gets the remaining guard time.
        /// </summary>
        public int GetGuardRemainingSeconds()
        {
            int startTime = this._startGuardTime - this._level.GetLogicTime();

            if (startTime <= 0)
            {
                startTime = 0;
            }

            return LogicMath.Max(LogicTime.GetTicksInSeconds(this._guardTime + startTime), 0);
        }

        /// <summary>
        ///     Sets the guard remaining seconds.
        /// </summary>
        public void SetGuardRemainingSeconds(int secs)
        {
            this._guardTime = LogicTime.GetSecondsInTicks(secs);

            int logicTime = this._level.GetLogicTime();
            int startGuardTime = logicTime;

            if (this._shieldTime >= logicTime)
            {
                startGuardTime = this._shieldTime;
            }

            this._startGuardTime = startGuardTime;
        }

        /// <summary>
        ///     Gets the remaining guard time.
        /// </summary>
        public int GetMaintenanceRemainingSeconds()
        {
            return LogicMath.Max(LogicTime.GetTicksInSeconds(this._maintenanceTime - this._level.GetLogicTime()), 0);
        }

        /// <summary>
        ///     Sets the maintenance remaining seconds.
        /// </summary>
        public void SetMaintenanceRemainingSeconds(int secs)
        {
            this._maintenanceTime = LogicTime.GetSecondsInTicks(secs) + this._level.GetLogicTime();
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

            if (this._replay != null)
            {
                this._replay.SubTick();
            }
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
        ///     Ends the attack preparation.
        /// </summary>
        public void EndAttackPreparation()
        {
            if (this._battleTimer != null)
            {
                int attackLength = LogicDataTables.GetGlobals().GetAttackLengthSecs();
                int battleRemainingSecs = this._battleTimer.GetRemainingSeconds(this._level.GetLogicTime());

                if (battleRemainingSecs > attackLength)
                {
                    int remainingPrepSecs = battleRemainingSecs - attackLength;

                    if (this._replay != null)
                    {
                        this._replay.RecordPreparationSkipTime(remainingPrepSecs);
                    }

                    this._skipPrepationSecs = remainingPrepSecs;
                    this._battleTimer.StartTimer(attackLength, this._level.GetLogicTime(), false, -1);
                }
            }
        }

        /// <summary>
        ///     Gets if the attacker is in attack preparation mode.
        /// </summary>
        public bool IsInAttackPreparationMode()
        {
            if (this._state == 2 || this._state == 5)
            {
                LogicAvatar homeOwnerAvatar = this._level.GetHomeOwnerAvatar();

                if (homeOwnerAvatar.IsClientAvatar())
                {
                    return this.GetRemainingAttackSeconds() > LogicDataTables.GetGlobals().GetAttackLengthSecs();
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets the remaining attack seconds.
        /// </summary>
        public int GetRemainingAttackSeconds()
        {
            if ((this._state == 2 || this._state == 5) && !this._battleOver)
            {
                if (!this._level.InvulnerabilityEnabled())
                {
                    if (this._battleTimer != null)
                    {
                        return LogicMath.Max(this._battleTimer.GetRemainingSeconds(this._level.GetLogicTime()), 1);
                    }
                }

                return 1;
            }

            return 0;
        }

        /// <summary>
        ///     Loads the home state.
        /// </summary>
        public void LoadHomeState(LogicClientHome home, LogicAvatar homeOwnerAvatar, int currentTimestamp, int secondsSinceLastSave, int secondsSinceLastMaintenance)
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

                this._secondsSinceLastMaintenance = secondsSinceLastMaintenance;
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

                int logicTime = this._level.GetLogicTime();
                int startGuardTime = logicTime;

                if (this._shieldTime >= logicTime)
                {
                    startGuardTime = this._shieldTime;
                }

                this._startGuardTime = startGuardTime;
                
                if (LogicDataTables.GetGlobals().UseVillageObjects())
                {
                    this._level.LoadVillageObjects();
                }
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