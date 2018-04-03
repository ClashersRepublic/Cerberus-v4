namespace LineageSoft.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using LineageSoft.Magic.Services.Core.Libs.NetMQ.Sockets;

    /// <summary>
    /// </summary>
    public sealed class NetMQQueueEventArgs<T> : EventArgs
    {
        /// <summary>
        /// </summary>
        public NetMQQueueEventArgs(NetMQQueue<T> queue)
        {
            this.Queue = queue;
        }

        /// <summary>
        /// </summary>
        public NetMQQueue<T> Queue { get; }
    }

    /// <summary>
    ///     Multi producer single consumer queue which you can poll on with a Poller.
    /// </summary>
    /// <typeparam name="T">Type of the item in queue</typeparam>
    public sealed class NetMQQueue<T> : IDisposable, ISocketPollable, IEnumerable<T>
    {
        private readonly PairSocket m_writer;
        private readonly PairSocket m_reader;
        private readonly ConcurrentQueue<T> m_queue;
        private readonly EventDelegator<NetMQQueueEventArgs<T>> m_eventDelegator;
        private Msg m_dequeueMsg;

        /// <summary>
        ///     Create new NetMQQueue.
        /// </summary>
        /// <param name="capacity">The capacity of the queue, use zero for unlimited</param>
        public NetMQQueue(int capacity = 0)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            this.m_queue = new ConcurrentQueue<T>();
            PairSocket.CreateSocketPair(out this.m_writer, out this.m_reader);

            this.m_writer.Options.SendHighWatermark = this.m_reader.Options.ReceiveHighWatermark = capacity / 2;

            this.m_eventDelegator = new EventDelegator<NetMQQueueEventArgs<T>>(() => { this.m_reader.ReceiveReady += this.OnReceiveReady; }, () => { this.m_reader.ReceiveReady -= this.OnReceiveReady; });

            this.m_dequeueMsg = new Msg();
            this.m_dequeueMsg.InitEmpty();
        }

        private void OnReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            this.m_eventDelegator.Fire(this, new NetMQQueueEventArgs<T>(this));
        }

        /// <summary>
        ///     Register for this event for notification when there are items in the queue. Queue must be added to a poller for
        ///     this to work.
        /// </summary>
        public event EventHandler<NetMQQueueEventArgs<T>> ReceiveReady
        {
            add
            {
                this.m_eventDelegator.Event += value;
            }
            remove
            {
                this.m_eventDelegator.Event -= value;
            }
        }

        NetMQSocket ISocketPollable.Socket
        {
            get
            {
                return this.m_reader;
            }
        }

        /// <summary>
        ///     Try to dequeue an item from the queue. Dequeueing and item is not thread safe.
        /// </summary>
        /// <param name="result">Will be filled with the item upon success</param>
        /// <param name="timeout">Timeout to try and dequeue and item</param>
        /// <returns>Will return false if it didn't succeed to dequeue an item after the timeout.</returns>
        public bool TryDequeue(out T result, TimeSpan timeout)
        {
            if (this.m_reader.TryReceive(ref this.m_dequeueMsg, timeout))
            {
                return this.m_queue.TryDequeue(out result);
            }

            result = default(T);
            return false;
        }

        /// <summary>
        ///     Dequeue an item from the queue, will block if queue is empty. Dequeueing and item is not thread safe.
        /// </summary>
        /// <returns>Dequeued item</returns>
        public T Dequeue()
        {
            this.m_reader.TryReceive(ref this.m_dequeueMsg, SendReceiveConstants.InfiniteTimeout);

            T result;
            this.m_queue.TryDequeue(out result);

            return result;
        }

        /// <summary>
        ///     Enqueue an item to the queue, will block if the queue is full.
        /// </summary>
        /// <param name="value"></param>
        public void Enqueue(T value)
        {
            this.m_queue.Enqueue(value);

            Msg msg = new Msg();
            msg.InitGC(EmptyArray<byte>.Instance, 0);

            lock (this.m_writer)
            {
                this.m_writer.TrySend(ref msg, SendReceiveConstants.InfiniteTimeout, false);
            }

            msg.Close();
        }

        #region IENumerator methods

        /// <summary>
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return this.m_queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            yield return this.GetEnumerator();
        }

        #endregion

        /// <summary>
        ///     Dispose the queue.
        /// </summary>
        public void Dispose()
        {
            this.m_eventDelegator.Dispose();
            this.m_writer.Dispose();
            this.m_reader.Dispose();
            this.m_dequeueMsg.Close();
        }
    }
}