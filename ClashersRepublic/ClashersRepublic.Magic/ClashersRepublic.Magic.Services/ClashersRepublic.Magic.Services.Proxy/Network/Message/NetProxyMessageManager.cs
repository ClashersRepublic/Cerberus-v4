namespace ClashersRepublic.Magic.Services.Proxy.Network.Message
{
    using System;
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Message.Account;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;
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
                case 10304:
                    this.UnbindServerMessageReceived((UnbindServerMessage) message);
                    break;

                case 10400:
                    this.ForwardPiranhaMessageReceived((ForwardPiranhaMessage) message);
                    break;

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
        ///     Sends the response <see cref="NetMessage"/> to the requester.
        /// </summary>
        internal static void SendResponseMessage(NetMessage requestMessage, NetMessage responseMessage)
        {
            NetMessageManager.SendMessage(requestMessage.GetServiceNodeType(), requestMessage.GetServiceNodeId(), requestMessage.GetSessionId(), responseMessage);
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
                        loginClientMessage.SetIPAddress(session.Client.GetAddress());
                        loginClientMessage.SetDeviceModel(session.Client.DeviceModel);

                        NetMessageManager.SendMessage(message.GetServiceNodeType(), message.GetServiceNodeId(), sessionId, loginClientMessage);
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
                    if (session.Client.State != 6 && session.Client.State != -1)
                    {
                        session.Client.Messaging.MessageManager.SendLoginFailedMessage(1, "Internal server error");
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
                    if (session.Client.State != 6 && session.Client.State != -1)
                    {
                        session.Client.Messaging.MessageManager.SendLoginFailedMessage(1, "Internal server error");
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

                        session.SetAccountId(loginOkMessage.AccountId);
                        session.Client.Messaging.MessageManager.SendMessage(loginOkMessage);
                        session.BindServer(10, NetManager.GetDocumentOwnerId(10, loginOkMessage.AccountId));

                        session.Client.State = 6;
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

                    if (message.GetServiceNodeType() == 2 || message.GetServiceNodeType() == 10)
                    {
                        NetworkTcpServerGateway.Disconnect(session.Client.Messaging.Connection);
                    }
                }
            }
        }
    }
}