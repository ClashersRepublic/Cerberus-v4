namespace ClashersRepublic.Magic.Services.Avatar.Network.Message
{
    using System;
    using ClashersRepublic.Magic.Logic.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Services.Avatar.Game;
    using ClashersRepublic.Magic.Services.Avatar.Network.Session;

    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Debug;
    using ClashersRepublic.Magic.Services.Core.Message.Home;
    using ClashersRepublic.Magic.Services.Core.Utils;

    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    internal class NetAvatarMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            if (message.GetSessionId() == null)
            {
                Logging.Warning("NetAvatarMessageManager::receiveMessage session id is not defined");
                return;
            }

            switch (message.GetMessageType())
            {
                case 10301:
                    this.ServerUnboundMessageReceived((ServerUnboundMessage) message);
                    break;
                case 10302:
                    this.ServerBoundMessageReceived((ServerBoundMessage) message);
                    break;
                case 10303:
                    this.UpdateServerEndPointMessageReceived((UpdateServerEndPointMessage) message);
                    break;

                case 10400:
                    this.ForwardPiranhaMessageReceived((ForwardPiranhaMessage) message);
                    break;

                case 10600:
                    this.ExecuteDebugCommandMessageReceived((ExecuteDebugCommandMessage)message);
                    break;

                case 20211:
                    this.AvatarEntryMessageReceived((AvatarEntryMessage) message);
                    break;

                case 20520:
                    this.AvatarProfileFullEntryMessageReceived((AvatarProfileFullEntryMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Sends the response <see cref="NetMessage"/> to the requester.
        /// </summary>
        internal static void SendResponseMessage(NetMessage requestMessage, NetMessage responseMessage)
        {
            NetMessageManager.SendMessage(requestMessage.GetServiceNodeType(), requestMessage.GetServiceNodeId(), requestMessage.GetSessionId(), responseMessage);
        }

        /// <summary>
        ///     Called when a <see cref="ServerBoundMessage"/> is received.
        /// </summary>
        internal void ServerBoundMessageReceived(ServerBoundMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                if (!AvatarAccountManager.TryGet(accountId, out AvatarAccount avatarAccount))
                {
                    if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, accountId) != ServiceCore.ServiceNodeId)
                    {
                        return;
                    }

                    avatarAccount = AvatarAccountManager.CreatePartyAccount(accountId);
                }
                else
                {
                    if (avatarAccount.Session != null)
                    {
                        NetAvatarSessionManager.Remove(avatarAccount.Session.SessionId);

                        avatarAccount.Session.Destruct();
                        avatarAccount.SetSession(null);
                    }
                }

                NetAvatarSession session = NetAvatarSessionManager.Create(avatarAccount, message.RemoveSessionId());

                int[] ids = message.RemoveEndPoints();

                for (int i = 0; i < 28; i++)
                {
                    if (ids[i] != -1)
                    {
                        session.SetServiceNodeId(i, ids[i]);
                    }
                }

                avatarAccount.SetSession(session);

                session.BindServer(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, NetManager.GetDocumentOwnerId(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, accountId));
                session.BindServer(NetUtils.SERVICE_NODE_TYPE_PARTY_CONTAINER, NetManager.GetDocumentOwnerId(NetUtils.SERVICE_NODE_TYPE_PARTY_CONTAINER, accountId));
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServerUnboundMessage"/> is received.
        /// </summary>
        internal void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (NetAvatarSessionManager.TryRemove(sessionId, out NetAvatarSession session))
            {
                DatabaseManager.Update(0, session.AvatarAccount.Id, LogicJSONParser.CreateJSONString(session.AvatarAccount.Save()));

                session.AvatarAccount.SetSession(null);
                session.Destruct();
            }
        }

        /// <summary>
        ///     Called when a <see cref="UpdateServerEndPointMessage"/> is received.
        /// </summary>
        internal void UpdateServerEndPointMessageReceived(UpdateServerEndPointMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetAvatarSessionManager.TryGet(sessionId, out NetAvatarSession session))
            {
                if (message.GetServerType() != ServiceCore.ServiceNodeType)
                {
                    session.SetServiceNodeId(message.GetServerType(), message.GetServerId());
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ForwardPiranhaMessage"/> is received.
        /// </summary>
        internal void ForwardPiranhaMessageReceived(ForwardPiranhaMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetAvatarSessionManager.TryGet(sessionId, out NetAvatarSession session))
            {
                PiranhaMessage piranhaMessage = message.RemovePiranhaMessage();

                if (piranhaMessage != null)
                {
                    try
                    {
                        piranhaMessage.Decode();
                        session.PiranhaMessageManager.ReceiveMessage(piranhaMessage);
                    }
                    catch (Exception exception)
                    {
                        Logging.Warning("NetHomeMessageManager::forwardPiranhaMessageReceived piranha message handle exception, trace: " + exception);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="AvatarEntryMessage"/> is received.
        /// </summary>
        internal void AvatarEntryMessageReceived(AvatarEntryMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetAvatarSessionManager.TryGet(sessionId, out NetAvatarSession session))
            {
                message.RemoveAvatarEntry().CopyTo(session.AvatarAccount.AvatarEntry);
                
                if (session.GetServiceNodeEndPoint(NetUtils.SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER) != null)
                {
                    AvatarEntryMessage avatarEntryMessage = new AvatarEntryMessage();
                    avatarEntryMessage.SetAvatarEntry(session.AvatarAccount.AvatarEntry);
                    session.SendMessage(NetUtils.SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER, avatarEntryMessage);
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="AvatarProfileFullEntryMessage"/> is received.
        /// </summary>
        internal void AvatarProfileFullEntryMessageReceived(AvatarProfileFullEntryMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetAvatarSessionManager.TryGet(sessionId, out NetAvatarSession session))
            {
                AvatarProfileMessage avatarProfileMessage = new AvatarProfileMessage();
                avatarProfileMessage.SetAvatarProfileFullEntry(message.RemoveAvatarProfileFullEntry());
                session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, avatarProfileMessage);
            }
        }

        /// <summary>
        ///     Called when a <see cref="ExecuteDebugCommandMessage"/> is received.
        /// </summary>
        private void ExecuteDebugCommandMessageReceived(ExecuteDebugCommandMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetAvatarSessionManager.TryGet(sessionId, out NetAvatarSession session))
            {
                DebugCommand debugCommand = message.RemoveDebugCommand();

                if (debugCommand.GetServiceNodeType() == ServiceCore.ServiceNodeType)
                {
                    switch (debugCommand.GetCommandType())
                    {
                        default:
                            Logging.Debug(string.Format("executeDebugCommandMessageReceived unknown debug command received ({0})", debugCommand.GetCommandType()));
                            break;
                    }
                }
                else
                {
                    Logging.Debug(string.Format("executeDebugCommandMessageReceived invalid debug command received ({0})", debugCommand.GetCommandType()));
                }
            }
        }
    }
}