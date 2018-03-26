namespace ClashersRepublic.Magic.Services.Avatar.Game.Message
{
    using ClashersRepublic.Magic.Logic.Message.Avatar;
    using ClashersRepublic.Magic.Services.Avatar.Network.Session;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Home;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        private NetAvatarSession _session;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(NetAvatarSession session)
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
                case 14325:
                    this.AskForAvatarProfileMessageReceived((AskForAvatarProfileMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Called when a <see cref="AskForAvatarProfileMessage"/> is received.
        /// </summary>
        internal void AskForAvatarProfileMessageReceived(AskForAvatarProfileMessage message)
        {
            LogicLong avatarId = message.RemoveAvatarId();
            LogicLong homeId = message.RemoveHomeId();

            AskForAvatarProfileFullEntryMessage askForAvatarProfileFullEntryMessage = new AskForAvatarProfileFullEntryMessage();
            askForAvatarProfileFullEntryMessage.SetAvatarId(avatarId);
            askForAvatarProfileFullEntryMessage.SetHomeId(homeId ?? avatarId);
            NetMessageManager.SendMessage(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, NetManager.GetDocumentOwnerId(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, avatarId), this._session.SessionId, askForAvatarProfileFullEntryMessage);
        }
    }
}