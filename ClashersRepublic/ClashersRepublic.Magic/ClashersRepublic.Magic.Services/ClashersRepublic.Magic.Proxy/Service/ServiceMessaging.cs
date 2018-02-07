namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;

    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Services.Logic.Message;

    using RabbitMQ.Client.Events;

    internal static class ServiceMessaging
    {
        private static MagicServiceMessageFactory _messageFactory;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceMessaging._messageFactory = MagicServiceMessageFactory.Instance;
        }

        /// <summary>
        ///     Called when the service gateway receive a message.
        /// </summary>
        internal static void OnReceive(BasicDeliverEventArgs rcvEvent)
        {
            byte[] buffer = rcvEvent.Body;
            int length = buffer.Length;

            if (length >= 5)
            {
                length -= 5;

                int messageType = buffer[1] | (buffer[0] << 8);
                int messageLength = buffer[4] | (buffer[3] << 8) | (buffer[2] << 16);

                if (length >= messageLength)
                {
                    byte[] messageByteArray = new byte[messageLength];
                    Array.Copy(buffer, 5, messageByteArray, 0, messageLength);

                    ServiceMessage message = ServiceMessaging._messageFactory.CreateMessageByType(messageType);

                    if (message != null)
                    {
                        message.SetMessageVersion(0);
                        message.GetByteStream().SetByteArray(messageByteArray, messageLength);

                        try
                        {
                            message.Decode();
                            ServiceProcessor.ReceiveMessage(message);
                        }
                        catch (Exception exception)
                        {
                            Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive message decode exception, trace: " + exception);
                        }

                        Logging.Debug(typeof(ServiceMessaging), "ServiceMessaging::onReceive message " + message.GetType().Name + " received");
                    }
                }
                else
                {
                    Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #2");
                }
            }
            else
            {
                Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #1");
            }
        }

        /// <summary>
        ///     Sends the message to specified queue.
        /// </summary>
        internal static void Send(ServiceMessage message, string exchangeName, string routingKey = null)
        {
            if (exchangeName == null)
            {
                throw new ArgumentNullException("exchangeName");
            }

            ServiceProcessor.SendMessage(message, exchangeName, routingKey);
        }

        /// <summary>
        ///     Called for send the specified message.
        /// </summary>
        internal static void OnWakeup(ServiceMessage message, string exchangeName, string routingKey)
        {
            message.Encode();

            int encodingLength = message.GetEncodingLength();
            byte[] encodingByteArray = message.GetByteStream().GetByteArray();

            byte[] buffer = new byte[5 + encodingLength];

            Array.Copy(encodingByteArray, 0, buffer, 5, encodingLength);
            ServiceMessaging.WriteHeader(message, buffer, encodingLength);
            ServiceGateway.Send(buffer, exchangeName, routingKey);

            message.Destruct();

            Logging.Debug(typeof(ServiceMessaging), "ServiceMessaging::onWakeup message " + message.GetType().Name + " sent");
        }

        /// <summary>
        ///     Writes the message header.
        /// </summary>
        private static void WriteHeader(ServiceMessage message, byte[] stream, int length)
        {
            int messageType = message.GetMessageType();

            stream[1] = (byte) messageType;
            stream[0] = (byte) (messageType >> 8);
            stream[4] = (byte) length;
            stream[3] = (byte) (length >> 8);
            stream[2] = (byte) (length >> 16);

            if (length > 0xFFFFFF)
            {
                Logging.Error(typeof(ServiceMessaging), "NetworkMessaging::writeHeader trying to send too big message, type " + messageType);
            }
        }
    }
}