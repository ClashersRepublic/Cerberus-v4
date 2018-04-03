namespace LineageSoft.Magic.Services.Chat.Game.Message
{
    using System;
    using LineageSoft.Magic.Logic.Message.Chat;
    using LineageSoft.Magic.Services.Core.Message.Admin;
    using LineageSoft.Magic.Services.Chat.Network.Session;

    using LineageSoft.Magic.Titan.Message;

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
            string chatMessage = message.RemoveMessage();
            
            if (chatMessage.StartsWith("/admin ", StringComparison.InvariantCultureIgnoreCase))
            {
                AdminCommand adminCommand = AdminCommandFactory.CreateDebugCommandByName(chatMessage.Substring(7).ToLowerInvariant());

                if (adminCommand != null)
                {
                    if (adminCommand.GetServiceNodeType() != -1)
                    {
                        ExecuteAdminCommandMessage executeAdminCommandMessage = new ExecuteAdminCommandMessage();
                        executeAdminCommandMessage.SetDebugCommand(adminCommand);
                        this._session.SendMessage(adminCommand.GetServiceNodeType(), executeAdminCommandMessage);
                        return;
                    }
                }
            }

            this._session.Room.ReceiveMessage(this._session.AvatarEntry, chatMessage);
        }
    }
}