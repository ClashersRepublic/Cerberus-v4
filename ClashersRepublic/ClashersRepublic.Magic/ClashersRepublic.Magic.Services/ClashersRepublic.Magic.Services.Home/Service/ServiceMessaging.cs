namespace ClashersRepublic.Magic.Services.Home.Service
{
    using System;

    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Logic.Message;

    using NetMQ;

    internal static class ServiceMessaging
    {
        /// <summary>
        ///     Called when a message is received.
        /// </summary>
        internal static void OnReceive(byte[] buffer, int length)
        {
            if (length >= 6)
            {
                length -= 6;

                int messageType = buffer[1] | buffer[0] << 8;
                int messageLength = buffer[5] | buffer[4] << 8 | buffer[3] << 16 | (buffer[2] & 0x7f) << 24;

                if (length >= messageLength)
                {
                    byte[] encodingByteArray = new byte[messageLength];
                    Array.Copy(buffer, 6, encodingByteArray, 0, messageLength);

                    ServiceMessage message = ServiceMessageFactory.CreateMessageByType(messageType);

                    if (message != null)
                    {
                        message.GetByteStream().SetByteArray(encodingByteArray, messageLength);

                        try
                        {
                            message.Decode();
                            ServiceProcessor.ReceiveMessage(message);
                        }
                        catch (Exception e)
                        {
                            Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive message decode failed, trace: " + e);
                        }
                    }
                    else
                    {
                        Logging.Warning(typeof(ServiceMessaging), "Ignoring message of unknown type " + messageType);
                    }
                }
                else
                {
                    Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive message is corrupted #2");
                }
            }
            else
            {
                Logging.Error(typeof(ServiceMessaging), "ServiceMessaging::onReceive message is corrupted #1");
            }
        }

        /// <summary>
        ///     Sends the message to specified server.
        /// </summary>
        internal static void Send(ServiceMessage message, NetMQSocket responseSocket)
        {
            ServiceProcessor.SendMessage(message, responseSocket);
        }

        /// <summary>
        ///     Called for send to server the specified message.
        /// </summary>
        internal static void OnWakeup(ServiceMessage message, NetMQSocket responseSocket)
        {
            message.Encode();

            int encodingLength = message.GetEncodingLength();
            byte[] encodingByteArray = message.GetByteStream().GetByteArray();
            byte[] packet = new byte[encodingLength + 6];

            ServiceMessaging.WriteHeader(message, encodingLength, packet);
            Array.Copy(encodingByteArray, 0, packet, 6, encodingLength);
            ServiceGateway.Send(packet, encodingLength + 6, responseSocket);
        }

        /// <summary>
        ///     Writes the message header to buffer.
        /// </summary>
        private static void WriteHeader(ServiceMessage message, int encodingLenght, byte[] buffer)
        {
            int messageType = message.GetMessageType();

            buffer[0] = (byte) (messageType >> 8);
            buffer[1] = (byte) (messageType);
            buffer[2] = (byte) (encodingLenght >> 24);
            buffer[3] = (byte) (encodingLenght >> 16);
            buffer[4] = (byte) (encodingLenght >> 8);
            buffer[5] = (byte) (encodingLenght);
        }
    }
}