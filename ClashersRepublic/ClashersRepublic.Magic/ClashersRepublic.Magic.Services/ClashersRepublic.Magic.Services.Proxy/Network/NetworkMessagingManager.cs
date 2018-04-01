namespace ClashersRepublic.Magic.Services.Proxy.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using System.Threading.Tasks;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Proxy.Network.ServerSocket;
    using ClashersRepublic.Magic.Titan.Message;

    internal static class NetworkMessagingManager
    {
        private static long _messagingCounter;

        private static ConcurrentDictionary<long, NetworkMessaging> _messagings;
        private static Thread _messagingThread;
        private static Thread _receiveThread;
        private static Thread _sendThread;
        private static ParallelOptions _sendOption;
        private static ParallelOptions _receiveOption;
        
        /// <summary>
        ///     Gets the total messagings.
        /// </summary>
        internal static int TotalMessagings
        {
            get
            {
                return NetworkMessagingManager._messagings.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetworkMessagingManager._sendOption = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2
            };

            NetworkMessagingManager._receiveOption = new ParallelOptions
            {
                MaxDegreeOfParallelism = 2
            };

            NetworkMessagingManager._messagings = new ConcurrentDictionary<long, NetworkMessaging>();
            NetworkMessagingManager._sendThread = new Thread(NetworkMessagingManager.SendLoop);
            NetworkMessagingManager._receiveThread = new Thread(NetworkMessagingManager.ReceiveLoop);
            NetworkMessagingManager._messagingThread = new Thread(NetworkMessagingManager.MessagingLoop);

            NetworkMessagingManager._sendThread.Start();
            NetworkMessagingManager._receiveThread.Start();
            NetworkMessagingManager._messagingThread.Start();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal static void Destruct()
        {
            foreach (NetworkMessaging messaging in NetworkMessagingManager._messagings.Values)
            {
                if (!messaging.IsDestructed())
                {
                    NetworkTcpServerGateway.Disconnect(messaging.Connection);
                }
            }
        }

        /// <summary>
        ///     Task for the messaging thread.
        /// </summary>
        private static void MessagingLoop()
        {
            while (true)
            {
                foreach (NetworkMessaging messaging in NetworkMessagingManager._messagings.Values)
                {
                    if (!messaging.MessageManager.IsAlive())
                    {
                        NetworkMessagingManager.DisconnectMessaging(messaging);
                    }
                }

                Thread.Sleep(500);
            }
        }

        /// <summary>
        ///     Task for the receive thread.
        /// </summary>
        private static void ReceiveLoop()
        {
            while (true)
            {
                Parallel.ForEach(NetworkMessagingManager._messagings.Values, NetworkMessagingManager._receiveOption, messaging =>
                {
                    while (messaging.NextMessage(out PiranhaMessage message))
                    {
                        try
                        {
                            messaging.MessageManager.ReceiveMessage(message);
                        }
                        catch (Exception exception)
                        {
                            Logging.Error("NetworkMessagingManager::receiveLoop exception while the process of message " + message.GetMessageType() + ", trace: " + exception);
                        }
                    }
                });

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Task for the send thread.
        /// </summary>
        private static void SendLoop()
        {
            while (true)
            {
                Parallel.ForEach(NetworkMessagingManager._messagings.Values, NetworkMessagingManager._sendOption, messaging =>
                {
                    messaging.OnWakeup();
                });

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Tries to add the specified <see cref="NetworkMessaging"/> instance.
        /// </summary>
        internal static bool TryAdd(NetworkMessaging messaging)
        {
            return NetworkMessagingManager._messagings.TryAdd(messaging.MessagingId = ++NetworkMessagingManager._messagingCounter, messaging);
        }

        /// <summary>
        ///     Tries to remove the specified <see cref="NetworkMessaging"/> instance.
        /// </summary>
        internal static bool TryRemove(NetworkMessaging messaging)
        {
            long id = messaging.MessagingId;
            messaging.MessagingId = -1;
            return NetworkMessagingManager._messagings.TryRemove(id, out _);
        }

        /// <summary>
        ///     Disconnects the specified messaging.
        /// </summary>
        internal static void DisconnectMessaging(NetworkMessaging messaging)
        {
            if (!messaging.IsDestructed())
            {
                if (messaging.Client.State == -1 ||
                    messaging.Client.State == 1)
                {
                    NetworkTcpServerGateway.Disconnect(messaging.Connection);
                }
                else
                {
                    messaging.Send(new DisconnectedMessage());
                    messaging.Client.State = -1;
                }
            }
        }
    }
}