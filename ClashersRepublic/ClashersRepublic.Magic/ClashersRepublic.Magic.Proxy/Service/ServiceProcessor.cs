namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;
    using RabbitMQ.Client.Events;

    internal static class ServiceProcessor
    {
        private static bool _initialized;

        private static Thread _sendThread;
        private static Thread _receiveThread;

        private static AutoResetEvent _sendEvent;
        private static AutoResetEvent _receiveEvent;

        private static ConcurrentQueue<QueueItem> _sendQueue;
        private static ConcurrentQueue<QueueItem> _receiveQueue;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ServiceProcessor._initialized)
            {
                return;
            }

            ServiceProcessor._initialized = true;
            ServiceProcessor._sendThread = new Thread(ServiceProcessor.SendMessageTask);
            ServiceProcessor._receiveThread = new Thread(ServiceProcessor.ReceiveMessageTask);
            ServiceProcessor._sendEvent = new AutoResetEvent(false);
            ServiceProcessor._receiveEvent = new AutoResetEvent(false);
            ServiceProcessor._sendQueue = new ConcurrentQueue<QueueItem>();
            ServiceProcessor._receiveQueue = new ConcurrentQueue<QueueItem>();

            ServiceProcessor._sendThread.Start();
            ServiceProcessor._receiveThread.Start();
        }

        /// <summary>
        ///     Tasks for the send message thread.
        /// </summary>
        internal static void SendMessageTask()
        {
            while (true)
            {
                ServiceProcessor._sendEvent.WaitOne();

                while (ServiceProcessor._sendQueue.TryDequeue(out QueueItem item))
                {
                    try
                    {
                        ServiceProcessor.InternalSendMessage(item.Message, item.ExchangeKey, item.RoutingKey);
                    }
                    catch (Exception exception)
                    {
                        Logging.Error(typeof(ServiceConnection), "An exception has been thrown while the sending of service message. trace: " + exception);
                    }
                }
            }
        }

        /// <summary>
        ///     Tasks for the send message thread.
        /// </summary>
        internal static void ReceiveMessageTask()
        {
            while (true)
            {
                while (true)
                {
                    ServiceProcessor._receiveEvent.WaitOne();

                    while (ServiceProcessor._receiveQueue.TryDequeue(out QueueItem item))
                    {
                        try
                        {
                            ServiceProcessor.InternalReceiveMessage(item.Message, item.Args);
                        }
                        catch (Exception exception)
                        {
                            Logging.Error(typeof(ServiceConnection), "An exception has been thrown while the handling of service message. trace: " + exception);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Handles the received message.
        /// </summary>
        internal static void InternalReceiveMessage(MagicServiceMessage message, BasicDeliverEventArgs args)
        {
            Logging.Info(typeof(ServiceProcessor), "Message " + message.GetType().Name + " Received");

            message.Decode();
            MessageManager.ReceiveMessage(message, args);
        }

        /// <summary>
        ///     Sends the specified message.
        /// </summary>
        internal static void InternalSendMessage(MagicServiceMessage message, string exchangeKey, string routingKey)
        {
            byte[] messageBytes = message.GetByteStream().GetBytes();
            byte[] packet = new byte[5 + messageBytes.Length];
            int messageType = message.GetMessageType();
            int messageLength = message.GetEncodingLength();

            if (messageLength > 0xFFFFFF)
            {
                Logging.Error(typeof(ServiceProcessor), "Trying to send too big message. type: " + messageType + " length: " + messageLength);
                return;
            }

            Array.Copy(messageBytes, 0, packet, 5, messageBytes.Length);

            packet[0] = (byte) (messageType >> 8);
            packet[1] = (byte) messageType;
            packet[2] = (byte) (messageLength >> 16);
            packet[3] = (byte) (messageLength >> 8);
            packet[4] = (byte) messageLength;

            ServiceConnection.Send(packet, exchangeKey, routingKey);
            Logging.Info(typeof(ServiceProcessor), "Message " + message.GetType().Name + " Sent");
        }

        /// <summary>
        ///     Enqueues the specified message to send it to client.
        /// </summary>
        internal static void EnqueueSentMessage(MagicServiceMessage message, string exchangeKey, string routingKey)
        {
            ServiceProcessor._sendQueue.Enqueue(new QueueItem(message, exchangeKey, routingKey));
            ServiceProcessor._sendEvent.Set();
        }

        /// <summary>
        ///     Enqueues the specified received message.
        /// </summary>
        internal static void EnqueueReceivedMessage(MagicServiceMessage message, BasicDeliverEventArgs args)
        {
            ServiceProcessor._receiveQueue.Enqueue(new QueueItem(message, args));
            ServiceProcessor._receiveEvent.Set();
        }

        private class QueueItem
        {
            internal readonly BasicDeliverEventArgs Args;

            internal readonly string ExchangeKey;
            internal readonly MagicServiceMessage Message;
            internal readonly string RoutingKey;

            /// <summary>
            ///     Initializes a new instance of the <see cref="QueueItem" /> class.
            /// </summary>
            internal QueueItem(MagicServiceMessage message, BasicDeliverEventArgs args)
            {
                this.Message = message;
                this.Args = args;
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="QueueItem" /> class.
            /// </summary>
            internal QueueItem(MagicServiceMessage message, string exchangeKey, string routingKey)
            {
                this.Message = message;
                this.ExchangeKey = exchangeKey;
                this.RoutingKey = routingKey;
            }
        }

        private static class MessageManager
        {
            /// <summary>
            ///     Receives a service message.
            /// </summary>
            internal static void ReceiveMessage(MagicServiceMessage message, BasicDeliverEventArgs args)
            {
                switch (message.GetMessageType())
                {
                    case 20101:
                    {
                        break;
                    }
                }
            }

            /// <summary>
            ///     Sends the specified message to service.
            /// </summary>
            private static void SendMessage(MagicServiceMessage message, string exchangeKey, string routingKey)
            {
                ServiceMessaging.SendMessage(message, exchangeKey, routingKey);
            }

            /// <summary>
            ///     Called when a <see cref="CreateAccountOkMessage" /> has been received.
            /// </summary>
            private static void CreateAccountOkMessageReceived(MagicServiceMessage message, BasicDeliverEventArgs args)
            {
            }
        }
    }
}