namespace ClashersRepublic.Magic.Proxy.Session
{
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Log;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.User;
    
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    using ClashersRepublic.Magic.Titan.Message;
    using NetMQ;

    internal class GameSession
    {
        private readonly NetMQSocket[] _serverEndPoints;

        internal string SessionId { get; }

        internal Client Client { get; }
        internal GameAccount Account { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameSession" /> class.
        /// </summary>
        internal GameSession(string sessionId, Client client, GameAccount account)
        {
            this.SessionId = sessionId;
            this.Client = client;
            this.Account = account;

            this._serverEndPoints = new NetMQSocket[28];
        }

        /// <summary>
        ///     Sets the server id for specified service type.
        /// </summary>
        internal void SetServerIDs(int serviceType, int serverId)
        {
            if (serviceType > -1 && serviceType < 28)
            {
                if (this._serverEndPoints[serviceType] != null)
                {
                    ServiceMessageManager.SendMessage(new ServiceNodeBoundedToSessionMessage
                    {
                        ServiceNodeType = serviceType,
                        ServiceNodeId = serverId
                    }, this.SessionId, this._serverEndPoints[serviceType]);
                }
                
                if (serverId != -1)
                {
                    NetMQSocket serverEndPoint = this._serverEndPoints[serviceType] = ServiceManager.GetServiceSocket(serviceType, serverId);

                    if (serverEndPoint != null)
                    {
                        ServiceMessageManager.SendMessage(new ServiceNodeUnboundedToSessionMessage
                        {
                            ServiceNodeType = serviceType,
                            ServiceNodeId = serverId
                        }, this.SessionId, serverEndPoint);
                    }
                    else
                    {
                        Logging.Warning(typeof(GameSession), "GameSession::setServerIDs server " + serviceType + "-" + serverId + " doesn't exist");
                    }
                }
                else
                {
                    this._serverEndPoints[serviceType] = null;
                }
            }
            else
            {
                Logging.Warning(this, "GameSession::setServerIDs serviceType out of bands " + serviceType + "/" + 28);
            }
        }

        /// <summary>
        ///     Clears all server ids.
        /// </summary>
        internal void ClearServerIDs()
        {
            for (int i = 0; i < 28; i++)
            {
                this.SetServerIDs(i, -1);
            }
        }

        /// <summary>
        ///     Forwards the specified message to the right server.
        /// </summary>
        internal void ForwardGameMessage(PiranhaMessage message)
        {
            int serviceType = message.GetServiceNodeType();

            if (serviceType != 0)
            {
                NetMQSocket serverEndPoint = this._serverEndPoints[serviceType];

                if (serverEndPoint != null)
                {
                    ServiceMessageManager.SendMessage(new ForwardPiranhaMessage
                    {
                        PiranhaMessage = message
                    }, this.SessionId, serverEndPoint);
                }
            }
        }
    }
}