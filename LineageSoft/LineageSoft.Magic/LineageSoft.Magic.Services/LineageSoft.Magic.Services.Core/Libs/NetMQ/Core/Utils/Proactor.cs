namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using AsyncIO;
    using JetBrains.Annotations;

    internal class Proactor : PollerBase
    {
        private const int CompletionStatusArraySize = 100;

        private readonly string m_name;
        private readonly CompletionPort m_completionPort;
        private Thread m_worker;
        private bool m_stopping;
        private bool m_stopped;

        private readonly Dictionary<AsyncSocket, Item> m_sockets;

        private class Item
        {
            public Item([NotNull] IProactorEvents proactorEvents)
            {
                this.ProactorEvents = proactorEvents;
                this.Cancelled = false;
            }

            [NotNull]
            public IProactorEvents ProactorEvents { get; }

            public bool Cancelled { get; set; }
        }

        public Proactor([NotNull] string name)
        {
            this.m_name = name;
            this.m_stopping = false;
            this.m_stopped = false;
            this.m_completionPort = CompletionPort.Create();
            this.m_sockets = new Dictionary<AsyncSocket, Item>();
        }

        public void Start()
        {
            this.m_worker = new Thread(this.Loop) {IsBackground = true, Name = this.m_name};
            this.m_worker.Start();
        }

        public void Stop()
        {
            this.m_stopping = true;
        }

        public void Destroy()
        {
            if (!this.m_stopped)
            {
                try
                {
                    this.m_worker.Join();
                }
                catch (Exception)
                {
                }

                this.m_stopped = true;

                this.m_completionPort.Dispose();
            }
        }

        public void SignalMailbox(IOThreadMailbox mailbox)
        {
            this.m_completionPort.Signal(mailbox);
        }

        public void AddSocket(AsyncSocket socket, IProactorEvents proactorEvents)
        {
            Item item = new Item(proactorEvents);
            this.m_sockets.Add(socket, item);

            this.m_completionPort.AssociateSocket(socket, item);
            this.AdjustLoad(1);
        }

        public void RemoveSocket(AsyncSocket socket)
        {
            this.AdjustLoad(-1);

            Item item = this.m_sockets[socket];
            this.m_sockets.Remove(socket);
            item.Cancelled = true;
        }

        /// <exception cref="ArgumentOutOfRangeException">The completionStatuses item must have a valid OperationType.</exception>
        private void Loop()
        {
            CompletionStatus[] completionStatuses = new CompletionStatus[Proactor.CompletionStatusArraySize];

            while (!this.m_stopping)
            {
                // Execute any due timers.
                int timeout = this.ExecuteTimers();

                int removed;

                if (!this.m_completionPort.GetMultipleQueuedCompletionStatus(timeout != 0 ? timeout : -1, completionStatuses, out removed))
                {
                    continue;
                }

                for (int i = 0; i < removed; i++)
                {
                    try
                    {
                        if (completionStatuses[i].OperationType == OperationType.Signal)
                        {
                            IOThreadMailbox mailbox = (IOThreadMailbox) completionStatuses[i].State;
                            mailbox.RaiseEvent();
                        }
                        // if the state is null we just ignore the completion status
                        else if (completionStatuses[i].State != null)
                        {
                            Item item = (Item) completionStatuses[i].State;

                            if (!item.Cancelled)
                            {
                                switch (completionStatuses[i].OperationType)
                                {
                                    case OperationType.Accept:
                                    case OperationType.Receive:
                                        item.ProactorEvents.InCompleted(
                                            completionStatuses[i].SocketError,
                                            completionStatuses[i].BytesTransferred);
                                        break;
                                    case OperationType.Connect:
                                    case OperationType.Disconnect:
                                    case OperationType.Send:
                                        item.ProactorEvents.OutCompleted(
                                            completionStatuses[i].SocketError,
                                            completionStatuses[i].BytesTransferred);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                    }
                    catch (TerminatingException)
                    {
                    }
                }
            }
        }
    }
}