namespace ClashersRepublic.Magic.Services.Home.Game.Mode
{
    using System;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Command.Server;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Home.Game.Command;
    using ClashersRepublic.Magic.Services.Home.Network.Session;
    using ClashersRepublic.Magic.Services.Home.Resource;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    internal class GameMode
    {
        private int _currentTimestamp;

        private Home _home;
        private LogicGameMode _logicGameMode;
        private LogicArrayList<LogicServerCommand> _bufferedServerCommands;

        private bool _defenseMode;

        /// <summary>
        ///     Gets the current session.
        /// </summary>
        private NetHomeSession Session
        {
            get
            {
                return this._home.Session;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameMode"/> class.
        /// </summary>
        internal GameMode(Home home)
        {
            this._home = home;
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
            }
        }

        /// <summary>
        ///     Saves the <see cref="LogicClientHome"/> and <see cref="LogicClientAvatar"/> instances.
        /// </summary>
        internal void Save()
        {
            if (this._logicGameMode != null)
            {
                Logging.Debug("GameMode::save game saved");

                if (this._logicGameMode.GetState() == 1 || this._logicGameMode.GetState() == 3)
                {
                    LogicJSONObject jsonObject = new LogicJSONObject();

                    this._logicGameMode.SaveToJSON(jsonObject);
                    this._home.ClientHome.SetHomeJSON(LogicJSONParser.CreateJSONString(jsonObject));

                    CompressibleStringHelper.Compress(this._home.ClientHome.GetCompressibleHomeJSON());
                }

                DatabaseManager.Update(this._home);
            }
            else
            {
                Logging.Debug("GameMode::save called when m_logicGameMode is NULL");
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
                this.Session.SendPiranhaMessage(1, new WaitingToGoHomeMessage());
            }
            else
            {
                this.SetHomeState();
            }
        }

        /// <summary>
        ///     Sets the home state.
        /// </summary>
        internal void SetHomeState()
        {
            LogicClientAvatar homeOwnerAvatar = this._home.ClientAvatar;
            LogicClientHome clientHome = this._home.ClientHome;

            int currentTimestamp = LogicTimeUtil.GetTimestamp();
            int secondsSinceLastSave = currentTimestamp - this._home.SaveTimestamp;

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
                    Logging.Debug("GameMode::init level JSON is NULL, load default");
                    compressibleHomeJSON.Set(HomeResourceManager.GetStartingHomeJSON());
                }

                CompressibleStringHelper.Compress(compressibleHomeJSON);
            }

            compressibleCalendarJSON.Set("{}");
            compressibleGlobalJSON.Set("{}");

            CompressibleStringHelper.Compress(compressibleCalendarJSON);
            CompressibleStringHelper.Compress(compressibleGlobalJSON);

            OwnHomeDataMessage ownHomeDataMessage = new OwnHomeDataMessage();

            ownHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
            ownHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
            ownHomeDataMessage.SetLogicClientAvatar(homeOwnerAvatar);
            ownHomeDataMessage.SetLogicClientHome(clientHome);
            ownHomeDataMessage.Encode();

            this.SetGameMode(clientHome, homeOwnerAvatar, null, currentTimestamp, secondsSinceLastSave, 0, GAME.HOME_STATE);
            this.Session.SendPiranhaMessage(1, ownHomeDataMessage);
        }

        /// <summary>
        ///     Sets the npc attack state.
        /// </summary>
        internal void SetNpcAttackState(LogicNpcData data)
        {
            LogicClientAvatar visitorAvatar = this._home.ClientAvatar;
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

            this.SetGameMode(clientHome, homeOwnerAvatar, visitorAvatar, currentTimestamp, 0, 0, GAME.ATTACK_STATE);
            this.Session.SendPiranhaMessage(1, npcDataMessage);
        }

        /// <summary>
        ///     Sets the gamemode.
        /// </summary>
        private void SetGameMode(LogicClientHome clientHome, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave, int state, GAME mode)
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
                this._currentTimestamp = currentTimestamp;

                switch (mode)
                {
                    case GAME.HOME_STATE:
                        this._logicGameMode.LoadHomeState(clientHome, homeOwnerAvatar, currentTimestamp, secondsSinceLastSave);
                        break;
                    case GAME.ATTACK_STATE:
                        if (homeOwnerAvatar.IsNpcAvatar())
                        {
                            switch (state)
                            {
                                case 0:
                                    if (state == 8) this._logicGameMode.LoadNpcDuelState(clientHome, homeOwnerAvatar, visitorAvatar, currentTimestamp, secondsSinceLastSave);
                                    else this._logicGameMode.LoadNpcAttackState(clientHome, homeOwnerAvatar, visitorAvatar, currentTimestamp, secondsSinceLastSave);

                                    break;
                            }
                        }

                        break;
                    default:
                        Logging.Warning("GameMode::setGameMode mode " + mode + " doesn't exist");
                        break;
                }
            }
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
                newServerCommandId = this._bufferedServerCommands.IndexOf(null);

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
                        this.Session.SendPiranhaMessage(1, availableServerCommand);
                    }
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

                    Logging.Debug("GameMode::isBufferedServerCommand a buffered server command exist but the command type is mismatched with the specified server command");
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
                int calculateTimestamp = this._currentTimestamp + LogicTime.GetTicksInSeconds(subTick);

                if (currentTimestamp >= calculateTimestamp)
                {
                    int serverTick = this._logicGameMode.GetLevel().GetLogicTime();

                    if (subTick > serverTick)
                    {
                        if (commands != null)
                        {
                            if (commands.Count != 0)
                            {
                                LogicCommandManager commandManager = this.GetCommandManager();

                                for (int i = 0; i < commands.Count; i++)
                                {
                                    commandManager.AddCommand(commands[i]);
                                }
                            }
                        }

                        do
                        {
                            this._logicGameMode.UpdateOneSubTick();
                        } while (++serverTick != subTick);

                        Logging.Debug(string.Format("GameMode::clientTurnReceived clientTurn received, tick: {0} checksum: {1}", subTick, checksum));
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

        private enum GAME
        {
            HOME_STATE = 1,
            ATTACK_STATE = 2
        }
    }
}