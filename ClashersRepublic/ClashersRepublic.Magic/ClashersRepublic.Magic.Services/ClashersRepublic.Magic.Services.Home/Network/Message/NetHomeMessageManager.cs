﻿namespace ClashersRepublic.Magic.Services.Home.Network.Message
{
    using ClashersRepublic.Magic.Services.Home.Game;
    using ClashersRepublic.Magic.Services.Home.Network.Session;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Titan.Math;

    internal class NetHomeMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            switch (message.GetMessageType())
            {
                case 10200:
                    this.CreateHomeMessageReceived((CreateHomeMessage) message);
                    break;
                case 10201:
                    this.AskForAvatarMessageReceived((AskForAvatarMessage) message);
                    break;

                case 10301:
                    this.ServerUnboundMessageReceived((ServerUnboundMessage) message);
                    break;
                case 10302:
                    this.ServerBoundMessageReceived((ServerBoundMessage) message);
                    break;
                case 10303:
                    this.UpdateServerEndPointMessageReceived((UpdateServerEndPointMessage) message);
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
        ///     Called when a <see cref="CreateHomeMessage"/> is received.
        /// </summary>
        internal void CreateHomeMessageReceived(CreateHomeMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, accountId) == ServiceCore.ServiceNodeId)
                {
                    if (!HomeManager.TryCreateHome(accountId, out _))
                    {
                        Logging.Warning(typeof(NetHomeMessageManager), "NetHomeMessageManager::createHomeMessageReceived home creation failed");
                    }
                }
                else
                {
                    Logging.Warning(typeof(NetHomeMessageManager), "NetHomeMessageManager::createHomeMessageReceived account id is not valid");
                }
            }
            else
            {
                Logging.Warning(typeof(NetHomeMessageManager), "NetHomeMessageManager::createHomeMessageReceived account id is equal at 0");
            }
        }

        /// <summary>
        ///     Called when a <see cref="AskForAvatarMessage"/> is received.
        /// </summary>
        internal void AskForAvatarMessageReceived(AskForAvatarMessage message)
        {
            LogicLong avatarId = message.RemoveAvatarId();

            if (!avatarId.IsZero())
            {
                if (HomeManager.TryGetHome(avatarId, out Home home))
                {
                    AvatarDataMessage response = new AvatarDataMessage();
                    response.SetLogicClientAvatar(home.ClientAvatar);
                    NetHomeMessageManager.SendResponseMessage(message, response);
                }
            }
        }
        
        /// <summary>
        ///     Called when a <see cref="ServerBoundMessage"/> is received.
        /// </summary>
        internal void ServerBoundMessageReceived(ServerBoundMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                if (HomeManager.TryGetHome(accountId, out Home home))
                {
                    NetHomeSession session = NetHomeSessionManager.TryCreate(home, message.GetSessionId());

                    if (session != null)
                    {
                        int[] ids = message.RemoveEndPoints();

                        for (int i = 0; i < 28; i++)
                        {
                            if (ids[i] != -1)
                            {
                                session.SetServiceNodeId(i, ids[i]);
                            }
                        }

                        home.SetSession(session);
                        home.GameMode.Init();
                    }
                    else
                    {
                        Logging.Warning(typeof(NetHomeMessageManager), "NetHomeManager::serverBoundMessageReceived pSession->NULL");
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServerUnboundMessage"/> is received.
        /// </summary>
        internal void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (sessionId != null)
            {
                if (NetHomeSessionManager.TryRemove(sessionId, out NetHomeSession session))
                {
                    session.Home.GameMode.DeInit();
                    session.Home.SetSession(null);
                    session.Destruct();
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="UpdateServerEndPointMessage"/> is received.
        /// </summary>
        internal void UpdateServerEndPointMessageReceived(UpdateServerEndPointMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                if (NetHomeSessionManager.TryGet(sessionId, out NetHomeSession session))
                {
                    if (message.GetServerType() != ServiceCore.ServiceNodeType)
                    {
                        session.SetServiceNodeId(message.GetServerType(), message.GetServerId());
                    }
                }
            }
        }
    }
}