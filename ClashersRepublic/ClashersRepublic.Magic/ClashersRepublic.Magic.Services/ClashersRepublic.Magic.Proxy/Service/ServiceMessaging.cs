namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using System.Text;
    using ClashersRepublic.Magic.Proxy.Debug;
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
                int messageSessionLength = (sbyte) packet[5];
                
                MagicServiceMessage message = ServiceMessaging._factory.CreateMessageByType(messageType);

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
    }
}