namespace ClashersRepublic.Magic.Services.Avatar.Network.Message
{
    using ClashersRepublic.Magic.Logic.Avatar;

    using ClashersRepublic.Magic.Services.Avatar.Game;
    using ClashersRepublic.Magic.Services.Avatar.Network.Session;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Titan.Math;

    internal class NetAvatarMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            switch (message.GetMessageType())
            {
                case 10200:
                    NetAvatarMessageManager.CreateAvatarMessageReceived((CreateAvatarMessage) message);
                    break;
                case 10201:
                    NetAvatarMessageManager.AskForAvatarMessageReceived((AskForAvatarMessage) message);
                    break;
                case 10202:
                    NetAvatarMessageManager.SetAvatarDataMessageReceived((SetAvatarDataMessage) message);
                    break;
                case 10302:
                    NetAvatarMessageManager.AskForBindServerMessageReceived((AskForBindServerMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Sends the response <see cref="NetMessage"/> to the requester.
        /// </summary>
        internal static void SendResponseMessage(NetMessage requestMessage, NetMessage responseMessage)
        {
            NetMessageManager.SendMessage(requestMessage.GetServiceNodeType(), requestMessage.GetServiceNodeId(), requestMessage.GetSessionId(), requestMessage.GetSessionIdLength(), responseMessage);
        }

        /// <summary>
        ///     Called when a <see cref="CreateAvatarMessage"/> is received.
        /// </summary>
        internal static void CreateAvatarMessageReceived(CreateAvatarMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                if (NetManager.GetServiceNodeId(ServiceCore.ServiceNodeType, accountId) == ServiceCore.ServiceNodeId)
                {
                    if (!AvatarManager.TryCreateAvatar(accountId, out _))
                    {
                        Logging.Warning(typeof(NetAvatarMessageManager), "NetAvatarMessageManager::createAvatarMessageReceived avatar creation failed");
                    }
                }
                else
                {
                    Logging.Warning(typeof(NetAvatarMessageManager), "NetAvatarMessageManager::createAvatarMessageReceived account id is not valid");
                }
            }
            else
            {
                Logging.Warning(typeof(NetAvatarMessageManager), "NetAvatarMessageManager::createAvatarMessageReceived account id is equal at 0");
            }
        }

        /// <summary>
        ///     Called when a <see cref="AskForAvatarMessage"/> is received.
        /// </summary>
        internal static void AskForAvatarMessageReceived(AskForAvatarMessage message)
        {
            LogicLong avatarId = message.RemoveAvatarId();

            if (!avatarId.IsZero())
            {
                if (AvatarManager.TryGetAvatar(avatarId, out Avatar avatar))
                {
                    AvatarDataMessage response = new AvatarDataMessage();
                    response.SetLogicClientAvatar(avatar.ClientAvatar);
                    NetAvatarMessageManager.SendResponseMessage(message, response);
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="SetAvatarDataMessage"/> is received.
        /// </summary>
        internal static void SetAvatarDataMessageReceived(SetAvatarDataMessage message)
        {
            LogicClientAvatar clientAvatar = message.RemoveLogicClientAvatar();

            if (AvatarManager.TryGetAvatar(clientAvatar.GetId(), out Avatar avatar))
            {
                if (clientAvatar.GetId() == avatar.ClientAvatar.GetId())
                {
                    avatar.SetClientAvatar(clientAvatar);
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="AskForBindServerMessage"/> is received.
        /// </summary>
        internal static void AskForBindServerMessageReceived(AskForBindServerMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                if (AvatarManager.TryGetAvatar(accountId, out Avatar avatar))
                {
                    NetAvatarSessionManager.TryCreate(avatar, message.GetSessionId());
                }
            }
        }
    }
}