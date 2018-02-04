namespace ClashersRepublic.Magic.Proxy.Network
{
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;

    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Proxy.Message;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class NetworkManager
    {
        private static long _connectionId;

        private static ConcurrentQueue<QueueItem> _sendMessageQueue;
        private static ConcurrentQueue<QueueItem> _receiveMessageQueue;
        private static ConcurrentDictionary<long, NetworkToken> _connections;

        private static Thread _sendWorker;
        private static Thread _receiveWorker;
        private static Thread _updateWorker;

        /// <summary>
        ///     Gets the number of connections.
        /// </summary>
        internal static int TotalConnections
        {
            get
            {
                return NetworkManager._connections.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetworkManager._connectionId = 1;
            NetworkManager._sendMessageQueue = new ConcurrentQueue<QueueItem>();
            NetworkManager._receiveMessageQueue = new ConcurrentQueue<QueueItem>();
            NetworkManager._connections = new ConcurrentDictionary<long, NetworkToken>();

            NetworkManager._sendWorker = new Thread(NetworkManager.SendMessages);
            NetworkManager._receiveWorker = new Thread(NetworkManager.ReceiveMessages);
            NetworkManager._updateWorker = new Thread(NetworkManager.Update);

            NetworkManager._sendWorker.Start();
            NetworkManager._receiveWorker.Start();
            NetworkManager._updateWorker.Start();
        }

        /// <summary>
        ///     Adds the connection.
        /// </summary>
        internal static bool AddConnection(NetworkToken token)
        {
            if (token.ConnectionId == 0)
            {
                token.ConnectionId = Interlocked.Increment(ref NetworkManager._connectionId);

                if (NetworkManager._connections.TryAdd(token.ConnectionId, token))
                {
                    return true;
                }

                Logging.Error(typeof(NetworkToken), "NetworkManager::addConnection connection with same id already added, id: " + token.ConnectionId);
            }
            else
            {
                Logging.Warning(typeof(NetworkToken), "NetworkManager::addConnection connection id already setted");
            }

            return false;
        }

        /// <summary>
        ///     Removes the specified connection.
        /// </summary>
        internal static bool RemoveConnection(NetworkToken token)
        {
            if (token.ConnectionId != 0)
            {
                if (NetworkManager._connections.TryRemove(token.ConnectionId, out _))
                {
                    return true;
                }

                Logging.Warning(typeof(NetworkToken), "NetworkManager::removeConnection connection not exist");
            }
            else
            {
                Logging.Warning(typeof(NetworkToken), "NetworkManager::removeConnection connection id not setted");
            }

            return false;
        }

        /// <summary>
        ///     Adds the message to receive message queue.
        /// </summary>
        internal static void ReceiveMessage(PiranhaMessage message, NetworkMessaging messaging)
        {
            NetworkManager._receiveMessageQueue.Enqueue(new QueueItem
            {
                Message = message,
                Messaging = messaging
            });
        }

        /// <summary>
        ///     Adds the message to send message queue.
        /// </summary>
        internal static void SendMessage(PiranhaMessage message, NetworkMessaging messaging)
        {
            NetworkManager._sendMessageQueue.Enqueue(new QueueItem
            {
                Message = message,
                Messaging = messaging
            });
        }

        /// <summary>
        ///     Tasks for the receive worker.
        /// </summary>
        private static void ReceiveMessages()
        {
            while (true)
            {
                while (NetworkManager._receiveMessageQueue.TryDequeue(out QueueItem item))
                {
                    item.Messaging.MessageManager.ReceiveMessage(item.Message);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Tasks for the receive worker.
        /// </summary>
        private static void SendMessages()
        {
            while (true)
            {
                while (NetworkManager._sendMessageQueue.TryDequeue(out QueueItem item))
                {
                    item.Messaging.OnWakeup(item.Message);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Tasks for the update worker.
        /// </summary>
        private static void Update()
        {
            NetworkToken[] tokens = NetworkManager._connections.Values.ToArray();

            for (int i = 0, timestamp = LogicTimeUtil.GetTimestamp(); i < tokens.Length; i++)
            {
                NetworkToken token = tokens[i];
                MessageManager messageManager = token.Messaging.MessageManager;

                if (timestamp - messageManager.LastKeepAliveTime >= 30)
                {
                    NetworkGateway.Disconnect(token.AsyncEvent);
                }
            }

            Thread.Sleep(50);
        }

        private class QueueItem
        {
            internal PiranhaMessage Message;
            internal NetworkMessaging Messaging;
        }
    }
}