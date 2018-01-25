namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Logic.Message;

    internal static class NetworkProcessor
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
            if (NetworkProcessor._initialized)
            {
                return;
            }

            NetworkProcessor._initialized = true;
            NetworkProcessor._sendThread = new Thread(NetworkProcessor.SendMessages);
            NetworkProcessor._handleThread = new Thread(NetworkProcessor.HandleMessages);
            NetworkProcessor._handleMessageEvent = new AutoResetEvent(false);
            NetworkProcessor._sendMessageEvent = new AutoResetEvent(false);
            NetworkProcessor._handleMessageQueue = new ConcurrentQueue<QueueEntry>();
            NetworkProcessor._sendMessageQueue = new ConcurrentQueue<QueueEntry>();

            NetworkProcessor._sendThread.Start();
            NetworkProcessor._handleThread.Start();
        }

        /// <summary>
        ///     Handles the specified message.
        /// </summary>
        private static void InternalHandleMessage(PiranhaMessage message, NetworkMessaging messaging)
        {
            try
            {
                if (messaging.Token.IsConnected())
                {
                    message.Decode();
                    messaging.Token.Client.MessageManager.ReceiveMessage(message);

                    Logging.Info(typeof(NetworkMessaging), "Message " + message.GetType().Name + " Received");
                }
            }
            catch (Exception exception)
            {
                Logging.Error(typeof(NetworkMessaging), "An exception has been throwed when the process of message type " + message.GetMessageType() + ". trace: " + exception);
            }
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        private static void InternalSendMessage(PiranhaMessage message, NetworkMessaging messaging)
        {
            try
            {
                if (messaging.Token.IsConnected())
                {
                    byte[] messageBytes = message.GetByteStream().GetBytes();
                    int messageType = message.GetMessageType();
                    int messageVersion = message.GetMessageVersion();
                    int messageLength = messageBytes.Length;

                    byte[] packet = new byte[7 + messageLength];

                    packet[1] = (byte) messageType;
                    packet[0] = (byte) (messageType >> 8);

                    packet[4] = (byte) messageLength;
                    packet[3] = (byte) (messageLength >> 8);
                    packet[2] = (byte) (messageLength >> 16);

                    packet[6] = (byte) messageVersion;
                    packet[5] = (byte) (messageVersion >> 8);

                    Array.Copy(messageBytes, 0, packet, 7, messageLength);
                    NetworkGateway.Send(packet, messaging.Token);

                    Logging.Info(typeof(NetworkMessaging), "Message " + message.GetType().Name + " Sent");
                }
            }
            catch (Exception exception)
            {
                Logging.Error(typeof(NetworkMessaging), "An exception has been throwed when the sending of message type " + message.GetMessageType() + ". trace: " + exception);
            }
        }

        /// <summary>
        ///     Handles the collection of messages.
        /// </summary>
        internal static void HandleMessages()
        {
            while (true)
            {
                NetworkProcessor._handleMessageEvent.WaitOne();

                while (NetworkProcessor._handleMessageQueue.TryDequeue(out QueueEntry entry))
                {
                    NetworkProcessor.InternalHandleMessage(entry.Message, entry.Messaging);
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
                NetworkProcessor._sendMessageEvent.WaitOne();

                while (NetworkProcessor._sendMessageQueue.TryDequeue(out QueueEntry entry))
                {
                    NetworkProcessor.InternalSendMessage(entry.Message, entry.Messaging);
                }
            }
        }

        /// <summary>
        ///     Enqueues the item to receive message queue.
        /// </summary>
        internal static void EnqueueReceivedMessage(PiranhaMessage message, NetworkMessaging messaging)
        {
            NetworkProcessor._handleMessageQueue.Enqueue(new QueueEntry(message, messaging));
            NetworkProcessor._handleMessageEvent.Set();
        }

        /// <summary>
        ///     Enqueues the item to receive message queue.
        /// </summary>
        internal static void EnqueueSentMessage(PiranhaMessage message, NetworkMessaging messaging)
        {
            NetworkProcessor._sendMessageQueue.Enqueue(new QueueEntry(message, messaging));
            NetworkProcessor._sendMessageEvent.Set();
        }

        private struct QueueEntry
        {
            internal readonly PiranhaMessage Message;
            internal readonly NetworkMessaging Messaging;

            /// <summary>
            ///     Initializes a new instance of the <see cref="QueueEntry" /> class.
            /// </summary>
            internal QueueEntry(PiranhaMessage message, NetworkMessaging messaging)
            {
                this.Message = message;
                this.Messaging = messaging;
            }
        }
    }
}