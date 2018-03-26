namespace ClashersRepublic.Magic.Services.GlobalChat.Game.Message
{
    using ClashersRepublic.Magic.Logic.Message.Chat;
    using ClashersRepublic.Magic.Services.GlobalChat.Network.Session;
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        private NetGlobalChatSession _session;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(NetGlobalChatSession session)
        {
            this._session = session;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this._session = null;
        }

        /// <summary>
        ///     Receives the specified <see cref="PiranhaMessage"/>.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            switch (message.GetMessageType())
            {
                case 14715:
                    this.SendGlobalChatLineMessageReceived((SendGlobalChatLineMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Called when a <see cref="SendGlobalChatLineMessage"/> is received.
        /// </summary>
        private void SendGlobalChatLineMessageReceived(SendGlobalChatLineMessage message)
        {
            this._session.Room.ReceiveMessage(this._session.AvatarEntry, message.RemoveMessage());
        }
    }
}