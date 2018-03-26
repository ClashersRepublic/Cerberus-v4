namespace ClashersRepublic.Magic.Services.Proxy.Network.Message
{
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Message.Account;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Core.Utils;

    using ClashersRepublic.Magic.Services.Proxy.Network.ServerSocket;
    using ClashersRepublic.Magic.Services.Proxy.Network.Session;

    using ClashersRepublic.Magic.Titan.Message;

    internal class NetProxyMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            switch (message.GetMessageType())
            {
                case 10303:
                    this.UpdateServerEndPointMessageReceived((UpdateServerEndPointMessage) message);
                    break;
                case 10304:
                    this.UnbindServerMessageReceived((UnbindServerMessage) message);
                    break;

                case 10400:
                    this.ForwardPiranhaMessageReceived((ForwardPiranhaMessage) message);
                    break;
                    
                case 20100:
                    this.LoginClientFailedMessageReceived((LoginClientFailedMessage) message);
                    break;
                case 20101:
                    this.LoginClientOkMessageReceived((LoginClientOkMessage) message);
                    break;

                default:
                    Logging.Debug("NetProxyMessageManager::receiveMessage no case for message type " + message.GetMessageType());
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
        ///     Called when the <see cref="LoginClientFailedMessage"/> is received.
        /// </summary>
        internal void LoginClientFailedMessageReceived(LoginClientFailedMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                if (NetProxySessionManager.TryGet(sessionId, out NetProxySession session))
                {
                    Logging.Debug("loginClientFailedMessageReceived login failed, error code: " + message.GetErrorCode());

                    if (session.Client.State != 6 && session.Client.State != -1)
                    {
                        if (message.GetErrorCode() == 1)
                        {
                            session.Client.Messaging.MessageManager.SendLoginFailedMessage(11, message.GetReason());
                        }
                        else
                        {
                            session.Client.Messaging.MessageManager.SendLoginFailedMessage(1, "Internal server error");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the <see cref="LoginClientOkMessage"/> is received.
        /// </summary>
        internal void LoginClientOkMessageReceived(LoginClientOkMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                if (NetProxySessionManager.TryGet(sessionId, out NetProxySession session))
                {
                    if (session.Client.State == 1)
                    {
                        LoginOkMessage loginOkMessage = new LoginOkMessage
                        {
                            AccountId = message.RemoveAccountId(),
                            HomeId = message.RemoveHomeId(),
                            PassToken = message.RemovePassToken(),
                            AccountCreatedDate = message.RemoveAccountCreatedDate(),
                            FacebookId = message.RemoveFacebookId(),
                            GamecenterId = message.RemoveGamecenterId(),
                            GoogleServiceId = message.RemoveGoogleServiceId(),
                            SessionCount = message.GetSessionCount(),
                            PlayTimeSeconds = message.GetPlayTimeSeconds(),
                            DaysSinceStartedPlaying = message.GetDaysSinceStartedPlaying(),

                            ServerMajorVersion = LogicVersion.MajorVersion,
                            ServerBuildVersion = LogicVersion.BuildVersion,
                            ContentVersion = ResourceManager.GetContentVersion(),
                            ContentUrlList = ResourceManager.ContentUrlList,
                            ChronosContentUrlList = ResourceManager.ChronosContentUrlList,

                            Region = "fr-FR"
                        };


                        session.SetAccountId(loginOkMessage.AccountId);
                        session.Client.Messaging.Send(loginOkMessage);
                        session.Client.State = 6;

                        session.BindServer(NetUtils.SERVICE_NODE_TYPE_AVATAR_CONTAINER, NetManager.GetDocumentOwnerId(NetUtils.SERVICE_NODE_TYPE_AVATAR_CONTAINER, loginOkMessage.AccountId));

                        if (message.GetChatAccountBanSeconds() != 0)
                        {
                            ChatAccountBanStatusMessage chatAccountBanStatusMessage = new ChatAccountBanStatusMessage();
                            chatAccountBanStatusMessage.SetBanSeconds(message.GetChatAccountBanSeconds());
                            session.Client.Messaging.Send(chatAccountBanStatusMessage);
                        }
                        else
                        {
                            session.BindServer(NetUtils.SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER, NetManager.GetRandomEndPoint(NetUtils.SERVICE_NODE_TYPE_GLOBAL_CHAT_CONTAINER).Id);
                        }
                    }
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
                if (NetProxySessionManager.TryGet(sessionId, out NetProxySession session))
                {
                    if (message.GetServerType() != ServiceCore.ServiceNodeType)
                    {
                        session.SetServiceNodeId(message.GetServerType(), message.GetServerId());
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the <see cref="ForwardPiranhaMessage"/> is received.
        /// </summary>
        internal void ForwardPiranhaMessageReceived(ForwardPiranhaMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                PiranhaMessage piranhaMessage = message.RemovePiranhaMessage();

                if (piranhaMessage != null)
                {
                    piranhaMessage.GetByteStream().SetOffset(piranhaMessage.GetByteStream().GetLength());

                    if (NetProxySessionManager.TryGet(sessionId, out NetProxySession session))
                    {
                        session.Client.Messaging.MessageManager.SendMessage(piranhaMessage);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the <see cref="UnbindServerMessage"/> is received.
        /// </summary>
        internal void UnbindServerMessageReceived(UnbindServerMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                if (NetProxySessionManager.TryGet(sessionId, out NetProxySession session))
                {
                    session.UnbindServer(message.GetServiceNodeType(), false);

                    if (message.GetServiceNodeType() == NetUtils.SERVICE_NODE_TYPE_ACCOUNT_DIRECTORY || message.GetServiceNodeType() == NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER)
                    {
                        NetworkTcpServerGateway.Disconnect(session.Client.Messaging.Connection);
                    }
                }
            }
        }
    }
}