namespace ClashersRepublic.Magic.Services.Home.Service
{
    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Network;
    using ClashersRepublic.Magic.Titan.Message;

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
                        ServiceMessageManager.SendMessage(new KeepAliveServerMessage(), message.GetProxySessionId());
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
        ///     Sends the specified message to specified proxy.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string sessionId)
        {
            ServiceMessaging.Send(message, sessionId);
        }
    }
}