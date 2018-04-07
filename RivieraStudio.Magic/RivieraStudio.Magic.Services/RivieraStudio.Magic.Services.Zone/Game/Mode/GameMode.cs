namespace RivieraStudio.Magic.Services.Zone.Game.Mode
{
    using System;

    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Command;
    using RivieraStudio.Magic.Logic.Command.Server;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Home;
    using RivieraStudio.Magic.Logic.Message.Home;
    using RivieraStudio.Magic.Logic.Mode;
    using RivieraStudio.Magic.Logic.Time;
    using RivieraStudio.Magic.Logic.Util;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;
    using RivieraStudio.Magic.Services.Core.Message.Avatar;
    using RivieraStudio.Magic.Services.Core.Message.Session;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Services.Zone.Game.Command;
    using RivieraStudio.Magic.Services.Zone.Network.Session;
    using RivieraStudio.Magic.Services.Zone.Resource;

    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

    internal class GameMode
    {
        private ZoneAccount _zoneAccount;
        private LogicGameMode _logicGameMode;
        private LogicArrayList<LogicServerCommand> _bufferedServerCommands;

        private bool _defenseMode;

        /// <summary>
        ///     Gets the current session.
        /// </summary>
        private NetZoneSession Session
        {
            get
            {
                return this._zoneAccount.Session;
            }
        }

        /// <summary>
        ///     Gets the executed command count since last save.
        /// </summary>
        internal int ExecutedCommandsSinceLastSave { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameMode"/> class.
        /// </summary>
        internal GameMode(ZoneAccount zoneAccount)
        {
            this._zoneAccount = zoneAccount;
            this._bufferedServerCommands = new LogicArrayList<LogicServerCommand>(40);
        }

        /// <summary>
        ///     Deinitializes the game mode. Called when the session is stopped.
        /// </summary>
        internal void DeInit()
        {
            if (this._logicGameMode != null)
            {
                this.Save();

                this._logicGameMode.Destruct();
                this._logicGameMode = null;

                DatabaseManagerNew.Update(0, this._zoneAccount.Id, LogicJSONParser.CreateJSONString(this._zoneAccount.Save()));
            }
        }

        /// <summary>
        ///     Saves the <see cref="LogicClientHome"/> and <see cref="LogicClientAvatar"/> instances.
        /// </summary>
        internal void Save()
        {
            if (this._logicGameMode != null)
            {
                if (this._logicGameMode.GetState() == 1 || this._logicGameMode.GetState() == 3)
                {
                    LogicJSONObject jsonObject = new LogicJSONObject();

                    this._logicGameMode.SaveToJSON(jsonObject);

                    this._zoneAccount.ClientHome.SetHomeJSON(LogicJSONParser.CreateJSONString(jsonObject));
                    this._zoneAccount.ClientHome.SetShieldDurationSeconds(this._logicGameMode.GetShieldRemainingSeconds());
                    this._zoneAccount.ClientHome.SetGuardDurationSeconds(this._logicGameMode.GetGuardRemainingSeconds());
                    this._zoneAccount.ClientHome.SetNextMaintenanceSeconds(this._logicGameMode.GetMaintenanceRemainingSeconds());

                    this.ExecutedCommandsSinceLastSave = 0;

                    // CompressibleStringHelper.Compress(this._zoneAccount.ClientHome.GetCompressibleHomeJSON());
                }
            }
            else
            {
                Logging.Warning("GameMode::save called when m_logicGameMode is NULL");
            }
        }

        /// <summary>
        ///     Initializes the game mode. Called when a new session is started.
        /// </summary>
        internal void Init()
        {
            if (this.Session == null)
            {
                throw new NullReferenceException("pSession->NULL");
            }

            if (this._defenseMode)
            {
                this.Session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, new WaitingToGoHomeMessage());
            }
            else
            {
                this.SetHomeState();
                this.SendAvatarEntryMessage();
            }
        }

        /// <summary>
        ///     Sets the home state.
        /// </summary>
        internal void SetHomeState()
        {
            LogicClientAvatar homeOwnerAvatar = this._zoneAccount.ClientAvatar;
            LogicClientHome clientHome = this._zoneAccount.ClientHome;

            int currentTimestamp = LogicTimeUtil.GetTimestamp();
            int secondsSinceLastSave = currentTimestamp - this._zoneAccount.SaveTimestamp;

            if (secondsSinceLastSave < 0)
            {
                secondsSinceLastSave = 0;
            }

            LogicCompressibleString compressibleHomeJSON = clientHome.GetCompressibleHomeJSON();
            LogicCompressibleString compressibleCalendarJSON = clientHome.GetCompressibleCalendarJSON();
            LogicCompressibleString compressibleGlobalJSON = clientHome.GetCompressibleGlobalJSON();

            if (!compressibleHomeJSON.IsCompressed())
            {
                if (compressibleHomeJSON.Get() == null)
                {
                    Logging.Warning("GameMode::init level JSON is NULL, load default");
                    compressibleHomeJSON.Set(HomeResourceManager.GetStartingHomeJSON());
                }
                
                CompressibleStringHelper.Compress(compressibleHomeJSON);
            }

            compressibleCalendarJSON.Set("{}");
            compressibleGlobalJSON.Set(HomeResourceManager.GetGlobalJSON());

            CompressibleStringHelper.Compress(compressibleCalendarJSON);
            CompressibleStringHelper.Compress(compressibleGlobalJSON);

            OwnHomeDataMessage ownHomeDataMessage = new OwnHomeDataMessage();

            ownHomeDataMessage.SetElapsedSecs(-1);
            ownHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
            ownHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
            ownHomeDataMessage.SetLogicClientAvatar(homeOwnerAvatar);
            ownHomeDataMessage.SetLogicClientHome(clientHome);
            ownHomeDataMessage.Encode();

            this.SetGameMode(clientHome, homeOwnerAvatar, null, currentTimestamp, secondsSinceLastSave, -1, 0, GAME.HOME_STATE);
            this.Session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, ownHomeDataMessage);
        }

        /// <summary>
        ///     Sets the npc attack state.
        /// </summary>
        internal void SetNpcAttackState(LogicNpcData data)
        {
            LogicClientAvatar visitorAvatar = this._zoneAccount.ClientAvatar;
            LogicNpcAvatar homeOwnerAvatar = LogicNpcAvatar.GetNpcAvatar(data);
            LogicClientHome clientHome = new LogicClientHome();

            int currentTimestamp = LogicTimeUtil.GetTimestamp();

            LogicCompressibleString compressibleHomeJSON = clientHome.GetCompressibleHomeJSON();
            LogicCompressibleString compressibleCalendarJSON = clientHome.GetCompressibleCalendarJSON();
            LogicCompressibleString compressibleGlobalJSON = clientHome.GetCompressibleGlobalJSON();
            
            compressibleHomeJSON.Set(HomeResourceManager.GetNpcLevelByData(data));
            compressibleCalendarJSON.Set("{}");
            compressibleGlobalJSON.Set("{}");

            CompressibleStringHelper.Compress(compressibleHomeJSON);
            CompressibleStringHelper.Compress(compressibleCalendarJSON);
            CompressibleStringHelper.Compress(compressibleGlobalJSON);

            NpcDataMessage npcDataMessage = new NpcDataMessage();

            npcDataMessage.SetCurrentTimestamp(currentTimestamp);
            npcDataMessage.SetLogicNpcAvatar(homeOwnerAvatar);
            npcDataMessage.SetLogicClientHome(clientHome);
            npcDataMessage.SetLogicClientAvatar(visitorAvatar);
            npcDataMessage.Encode();

            this.SetGameMode(clientHome, homeOwnerAvatar, visitorAvatar, currentTimestamp, 0, -1, 0, GAME.ATTACK_STATE);
            this.Session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, npcDataMessage);
        }

        /// <summary>
        ///     Sets the gamemode.
        /// </summary>
        private void SetGameMode(LogicClientHome clientHome, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave, int elapsedSecs, int state, GAME mode)
        {
            if (this._logicGameMode != null)
            {
                this.DeInit();
            }

            if (clientHome != null)
            {
                LogicCompressibleString compressibleHomeJSON = clientHome.GetCompressibleHomeJSON();
                LogicCompressibleString compressibleCalendarJSON = clientHome.GetCompressibleCalendarJSON();
                LogicCompressibleString compressibleGlobalJSON = clientHome.GetCompressibleGlobalJSON();

                if (compressibleHomeJSON.IsCompressed())
                {
                    CompressibleStringHelper.Uncompress(compressibleHomeJSON);
                }

                if (compressibleCalendarJSON.IsCompressed())
                {
                    CompressibleStringHelper.Uncompress(compressibleCalendarJSON);
                }

                if (compressibleGlobalJSON.IsCompressed())
                {
                    CompressibleStringHelper.Uncompress(compressibleGlobalJSON);
                }
                
                this._logicGameMode = new LogicGameMode();
                this._logicGameMode.GetCommandManager().SetListener(new CommandManagerListener(this));
                
                switch (mode)
                {
                    case GAME.HOME_STATE:
                        this._logicGameMode.LoadHomeState(clientHome, homeOwnerAvatar, currentTimestamp, secondsSinceLastSave, elapsedSecs);
                        break;
                    case GAME.ATTACK_STATE:
                        if (homeOwnerAvatar.IsNpcAvatar())
                        {
                            switch (state)
                            {
                                case 8:
                                    this._logicGameMode.LoadNpcDuelState(clientHome, homeOwnerAvatar, visitorAvatar, currentTimestamp, secondsSinceLastSave, elapsedSecs);
                                    break;
                                default:
                                    this._logicGameMode.LoadNpcAttackState(clientHome, homeOwnerAvatar, visitorAvatar, currentTimestamp, secondsSinceLastSave, elapsedSecs);
                                    break;
                            }
                        }

                        break;
                    default:
                        Logging.Warning("GameMode::setGameMode mode " + mode + " doesn't exist");
                        return;
                }
            }

            this.SendAvatarEntryMessage();
        }

        /// <summary>
        ///     Adds the specified available <see cref="LogicServerCommand"/> instance.
        /// </summary>
        internal void AddAvailableServerCommand(LogicServerCommand serverCommand)
        {
            int currentServerCommandId = serverCommand.GetId();
            int newServerCommandId = -1;

            if (currentServerCommandId == -1)
            {
                newServerCommandId = -1;

                for (int i = 0; i < this._bufferedServerCommands.Count; i++)
                {
                    if (this._bufferedServerCommands[i] == null)
                    {
                        newServerCommandId = i;
                        break;
                    }
                }

                if (newServerCommandId != -1)
                {
                    this._bufferedServerCommands[newServerCommandId] = serverCommand;
                }
                else
                {
                    this._bufferedServerCommands.Add(null);
                    this._bufferedServerCommands[newServerCommandId = this._bufferedServerCommands.Count - 1] = serverCommand;
                }
            }
            else
            {
                if (this._bufferedServerCommands.Count <= currentServerCommandId)
                {
                    do
                    {
                        this._bufferedServerCommands.Add(null);
                    } while (this._bufferedServerCommands.Count <= currentServerCommandId);

                    this._bufferedServerCommands[newServerCommandId = currentServerCommandId] = serverCommand;
                }
                else if (this._bufferedServerCommands[newServerCommandId = currentServerCommandId] == null)
                {
                    this._bufferedServerCommands[newServerCommandId = currentServerCommandId] = serverCommand;
                }
                else
                {
                    Logging.Warning("GameMode::addAvailableServerCommand trying to override a server command");
                }
            }

            if (newServerCommandId != -1)
            {
                serverCommand.SetId(newServerCommandId);

                if (this.Session != null)
                {
                    if (this._logicGameMode.GetState() == 1)
                    {
                        AvailableServerCommand availableServerCommand = new AvailableServerCommand();
                        availableServerCommand.SetServerCommand(serverCommand);
                        this.Session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, availableServerCommand);
                    }

                    Logging.Print(string.Format("GameMode::addAvailableServerCommand cmd: {0} id: {1}", serverCommand.GetCommandType(), serverCommand.GetId()));
                }
            }
        }

        /// <summary>
        ///     Gets if the specified <see cref="LogicServerCommand"/> instance is a buffered server command.
        /// </summary>
        internal bool IsBufferedServerCommand(LogicServerCommand serverCommand)
        {
            int serverCommandId = serverCommand.GetId();

            if (serverCommandId != -1)
            {
                if (this._bufferedServerCommands.Count > serverCommandId)
                {
                    LogicServerCommand bufferedServerCommand = this._bufferedServerCommands[serverCommandId];

                    if (bufferedServerCommand.GetCommandType() == serverCommand.GetCommandType())
                    {
                        return true;
                    }

                    Logging.Print("GameMode::isBufferedServerCommand a buffered server command exist but the command type is mismatched with the specified server command");
                }
            }

            return false;
        }

        /// <summary>
        ///     Removes the specified <see cref="LogicServerCommand"/> instance of the buffered server commands.
        /// </summary>
        internal void RemoveServerCommand(LogicServerCommand serverCommand)
        {
            int serverCommandId = serverCommand.GetId();

            if (serverCommandId != -1)
            {
                if (this._bufferedServerCommands.Count > serverCommandId)
                {
                    this._bufferedServerCommands[serverCommandId] = null;
                }
                else
                {
                    Logging.Warning(string.Format("GameMode::removeServerCommand idx is out of bands {0}/{1}", serverCommandId, this._bufferedServerCommands.Count));
                }
            }
            else
            {
                Logging.Warning("GameMode::removeServerCommand id is not set");
            }
        }

        /// <summary>
        ///     Gets the <see cref="LogicCommandManager"/> instance.
        /// </summary>
        /// <returns></returns>
        internal LogicCommandManager GetCommandManager()
        {
            return this._logicGameMode.GetCommandManager();
        }

        /// <summary>
        ///     Called when client turn message is received.
        /// </summary>
        internal void ClientTurnReceived(int subTick, int checksum, LogicArrayList<LogicCommand> commands)
        {
            if (subTick > -1)
            {
                int currentTimestamp = LogicTimeUtil.GetTimestamp();
                int calculateTimestamp = this._logicGameMode.GetCurrentTimestamp() + LogicTime.GetTicksInSeconds(subTick);

                this._zoneAccount.SetSaveTimestamp(currentTimestamp);

                if (currentTimestamp >= calculateTimestamp)
                {
                    int serverTick = this._logicGameMode.GetLevel().GetLogicTime();

                    if (subTick > serverTick)
                    {
                        bool commandExecuted = false;

                        if (commands != null)
                        {
                            if (commands.Count != 0)
                            {
                                LogicCommandManager commandManager = this.GetCommandManager();

                                for (int i = 0; i < commands.Count; i++)
                                {
                                    LogicCommand cmd = commands[i];

                                    if (cmd.IsServerCommand())
                                    {
                                        LogicServerCommand sCmd = (LogicServerCommand) cmd;

                                        if (sCmd.GetId() != -1)
                                        {
                                            if (this.IsBufferedServerCommand(sCmd))
                                            {
                                                cmd = this._bufferedServerCommands[sCmd.GetId()];

                                                if (cmd.GetExecuteSubTick() == -1)
                                                {
                                                    cmd.SetExecuteSubTick(sCmd.GetExecuteSubTick());
                                                    commandManager.AddCommand(cmd);
                                                }
                                                else
                                                {
                                                    Logging.Warning("GameMode::clientTurnReceived setExecuteSubTick called twice!");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Logging.Warning("GameMode::clientTurnReceived id is not set!");
                                        }

                                        continue;
                                    }
                                    
                                    commandManager.AddCommand(cmd);
                                }

                                commandExecuted = true;
                            }
                        }

                        do
                        {
                            this._logicGameMode.UpdateOneSubTick();
                        } while (++serverTick != subTick);

                        if (commandExecuted)
                        {
                            this.SendAvatarEntryMessage();
                        }

                        int serverChecksum = this._logicGameMode.CalculateChecksum(false);

                        if (this._logicGameMode.GetState() == 1)
                        {
                            if (this._logicGameMode.GetLevel().GetHomeOwnerAvatar().IsMissionCompleted((LogicMissionData) LogicDataTables.GetTable(20).GetItemAt(2)))
                            {
                                if (serverChecksum != checksum)
                                {
                                    OutOfSyncMessage outOfSyncMessage = new OutOfSyncMessage();

                                    outOfSyncMessage.SetClientChecksum(checksum);
                                    outOfSyncMessage.SetClientChecksum(serverChecksum);

                                    this.Session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, outOfSyncMessage);
                                    this.Session.SendMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, new UnbindServerMessage());

                                    NetZoneSessionManager.Remove(this.Session.SessionId);

                                    this.DeInit();
                                    this.Session.Destruct();

                                    Logging.Print(string.Format("GameMode::clientTurnReceived out of sync, checksum: {0} server checksum: {1}", checksum, serverChecksum));

                                    return;
                                }
                            }
                        }

                        Logging.Print(string.Format("GameMode::clientTurnReceived clientTurn received, tick: {0} checksum: {1}", subTick, checksum));
                    }
                    else
                    {
                        Logging.Warning("GameMode::clientTurnReceived client turn ignored");
                    }
                }
                else
                {
                    Logging.Warning("GameMode::clientTurnReceived subTick is too high! (" + subTick + ")");
                }
            }
            else
            {
                Logging.Warning("GameMode::clientTurnReceived subTick is negative");
            }
        }

        /// <summary>
        ///     Sends a <see cref="AvatarEntryMessage"/> to the avatar service node.
        /// </summary>
        internal void SendAvatarEntryMessage()
        {
            AvatarEntryMessage avatarEntryMessage = new AvatarEntryMessage();
            AvatarEntry entry = new AvatarEntry();
            entry.SetData(this._zoneAccount.ClientAvatar);
            avatarEntryMessage.SetAvatarEntry(entry);
            this.Session.SendMessage(NetUtils.SERVICE_NODE_TYPE_AVATAR_CONTAINER, avatarEntryMessage);
        }

        private enum GAME
        {
            HOME_STATE = 1,
            ATTACK_STATE = 2
        }
    }
}