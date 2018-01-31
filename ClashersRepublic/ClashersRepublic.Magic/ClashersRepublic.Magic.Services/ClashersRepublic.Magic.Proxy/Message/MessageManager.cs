namespace ClashersRepublic.Magic.Proxy.Message
{
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.User;

    using ClashersRepublic.Magic.Services.Logic.Resource;

    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    internal class MessageManager
    {
        internal Client Client;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(Client client)
        {
            this.Client = client;
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            if (this.Client.State != 6)
            {
                if (messageType != 10101 && messageType != 10121)
                {
                    return;
                }
            }

            switch (messageType)
            {
                case 10101:
                {
                    this.LoginMessage((LoginMessage) message);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends the specified <see cref="PiranhaMessage"/> to client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this.Client.NetworkToken.Messaging.Send(message);
        }

        /// <summary>
        ///     Sends a <see cref="LoginFailedMessage"/> to client.
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
                    message.UpdateUrl = ResourceManager.GetAppStoreUrl(this.Client.DeviceType == 0 ? this.Client.AndroidClient ? 2 : 0 : this.Client.DeviceType);
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
        ///     Called when a <see cref="LoginMessage"/> has been received.
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
                else
                {
                    this.SendLoginFailedMessage(7);
                }
            }
            else
            {
                this.SendLoginFailedMessage(8);
            }

            return false;
        }
    }
}