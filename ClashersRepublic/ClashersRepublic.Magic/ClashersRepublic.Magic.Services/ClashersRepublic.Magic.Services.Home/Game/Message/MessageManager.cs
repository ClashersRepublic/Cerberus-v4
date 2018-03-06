namespace ClashersRepublic.Magic.Services.Home.Game.Message
{
    using ClashersRepublic.Magic.Logic.Message.Home;

    using ClashersRepublic.Magic.Services.Home.Game.Mode;
    using ClashersRepublic.Magic.Services.Home.Network.Session;

    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        private NetHomeSession _session;
        private GameMode _gameMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(NetHomeSession session, GameMode gameMode)
        {
            this._session = session;
            this._gameMode = gameMode;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this._session = null;
            this._gameMode = null;
        }

        /// <summary>
        ///     Receives the specicified <see cref="PiranhaMessage"/>.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            switch (message.GetMessageType())
            {
                case 14102:
                    this.EndClientTurnMessageReceived((EndClientTurnMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Called when a <see cref="EndClientTurnMessage"/> is received.
        /// </summary>
        internal void EndClientTurnMessageReceived(EndClientTurnMessage message)
        {
            this._gameMode.ClientTurnReceived(message.GetSubTick(), message.GetChecksum(), message.GetCommands());
        }
    }
}