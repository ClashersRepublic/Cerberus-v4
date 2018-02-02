namespace ClashersRepublic.Magic.Client.Game.Network
{
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        internal Messaging Messaging;
        internal ServerConnection ServerConnection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
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
                if (messageType != 20103 || messageType != 20104)
                {
                    return;
                }
            }

            switch (messageType)
            {
                case 20103:
                {
                    break;
                }
            }
        }
    }
}