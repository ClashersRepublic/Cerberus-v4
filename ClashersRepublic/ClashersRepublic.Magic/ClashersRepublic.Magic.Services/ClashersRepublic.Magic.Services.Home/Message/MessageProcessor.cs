namespace ClashersRepublic.Magic.Services.Home.Message
{
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Home.Sessions;
    using ClashersRepublic.Magic.Titan.Message;

    internal static class MessageProcessor
    {
        private static Thread _sendMessageThread;
        // private static Thread _receiveMessageThread;

        private static ConcurrentQueue<QueueItem> _sendMessageQueue;
        // private static ConcurrentQueue<QueueItem> _receiveMessageQueue;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            MessageProcessor._sendMessageThread = new Thread(MessageProcessor.SendTask);
            // MessageProcessor._receiveMessageThread = new Thread(MessageProcessor.ReceiveTask);

            MessageProcessor._sendMessageQueue = new ConcurrentQueue<QueueItem>();
            // MessageProcessor._receiveMessageQueue = new ConcurrentQueue<QueueItem>();

            MessageProcessor._sendMessageThread.Start();
            // MessageProcessor._receiveMessageThread.Start();
        }

        /// <summary>
        ///     Task for the send thread.
        /// </summary>
        private static void SendTask()
        {
            while (true)
            {
                while (MessageProcessor._sendMessageQueue.TryDequeue(out QueueItem item))
                {
                    item.Session.ForwardPiranhaMessage(item.Message);
                    item.Destruct();
                }

                Thread.Sleep(1);
            }
        }

        /*
        /// <summary>
        ///     Task for the receive thread.
        /// </summary>
        private static void ReceiveTask()
        {
            while (true)
            {
                while (MessageProcessor._receiveMessageQueue.TryDequeue(out QueueItem item))
                {
                    item.Session.MessageManager.ReceiveMessage(item.Message);
                    item.Destruct();
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Receives the specified message.
        /// </summary>
        internal static void ReceiveMessage(PiranhaMessage message, GameSession gameSession)
        {
            MessageProcessor._receiveMessageQueue.Enqueue(new QueueItem(message, gameSession));
        }
        */

        /// <summary>
        ///     Sends the specifed message.
        /// </summary>
        internal static void SendMessage(PiranhaMessage message, GameSession gameSession)
        {
            MessageProcessor._sendMessageQueue.Enqueue(new QueueItem(message, gameSession));
        }

        private struct QueueItem
        {
            internal PiranhaMessage Message;
            internal GameSession Session;

            /// <summary>
            ///     Initializes a new instance of the <see cref="QueueItem"/> struct.
            /// </summary>
            internal QueueItem(PiranhaMessage message, GameSession session)
            {
                this.Message = message;
                this.Session = session;
            }

            /// <summary>
            ///     Destructs this instance.
            /// </summary>
            internal void Destruct()
            {
                this.Message = null;
                this.Session = null;
            }
        }
    }
}