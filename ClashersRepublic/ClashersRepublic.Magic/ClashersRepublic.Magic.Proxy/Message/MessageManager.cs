namespace ClashersRepublic.Magic.Proxy.Message
{
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Proxy.Logic;

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
        ///     Sends a <see cref="LoginFailedMessage"/> to client.
        /// </summary>
        internal void SendLoginFailedMessage(int errorCode, params object[] args)
        {
            LoginFailedMessage message = new LoginFailedMessage {ErrorCode = errorCode};

            if (args.Length > 0)
            {
                message.Reason = (string) args[0];
            }

            switch (errorCode)
            {
                case 7:
                {
                    if (args.Length > 1)
                    {
                        message.EndMaintenanceTime = (int) args[1];
                    }

                    break;
                }

                case 8:
                {
                    break;
                }
            }
        }
        
        /// <summary>
        ///     Called when a <see cref="LoginMessage"/> has been received.
        /// </summary>
        internal void LoginMessageReceived(LoginMessage message)
        {
            this.DeviceType = message.AndroidClient ? 2 : 1;

            if (this.State == 1)
            {
                if (message.ClientMajorVersion == LogicVersion.MajorVersion && message.ClientBuildVersion == LogicVersion.BuildVersion)
                {

                }
                else
                {
                    this.SendLoginFailedMessage(8, null, this.DeviceType);
                }
            }
        }
    }
}