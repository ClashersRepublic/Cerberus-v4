namespace ClashersRepublic.Magic.Services.Home.Service
{
    using System;
    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Network;

    internal static class ServiceMessageManager
    {
        /// <summary>
        ///     Receives the specified <see cref="ServiceMessage"/> class.
        /// </summary>
        internal static void ReceiveMessage(ServiceMessage message)
        {
            int messageType = message.GetMessageType();

            if (messageType < 20000)
            {
                switch (messageType)
                {
                    case 10108:
                    {
                        ServiceMessageManager.SendResponseMessage(new KeepAliveServerMessage(), message);
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
        ///     Sends a request message to specified server.
        /// </summary>
        internal static void SendRequestMessage(ServiceMessage requestMessage, string exchangeName, string routingKey)
        {
            requestMessage.SetExchangeName(ServiceGateway.ExchangeName);
            requestMessage.SetRoutingKey(ServiceGateway.QueueName);

            ServiceMessaging.Send(requestMessage, exchangeName, routingKey);
        }

        /// <summary>
        ///     Sends the response message to requester.
        /// </summary>
        internal static void SendResponseMessage(ServiceMessage responseMessage, ServiceMessage requestMessage)
        {
            string exchangeName = requestMessage.GetExchangeName();
            string routingKey = requestMessage.GetRoutingKey();
            string proxySessionId = requestMessage.GetProxySessionId();

            if (exchangeName != null)
            {
                if (routingKey != null)
                {
                    responseMessage.SetExchangeName(ServiceGateway.ExchangeName);
                    responseMessage.SetRoutingKey(ServiceGateway.QueueName);
                    responseMessage.SetProxySessionId(proxySessionId);

                    ServiceMessaging.Send(responseMessage, exchangeName, routingKey);
                }
            }
        }

        /// <summary>
        ///     Sends the message to specified exchange and routing key.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string exchangeName, string routingKey = null)
        {
            if (exchangeName == null)
            {
                throw new ArgumentNullException("exchangeName");
            }

            ServiceMessaging.Send(message, exchangeName, routingKey);
        }
    }
}