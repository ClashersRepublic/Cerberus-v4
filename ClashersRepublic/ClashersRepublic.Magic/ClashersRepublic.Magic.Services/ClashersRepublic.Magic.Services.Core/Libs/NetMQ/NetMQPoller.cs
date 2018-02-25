namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Utils;
    using JetBrains.Annotations;
    using Switch = ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Utils.Switch;

    /// <summary>
    /// </summary>
    public sealed class NetMQPoller :
        TaskScheduler,
        INetMQPoller, ISocketPollableCollection, IEnumerable, IDisposable
    {
        private readonly List<NetMQSocket> m_sockets = new List<NetMQSocket>();
        private readonly List<NetMQTimer> m_timers = new List<NetMQTimer>();
        private readonly Dictionary<Socket, Action<Socket>> m_pollinSockets = new Dictionary<Socket, Action<Socket>>();
        private readonly Switch m_switch = new Switch(false);
        private readonly NetMQSelector m_netMqSelector = new NetMQSelector();
        private readonly StopSignaler m_stopSignaler = new StopSignaler();

        private NetMQSelector.Item[] m_pollSet;
        private NetMQSocket[] m_pollact;

        private volatile bool m_isPollSetDirty = true;
        private int m_disposeState = (int) DisposeState.Undisposed;

        #if NET35
        private Thread m_pollerThread;
        #endif

        #region Scheduling

        #if !NET35

        private readonly NetMQQueue<Task> m_tasksQueue = new NetMQQueue<Task>();
        private readonly ThreadLocal<bool> m_isSchedulerThread = new ThreadLocal<bool>(() => false);

        /// <summary>
        ///     Get whether the caller is running on the scheduler's thread.
        ///     If <c>true</c>, the caller can execute tasks directly (inline).
        ///     If <c>false</c>, the caller must start a <see cref="Task" /> on this scheduler.
        /// </summary>
        /// <remarks>
        ///     This property enables avoiding the creation of a <see cref="Task" /> object and
        ///     potential delays to its execution due to queueing. In most cases this is just
        ///     an optimisation.
        /// </remarks>
        /// <example>
        ///     <code>
        /// if (scheduler.CanExecuteTaskInline)
        /// {
        ///     socket.Send(...);
        /// }
        /// else
        /// {
        ///     var task = new Task(() => socket.Send(...));
        ///     task.Start(scheduler);
        /// }
        /// </code>
        /// </example>
        public bool CanExecuteTaskInline
        {
            get
            {
                return this.m_isSchedulerThread.Value;
            }
        }

        /// <summary>
        /// </summary>
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            this.CheckDisposed();

            return this.CanExecuteTaskInline && this.TryExecuteTask(task);
        }

        /// <summary>
        ///     Returns 1, as <see cref="NetMQPoller" /> runs a single thread and all tasks must execute on that thread.
        /// </summary>
        public override int MaximumConcurrencyLevel
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        ///     Not supported.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown.</exception>
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            // this is not supported, also it's only important for debug purposes and doesn't get called in real time.
            throw new NotSupportedException();
        }

        /// <summary>
        /// </summary>
        protected override void QueueTask(Task task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task));
            }

            this.CheckDisposed();

            this.m_tasksQueue.Enqueue(task);
        }

        private void Run([NotNull] Action action)
        {
            if (this.CanExecuteTaskInline)
            {
                action();
            }
            else
            {
                new Task(action).Start(this);
            }
        }
        #else
        private void Run(Action action)
        {
            action();
        }
        #endif

        #endregion

        /// <summary>
        /// </summary>
        public NetMQPoller()
        {
            this.m_sockets.Add(((ISocketPollable) this.m_stopSignaler).Socket);

            #if !NET35

            this.m_tasksQueue.ReceiveReady += delegate
            {
                Debug.Assert(this.m_disposeState != (int) DisposeState.Disposed);
                Debug.Assert(this.IsRunning);

                // Try to dequeue and execute all pending tasks
                Task task;
                while (this.m_tasksQueue.TryDequeue(out task, TimeSpan.Zero))
                {
                    this.TryExecuteTask(task);
                }
            };

            this.m_sockets.Add(((ISocketPollable) this.m_tasksQueue).Socket);
            #endif
        }

        /// <summary>
        ///     Get whether this object is currently polling its sockets and timers.
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return this.m_switch.Status;
            }
        }

        #region Add / Remove

        /// <summary>
        /// </summary>
        public void Add(ISocketPollable socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.CheckDisposed();

            this.Run(() =>
            {
                if (this.m_sockets.Contains(socket.Socket))
                {
                    return;
                }

                this.m_sockets.Add(socket.Socket);

                socket.Socket.EventsChanged += this.OnSocketEventsChanged;
                this.m_isPollSetDirty = true;
            });
        }

        /// <summary>
        /// </summary>
        public void Add([NotNull] NetMQTimer timer)
        {
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            this.CheckDisposed();

            this.Run(() => this.m_timers.Add(timer));
        }

        /// <summary>
        /// </summary>
        public void Add([NotNull] Socket socket, [NotNull] Action<Socket> callback)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            this.CheckDisposed();

            this.Run(() =>
            {
                if (this.m_pollinSockets.ContainsKey(socket))
                {
                    return;
                }

                this.m_pollinSockets.Add(socket, callback);
                this.m_isPollSetDirty = true;
            });
        }

        /// <summary>
        /// </summary>
        public void Remove(ISocketPollable socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.CheckDisposed();

            this.Run(() =>
            {
                socket.Socket.EventsChanged -= this.OnSocketEventsChanged;
                this.m_sockets.Remove(socket.Socket);
                this.m_isPollSetDirty = true;
            });
        }

        /// <summary>
        /// </summary>
        public void Remove([NotNull] NetMQTimer timer)
        {
            if (timer == null)
            {
                throw new ArgumentNullException(nameof(timer));
            }

            this.CheckDisposed();

            timer.When = -1;

            this.Run(() => this.m_timers.Remove(timer));
        }

        /// <summary>
        /// </summary>
        public void Remove([NotNull] Socket socket)
        {
            if (socket == null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            this.CheckDisposed();

            this.Run(() =>
            {
                this.m_pollinSockets.Remove(socket);
                this.m_isPollSetDirty = true;
            });
        }

        #endregion

        #region Start / Stop

        /// <summary>
        ///     Runs the poller in a background thread, returning immediately.
        /// </summary>
        public void RunAsync()
        {
            this.CheckDisposed();
            if (this.IsRunning)
            {
                throw new InvalidOperationException("NetMQPoller is already running");
            }

            Thread thread = new Thread(this.Run) {Name = "NetMQPollerThread"};
            thread.Start();

            this.m_switch.WaitForOn();
        }

        /// <summary>
        ///     Runs the poller on the caller's thread. Only returns when <see cref="Stop" /> or <see cref="StopAsync" /> are
        ///     called from another thread.
        /// </summary>
        public void Run()
        {
            this.CheckDisposed();
            if (this.IsRunning)
            {
                throw new InvalidOperationException("NetMQPoller is already running");
            }

            #if NET35
            m_pollerThread = Thread.CurrentThread;
            #else
            SynchronizationContext oldSynchronisationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new NetMQSynchronizationContext(this));
            this.m_isSchedulerThread.Value = true;
            #endif
            this.m_stopSignaler.Reset();

            this.m_switch.SwitchOn();
            try
            {
                // Recalculate all timers now
                foreach (NetMQTimer timer in this.m_timers)
                {
                    if (timer.Enable)
                    {
                        timer.When = Clock.NowMs() + timer.Interval;
                    }
                }

                // Run until stop is requested
                while (!this.m_stopSignaler.IsStopRequested)
                {
                    if (this.m_isPollSetDirty)
                    {
                        this.RebuildPollset();
                    }

                    long pollStart = Clock.NowMs();

                    // Set tickless to "infinity"
                    long tickless = pollStart + int.MaxValue;

                    // Find the When-value of the earliest timer..
                    foreach (NetMQTimer timer in this.m_timers)
                    {
                        // If it is enabled but no When is set yet,
                        if (timer.When == -1 && timer.Enable)
                        {
                            // Set this timer's When to now plus it's Interval.
                            timer.When = pollStart + timer.Interval;
                        }

                        // If it has a When and that is earlier than the earliest found thus far,
                        if (timer.When != -1 && tickless > timer.When)
                        {
                            // save that value.
                            tickless = timer.When;
                        }
                    }

                    // Compute a timeout value - how many milliseconds from now that the earliest-timer will expire.
                    long timeout = tickless - pollStart;

                    // Use zero to indicate it has already expired.
                    if (timeout < 0)
                    {
                        timeout = 0;
                    }

                    bool isItemAvailable = false;

                    if (this.m_pollSet.Length != 0)
                    {
                        isItemAvailable = this.m_netMqSelector.Select(this.m_pollSet, this.m_pollSet.Length, timeout);
                    }
                    else if (timeout > 0)
                    {
                        //TODO: Do we really want to simply sleep and return, doing nothing during this interval?
                        //TODO: Should a large value be passed it will sleep for a month literally.
                        //      Solution should be different, but sleep is more natural here than in selector (timers are not selector concern).
                        Debug.Assert(timeout <= int.MaxValue);
                        Thread.Sleep((int) timeout);
                    }

                    // Get the expected end time in case we time out. This looks redundant but, unfortunately,
                    // it happens that Poll takes slightly less than the requested time and 'Clock.NowMs() >= timer.When'
                    // may not true, even if it is supposed to be. In other words, even when Poll times out, it happens
                    // that 'Clock.NowMs() < pollStart + timeout'
                    long expectedPollEnd = !isItemAvailable ? pollStart + timeout : -1L;

                    // that way we make sure we can continue the loop if new timers are added.
                    // timers cannot be removed
                    foreach (NetMQTimer timer in this.m_timers)
                    {
                        if ((Clock.NowMs() >= timer.When || expectedPollEnd >= timer.When) && timer.When != -1)
                        {
                            timer.InvokeElapsed(this);

                            if (timer.Enable)
                            {
                                timer.When = Clock.NowMs() + timer.Interval;
                            }
                        }
                    }

                    for (int i = 0; i < this.m_pollSet.Length; i++)
                    {
                        NetMQSelector.Item item = this.m_pollSet[i];

                        if (item.Socket != null)
                        {
                            NetMQSocket socket = this.m_pollact[i];

                            if (item.ResultEvent.HasError())
                            {
                                if (++socket.Errors > 1)
                                {
                                    this.Remove(socket);
                                    item.ResultEvent = PollEvents.None;
                                }
                            }
                            else
                            {
                                socket.Errors = 0;
                            }

                            if (item.ResultEvent != PollEvents.None)
                            {
                                socket.InvokeEvents(this, item.ResultEvent);
                            }
                        }
                        else if (item.ResultEvent.HasError() || item.ResultEvent.HasIn())
                        {
                            Action<Socket> action;
                            if (this.m_pollinSockets.TryGetValue(item.FileDescriptor, out action))
                            {
                                action(item.FileDescriptor);
                            }
                        }
                    }
                }
            }
            finally
            {
                try
                {
                    foreach (NetMQSocket socket in this.m_sockets.ToList())
                    {
                        this.Remove(socket);
                    }
                }
                finally
                {
                    #if NET35
                    m_pollerThread = null;
                    #else
                    this.m_isSchedulerThread.Value = false;
                    SynchronizationContext.SetSynchronizationContext(oldSynchronisationContext);
                    #endif
                    this.m_switch.SwitchOff();
                }
            }
        }

        /// <summary>
        ///     Stops the poller, blocking until stopped.
        /// </summary>
        public void Stop()
        {
            this.CheckDisposed();
            if (!this.IsRunning)
            {
                throw new InvalidOperationException("NetMQPoller is not running");
            }

            // Signal the poller to stop
            this.m_stopSignaler.RequestStop();

            // If 'stop' was requested from the scheduler thread, we cannot block
            #if NET35
            if (m_pollerThread != Thread.CurrentThread)
            #else
            if (!this.m_isSchedulerThread.Value)
                #endif
            {
                this.m_switch.WaitForOff();
                Debug.Assert(!this.IsRunning);
            }
        }

        /// <summary>
        ///     Stops the poller, returning immediately and most likely before the poller has actually stopped.
        /// </summary>
        public void StopAsync()
        {
            this.CheckDisposed();
            if (!this.IsRunning)
            {
                throw new InvalidOperationException("NetMQPoller is not running");
            }

            this.m_stopSignaler.RequestStop();
        }

        #endregion

        private void OnSocketEventsChanged(object sender, NetMQSocketEventArgs e)
        {
            // when the sockets SendReady or ReceiveReady changed we marked the poller as dirty in order to reset the poll events
            this.m_isPollSetDirty = true;
        }

        private void RebuildPollset()
        {
            #if !NET35
            Debug.Assert(this.m_isSchedulerThread.Value);
            #endif

            // Recreate the m_pollSet and m_pollact arrays.
            this.m_pollSet = new NetMQSelector.Item[this.m_sockets.Count + this.m_pollinSockets.Count];
            this.m_pollact = new NetMQSocket[this.m_sockets.Count];

            // For each socket in m_sockets,
            // put a corresponding SelectItem into the m_pollSet array and a reference to the socket itself into the m_pollact array.
            uint index = 0;

            foreach (NetMQSocket socket in this.m_sockets)
            {
                this.m_pollSet[index] = new NetMQSelector.Item(socket, socket.GetPollEvents());
                this.m_pollact[index] = socket;
                index++;
            }

            foreach (Socket socket in this.m_pollinSockets.Keys)
            {
                this.m_pollSet[index] = new NetMQSelector.Item(socket, PollEvents.PollError | PollEvents.PollIn);
                index++;
            }

            // Mark this as NOT having any fresh events to attend to, as yet.
            this.m_isPollSetDirty = false;
        }

        #region IEnumerable

        /// <summary>This class only implements <see cref="IEnumerable" /> in order to support collection initialiser syntax.</summary>
        /// <returns>An empty enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            yield break;
        }

        #endregion

        #region IDisposable

        private enum DisposeState
        {
            Undisposed = 0,
            Disposing = 1,
            Disposed = 2
        }

        private void CheckDisposed()
        {
            if (this.m_disposeState == (int) DisposeState.Disposed)
            {
                throw new ObjectDisposedException("NetMQPoller");
            }
        }

        /// <summary>
        ///     Stops and disposes the poller. The poller may not be used once disposed.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref this.m_disposeState, (int) DisposeState.Disposing, 0) == (int) DisposeState.Disposing)
            {
                return;
            }

            // If this poller is already started, signal the polling thread to stop
            // and wait for it.
            if (this.IsRunning)
            {
                this.m_stopSignaler.RequestStop();
                this.m_switch.WaitForOff();
                Debug.Assert(!this.IsRunning);
            }

            this.m_stopSignaler.Dispose();
            #if !NET35
            this.m_tasksQueue.Dispose();
            #endif

            foreach (NetMQSocket socket in this.m_sockets)
            {
                socket.EventsChanged -= this.OnSocketEventsChanged;
            }

            this.m_disposeState = (int) DisposeState.Disposed;
        }

        #endregion

        #region ISynchronizeInvoke

        #if NET40
        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args)
        {
            var task = new Task<object>(() => method.DynamicInvoke(args));
            task.Start(this);
            return task;
        }

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result)
        {
            var task = (Task<object>)result;
            return task.Result;
        }

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args)
        {
            if (CanExecuteTaskInline)
                return method.DynamicInvoke(args);

            var task = new Task<object>(() => method.DynamicInvoke(args));
            task.Start(this);
            return task.Result;
        }

        bool ISynchronizeInvoke.InvokeRequired => !CanExecuteTaskInline;
        #endif

        #endregion

        #region Synchronisation context

        #if !NET35
        private sealed class NetMQSynchronizationContext : SynchronizationContext
        {
            private readonly NetMQPoller m_poller;

            public NetMQSynchronizationContext(NetMQPoller poller)
            {
                this.m_poller = poller;
            }

            /// <summary>Dispatches an asynchronous message to a synchronization context.</summary>
            public override void Post(SendOrPostCallback d, object state)
            {
                Task task = new Task(() => d(state));
                task.Start(this.m_poller);
            }

            /// <summary>Dispatches a synchronous message to a synchronization context.</summary>
            public override void Send(SendOrPostCallback d, object state)
            {
                Task task = new Task(() => d(state));
                task.Start(this.m_poller);
                task.Wait();
            }
        }
        #endif

        #endregion
    }
}