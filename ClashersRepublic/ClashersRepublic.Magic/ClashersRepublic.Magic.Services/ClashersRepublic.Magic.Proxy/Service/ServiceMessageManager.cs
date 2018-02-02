﻿namespace ClashersRepublic.Magic.Proxy.Service
{
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Services.Logic.Message;

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
    }
}