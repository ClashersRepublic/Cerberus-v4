﻿namespace ClashersRepublic.Magic.Proxy.Message
{
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Message.Account;

    using ClashersRepublic.Magic.Proxy.Logic;
    using ClashersRepublic.Magic.Proxy.Service;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Logic.Resource;

    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;
    using ClashersRepublic.Magic.Titan.Util;

    internal class MessageManager
    {
        internal readonly Client Client;

        internal int DeviceType;
        internal int State;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager" /> class.
        /// </summary>
        internal MessageManager(Client client)
        {
            this.State = 1;
            this.Client = client;
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            if (this.State != -1)
            {
                if (this.State != 6)
                {
                    if (this.State == 1)
                    {
                        if (messageType != 10100 && messageType != 10101)
                        {
                            return;
                        }
                    }
                    else if (this.State == 2)
                    {
                        if (messageType != 10131)
                        {
                            return;
                        }
                    }
                }
            }
            else
            {
                return;
            }

            switch (messageType)
            {
                case 10101:
                {
                    this.LoginMessageReceived((LoginMessage) message);
                    break;
                }

                case 10131:
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends the specified message to client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            this.Client.Token.Messaging.SendMessage(message);
        }

        /// <summary>
        ///     Sends the specified message to client and change the state.
        /// </summary>
        internal void SendChangingStateMessage(int newState, PiranhaMessage message)
        {
            if (this.State != -1)
            {
                this.State = newState;
                this.SendMessage(message);
            }
        }

        /// <summary>
        ///     Sends a <see cref="LoginFailedMessage" /> to client.
        /// </summary>
        internal void SendLoginFailedMessage(int errorCode, string reason)
        {
            LoginFailedMessage message = new LoginFailedMessage
            {
                ErrorCode = errorCode,
                Reason = reason
            };

            switch (errorCode)
            {
                case 7:
                {
                    message.ContentUrlList = ResourceManager.ContentUrlList;
                    ZLibHelper.ConpressInZLibFormat(LogicStringUtil.GetBytes(ResourceManager.FingerprintJson), out message.ResourceFingerprintData);

                    break;
                }

                case 8:
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ClientHelloMessage"/> has been received.
        /// </summary>
        internal void ClientHelloMessageReceived(ClientHelloMessage message)
        {
            if (message.Protocol == 1)
            {
                if (message.MajorVersion == LogicVersion.MajorVersion)
                {
                    if (message.BuildVersion == LogicVersion.BuildVersion)
                    {
                        if (message.ContentHash == ResourceManager.FingerprintSha)
                        {
                            this.SendMessage(new ServerHelloMessage());
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="LoginMessage" /> has been received.
        /// </summary>
        internal void LoginMessageReceived(LoginMessage message)
        {
            this.DeviceType = message.AndroidClient ? 2 : 1;

            if (this.State == 1)
            {
                if (message.ClientMajorVersion == LogicVersion.MajorVersion && message.ClientBuildVersion == LogicVersion.BuildVersion)
                {
                    if (message.ResourceSha == ResourceManager.FingerprintSha)
                    {
                        if (message.AccountId.IsZero())
                        {
                            if (message.PassToken == null)
                            {

                            }
                            else
                            {
                                this.SendLoginFailedMessage(1, null);
                            }
                        }
                        else
                        {
                            if (message.PassToken != null)
                            {

                            }
                        }
                    }
                    else
                    {
                        this.SendLoginFailedMessage(7, null);
                    }
                }
                else
                {
                    this.SendLoginFailedMessage(8, null);
                }
            }
        }
    }
}