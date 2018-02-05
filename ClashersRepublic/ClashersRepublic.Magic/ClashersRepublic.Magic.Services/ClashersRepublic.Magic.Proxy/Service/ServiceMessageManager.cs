namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Proxy.Session;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;
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
                    case 20140:
                    {
                        ServiceMessageManager.ForwardServerMessageReceived((ForwardServerMessage) message);
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
        ///     Called when a <see cref="ForwardServerMessage"/> has been received.
        /// </summary>
        internal static void ForwardServerMessageReceived(ForwardServerMessage message)
        {
            if (message.Message != null)
            {
                Console.WriteLine(BitConverter.ToString(message.Message.GetByteStream().GetByteArray()));

                if (GameSessionManager.GetSession(message.GetSessionId(), out GameSession session))
                {
                    if (session.Client.State == 6)
                    {
                        session.Client.NetworkToken.Messaging.MessageManager.SendMessage(message.Message);
                    }
                }
            }
        }
    }
}