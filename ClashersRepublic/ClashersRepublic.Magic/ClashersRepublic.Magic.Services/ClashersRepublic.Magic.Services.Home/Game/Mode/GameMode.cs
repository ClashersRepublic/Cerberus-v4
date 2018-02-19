namespace ClashersRepublic.Magic.Services.Home.Game.Mode
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Command.Listener;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Logic.Utils;
    using ClashersRepublic.Magic.Services.Home.Home;
    using ClashersRepublic.Magic.Services.Home.Sessions;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    internal class GameMode
    {
        private bool _offlineMode;
        private bool _defenseMode;

        private GameHome _gameHome;
        private GameSession _gameSession;
        private LogicGameMode _logicGameMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameMode"/> class.
        /// </summary>
        internal GameMode(GameHome gameHome)
        {
            this._gameHome = gameHome;
        }
        
        /// <summary>
        ///     Sends the <see cref="PiranhaMessage"/> to the client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this._gameSession.ForwardPiranhaMessage(message, 1);
        }

        /// <summary>
        ///     Sets the <see cref="_gameSession"/> instance.
        /// </summary>
        internal void SetSession(GameSession session)
        {
            this._gameSession = session;
        }

        /// <summary>
        ///     Sets the offline mode.
        /// </summary>
        internal void SetOfflineMode(bool state)
        {
            if (this._offlineMode != state)
            {
                if (state)
                {
                    this.SaveOfflineState();
                    this.SetGameMode(null, null, -1, -1, -1);
                }

                this._offlineMode = state;
            }
        }

        /// <summary>
        ///     Loads the game.
        /// </summary>
        internal void LoadMode()
        {
            if (this._defenseMode)
            {
                this.SendMessage(new WaitingToGoHomeMessage());
            }
            else
            {
                Logging.DoAssert(this, this._gameSession != null, "GameMode::setOfflineMode session is NULL");

                int currentTimestamp = LogicTimeUtil.GetTimestamp();
                int secondsSinceLastSave = currentTimestamp - this._gameHome.SaveTimestamp;

                LogicClientHome logicClientHome = this._gameHome.LogicClientHomeInstance;
                LogicClientAvatar logicClientAvatar = this._gameHome.LogicClientAvatarInstance;

                this.SetGameMode(logicClientHome, logicClientAvatar, currentTimestamp, secondsSinceLastSave, 1);

                OwnHomeDataMessage ownHomeDataMessage = new OwnHomeDataMessage();

                ownHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
                ownHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);
                ownHomeDataMessage.SetLogicClientAvatar(logicClientAvatar);
                ownHomeDataMessage.SetLogicClientHome(logicClientHome);

                this.SendMessage(ownHomeDataMessage);
            }
        }

        /// <summary>
        ///     Sets game datas.
        /// </summary>
        internal void SetGameMode(LogicClientHome home, LogicAvatar avatar, int currentTimestamp, int secondsSinceLastSave, int mode)
        {
            if (this._logicGameMode != null)
            {
                this._logicGameMode.Destruct();
                this._logicGameMode = null;
            }

            if (home != null)
            {
                home.SetCalendarJSON("{}");
                home.SetGlobalJSON("{}");

                LogicCompressibleString compressibleString = home.GetCompressibleHomeJSON();

                if (compressibleString.IsCompressed())
                {
                    CompressibleStringHelper.Uncompress(compressibleString);
                }

                this._logicGameMode = new LogicGameMode();
                this._logicGameMode.GetCommandManager().SetListener(new LogicCommandManagerListener());

                switch (mode)
                {
                    case 1:
                        this._logicGameMode.LoadHomeState(home, avatar, currentTimestamp, secondsSinceLastSave);
                        break;
                    default:
                        Logging.Warning(this, "GameMode::setGameMode mode " + mode + " doesn't exist");
                        break;
                }
            }
        }

        /// <summary>
        ///     Called when end turn of the client is received.
        /// </summary>
        internal void EndTurnReceived(EndClientTurnMessage message)
        {
            if (this._offlineMode)
            {
                Logging.Warning(this, "GameMode::endTurnReceived endTurn in offline mode");
                return;
            }

            int subTick = message.GetSubTick();
            int checksum = message.GetChecksum();
            var commands = message.GetCommands();

            if (commands != null)
            {
                LogicCommandManager commandManager = this.GetCommandManager();

                for (int i = 0; i < commands.Count; i++)
                {
                    commandManager.AddCommand(commands[i]);
                }
            }
            
            if (this._logicGameMode.GetLogicTime() < subTick)
            {
                do
                {
                    this._logicGameMode.UpdateOneSubTick();
                } while (this._logicGameMode.GetLogicTime() < subTick);
            }
        }

        /// <summary>
        ///     Saves the offline state.
        /// </summary>
        internal void SaveOfflineState()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();
            this._logicGameMode.SaveToJSON(jsonObject);
            this._gameHome.LogicClientHomeInstance.SetHomeJSON(LogicJSONParser.CreateJSONString(jsonObject));
            CompressibleStringHelper.Compress(this._gameHome.LogicClientHomeInstance.GetCompressibleHomeJSON());
            this._gameHome.UpdateDatas();
        }

        /// <summary>
        ///     Gets the <see cref="LogicLevel"/> instance.
        /// </summary>
        internal LogicLevel GetLogicLevel()
        {
            return this._logicGameMode.GetLevel();
        }

        /// <summary>
        ///     Gets the <see cref="LogicCommandManager"/> instance.
        /// </summary>
        internal LogicCommandManager GetCommandManager()
        {
            return this._logicGameMode.GetCommandManager();
        }

        /// <summary>
        ///     Gets the player <see cref="LogicAvatar"/> instance.
        /// </summary>
        internal LogicAvatar GetPlayerAvatar()
        {
            if (this._logicGameMode.GetLevel() != null)
            {
                int state = this._logicGameMode.GetState();

                if (state == 2 || state == 4)
                {
                    return this._logicGameMode.GetLevel().GetVisitorAvatar();
                }

                return this._logicGameMode.GetLevel().GetHomeOwnerAvatar();
            }

            return null;
        }

        /// <summary>
        ///     Gets the cloned instance of the player <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public LogicClientAvatar GetClonedPlayerAvatar()
        {
            LogicClientAvatar clientAvatar = new LogicClientAvatar();
            LogicClientAvatar playerAvatar = (LogicClientAvatar) this.GetPlayerAvatar();
            ByteStream byteStream = new ByteStream(0);

            playerAvatar.Encode(byteStream);
            byteStream.ResetOffset();
            clientAvatar.Decode(byteStream);

            return clientAvatar;
        }
    }
}