namespace ClashersRepublic.Magic.Services.Home.Message
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Home.Sessions;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Titan.Message;

    internal static class MessageProcessor
    {
        private static Thread _receiveMessageThread;
        private static ConcurrentQueue<QueueItem> _receiveMessageQueue;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            MessageProcessor._receiveMessageThread = new Thread(MessageProcessor.ReceiveTask);
            MessageProcessor._receiveMessageQueue = new ConcurrentQueue<QueueItem>();
            MessageProcessor._receiveMessageThread.Start();
        }
        
        /// <summary>
        ///     Task for the receive thread.
        /// </summary>
        private static void ReceiveTask()
        {
            while (true)
            {
                while (MessageProcessor._receiveMessageQueue.TryDequeue(out QueueItem item))
                {
                    try
                    {
                        Logging.Debug(typeof(MessageProcessor), "MessageProcessor::receiveTask message " + item.Message.GetType().Name + " received");

                        item.Message.GetByteStream().SetOffset(0);
                        item.Message.Decode();
                        item.Session.MessageManager.ReceiveMessage(item.Message);
                        item.Destruct();
                    }
                    catch (Exception exception)
                    {
                        Logging.Error(typeof(MessageProcessor), "MessageProcessor::receiveTask message handle failed, trace: " + exception);
                    }
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