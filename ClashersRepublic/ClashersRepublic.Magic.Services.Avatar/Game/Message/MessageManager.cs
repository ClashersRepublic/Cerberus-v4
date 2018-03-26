namespace ClashersRepublic.Magic.Services.Avatar.Game.Message
{
    using ClashersRepublic.Magic.Logic.Command.Server;
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
                case 10212:
                    this.ChangeAvatarNameMessageReceived((ChangeAvatarNameMessage) message);
                    break;
                case 14325:
                    this.AskForAvatarProfileMessageReceived((AskForAvatarProfileMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Called when a <see cref="ChangeAvatarNameMessage"/> is received.
        /// </summary>
        internal void ChangeAvatarNameMessageReceived(ChangeAvatarNameMessage message)
        {
            if (message.GetNameSetByUser() && !this._session.AvatarAccount.AvatarEntry.GetNameSetByUser())
            {
                string avatarName = message.RemoveAvatarName();

                if (!string.IsNullOrWhiteSpace(avatarName))
                {
                    if (avatarName.Length <= 12)
                    {
                        avatarName = ServiceAvatar.Regex.Replace(avatarName, " ");

                        if (avatarName.Length >= 2)
                        {
                            AllowServerCommandMessage allowServerCommandMessage = new AllowServerCommandMessage();
                            LogicChangeAvatarNameCommand serverCommand = new LogicChangeAvatarNameCommand();

                            serverCommand.SetAvatarName(avatarName);
                            serverCommand.SetAvatarNameChangeState(0);
                            allowServerCommandMessage.SetServerCommand(serverCommand);

                            this._session.SendMessage(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, allowServerCommandMessage);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="AskForAvatarProfileMessage"/> is received.
        /// </summary>
        internal void AskForAvatarProfileMessageReceived(AskForAvatarProfileMessage message)
        {
            LogicLong avatarId = message.RemoveAvatarId();
            LogicLong homeId = message.RemoveHomeId();

            if (AvatarAccountManager.TryGet(avatarId, out AvatarAccount _))
            {
                AskForAvatarProfileFullEntryMessage askForAvatarProfileFullEntryMessage = new AskForAvatarProfileFullEntryMessage();
                askForAvatarProfileFullEntryMessage.SetAvatarId(avatarId);
                askForAvatarProfileFullEntryMessage.SetHomeId(homeId ?? avatarId);
                NetMessageManager.SendMessage(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, NetManager.GetDocumentOwnerId(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, avatarId), this._session.SessionId, askForAvatarProfileFullEntryMessage);
            }
        }
    }
}