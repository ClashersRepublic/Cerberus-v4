namespace ClashersRepublic.Magic.Services.Account.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Services.Account;

    public class ServiceConnectionProcessor
    {
        private static bool _initialized;

        private static Thread _handleThread;
        private static Thread _sendThread;

        private static AutoResetEvent _handleMessageEvent;
        private static AutoResetEvent _sendMessageEvent;

        private static ConcurrentQueue<QueueEntry> _handleMessageQueue;
        private static ConcurrentQueue<QueueEntry> _sendMessageQueue;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ServiceConnectionProcessor._initialized)
            {
                return;
            }

            ServiceConnectionProcessor._initialized = true;
            ServiceConnectionProcessor._sendThread = new Thread(ServiceConnectionProcessor.SendMessages);
            ServiceConnectionProcessor._handleThread = new Thread(ServiceConnectionProcessor.HandleMessages);
            ServiceConnectionProcessor._handleMessageEvent = new AutoResetEvent(false);
            ServiceConnectionProcessor._sendMessageEvent = new AutoResetEvent(false);
            ServiceConnectionProcessor._handleMessageQueue = new ConcurrentQueue<QueueEntry>();
            ServiceConnectionProcessor._sendMessageQueue = new ConcurrentQueue<QueueEntry>();

            ServiceConnectionProcessor._sendThread.Start();
            ServiceConnectionProcessor._handleThread.Start();
        }

        /// <summary>
        ///     Handles the specified message.
        /// </summary>
        private static void InternalHandleMessage(PiranhaMessage message, string routingKey)
        {
            try
            {
                message.Decode();

                MessageManager.ReceiveMessage(message, routingKey);
                Logging.Info(typeof(ServiceConnectionProcessor), message.GetType().Name);
            }
            catch (Exception exception)
            {
                Logging.Error(typeof(ServiceConnectionProcessor), "An exception has been throwed when the process of message type " + message.GetMessageType() + ". trace: " + exception);
            }
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        private static void InternalSendMessage(PiranhaMessage message, string exchangeKey, string routingKey)
        {
            message.Encode();

            byte[] messageBytes = message.GetByteStream().GetBytes();
            int messageType = message.GetMessageType();
            int messageVersion = message.GetMessageVersion();
            int messageLength = messageBytes.Length;

            byte[] packet = new byte[4 + messageLength];

            packet[1] = (byte) messageType;
            packet[0] = (byte) (messageType >> 8);

            packet[3] = (byte) messageVersion;
            packet[2] = (byte) (messageVersion >> 8);

            Array.Copy(messageBytes, 0, packet, 4, messageLength);
            ServiceConnection.SendToService(exchangeKey, routingKey, packet);

            Logging.Info(typeof(ServiceConnectionProcessor), message.GetType().Name);
        }

        /// <summary>
        ///     Handles the collection of messages.
        /// </summary>
        internal static void HandleMessages()
        {
            while (true)
            {
                ServiceConnectionProcessor._handleMessageEvent.WaitOne();

                while (ServiceConnectionProcessor._handleMessageQueue.TryDequeue(out QueueEntry entry))
                {
                    ServiceConnectionProcessor.InternalHandleMessage(entry.Message, entry.RoutingKey);
                }
            }
        }

        /// <summary>
        ///     Sends the collection of messages.
        /// </summary>
        internal static void SendMessages()
        {
            while (true)
            {
                ServiceConnectionProcessor._sendMessageEvent.WaitOne();

                while (ServiceConnectionProcessor._sendMessageQueue.TryDequeue(out QueueEntry entry))
                {
                    ServiceConnectionProcessor.InternalSendMessage(entry.Message, entry.Exchange, entry.RoutingKey);
                }
            }
        }

        /// <summary>
        ///     Enqueues the item to receive message queue.
        /// </summary>
        internal static void EnqueueReceivedMessage(PiranhaMessage message, string routingKey)
        {
            ServiceConnectionProcessor._handleMessageQueue.Enqueue(new QueueEntry(message, string.Empty, routingKey));
            ServiceConnectionProcessor._handleMessageEvent.Set();
        }

        /// <summary>
        ///     Enqueues the item to receive message queue.
        /// </summary>
        internal static void EnqueueSentMessage(PiranhaMessage message, string exchange, string routingKey)
        {
            ServiceConnectionProcessor._sendMessageQueue.Enqueue(new QueueEntry(message, exchange, routingKey));
            ServiceConnectionProcessor._sendMessageEvent.Set();
        }

        private struct QueueEntry
        {
            internal readonly PiranhaMessage Message;

            internal readonly string Exchange;
            internal readonly string RoutingKey;

            /// <summary>
            ///     Initializes a new instance of the <see cref="ServiceConnectionProcessor.QueueEntry" /> class.
            /// </summary>
            internal QueueEntry(PiranhaMessage message, string exchange, string routingKey)
            {
                this.Message = message;
                this.Exchange = exchange;
                this.RoutingKey = routingKey;
            }
        }

        private static class MessageManager
        {
            /// <summary>
            ///     Receives the service message.
            /// </summary>
            internal static void ReceiveMessage(PiranhaMessage message, string routingKey)
            {
            }
        }
    }
}