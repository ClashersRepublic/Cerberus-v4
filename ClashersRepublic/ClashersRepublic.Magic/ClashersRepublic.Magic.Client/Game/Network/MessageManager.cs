namespace ClashersRepublic.Magic.Client.Game.Network
{
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        internal Messaging Messaging;
        internal ServerConnection ServerConnection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager" /> class.
        /// </summary>
        internal MessageManager(Messaging messaging, ServerConnection serverConnection)
        {
            this.Messaging = messaging;
            this.ServerConnection = serverConnection;
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            if (this.ServerConnection.State != 6)
            {
                if (messageType != 20103 && messageType != 20104)
                {
                    return;
                }
            }

            switch (messageType)
            {
                case 20103:
                {
                    this.LoginFailedMessageReceived((LoginFailedMessage) message);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends the specified message to server.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this.Messaging.Send(message);
        }

        /// <summary>
        ///     Called when a login failed message has been received.
        /// </summary>
        internal void LoginFailedMessageReceived(LoginFailedMessage message)
        {
            Debugger.Error("LoginFailedMessage, errorCode: " + message.ErrorCode);
        }
    }
}