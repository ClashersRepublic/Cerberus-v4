namespace ClashersRepublic.Magic.Services.Zone.Network.Message
{
    using System;

    using ClashersRepublic.Magic.Logic.Command.Server;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message.Avatar;
    using ClashersRepublic.Magic.Logic.Util;

    using ClashersRepublic.Magic.Services.Zone.Game;
    using ClashersRepublic.Magic.Services.Zone.Network.Session;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Home;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    internal class NetZoneMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            if (message.GetSessionId() == null)
            {
                Logging.Warning("NetZoneMessageManager::receiveMessage session id is not defined");
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

                case 10520:
                    this.AskForAvatarProfileFullEntryMessageReceived((AskForAvatarProfileFullEntryMessage) message);
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
                if (!ZoneAccountManager.TryGet(accountId, out ZoneAccount zoneAccount))
                {
                    if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, accountId) != ServiceCore.ServiceNodeId)
                    {
                        return;
                    }

                    zoneAccount = ZoneAccountManager.CreateZoneAccount(accountId);
                }
                else
                {
                    if (zoneAccount.Session != null)
                    {
                        NetZoneSessionManager.Remove(zoneAccount.Session.SessionId);

                        zoneAccount.GameMode.DeInit();
                        zoneAccount.Session.Destruct();
                        zoneAccount.SetSession(null);
                    }
                }

                NetZoneSession session = NetZoneSessionManager.Create(zoneAccount, message.RemoveSessionId());

                int[] ids = message.RemoveEndPoints();

                for (int i = 0; i < 28; i++)
                {
                    if (ids[i] != -1)
                    {
                        session.SetServiceNodeId(i, ids[i]);
                    }
                }

                zoneAccount.SetSession(session);
                zoneAccount.GameMode.Init();
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServerUnboundMessage"/> is received.
        /// </summary>
        internal void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (NetZoneSessionManager.TryRemove(sessionId, out NetZoneSession session))
            {
                session.ZoneAccount.GameMode.DeInit();
                session.ZoneAccount.SetSession(null);
                session.Destruct();
            }
        }

        /// <summary>
        ///     Called when a <see cref="UpdateServerEndPointMessage"/> is received.
        /// </summary>
        internal void UpdateServerEndPointMessageReceived(UpdateServerEndPointMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetZoneSessionManager.TryGet(sessionId, out NetZoneSession session))
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

            if (NetZoneSessionManager.TryGet(sessionId, out NetZoneSession session))
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
                        Logging.Warning("NetZoneMessageManager::forwardPiranhaMessageReceived piranha message handle exception, trace: " + exception);
                    }
                }
            }
        }

        /// <summary>
        ///     Calledw when a <see cref="AllowServerCommandMessage"/> is received.
        /// </summary>
        internal void AllowServerCommandMessageReceived(AllowServerCommandMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetZoneSessionManager.TryGet(sessionId, out NetZoneSession session))
            {
                LogicServerCommand serverCommand = message.RemoveServerCommand();

                if (serverCommand != null)
                {
                    session.ZoneAccount.GameMode.AddAvailableServerCommand(serverCommand);
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="AskForAvatarProfileFullEntryMessage"/> is received.
        /// </summary>
        private void AskForAvatarProfileFullEntryMessageReceived(AskForAvatarProfileFullEntryMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (ZoneAccountManager.TryGet(message.RemoveAvatarId(), out ZoneAccount account))
            {
                AvatarProfileFullEntryMessage avatarProfileFullEntryMessage = new AvatarProfileFullEntryMessage();
                AvatarProfileFullEntry entry = new AvatarProfileFullEntry();
                LogicCompressibleString compressibleString = account.ClientHome.GetCompressibleHomeJSON().Clone();

                if (!compressibleString.IsCompressed())
                {
                    CompressibleStringHelper.Compress(compressibleString);
                }

                entry.SetLogicClientAvatar(account.ClientAvatar); 
                entry.SetCompressedHomeJSON(compressibleString.RemoveCompressed());
                avatarProfileFullEntryMessage.SetAvatarProfileFullEntry(entry);
                compressibleString.Destruct();

                NetMessageManager.SendMessage(message.GetServiceNodeType(), message.GetServiceNodeId(), sessionId, avatarProfileFullEntryMessage);
            }
        }
    }
}