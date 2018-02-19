namespace ClashersRepublic.Magic.Services.Home.Message
{
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Services.Home.Service;
    using ClashersRepublic.Magic.Services.Home.Sessions;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        private GameSession _gameSession;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(GameSession gameSession)
        {
            this._gameSession = gameSession;
        }

        /// <summary>
        ///     Receives the specified <see cref="PiranhaMessage"/> message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            if (message.IsServerToClientMessage() || message.GetServiceNodeType() != ServiceManager.SERVICE_TYPE)
            {
                Logging.Warning(this, "MessageManager::receiveMessage invalid message, type: " + message.GetMessageType());
            }
            else
            {
                switch (message.GetMessageType())
                {
                    case 14102:
                        this.EndClientTurnMessageReceived((EndClientTurnMessage) message);
                        break;
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="EndClientTurnMessage"/> is received.
        /// </summary>
        internal void EndClientTurnMessageReceived(EndClientTurnMessage message)
        {
            // EndClientTurnMessageReceived.
        }
    }
}