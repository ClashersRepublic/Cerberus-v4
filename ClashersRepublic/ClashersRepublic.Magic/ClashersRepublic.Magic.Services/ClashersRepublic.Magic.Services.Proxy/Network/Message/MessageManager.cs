namespace ClashersRepublic.Magic.Services.Proxy.Network.Message
{
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Proxy.Network.Session;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;
    using ClashersRepublic.Magic.Titan.Util;

    internal class MessageManager
    {
        private int _lastKeepAliveTime;

        private NetworkClient _client;
        private NetworkMessaging _messaging;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(NetworkMessaging messaging)
        {
            this._client = messaging.Client;
            this._messaging = messaging;
        }

        /// <summary>
        ///     Receives the <see cref="PiranhaMessage"/>.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            if (this._client.State != -1)
            {
                if (this._client.State != 6)
                {
                    if (messageType != 10100 && messageType != 10101 && messageType != 10108)
                    {
                        return;
                    }
                }
                else
                {
                    if (message.GetServiceNodeType() != ServiceCore.ServiceNodeType)
                    {
                        NetProxySession session = this._client.GetSession();
                        NetSocket socket = session.GetServiceNodeEndPoint(message.GetServiceNodeType());

                        if (socket != null)
                        {
                            ForwardPiranhaMessage forwardPiranhaMessage = new ForwardPiranhaMessage();
                            forwardPiranhaMessage.SetPiranhaMessage(message);
                            NetMessageManager.SendMessage(socket, session.SessionId, session.SessionId.Length, forwardPiranhaMessage);
                        }
                        else
                        {
                            Logging.Debug("MessageManager::receiveMessage no server for service " + message.GetServiceNodeType());
                        }

                        return;
                    }
                }
            }
            else
            {
                return;
            }

            switch (message.GetMessageType())
            {
                case 10100:
                    this.ClientHelloMessageReceived((ClientHelloMessage) message);
                    break;
                case 10101:
                    this.LoginMessageReceived((LoginMessage) message);
                    break;
                case 10108:
                    this.KeepAliveMessageReceived((KeepAliveMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Sends the <see cref="PiranhaMessage"/> to client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this._messaging.Send(message);
        }

        /// <summary>
        ///     Sends the <see cref="LoginFailedMessage"/> to client.
        /// </summary>
        internal void SendLoginFailedMessage(int errorCode, string reason = null)
        {
            LoginFailedMessage loginFailedMessage = new LoginFailedMessage
            {
                ErrorCode = errorCode,
                Reason = reason,
                ContentUrlList = ResourceManager.ContentUrlList
            };
            
            switch (errorCode)
            {
                case 7:
                    loginFailedMessage.ContentUrl = "https://raw.githubusercontent.com/Mimi8298/magic-assets/master/";
                    loginFailedMessage.CompressedFingerprintJson = ResourceManager.CompressedFingeprintJson;
                    break;
                case 8:
                    loginFailedMessage.UpdateUrl = ResourceManager.GetAppStoreUrl(0);
                    break;
            }

            this.SendMessage(loginFailedMessage);
        }

        /// <summary>
        ///     Called when the <see cref="ClientHelloMessage"/> is received.
        /// </summary>
        private void ClientHelloMessageReceived(ClientHelloMessage message)
        {
            if (message.GetProtocol() == 1)
            {
                if (message.GetMajorVersion() == LogicVersion.MajorVersion && message.GetBuildVersion() == LogicVersion.BuildVersion)
                {
                    this.SendLoginFailedMessage(7);
                }
                else
                {
                    this.SendLoginFailedMessage(8);
                }
            }
            else
            {
                this.SendLoginFailedMessage(8);
            }
        }

        /// <summary>
        ///     Called when the <see cref="LoginMessage"/> is received.
        /// </summary>
        private void LoginMessageReceived(LoginMessage message)
        {
            this._messaging.ScramblerSeed = message.ScramblerSeed;
            this._client.DeviceModel = message.Device;

            if (message.ClientMajorVersion == LogicVersion.MajorVersion && message.ClientBuildVersion == LogicVersion.BuildVersion)
            {
                if (message.ResourceSha == ResourceManager.FingerprintSha)
                {
                    if (message.AccountId.IsZero())
                    {
                        if (message.PassToken == null)
                        {
                            NetSocket socket = NetManager.GetRandomEndPoint(2);

                            if (socket != null)
                            {
                                NetProxySession session = NetProxySessionManager.TryCreate(this._client);

                                if (session != null)
                                {
                                    this._client.SetSession(session);
                                    byte[] sessionId = session.SessionId;

                                    session.SetServiceNodeId(1, ServiceCore.ServiceNodeId);
                                    session.SetServiceNodeId(2, socket.Id);

                                    NetMessageManager.SendMessage(socket, sessionId, sessionId.Length, new CreateAccountMessage());
                                }
                                else
                                {
                                    this.SendLoginFailedMessage(1);
                                }
                            }
                            else
                            {
                                this.SendLoginFailedMessage(1, "Internal server error");
                            }
                        }
                        else
                        {
                            this.SendLoginFailedMessage(1);
                        }
                    }
                    else
                    {
                        if (message.PassToken != null)
                        {
                            NetSocket socket = NetManager.GetServiceNodeEndPoint(2, NetManager.GetDocumentOwnerId(2, message.AccountId));

                            if (socket != null)
                            {
                                NetProxySession session = NetProxySessionManager.TryCreate(this._client);

                                if (session != null)
                                {
                                    this._client.SetSession(session);
                                    byte[] sessionId = session.SessionId;

                                    session.SetServiceNodeId(1, ServiceCore.ServiceNodeId);
                                    session.SetServiceNodeId(2, socket.Id);

                                    LoginClientMessage loginClientMessage = new LoginClientMessage();
                                    loginClientMessage.SetAccountId(message.AccountId);
                                    loginClientMessage.SetPassToken(message.PassToken);
                                    loginClientMessage.SetIPAddress(this._client.NetworkToken.ClientIP);
                                    loginClientMessage.SetDeviceModel(this._client.DeviceModel);
                                    NetMessageManager.SendMessage(socket, sessionId, sessionId.Length, loginClientMessage);
                                }
                                else
                                {
                                    this.SendLoginFailedMessage(1);
                                }
                            }
                            else
                            {
                                this.SendLoginFailedMessage(1, "Internal server error");
                            }
                        }
                        else
                        {
                            this.SendLoginFailedMessage(1);
                        }
                    }
                }
                else
                {
                    this.SendLoginFailedMessage(7);
                }
            }
            else
            {
                this.SendLoginFailedMessage(8);
            }
        }

        /// <summary>
        ///     Called when the <see cref="KeepAliveMessage"/> is received.
        /// </summary>
        private void KeepAliveMessageReceived(KeepAliveMessage message)
        {
            this._lastKeepAliveTime = LogicTimeUtil.GetTimestamp();

            if (this._client.State == 6)
            {
                this.SendMessage(new KeepAliveServerMessage());
            }
        }
    }
}