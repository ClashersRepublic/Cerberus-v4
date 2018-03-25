namespace ClashersRepublic.Magic.Services.Zone.Network.Message
{
    using System;
    using ClashersRepublic.Magic.Logic.Command.Server;
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
                if (!ZoneAccountManager.TryGet(accountId, out ZoneAccount home))
                {
                    if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, accountId) != ServiceCore.ServiceNodeId)
                    {
                        return;
                    }

                    home = ZoneAccountManager.CreateZoneAccount(accountId);
                }

                NetZoneSession session = NetZoneSessionManager.Create(home, message.RemoveSessionId());

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
        ///     Calledw hen a <see cref="AllowServerCommandMessage"/> is received.
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
    }
}