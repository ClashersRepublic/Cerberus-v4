namespace ClashersRepublic.Magic.Proxy.Service
{
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Services.Logic.Message;

    using NetMQ;

    internal static class ServiceProcessor
    {
        private static Thread _sendMessageWorker;
        private static Thread _receiveMessageWorker;

        private static ConcurrentQueue<QueueItem> _sendMessageQueue;
        private static ConcurrentQueue<ServiceMessage> _receiveMessageQueue;
        
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceProcessor._sendMessageWorker = new Thread(ServiceProcessor.SendMessages);
            ServiceProcessor._receiveMessageWorker = new Thread(ServiceProcessor.ReceiveMessages);
            ServiceProcessor._sendMessageQueue = new ConcurrentQueue<QueueItem>();
            ServiceProcessor._receiveMessageQueue = new ConcurrentQueue<ServiceMessage>();

            ServiceProcessor._sendMessageWorker.Start();
            ServiceProcessor._receiveMessageWorker.Start();
        }

        /// <summary>
        ///     Tasks for the receive worker.
        /// </summary>
        private static void ReceiveMessages()
        {
            while (true)
            {
                while (ServiceProcessor._receiveMessageQueue.TryDequeue(out ServiceMessage item))
                {
                    ServiceMessageManager.ReceiveMessage(item);
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
                while (ServiceProcessor._sendMessageQueue.TryDequeue(out QueueItem item))
                {
                    ServiceMessaging.OnWakeup(item.Message, item.Socket);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Enqueues the specified message.
        /// </summary>
        internal static void ReceiveMessage(ServiceMessage message)
        {
            ServiceProcessor._receiveMessageQueue.Enqueue(message);
        }

        /// <summary>
        ///     Enqueues the specified message.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, NetMQSocket socket)
        {
            ServiceProcessor._sendMessageQueue.Enqueue(new QueueItem
            {
                Message = message,
                Socket = socket
            });
        }

        private struct QueueItem
        {
            internal ServiceMessage Message;
            internal NetMQSocket Socket;
        }
    }
}