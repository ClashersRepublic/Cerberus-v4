namespace ClashersRepublic.Magic.Services.Home.Game.Mode
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Services.Home.Sessions;
    using ClashersRepublic.Magic.Services.Logic.Service.Setting;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    internal class GameMode
    {
        internal GameSession GameSession { get; }
        internal LogicGameMode LogicGameMode { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameMode"/> class.
        /// </summary>
        internal GameMode(GameSession gameSession)
        {
            this.GameSession = gameSession;
            this.LogicGameMode = new LogicGameMode();
        }

        /// <summary>
        ///     Called when the game is started.
        /// </summary>
        internal void OnStart()
        {
            this.LoadHomeState();
        }

        /// <summary>
        ///     Called when the game is exited.
        /// </summary>
        internal void OnExit()
        {
            if (!this.GameSession.GameHome.IsInDefenseState())
            {
                this.GameSession.GameHome.SetState(0);
            }
        }

        /// <summary>
        ///     Loads the home state.
        /// </summary>
        internal void LoadHomeState()
        {
            if (this.GameSession.GameHome.IsInDefenseState())
            {
                WaitingToGoHomeMessage message = new WaitingToGoHomeMessage();
                message.SetEstimatedTimeSeconds(0);
                this.SendMessage(message);
            }
            else
            {
                LogicClientHome logicClientHome = this.GameSession.GameHome.LogicClientHomeInstance;
                LogicClientAvatar logicClientAvatar = this.GameSession.GameHome.LogicClientAvatarInstance;

                logicClientHome.SetGlobalJSON(ServiceLogicConfig.GetGlobalsJson());
                logicClientHome.SetCalendarJSON(ServiceLogicConfig.GetCalendarJson());

                int currentTimestamp = LogicTimeUtil.GetTimestamp();
                int secondsSinceLastSave = currentTimestamp - this.GameSession.GameHome.SaveTimestamp;

                if (secondsSinceLastSave < 0)
                {
                    secondsSinceLastSave = 0;
                }

                this.LogicGameMode.LoadHomeState(logicClientHome, logicClientAvatar, secondsSinceLastSave, currentTimestamp);
                
                OwnHomeDataMessage ownHomeDataMessage = new OwnHomeDataMessage();

                ownHomeDataMessage.SetCurrentTimestamp(currentTimestamp);
                ownHomeDataMessage.SetLogicClientHome(logicClientHome);
                ownHomeDataMessage.SetLogicClientAvatar(logicClientAvatar);
                ownHomeDataMessage.SetSecondsSinceLastSave(secondsSinceLastSave);

                this.SendMessage(ownHomeDataMessage);
            }
        }

        /// <summary>
        ///     Sends the <see cref="PiranhaMessage"/> to the client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this.GameSession.ForwardPiranhaMessage(message, 1);
        }
    }
}