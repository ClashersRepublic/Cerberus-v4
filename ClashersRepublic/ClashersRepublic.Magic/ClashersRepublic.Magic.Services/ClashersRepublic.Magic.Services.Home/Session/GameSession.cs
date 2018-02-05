namespace ClashersRepublic.Magic.Services.Home.Session
{
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Services.Home.Player;
    using ClashersRepublic.Magic.Services.Home.Service;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Titan.Message;

    internal class GameSession
    {
        internal string SessionId { get; }
        internal int ServerId { get; }

        internal GamePlayer Player { get; }
        internal LogicGameMode LogicGameMode { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameSession"/> class.
        /// </summary>
        internal GameSession(int serverId, string sessionId, GamePlayer player)
        {
            this.ServerId = serverId;
            this.SessionId = sessionId;
            this.Player = player;

            this.LogicGameMode = new LogicGameMode();
        }

        /// <summary>
        ///     Sends the <see cref="ServiceMessage"/> to proxy.
        /// </summary>
        internal void SendToProxy(ServiceMessage message)
        {
            ServiceMessageManager.SendMessage(message, ServiceExchangeName.SERVICE_PROXY_NAME, this.ServerId, this.SessionId);
        }

        /// <summary>
        ///     Sends the <see cref="PiranhaMessage"/> in a <see cref="ForwardServerMessage"/> to proxy.
        /// </summary>
        internal void SendToProxy(PiranhaMessage message)
        {
            message.Encode();

            this.SendToProxy(new ForwardServerMessage
            {
                Message = message
            });
        }
    }
}