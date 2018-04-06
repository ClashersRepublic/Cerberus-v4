namespace RivieraStudio.Magic.Services.Zone.Network.Message
{
    using System;
    using RivieraStudio.Magic.Logic.Command.Server;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Message.Avatar;
    using RivieraStudio.Magic.Logic.Message.Home;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Services.Zone.Game;
    using RivieraStudio.Magic.Services.Zone.Network.Session;
    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Message;
    using RivieraStudio.Magic.Services.Core.Message.Admin;
    using RivieraStudio.Magic.Services.Core.Message.Home;
    using RivieraStudio.Magic.Services.Core.Message.Network;
    using RivieraStudio.Magic.Services.Core.Message.Session;
    using RivieraStudio.Magic.Services.Core.Network;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Message;

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

                case 10511:
                    this.AllowServerCommandMessageReceived((AllowServerCommandMessage) message);
                    break;
                case 10520:
                    this.AskForAvatarProfileFullEntryMessageReceived((AskForAvatarProfileFullEntryMessage) message);
                    break;

                case 10600:
                    this.ExecuteAdminCommandMessageReceived((ExecuteAdminCommandMessage) message);
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
        private void ServerBoundMessageReceived(ServerBoundMessage message)
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
        private void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (NetZoneSessionManager.TryRemove(sessionId, out NetZoneSession session))
            {
                session.ZoneAccount.GameMode.DeInit();
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
        private void ForwardPiranhaMessageReceived(ForwardPiranhaMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetZoneSessionManager.TryGet(sessionId, out NetZoneSession session))
            {
                if (session.PiranhaMessageManager == null)
                {
                    Logging.Print("NetZoneMessage::forwardPiranhaMessageReceived session disposed");
                    return;
                }

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
        private void AllowServerCommandMessageReceived(AllowServerCommandMessage message)
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
                if (account.GameMode.ExecutedCommandsSinceLastSave != 0)
                {
                    account.GameMode.Save();
                }

                AvatarProfileFullEntryMessage avatarProfileFullEntryMessage = new AvatarProfileFullEntryMessage();
                AvatarProfileFullEntry entry = new AvatarProfileFullEntry();
                LogicCompressibleString compressibleString = account.ClientHome.GetCompressibleHomeJSON();

                if (!compressibleString.IsCompressed())
                {
                    CompressibleStringHelper.Compress(compressibleString);
                }
                
                entry.SetLogicClientAvatar(account.ClientAvatar);
                entry.SetCompressedHomeJSON(compressibleString.GetCompressed());
                avatarProfileFullEntryMessage.SetAvatarProfileFullEntry(entry);
                
                NetMessageManager.SendMessage(message.GetServiceNodeType(), message.GetServiceNodeId(), sessionId, avatarProfileFullEntryMessage);
            }
        }

        /// <summary>
        ///     Called when a <see cref="ExecuteAdminCommandMessage"/> is received.
        /// </summary>
        private void ExecuteAdminCommandMessageReceived(ExecuteAdminCommandMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetZoneSessionManager.TryGet(sessionId, out NetZoneSession session))
            {
                AdminCommand adminCommand = message.RemoveDebugCommand();

                if (adminCommand.GetServiceNodeType() == ServiceCore.ServiceNodeType)
                {
                    switch (adminCommand.GetCommandType())
                    {
                        case 1000:
                            ZoneAccount account = session.ZoneAccount;
                            NetZoneSessionManager.Remove(sessionId);

                            session.SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER, new OutOfSyncMessage());
                            session.SendMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, new UnbindServerMessage());

                            NetZoneSessionManager.Remove(sessionId);

                            session.ZoneAccount.GameMode.DeInit();
                            session.Destruct();

                            ZoneAccountManager.UpdateZoneAccount(account.Id, new ZoneAccount(account.Id));
                            break;
                        case 1001:
                            LogicDiamondsAddedCommand diamondsAddedCommand = new LogicDiamondsAddedCommand();

                            diamondsAddedCommand.SetData(true, 14000, -1, false, 0, null);
                            session.ZoneAccount.GameMode.AddAvailableServerCommand(diamondsAddedCommand);

                            break;
                        default:
                            Logging.Print(string.Format("executeAdminCommandMessageReceived unknown debug command received ({0})", adminCommand.GetCommandType()));
                            break;
                    }
                }
                else
                {
                    Logging.Print(string.Format("executeAdminCommandMessageReceived invalid debug command received ({0})", adminCommand.GetCommandType()));
                }
            }
        }
    }
}