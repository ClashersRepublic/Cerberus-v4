namespace ClashersRepublic.Magic.Services.Account.Service
{
    using System;

    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Factory;

    using RabbitMQ.Client.Events;

    public static class ServiceMessaging
    {
        private static bool _initialized;
        private static MagicServiceMessageFactory _messageFactory;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ServiceMessaging._initialized)
            {
                return;
            }

            ServiceMessaging._initialized = true;
            ServiceMessaging._messageFactory = MagicServiceMessageFactory.Instance;
        }
        
        /// <summary>
        ///     Called when a service message has been received.
        /// </summary>
        internal static void ReceiveMessage(byte[] content, BasicDeliverEventArgs args)
        {
            if (content.Length > 5)
            {
                int messageType = content[0] << 8 | content[1];
                int messageLenght = content[2] << 16 | content[3] << 8 | content[4];

                if (content.Length - 5 >= messageLenght)
                {
                    byte[] messageBytes = new byte[messageLenght];
                    Array.Copy(content, 5, messageBytes, 0, messageLenght);

                    MagicServiceMessage message = ServiceMessaging._messageFactory.CreateMessageByType(messageType);

                    if (message != null)
                    {
                        message.GetByteStream().SetByteArray(messageBytes, messageLenght);
                        message.SetMessageVersion(0);

                        ServiceProcessor.EnqueueReceivedMessage(message, args);
                    }
                    else
                    {
                        Logging.Error(typeof(ServiceMessaging), "Unknown message type received. type: " + messageType);
                    }
                }
                else
                {
                    Logging.Error(typeof(ServiceMessaging), "Message length is superior than available bytes length");
                }
            }
            else
            {
                Logging.Error(typeof(ServiceMessaging), "Message bytes is too small. length: " + content.Length);
            }
        }

        /// <summary>
        ///     Sends the specified message to client.
        /// </summary>
        internal static void SendMessage(MagicServiceMessage message, string exchangeKey, string routingKey)
        {
            message.Encode();
            ServiceProcessor.EnqueueSentMessage(message, exchangeKey, routingKey);
        }
    }
}