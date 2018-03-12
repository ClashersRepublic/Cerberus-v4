namespace ClashersRepublic.Magic.Services.Proxy.Network.Handler
{
    using System;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Titan.Message;

    internal static class NetworkMessagingManager
    {
        private static Thread _sendThread;
        private static Thread _receiveThread;
        private static NetworkMessaging[] _messagings;

        private static int _messagingOffset;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetworkMessagingManager._sendThread = new Thread(NetworkMessagingManager.SendTask);
            NetworkMessagingManager._receiveThread = new Thread(NetworkMessagingManager.ReceiveTask);
            NetworkMessagingManager._messagings = new NetworkMessaging[ushort.MaxValue];

            NetworkMessagingManager._sendThread.Start();
            NetworkMessagingManager._receiveThread.Start();
        }

        /// <summary>
        ///     Task for the receive thread.
        /// </summary>
        private static void ReceiveTask()
        {
            while (true)
            {
                for (int i = 0; i < NetworkMessagingManager._messagingOffset; i++)
                {
                    NetworkMessaging messaging = NetworkMessagingManager._messagings[i];

                    if (messaging != null)
                    {
                        if (!messaging.IsDestructed())
                        {
                            while (messaging.NextMessage(out PiranhaMessage message))
                            {
                                messaging.MessageManager.ReceiveMessage(message);
                            }
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Task for the send thread.
        /// </summary>
        private static void SendTask()
        {
            while (true)
            {
                for (int i = 0; i < NetworkMessagingManager._messagingOffset; i++)
                {
                    NetworkMessaging messaging = NetworkMessagingManager._messagings[i];

                    if (messaging != null)
                    {
                        if (!messaging.IsDestructed())
                        {
                            messaging.OnWakeup();
                        }
                    }
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Adds the specified <see cref="NetworkMessaging"/> instance.
        /// </summary>
        internal static bool TryAdd(NetworkMessaging messaging)
        {
            if (messaging.MessagingId > -1)
            {
                int available = Array.IndexOf(NetworkMessagingManager._messagings, null);

                if (available != -1)
                {
                    NetworkMessagingManager._messagings[messaging.MessagingId = available] = messaging;

                    if (available >= NetworkMessagingManager._messagingOffset)
                    {
                        NetworkMessagingManager._messagingOffset = available + 1;
                    }

                    return true;
                }

                Logging.Error("NetworkMessagingManager::tryAdd messagings is FULL");
            }

            return false;
        }

        /// <summary>
        ///     Tries to remove the specified <see cref="NetworkMessaging"/> instance.
        /// </summary>
        internal static bool TryRemove(NetworkMessaging messaging)
        {
            bool success = messaging.MessagingId > -1;

            if (success)
            {
                NetworkMessagingManager._messagings[messaging.MessagingId] = null;
                messaging.MessagingId = -1;
            }

            return success;
        }
    }
}