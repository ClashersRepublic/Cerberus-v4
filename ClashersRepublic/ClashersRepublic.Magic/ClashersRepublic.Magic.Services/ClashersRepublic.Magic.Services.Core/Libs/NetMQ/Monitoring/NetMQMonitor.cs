#if !NET35
#endif

namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Monitoring
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AsyncIO;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Sockets;
    using JetBrains.Annotations;

    /// <summary>
    ///     Monitors a <see cref="NetMQSocket" /> for events, raising them via events.
    /// </summary>
    /// <remarks>
    ///     To run a monitor instance, either:
    ///     <list type="bullet">
    ///         <item>Call <see cref="Start" /> (blocking) and <see cref="Stop" />, or</item>
    ///         <item>Call <see cref="AttachToPoller" /> and <see cref="DetachFromPoller" />.</item>
    ///     </list>
    /// </remarks>
    public class NetMQMonitor : IDisposable
    {
        [NotNull] private readonly NetMQSocket m_monitoringSocket;
        private readonly bool m_ownsMonitoringSocket;
        [CanBeNull] private ISocketPollableCollection m_attachedPoller;
        private int m_cancel;

        private readonly ManualResetEvent m_isStoppedEvent = new ManualResetEvent(true);

        /// <summary>
        /// </summary>
        public NetMQMonitor([NotNull] NetMQSocket monitoredSocket, [NotNull] string endpoint, SocketEvents eventsToMonitor)
        {
            this.Endpoint = endpoint;
            this.Timeout = TimeSpan.FromSeconds(0.5);

            monitoredSocket.Monitor(endpoint, eventsToMonitor);

            this.m_monitoringSocket = new PairSocket();
            this.m_monitoringSocket.Options.Linger = TimeSpan.Zero;
            this.m_monitoringSocket.ReceiveReady += this.Handle;

            this.m_ownsMonitoringSocket = true;
        }

        /// <summary>
        ///     Initialises a monitor on <paramref name="socket" /> for a specified <paramref name="endpoint" />.
        /// </summary>
        /// <remarks>
        ///     This constructor matches the signature used by clrzmq.
        /// </remarks>
        /// <param name="socket">The socket to monitor.</param>
        /// <param name="endpoint">a string denoting the endpoint which will be the monitoring address</param>
        /// <param name="ownsSocket">
        ///     A flag indicating whether ownership of <paramref name="socket" /> is transferred to the monitor.
        ///     If <c>true</c>, disposing the monitor will also dispose <paramref name="socket" />.
        /// </param>
        public NetMQMonitor([NotNull] NetMQSocket socket, [NotNull] string endpoint, bool ownsSocket = false)
        {
            this.Endpoint = endpoint;
            this.Timeout = TimeSpan.FromSeconds(0.5);
            this.m_monitoringSocket = socket;
            this.m_monitoringSocket.ReceiveReady += this.Handle;

            this.m_ownsMonitoringSocket = ownsSocket;
        }

        /// <summary>
        ///     The monitoring address.
        /// </summary>
        public string Endpoint { get; }

        /// <summary>
        ///     Get whether this monitor is currently running.
        /// </summary>
        /// <remarks>
        ///     Start the monitor running via either <see cref="Start" /> or <see cref="AttachToPoller" />.
        ///     Stop the monitor via either <see cref="Stop" /> or <see cref="DetachFromPoller" />.
        /// </remarks>
        public bool IsRunning { get; private set; }

        /// <summary>
        ///     Gets and sets the timeout interval for poll iterations when using <see cref="Start" /> and <see cref="Stop" />.
        /// </summary>
        /// <remarks>
        ///     The higher the number the longer it may take the to stop the monitor.
        ///     This value has no effect when the monitor is run via <see cref="AttachToPoller" />.
        /// </remarks>
        public TimeSpan Timeout { get; set; }

        #region Events

        /// <summary>
        ///     Raised whenever any monitored event fires.
        /// </summary>
        public event EventHandler<NetMQMonitorEventArgs> EventReceived;

        /// <summary>
        ///     Occurs when a connection is made to a socket.
        /// </summary>
        public event EventHandler<NetMQMonitorSocketEventArgs> Connected;

        /// <summary>
        ///     Occurs when a synchronous connection attempt failed, and its completion is being polled for.
        /// </summary>
        public event EventHandler<NetMQMonitorErrorEventArgs> ConnectDelayed;

        /// <summary>
        ///     Occurs when an asynchronous connect / reconnection attempt is being handled by a reconnect timer.
        /// </summary>
        public event EventHandler<NetMQMonitorIntervalEventArgs> ConnectRetried;

        /// <summary>
        ///     Occurs when a socket is bound to an address and is ready to accept connections.
        /// </summary>
        public event EventHandler<NetMQMonitorSocketEventArgs> Listening;

        /// <summary>
        ///     Occurs when a socket could not bind to an address.
        /// </summary>
        public event EventHandler<NetMQMonitorErrorEventArgs> BindFailed;

        /// <summary>
        ///     Occurs when a connection from a remote peer has been established with a socket's listen address.
        /// </summary>
        public event EventHandler<NetMQMonitorSocketEventArgs> Accepted;

        /// <summary>
        ///     Occurs when a connection attempt to a socket's bound address fails.
        /// </summary>
        public event EventHandler<NetMQMonitorErrorEventArgs> AcceptFailed;

        /// <summary>
        ///     Occurs when a connection was closed.
        /// </summary>
        public event EventHandler<NetMQMonitorSocketEventArgs> Closed;

        /// <summary>
        ///     Occurs when a connection couldn't be closed.
        /// </summary>
        public event EventHandler<NetMQMonitorErrorEventArgs> CloseFailed;

        /// <summary>
        ///     Occurs when the stream engine (TCP and IPC specific) detects a corrupted / broken session.
        /// </summary>
        public event EventHandler<NetMQMonitorSocketEventArgs> Disconnected;

        #endregion

        private void Handle(object sender, NetMQSocketEventArgs socketEventArgs)
        {
            MonitorEvent monitorEvent = MonitorEvent.Read(this.m_monitoringSocket.SocketHandle);

            switch (monitorEvent.Event)
            {
                case SocketEvents.Connected:
                    this.InvokeEvent(this.Connected, new NetMQMonitorSocketEventArgs(this, monitorEvent.Addr, (AsyncSocket) monitorEvent.Arg, SocketEvents.Connected));
                    break;
                case SocketEvents.ConnectDelayed:
                    this.InvokeEvent(this.ConnectDelayed, new NetMQMonitorErrorEventArgs(this, monitorEvent.Addr, (ErrorCode) monitorEvent.Arg, SocketEvents.ConnectDelayed));
                    break;
                case SocketEvents.ConnectRetried:
                    this.InvokeEvent(this.ConnectRetried, new NetMQMonitorIntervalEventArgs(this, monitorEvent.Addr, (int) monitorEvent.Arg, SocketEvents.ConnectRetried));
                    break;
                case SocketEvents.Listening:
                    this.InvokeEvent(this.Listening, new NetMQMonitorSocketEventArgs(this, monitorEvent.Addr, (AsyncSocket) monitorEvent.Arg, SocketEvents.Listening));
                    break;
                case SocketEvents.BindFailed:
                    this.InvokeEvent(this.BindFailed, new NetMQMonitorErrorEventArgs(this, monitorEvent.Addr, (ErrorCode) monitorEvent.Arg, SocketEvents.BindFailed));
                    break;
                case SocketEvents.Accepted:
                    this.InvokeEvent(this.Accepted, new NetMQMonitorSocketEventArgs(this, monitorEvent.Addr, (AsyncSocket) monitorEvent.Arg, SocketEvents.Accepted));
                    break;
                case SocketEvents.AcceptFailed:
                    this.InvokeEvent(this.AcceptFailed, new NetMQMonitorErrorEventArgs(this, monitorEvent.Addr, (ErrorCode) monitorEvent.Arg, SocketEvents.AcceptFailed));
                    break;
                case SocketEvents.Closed:
                    this.InvokeEvent(this.Closed, new NetMQMonitorSocketEventArgs(this, monitorEvent.Addr, (AsyncSocket) monitorEvent.Arg, SocketEvents.Closed));
                    break;
                case SocketEvents.CloseFailed:
                    this.InvokeEvent(this.CloseFailed, new NetMQMonitorErrorEventArgs(this, monitorEvent.Addr, (ErrorCode) monitorEvent.Arg, SocketEvents.CloseFailed));
                    break;
                case SocketEvents.Disconnected:
                    this.InvokeEvent(this.Disconnected, new NetMQMonitorSocketEventArgs(this, monitorEvent.Addr, (AsyncSocket) monitorEvent.Arg, SocketEvents.Disconnected));
                    break;
                default:
                    throw new Exception("unknown event " + monitorEvent.Event);
            }
        }

        private void InvokeEvent<T>(EventHandler<T> handler, T args) where T : NetMQMonitorEventArgs
        {
            this.EventReceived?.Invoke(this, args);
            handler?.Invoke(this, args);
        }

        private void InternalStart()
        {
            this.m_isStoppedEvent.Reset();
            this.IsRunning = true;
            this.m_monitoringSocket.Connect(this.Endpoint);
        }

        private void InternalClose()
        {
            try
            {
                this.m_monitoringSocket.Disconnect(this.Endpoint);
            }
            catch (Exception)
            {
            }
            finally
            {
                this.IsRunning = false;
                this.m_isStoppedEvent.Set();
            }
        }

        /// <summary>
        /// </summary>
        public void AttachToPoller([NotNull] ISocketPollableCollection poller)
        {
            if (poller == null)
            {
                throw new ArgumentNullException(nameof(poller));
            }

            if (this.IsRunning)
            {
                throw new InvalidOperationException("Monitor already started");
            }

            if (Interlocked.CompareExchange(ref this.m_attachedPoller, poller, null) != null)
            {
                throw new InvalidOperationException("Already attached to a poller");
            }

            this.InternalStart();
            poller.Add(this.m_monitoringSocket);
        }

        /// <summary>
        /// </summary>
        public void DetachFromPoller()
        {
            if (this.m_attachedPoller == null)
            {
                throw new InvalidOperationException("Not attached to a poller");
            }

            this.m_attachedPoller.Remove(this.m_monitoringSocket);
            this.m_attachedPoller = null;
            this.InternalClose();
        }

        /// <summary>
        ///     Start monitor the socket, the method doesn't start a new thread and will block until the monitor poll is stopped
        /// </summary>
        /// <exception cref="InvalidOperationException">The Monitor must not have already started nor attached to a poller.</exception>
        public void Start()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("Monitor already started");
            }

            if (this.m_attachedPoller != null)
            {
                throw new InvalidOperationException("Monitor attached to a poller");
            }

            try
            {
                this.InternalStart();

                while (this.m_cancel == 0)
                {
                    this.m_monitoringSocket.Poll(this.Timeout);
                }
            }
            finally
            {
                this.InternalClose();
            }
        }

        #if !NET35
        /// <summary>
        ///     Start a background task for the monitoring operation.
        /// </summary>
        /// <returns></returns>
        public Task StartAsync()
        {
            if (this.IsRunning)
            {
                throw new InvalidOperationException("Monitor already started");
            }

            if (this.m_attachedPoller != null)
            {
                throw new InvalidOperationException("Monitor attached to a poller");
            }

            return Task.Factory.StartNew(this.Start);
        }
        #endif

        /// <summary>
        ///     Stop monitoring. Blocks until monitoring completed.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     If this monitor is attached to a poller you must detach it first and not
        ///     use the stop method.
        /// </exception>
        public void Stop()
        {
            if (this.m_attachedPoller != null)
            {
                throw new InvalidOperationException("Monitor attached to a poller, please detach from poller and don't use the stop method");
            }

            Interlocked.Exchange(ref this.m_cancel, 1);
            this.m_isStoppedEvent.WaitOne();
        }

        #region Dispose

        /// <summary>
        ///     Release and dispose of any contained resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Release and dispose of any contained resources.
        /// </summary>
        /// <param name="disposing">true if releasing managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.m_attachedPoller != null)
            {
                this.DetachFromPoller();
            }
            else if (!this.m_isStoppedEvent.WaitOne(0))
            {
                this.Stop();
            }

            this.m_monitoringSocket.ReceiveReady -= this.Handle;

            #if NET35
            m_isStoppedEvent.Close();
                        #else
            this.m_isStoppedEvent.Dispose();
            #endif

            if (this.m_ownsMonitoringSocket)
            {
                this.m_monitoringSocket.Dispose();
            }
        }

        #endregion
    }
}