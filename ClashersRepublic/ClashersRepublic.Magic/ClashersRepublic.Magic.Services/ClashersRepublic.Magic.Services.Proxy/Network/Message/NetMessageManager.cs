namespace ClashersRepublic.Magic.Services.Proxy.Network.Message
{
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Proxy.Network.Session;

    internal class NetMessageManager : INetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public void ReceiveMessage(NetMessage message)
        {
            switch (message.GetMessageType())
            {
                case 20101:
                    this.CreateAccountOkMessageReceived((CreateAccountOkMessage) message);
                    break;
                case 20102:
                    this.CreateAccountFailedMessageReceived((CreateAccountFailedMessage) message);
                    break;
                case 20103:
                    this.LoginClientFailedMessageReceived((LoginClientFailedMessage) message);
                    break;
                case 20104:
                    this.LoginClientOkMessageReceived((LoginClientOkMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Sends the message to the specified service node.
        /// </summary>
        internal void SendMessage(NetMessage message, int serviceNodeType, int serviceNodeId, byte[] sessionId, int sessionIdLength)
        {
            NetMessaging.Send(serviceNodeType, serviceNodeId, sessionId, sessionIdLength, message);
        }

        /// <summary>
        ///     Sends the response message to requester.
        /// </summary>
        internal void SendResponseMessage(NetMessage requestMessage, NetMessage responseMessage)
        {
            int sessionIdLength = requestMessage.GetSessionIdLength();
            byte[] sessionId = requestMessage.RemoveSessionId();

            NetMessaging.Send(requestMessage.GetServiceNodeType(), requestMessage.GetServiceNodeId(), sessionId, sessionIdLength, responseMessage);
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountOkMessage"/> is received.
        /// </summary>
        internal void CreateAccountOkMessageReceived(CreateAccountOkMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (sessionId != null)
            {
                if (NetProxySessionManager.TryGet(NetProxySessionManager.ConvertSessionIdToSessionName(sessionId), out NetProxySession session))
                {
                    if (session.Client.State == 1)
                    {
                        LoginClientMessage loginClientMessage = new LoginClientMessage();
                        loginClientMessage.SetAccountId(message.RemoveAccountId());
                        loginClientMessage.SetPassToken(message.RemovePassToken());
                        NetMessaging.Send(message.GetServiceNodeType(), message.GetServiceNodeId(), sessionId, sessionId.Length, loginClientMessage);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountFailedMessage"/> is received.
        /// </summary>
        internal void CreateAccountFailedMessageReceived(CreateAccountFailedMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                if (NetProxySessionManager.TryGet(NetProxySessionManager.ConvertSessionIdToSessionName(sessionId), out NetProxySession session))
                {
                    if (session.Client.State == 1)
                    {
                        session.Client.MessageManager.SendLoginFailedMessage(1, "Internal server error");
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the <see cref="LoginClientFailedMessage"/> is received.
        /// </summary>
        internal void LoginClientFailedMessageReceived(LoginClientFailedMessage message)
        {
            byte[] sessionId = message.RemoveSessionId();

            if (sessionId != null)
            {
                if (NetProxySessionManager.TryGet(NetProxySessionManager.ConvertSessionIdToSessionName(sessionId), out NetProxySession session))
                {
                    if (session.Client.State == 1)
                    {
                        session.Client.MessageManager.SendLoginFailedMessage(1, "Internal server error");
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
                if (NetProxySessionManager.TryGet(NetProxySessionManager.ConvertSessionIdToSessionName(sessionId), out NetProxySession session))
                {
                    if (session.Client.State == 1)
                    {
                        LoginOkMessage loginOkMessage = new LoginOkMessage();
                        loginOkMessage.AccountId = message.RemoveAccountId();
                        loginOkMessage.HomeId = message.RemoveHomeId();
                        loginOkMessage.PassToken = message.RemovePassToken();
                        loginOkMessage.AccountCreatedDate = message.RemoveAccountCreatedDate();
                        loginOkMessage.FacebookId = message.RemoveFacebookId();
                        loginOkMessage.GamecenterId = message.RemoveGamecenterId();
                        loginOkMessage.GoogleServiceId = message.RemoveGoogleServiceId();
                        loginOkMessage.SessionCount = message.GetSessionCount();
                        loginOkMessage.PlayTimeSeconds = message.GetPlayTimeSeconds();
                        loginOkMessage.DaysSinceStartedPlaying = message.GetDaysSinceStartedPlaying();
                        loginOkMessage.Region = "fr-FR";

                        loginOkMessage.ServerMajorVersion = LogicVersion.MajorVersion;
                        loginOkMessage.ServerBuildVersion = LogicVersion.BuildVersion;
                        loginOkMessage.ContentVersion = ResourceManager.GetContentVersion();
                        loginOkMessage.ContentUrlList = ResourceManager.ContentUrlList;
                        loginOkMessage.ChronosContentUrlList = ResourceManager.ChronosContentUrlList;

                        session.Client.MessageManager.SendMessage(loginOkMessage);
                    }
                }
            }
        }
    }
}