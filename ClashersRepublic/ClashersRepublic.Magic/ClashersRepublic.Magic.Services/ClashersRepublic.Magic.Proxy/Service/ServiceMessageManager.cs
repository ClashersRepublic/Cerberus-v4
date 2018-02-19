namespace ClashersRepublic.Magic.Proxy.Service
{
    using ClashersRepublic.Magic.Proxy.Session;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    
    using NetMQ;

    internal static class ServiceMessageManager
    {
        /// <summary>
        ///     Receives the specified <see cref="ServiceMessage" /> class.
        /// </summary>
        internal static void ReceiveMessage(ServiceMessage message)
        {
            int messageType = message.GetMessageType();

            switch (messageType)
            {
                case 10300:
                    ServiceMessageManager.ForwardPiranhaMessageReceived((ForwardPiranhaMessage)message);
                    break;

                default:
                {
                    Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::receiveMessage no case exist for message type " + messageType);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends message to the specified socket.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, NetMQSocket responseSocket)
        {
            message.SetServerId(Config.ServerId);
            message.SetServiceType(ServiceManager.SERVICE_TYPE);
            
            ServiceMessaging.Send(message, responseSocket);
        }

        /// <summary>
        ///     Sends message to the specified socket.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string sessionId, NetMQSocket responseSocket)
        {
            message.SetServerId(Config.ServerId);
            message.SetServiceType(ServiceManager.SERVICE_TYPE);
            message.SetSessionId(sessionId);

            ServiceMessaging.Send(message, responseSocket);
        }

        /// <summary>
        ///     Sends message to the specified service node.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, int serviceType, int serverId)
        {
            NetMQSocket socket = ServiceManager.GetServiceSocket(serviceType, serverId);

            if (socket != null)
            {
                ServiceMessageManager.SendMessage(message, socket);
            }
        }

        /// <summary>
        ///     Sends message to the specified service node.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string sessionId, int serviceType, int serverId)
        {
            NetMQSocket socket = ServiceManager.GetServiceSocket(serviceType, serverId);

            if (socket != null)
            {
                ServiceMessageManager.SendMessage(message, sessionId, socket);
            }
        }

        /// <summary>
        ///     Called when a <see cref="ForwardPiranhaMessage"/> is received.
        /// </summary>
        internal static void ForwardPiranhaMessageReceived(ForwardPiranhaMessage message)
        {
            if (message.PiranhaMessage != null)
            {
                if (GameSessionManager.GetSession(message.GetSessionId(), out GameSession session))
                {
                    session.SetServiceNode(message.GetServiceType(), message.GetServerId());

                    if (session.Client.State == 6)
                    {
                        session.Client.NetworkToken.Messaging.MessageManager.SendMessage(message.PiranhaMessage);
                    }
                }
                else
                {
                    Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::forwardPiranhaMessageReceived session doesn't exist");
                }
            }
            else
            {
                Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::forwardPiranhaMessageReceived piranha message is NULL");
            }
        }
    }
}