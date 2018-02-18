namespace ClashersRepublic.Magic.Proxy.Service
{
    using ClashersRepublic.Magic.Proxy.Log;
    using ClashersRepublic.Magic.Proxy.Session;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;

    using ClashersRepublic.Magic.Titan.Debug;

    using NetMQ;

    internal static class ServiceMessageManager
    {
        /// <summary>
        ///     Receives the specified <see cref="ServiceMessage" /> class.
        /// </summary>
        internal static void ReceiveMessage(ServiceMessage message)
        {
            int messageType = message.GetMessageType();

            if (messageType < 20000)
            {
                switch (messageType)
                {
                    default:
                    {
                        Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::receiveMessage no case exist for message type " + messageType);
                        break;
                    }
                }
            }
            else
            {
                switch (messageType)
                {
                    case 10300:
                    {
                        ServiceMessageManager.ForwardServerMessageReceived((ForwardPiranhaMessage) message);
                        break;
                    }

                    default:
                    {
                        Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::receiveMessage no case exist for message type " + messageType);
                        break;
                    }
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
        ///     Called when a <see cref="ForwardPiranhaMessage"/> has been received.
        /// </summary>
        internal static void ForwardServerMessageReceived(ForwardPiranhaMessage message)
        {
            if (message.PiranhaMessage != null)
            {
                if (GameSessionManager.GetSession(message.GetSessionId(), out GameSession session))
                {
                    if (session.Client.State == 6)
                    {
                        session.Client.NetworkToken.Messaging.MessageManager.SendMessage(message.PiranhaMessage);
                    }
                }
            }
        }
    }
}