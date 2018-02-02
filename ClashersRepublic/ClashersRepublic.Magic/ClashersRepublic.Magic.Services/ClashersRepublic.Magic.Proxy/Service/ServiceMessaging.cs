﻿namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using System.Text;
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Proxy.Helper;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;

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

                ServiceMessage message = ServiceMessaging._factory.CreateMessageByType(messageType);

                if (message != null)
                {
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
                            Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive Message decodage exception, trace: " + exception);
                        }
                    }
                    else
                    {
                        Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive packet is corrupted #3");
                    }
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