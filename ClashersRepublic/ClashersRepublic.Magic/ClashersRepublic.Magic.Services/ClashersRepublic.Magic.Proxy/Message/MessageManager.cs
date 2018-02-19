namespace ClashersRepublic.Magic.Proxy.Message
{
    using System;

    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Logic.Message.Google;

    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.Session;
    using ClashersRepublic.Magic.Proxy.User;

    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Services.Logic.Message.Game;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Services.Logic.Service;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;
    using ClashersRepublic.Magic.Titan.Util;

    internal class MessageManager
    {
        internal Client Client;
        internal NetworkMessaging Messaging;

        /// <summary>
        ///     Gets the last keep alive time.
        /// </summary>
        internal int LastKeepAliveTime { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager" /> class.
        /// </summary>
        internal MessageManager(Client client, NetworkMessaging messaging)
        {
            this.LastKeepAliveTime = LogicTimeUtil.GetTimestamp();

            this.Client = client;
            this.Messaging = messaging;
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            if (this.Client.State != 0)
            {
                if (this.Client.State != 6)
                {
                    if (messageType != 10100 && messageType != 10101 && messageType != 10108 && messageType != 10121)
                    {
                        return;
                    }
                }
            }
            else
            {
                return;
            }

            switch (messageType)
            {
                case 10100:
                {
                    this.ClientHelloMessageReceived((ClientHelloMessage) message);
                    break;
                }
                case 10101:
                {
                    this.LoginMessageReceived((LoginMessage) message);
                    break;
                }

                case 10108:
                {
                    this.KeepAliveMessageReceived((KeepAliveMessage) message);
                    break;
                }

                case 14262:
                {
                    this.BindGoogleServiceAccountMessageReceived((BindGoogleServiceAccountMessage) message);
                    break;
                }

                default:
                {
                    Logging.Warning(this, "MessageManager::receiveMessage no case for message type " + messageType);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends the specified <see cref="PiranhaMessage" /> to client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this.Client.NetworkToken.Messaging.Send(message);
        }

        /// <summary>
        ///     Sends a <see cref="LoginFailedMessage" /> to client.
        /// </summary>
        internal void SendLoginFailedMessage(int errorCode, string reason = null, string param = null)
        {
            LoginFailedMessage message = new LoginFailedMessage
            {
                ErrorCode = errorCode,
                Reason = reason,
                ContentUrlList = ResourceManager.ContentUrlList
            };

            switch (errorCode)
            {
                case 7:
                {
                    ZLibHelper.ConpressInZLibFormat(LogicStringUtil.GetBytes(ResourceManager.FingerprintJson), out message.ResourceFingerprintData);
                    break;
                }

                case 8:
                {
                    if (this.Client.Defines.DeviceType == 0)
                    {
                        message.UpdateUrl = ResourceManager.GetAppStoreUrl(this.Client.Defines.AndroidClient ? 1 : 2);
                    }
                    else
                    {
                        message.UpdateUrl = ResourceManager.GetAppStoreUrl(this.Client.Defines.DeviceType);
                    }

                    break;
                }

                case 9:
                {
                    message.RedirectDomain = param;
                    break;
                }

                case 10:
                {
                    break;
                }
            }

            this.SendMessage(message);
        }

        /// <summary>
        ///     Sends a <see cref="LoginOkMessage"/> to client.
        /// </summary>
        internal void SendLoginOkMessage(GameAccount account)
        {
            if (this.Client.State != 0)
            {
                if (this.Client.State != 6)
                {
                    this.SendMessage(new LoginOkMessage
                    {
                        AccountId = account.Id,
                        HomeId = account.Id,
                        ServerTime = LogicTimeUtil.GetTimestampMS(),
                        AccountCreatedDate = account.AccountCreationDate,
                        PassToken = account.PassToken,
                        Region = "fr-FR",

                        ServerMajorVersion = 9,
                        ServerBuildVersion = 256,
                        
                        ServerEnvironment = ServiceConfig.GetEnvironment(),
                        ContentVersion = ResourceManager.GetContentVersion(),
                        ContentUrlList = ResourceManager.ContentUrlList,
                        ChronosContentUrlList = ResourceManager.ChronosContentUrlList
                    });

                    this.Client.State = 6;
                }
                else
                {
                    Logging.Warning(this, "MessageManager::sendLoginOkMessage client already logged");
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ClientHelloMessageReceived"/> has been received.
        /// </summary>
        internal void ClientHelloMessageReceived(ClientHelloMessage message)
        {
            if (this.CheckClientVersion(message.MajorVersion, message.BuildVersion, message.ContentHash))
            {
                this.SendLoginFailedMessage(10, "Internal server error");
            }
        }

        /// <summary>
        ///     Called when a <see cref="LoginMessageReceived" /> has been received.
        /// </summary>
        internal void LoginMessageReceived(LoginMessage message)
        {
            if (this.CheckClientVersion(message.ClientMajorVersion, message.ClientBuildVersion, message.ResourceSha))
            {
                this.Messaging.ScramblerSeed = message.ScramblerSeed;

                if (message.AccountId.IsZero())
                {
                    if (message.PassToken == null)
                    {
                        GameAccount account = GameAccountManager.CreateAccount();

                        if (account != null)
                        {
                            ServiceMessageManager.SendMessage(new CreateDataMessage
                            {
                                Id = account.Id
                            }, 10, account.Id.GetHigherInt());

                            GameSessionManager.CreateSession(this.Client, account);
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
                        int result = GameAccountManager.GetAccount(message.AccountId, out GameAccount account);

                        if (result == 0)
                        {
                            if (message.PassToken == account.PassToken)
                            {
                                GameSession currentSession = account.CurrentSession;

                                if (currentSession != null)
                                {
                                    // TODO: Close the current session.
                                }

                                GameSessionManager.CreateSession(this.Client, account);
                            }
                        }
                        else if (result == 1)
                        {
                            this.SendLoginFailedMessage(1, "Internal server error");
                        }
                        else
                        {
                            this.SendLoginFailedMessage(1);
                        }
                    }
                    else
                    {
                        this.SendLoginFailedMessage(1);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="KeepAliveMessage"/> has been received.
        /// </summary>
        internal void KeepAliveMessageReceived(KeepAliveMessage message)
        {
            this.LastKeepAliveTime = LogicTimeUtil.GetTimestamp();

            if (this.Client.State == 6)
            {
                this.SendMessage(new KeepAliveServerMessage());
            }
        }

        /// <summary>
        ///     Called when a <see cref="BindGoogleServiceAccountMessage"/> has been received.
        /// </summary>
        internal void BindGoogleServiceAccountMessageReceived(BindGoogleServiceAccountMessage message)
        {
            Console.WriteLine("ServiceID: " + message.GoogleServiceId + " AuthCode: " + message.AuthCode);
        }

        /// <summary>
        ///     Checks the client version.
        /// </summary>
        internal bool CheckClientVersion(int majorVersion, int buildVersion, string contentHash)
        {
            if (majorVersion == LogicVersion.MajorVersion && buildVersion == LogicVersion.BuildVersion)
            {
                if (contentHash == ResourceManager.FingerprintSha)
                {
                    return true;
                }

                this.SendLoginFailedMessage(7);
            }
            else
            {
                this.SendLoginFailedMessage(8);
            }

            return false;
        }
    }
}