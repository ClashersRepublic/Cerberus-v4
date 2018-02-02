namespace ClashersRepublic.Magic.Services.Home.Service
{
    using System;
    using System.Text;
    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Home.Helper;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class ServiceMessaging
    {
        internal static MagicServiceMessageFactory _factory;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceMessaging._factory = MagicServiceMessageFactory.Instance;
        }

        /// <summary>
        ///     Called when a message has been received by service gateway.
        /// </summary>
        internal static void OnReceive(byte[] packet, int length)
        {
            if (length >= 6)
            {
                length -= 6;

                int messageType = packet[1] | packet[0] << 8;
                int messageLength = packet[4] | packet[3] << 8 | packet[2] << 16;
                int messageSessionLength = (sbyte) packet[5];

                ServiceMessage message = ServiceMessaging._factory.CreateMessageByType(messageType);

                if (message != null)
                {
                    if (messageSessionLength <= -1)
                    {
                        if (messageSessionLength != -1)
                        {
                            Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #1");
                        }
                    }
                    else
                    {
                        if (length < messageSessionLength)
                        {
                            Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #2");
                        }
                        else
                        {
                            length -= messageSessionLength;

                            byte[] sessionId = new byte[messageSessionLength];
                            Array.Copy(packet, 6, sessionId, 0, messageSessionLength);
                            message.SetProxySessionId(Encoding.UTF8.GetString(sessionId));
                        }
                    }

                    if (length >= messageLength)
                    {
                        byte[] messageBytes = new byte[messageLength];
                        Array.Copy(packet, 6, messageBytes, 0, messageLength);

                        message.GetByteStream().SetByteArray(messageBytes, messageLength);

                        try
                        {
                            message.Decode();
                            ServiceProcessor.EnqueueReceiveAction(() => ServiceMessageManager.ReceiveMessage(message));
                        }
                        catch (Exception exception)
                        {
                            Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive Message decode exception, trace: " + exception);
                        }
                    }
                    else
                    {
                        Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #3");
                    }
                }
                else
                {
                    Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive ignoring message of unknown type " + messageType);
                }
            }
            else
            {
                Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #4");
            }
        }

        /// <summary>
        ///     Sends the specified message to specified proxy.
        /// </summary>
        internal static void Send(ServiceMessage message, string sessionId)
        {
            if (message.IsServerToClientMessage())
            {
                if (message.GetProxySessionId() == null)
                {
                    Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::send proxy session id is NULL");
                    return;
                }

                int serverId = Convert.ToByte(message.GetProxySessionId().Substring(22, 2), 16);
                Console.WriteLine("SERVER_ID : " + serverId);
                ServiceProcessor.EnqueueSendAction(() => ServiceMessaging.OnWakeup(message, ServiceExchangeName.PROXY_EXCHANGE, ServiceExchangeName.START_PROXY_QUEUE_NAME + serverId));
            }
        }

        /// <summary>
        ///     Sends the specified message to specified proxy.
        /// </summary>
        internal static void Send(ServiceMessage message, string exchange, string routingKey)
        {
            if (message.IsServerToClientMessage())
            {
                ServiceProcessor.EnqueueSendAction(() => ServiceMessaging.OnWakeup(message, exchange, routingKey));
            }
        }

        /// <summary>
        ///     Internal method for send the specified <see cref="ServiceMessage"/> to proxy.
        /// </summary>
        private static void OnWakeup(ServiceMessage message, string exchange, string routingKey)
        {
            message.Encode();

            int encodingLength = message.GetEncodingLength();
            int sessionLength = message.GetProxySessionIdLength();
            byte[] messageByteArray = message.GetByteStream().GetByteArray();
            byte[] messageSessionId = message.GetProxySessionId().ToByteArray();
            byte[] packet = new byte[message.GetEncodingLength() + encodingLength + sessionLength];
            Array.Copy(messageSessionId, 0, packet, 6, sessionLength);
            Array.Copy(messageByteArray, 0, packet, 6 + sessionLength, encodingLength);
            ServiceMessaging.WriteHeader(message, packet, encodingLength);
            
            ServiceGateway.Send(packet, exchange, routingKey);
        }

        /// <summary>
        ///     Writes the message header.
        /// </summary>
        private static void WriteHeader(ServiceMessage message, byte[] stream, int length)
        {
            int messageType = message.GetMessageType();
            int messageSessionIdLength = message.GetProxySessionIdLength();

            stream[1] = (byte) (messageType);
            stream[0] = (byte) (messageType >> 8);
            stream[4] = (byte) (length);
            stream[3] = (byte) (length >> 8);
            stream[2] = (byte) (length >> 16);
            stream[5] = (byte) (messageSessionIdLength & 127);

            if (length > 0xFFFFFF)
            {
                Logging.Error(typeof(ServiceMessaging), "NetworkMessaging::writeHeader trying to send too big message, type " + messageType);
            }
        }
    }
}