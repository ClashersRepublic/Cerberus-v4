namespace ClashersRepublic.Magic.Proxy.Message
{
    using System;
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.User;
    using ClashersRepublic.Magic.Services.Logic.Resource;
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
            this.Client = client;
            this.Messaging = messaging;
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            if (this.Client.State != 6)
            {
                if (messageType != 10100 && messageType != 10101 && messageType != 10121)
                {
                    return;
                }
            }

            switch (messageType)
            {
                case 10100:
                {
                    this.ClientHelloMessage((ClientHelloMessage) message);
                    break;
                }
                case 10101:
                {
                    this.LoginMessage((LoginMessage) message);
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
                        if (this.Client.Defines.AndroidClient)
                        {
                            message.UpdateUrl = ResourceManager.GetAppStoreUrl(1);
                        }
                        else
                        {
                            message.UpdateUrl = ResourceManager.GetAppStoreUrl(2);
                        }
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
        ///     Called when a <see cref="ClientHelloMessage"/> has been received.
        /// </summary>
        internal void ClientHelloMessage(ClientHelloMessage message)
        {
            if (this.CheckClientVersion(message.MajorVersion, message.BuildVersion, message.ContentHash))
            {
                this.SendLoginFailedMessage(10, "Internal server error");
            }
        }

        /// <summary>
        ///     Called when a <see cref="LoginMessage" /> has been received.
        /// </summary>
        internal void LoginMessage(LoginMessage message)
        {
            if (this.CheckClientVersion(message.ClientMajorVersion, message.ClientBuildVersion, message.ResourceSha))
            {
                if (message.AccountId.IsZero())
                {
                    if (message.PassToken != null)
                    {
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