namespace ClashersRepublic.Magic.Services.Home.Game.Mode
{
    using System;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Command.Listener;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Logic.Util;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Home.Network.Session;
    using ClashersRepublic.Magic.Services.Home.Resource;
    using ClashersRepublic.Magic.Titan.Util;

    internal class GameMode
    {
        private Home _home;
        private LogicGameMode _logicGameMode;

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
        }

        /// <summary>
        ///     Deinitializes the game mode. Called when the session is stopped.
        /// </summary>
        internal void DeInit()
        {
            Console.WriteLine("DEINIT");
        }

        /// <summary>
        ///     Initializes the game mode. Called when a new session is started.
        /// </summary>
        internal void Init()
        {
            Console.WriteLine("INIT");

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
                        compressibleHomeJSON.Set(HomeResourceManager.GetStartingHomeJSON());
                    }

                    // CompressibleStringHelper.Compress(compressibleHomeJSON);
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

                this.SetGameMode(clientHome, homeOwnerAvatar, null, currentTimestamp, secondsSinceLastSave, GAME.HOME_STATE);
                this.Session.SendPiranhaMessage(1, ownHomeDataMessage);
            }
        }

        /// <summary>
        ///     Sets the gamemode.
        /// </summary>
        private void SetGameMode(LogicClientHome clientHome, LogicAvatar homeOwnerAvatar, LogicAvatar visitorAvatar, int currentTimestamp, int secondsSinceLastSave, GAME mode)
        {
            if (this._logicGameMode != null)
            {
                this._logicGameMode.Destruct();
                this._logicGameMode = null;
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
                this._logicGameMode.GetCommandManager().SetListener(new LogicCommandManagerListener()); // TODO: Create LogicCommandManagerListener

                switch (mode)
                {
                    case GAME.HOME_STATE:
                        this._logicGameMode.LoadHomeState(clientHome, homeOwnerAvatar, currentTimestamp, secondsSinceLastSave);
                        break;
                    default:
                        Logging.Warning(this, "GameMode::setGameMode mode " + mode + " doesn't exist");
                        break;
                }
            }
        }

        private enum GAME
        {
            HOME_STATE = 1
        }
    }
}