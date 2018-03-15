namespace ClashersRepublic.Magic.Dos.Network
{
    using System;

    using ClashersRepublic.Magic.Dos.Bot;
    using ClashersRepublic.Magic.Dos.Debug;

    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    internal class MessageManager
    {
        private Client _client;
        private DateTime _keepAliveSendTime;
        private LogicArrayList<int> _pings;

        private int _subTick;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(Client client)
        {
            this._client = client;
            this._pings = new LogicArrayList<int>();
        }

        /// <summary>
        ///     Gets the ping of client.
        /// </summary>
        internal int GetPing()
        {
            if (this._pings.Count > 0)
            {
                int cnt = this._pings.Count;
                int sum = 0;

                for (int i = 0; i < cnt; i++)
                {
                    sum += this._pings[i];
                }

                return sum / cnt;
            }

            return -1;
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
                    // Logging.Warning(this, "MessageManager::receiveMessage no case for message type " + messageType);
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
            this._pings.Add((int) DateTime.UtcNow.Subtract(this._keepAliveSendTime).TotalMilliseconds);
        }

        /// <summary>
        ///     Updates this instance.
        /// </summary>
        internal void Update(float time)
        {
            this._subTick += (int) (time * 62.5d);

            // if (this._timeSinceLastKeepAlive > 5f)
            {
                this.SendKeepAliveMessage();
            }

            this.SendClientTurn();
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
        ///     Sends the client turn.
        /// </summary>
        internal void SendClientTurn()
        {
            EndClientTurnMessage clientTurnMessage = new EndClientTurnMessage();
            clientTurnMessage.SetSubTick(this._subTick);
            clientTurnMessage.SetChecksum(-1);
            this._client.SendMessage(clientTurnMessage);
        }
    }
}