namespace ClashersRepublic.Magic.Dos.Network
{
    using System;
    using ClashersRepublic.Magic.Dos.Bot;
    using ClashersRepublic.Magic.Dos.Debug;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        private Client _client;

        private DateTime _keepAliveSendTime;
        private DateTime _keepAliveServerReceivedTime;

        private bool _pingTest;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(Client client)
        {
            this._client = client;
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            int messageType = message.GetMessageType();

            switch (messageType)
            {
                case 20103:
                {
                    this.LoginFailedMessageReceived((LoginFailedMessage) message);
                    break;
                }

                case 20108:
                {
                    this.KeepAliveServerReceived((KeepAliveServerMessage) message);
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
        ///     Called when a <see cref="LoginFailedMessage"/> has been received.
        /// </summary>
        internal void LoginFailedMessageReceived(LoginFailedMessage message)
        {
            Logging.Debug(this, "MessageManager::loginFailedMessageReceived errorCode: " + message.ErrorCode);
        }

        /// <summary>
        ///     Called when a <see cref="KeepAliveServerMessage"/> has been received.
        /// </summary>
        internal void KeepAliveServerReceived(KeepAliveServerMessage message)
        {
            this._keepAliveServerReceivedTime = DateTime.UtcNow;

            if (this._pingTest)
            {
                Console.WriteLine("PING: " + this._keepAliveServerReceivedTime.Subtract(this._keepAliveSendTime).TotalMilliseconds);
            }

            this._pingTest = false;
        }

        /// <summary>
        ///     Sends a keep alive message to server.
        /// </summary>
        internal void SendKeepAliveMessage()
        {
            this._keepAliveSendTime = DateTime.UtcNow;
            this._client.SendMessage(new KeepAliveMessage());
        }

        /// <summary>
        ///     Creates a ping test.
        /// </summary>
        internal void Ping()
        {
            this._pingTest = true;
            this._keepAliveSendTime = DateTime.UtcNow;
            this._client.SendMessage(new KeepAliveMessage());
        }
    }
}