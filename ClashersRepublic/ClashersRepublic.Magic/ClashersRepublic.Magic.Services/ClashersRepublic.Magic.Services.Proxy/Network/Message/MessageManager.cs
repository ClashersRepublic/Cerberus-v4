namespace ClashersRepublic.Magic.Services.Proxy.Network.Message
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;
    using ClashersRepublic.Magic.Titan.Util;

    internal class MessageManager
    {
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

            if (this._client.State != 6)
            {
                if (messageType != 10100 && messageType != 10101 && messageType != 10108)
                {
                    return;
                }
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
                if (message.GetMajorVersion() == 9 && message.GetBuildVersion() == 256)
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
            if (message.ClientMajorVersion == 9 && message.ClientBuildVersion == 256)
            {
                if (message.ResourceSha == ResourceManager.FingerprintSha)
                {
                    if (!message.AccountId.IsZero())
                    {
                        if (message.PassToken == null)
                        {

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
        }
    }
}