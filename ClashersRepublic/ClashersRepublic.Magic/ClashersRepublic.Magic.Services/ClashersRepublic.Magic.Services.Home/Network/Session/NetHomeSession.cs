﻿namespace ClashersRepublic.Magic.Services.Home.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Services.Home.Game;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Titan.Message;

    internal class NetHomeSession : NetSession
    {
        internal Home Home { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetHomeSession"/> class.
        /// </summary>
        internal NetHomeSession(Home home, byte[] sessionId, string sessionName) : base(sessionId, sessionName)
        {
            this.Home = home;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this.Home = null;
        }

        /// <summary>
        ///     Forwards the specified <see cref="PiranhaMessage"/> to the service.
        /// </summary>
        internal void SendPiranhaMessage(int serviceNodeType, PiranhaMessage message)
        {
            NetSocket socket = this._serviceNodeSockets[serviceNodeType];

            if (socket != null)
            {
                if (message.GetEncodingLength() == 0)
                {
                    message.Encode();
                }

                ForwardPiranhaMessage forwardPiranhaMessage = new ForwardPiranhaMessage();
                forwardPiranhaMessage.SetPiranhaMessage(message);
                NetMessageManager.SendMessage(socket, this.SessionId, this.SessionId.Length, forwardPiranhaMessage);
            }
        }
    }
}