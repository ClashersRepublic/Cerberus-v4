namespace ClashersRepublic.Magic.Services.Party.Network.Message
{
    using System;
    
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Party.Game;
    using ClashersRepublic.Magic.Services.Party.Network.Session;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    internal class NetPartyMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            if (message.GetSessionId() == null)
            {
                Logging.Warning("NetPartyMessageManager::receiveMessage session id is not defined");
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
                if (!PartyAccountManager.TryGet(accountId, out PartyAccount partyAccount))
                {
                    if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, accountId) != ServiceCore.ServiceNodeId)
                    {
                        return;
                    }

                    partyAccount = PartyAccountManager.CreatePartyAccount(accountId);
                }
                else
                {
                    if (partyAccount.Session != null)
                    {
                        NetPartySessionManager.Remove(partyAccount.Session.SessionId);

                        partyAccount.Session.Destruct();
                        partyAccount.SetSession(null);
                    }
                }

                NetPartySession session = NetPartySessionManager.Create(partyAccount, message.RemoveSessionId());

                int[] ids = message.RemoveEndPoints();

                for (int i = 0; i < 28; i++)
                {
                    if (ids[i] != -1)
                    {
                        session.SetServiceNodeId(i, ids[i]);
                    }
                }

                partyAccount.SetSession(session);
                session.SendAvatarStreamMessage();
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServerUnboundMessage"/> is received.
        /// </summary>
        internal void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();
            if (NetPartySessionManager.TryRemove(sessionId, out NetPartySession session))
            {
                session.PartyAccount.SetSession(null);
                session.Destruct();
            }
        }

        /// <summary>
        ///     Called when a <see cref="UpdateServerEndPointMessage"/> is received.
        /// </summary>
        internal void UpdateServerEndPointMessageReceived(UpdateServerEndPointMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (NetPartySessionManager.TryGet(sessionId, out NetPartySession session))
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

            if (NetPartySessionManager.TryGet(sessionId, out NetPartySession session))
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
                        Logging.Warning("NetPartyMessageManager::forwardPiranhaMessageReceived piranha message handle exception, trace: " + exception);
                    }
                }
            }
        }
    }
}