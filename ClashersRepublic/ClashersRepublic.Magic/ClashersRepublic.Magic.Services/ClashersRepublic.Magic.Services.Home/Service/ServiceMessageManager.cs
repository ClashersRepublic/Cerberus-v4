namespace ClashersRepublic.Magic.Services.Home.Service
{
    using System;

    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Home.Message;
    using ClashersRepublic.Magic.Services.Home.Player;
    using ClashersRepublic.Magic.Services.Home.Session;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Client;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;

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
                    case 10140:
                    {
                        ServiceMessageManager.ForwardClientMessageReceived((ForwardClientMessage) message);
                        break;
                    }

                    case 10198:
                    {
                        ServiceMessageManager.ClientConnectedMessageReceived((ClientConnectedMessage) message);
                        break;
                    }

                    case 10199:
                    {
                        ServiceMessageManager.ClientDisconnectedMessageReceived((ClientDisconnectedMessage) message);
                        break;
                    }

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
                    default:
                    {
                        Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::receiveMessage no case exist for message type " + messageType);
                        break;
                    }
                }
            }
        }

        /// <summary>
        ///     Sends the response message to requester.
        /// </summary>
        internal static void SendResponseMessage(ServiceMessage responseMessage, ServiceMessage requestMessage)
        {
            ServiceMessageManager.SendMessage(responseMessage, requestMessage.GetServiceType(), requestMessage.GetServerId(), requestMessage.GetSessionId());
        }

        /// <summary>
        ///     Sends the message to specified exchange and routing key.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string serviceType, int serverId, string sessionId = null)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException("serviceType");
            }

            message.SetSeviceType(ServiceGateway.ServiceType);
            message.SetServerId(Config.ServerId);
            message.SetSessionId(sessionId);

            ServiceMessaging.Send(message, ServiceExchangeName.BuildExchangeName(serviceType), ServiceExchangeName.BuildQueueName(serviceType, serverId));
        }

        /// <summary>
        ///     Called when a <see cref="ForwardClientMessage"/> has been received.
        /// </summary>
        internal static void ForwardClientMessageReceived(ForwardClientMessage message)
        {
            if (message.Message != null)
            {
                message.Message.GetByteStream().SetOffset(0);
                message.Message.Decode();
                
                if (GameSessionManager.GetSession(message.GetSessionId(), out GameSession session))
                {
                    MessageManager.ReceiveMessage(message.Message, session);
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ClientConnectedMessage"/> has been received.
        /// </summary>
        internal static void ClientConnectedMessageReceived(ClientConnectedMessage message)
        {
            GameSessionManager.OnClientConnected(message.GetServerId(), message.GetSessionId(), message.AccountId, message.IsNewClient);
        }

        /// <summary>
        ///     Called when a <see cref="ClientDisconnectedMessage"/> has been received.
        /// </summary>
        internal static void ClientDisconnectedMessageReceived(ClientDisconnectedMessage message)
        {
            GameSessionManager.OnClientDisconnected(message.GetSessionId());
        }
    }
}