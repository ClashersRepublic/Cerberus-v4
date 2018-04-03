namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>
    ///     Forwards messages bidirectionally between two sockets. You can also specify a control socket tn which proxied
    ///     messages will be sent.
    /// </summary>
    /// <remarks>
    ///     This class must be explicitly started by calling <see cref="Start" />. If an external <see cref="Poller" /> has
    ///     been specified,
    ///     then that call will block until <see cref="Stop" /> is called.
    ///     <para />
    ///     If using an external <see cref="NetMQPoller" />, ensure the front and back end sockets have been added to it.
    ///     <para />
    ///     Users of this class must call <see cref="Stop" /> when messages should no longer be proxied.
    /// </remarks>
    public class Proxy
    {
        [NotNull] private readonly NetMQSocket m_frontend;
        [NotNull] private readonly NetMQSocket m_backend;
        [CanBeNull] private readonly NetMQSocket m_controlIn;
        [CanBeNull] private readonly NetMQSocket m_controlOut;
        [CanBeNull] private INetMQPoller m_poller;
        private readonly bool m_externalPoller;

        private int m_state = Proxy.StateStopped;

        private const int StateStopped = 0;
        private const int StateStarting = 1;
        private const int StateStarted = 2;
        private const int StateStopping = 3;

        /// <summary>
        ///     Create a new instance of a Proxy (NetMQ.Proxy)
        ///     with the given sockets to serve as a front-end, a back-end, and a control socket.
        /// </summary>
        /// <param name="frontend">the socket that messages will be forwarded from</param>
        /// <param name="backend">the socket that messages will be forwarded to</param>
        /// <param name="controlIn">
        ///     this socket will have incoming messages also sent to it - you can set this to null if not
        ///     needed
        /// </param>
        /// <param name="controlOut">
        ///     this socket will have outgoing messages also sent to it - you can set this to null if not
        ///     needed
        /// </param>
        /// <param name="poller">an optional external poller to use within this proxy</param>
        public Proxy([NotNull] NetMQSocket frontend, [NotNull] NetMQSocket backend, [CanBeNull] NetMQSocket controlIn, [CanBeNull] NetMQSocket controlOut, [CanBeNull] INetMQPoller poller = null)
        {
            if (poller != null)
            {
                this.m_externalPoller = true;
                this.m_poller = poller;
            }

            this.m_frontend = frontend;
            this.m_backend = backend;
            this.m_controlIn = controlIn;
            this.m_controlOut = controlOut ?? controlIn;
        }

        /// <summary>
        ///     Create a new instance of a Proxy (NetMQ.Proxy)
        ///     with the given sockets to serve as a front-end, a back-end, and a control socket.
        /// </summary>
        /// <param name="frontend">the socket that messages will be forwarded from</param>
        /// <param name="backend">the socket that messages will be forwarded to</param>
        /// <param name="control">this socket will have messages also sent to it - you can set this to null if not needed</param>
        /// <param name="poller">an optional external poller to use within this proxy</param>
        /// <exception cref="InvalidOperationException">
        ///     <paramref name="poller" /> is not <c>null</c> and either
        ///     <paramref name="frontend" /> or <paramref name="backend" /> are not contained within it.
        /// </exception>
        public Proxy([NotNull] NetMQSocket frontend, [NotNull] NetMQSocket backend, [CanBeNull] NetMQSocket control = null, [CanBeNull] INetMQPoller poller = null)
            : this(frontend, backend, control, null, poller)
        {
        }

        /// <summary>
        ///     Start proxying messages between the front and back ends. Blocks, unless using an external
        ///     <see cref="NetMQPoller" />.
        /// </summary>
        /// <exception cref="InvalidOperationException">The proxy has already been started.</exception>
        public void Start()
        {
            if (Interlocked.CompareExchange(ref this.m_state, Proxy.StateStarting, Proxy.StateStopped) != Proxy.StateStopped)
            {
                throw new InvalidOperationException("Proxy has already been started");
            }

            this.m_frontend.ReceiveReady += this.OnFrontendReady;
            this.m_backend.ReceiveReady += this.OnBackendReady;

            if (this.m_externalPoller)
            {
                this.m_state = Proxy.StateStarted;
            }
            else
            {
                this.m_poller = new NetMQPoller {this.m_frontend, this.m_backend};
                this.m_state = Proxy.StateStarted;
                this.m_poller.Run();
            }
        }

        /// <summary>
        ///     Stops the proxy, blocking until the underlying <see cref="NetMQPoller" /> has completed.
        /// </summary>
        /// <exception cref="InvalidOperationException">The proxy has not been started.</exception>
        public void Stop()
        {
            if (Interlocked.CompareExchange(ref this.m_state, Proxy.StateStopping, Proxy.StateStarted) != Proxy.StateStarted)
            {
                throw new InvalidOperationException("Proxy has not been started");
            }

            if (!this.m_externalPoller)
            {
                this.m_poller.Stop();
                this.m_poller.Dispose();
                this.m_poller = null;
            }

            this.m_frontend.ReceiveReady -= this.OnFrontendReady;
            this.m_backend.ReceiveReady -= this.OnBackendReady;

            this.m_state = Proxy.StateStopped;
        }

        private void OnFrontendReady(object sender, NetMQSocketEventArgs e)
        {
            Proxy.ProxyBetween(this.m_frontend, this.m_backend, this.m_controlIn);
        }

        private void OnBackendReady(object sender, NetMQSocketEventArgs e)
        {
            Proxy.ProxyBetween(this.m_backend, this.m_frontend, this.m_controlOut);
        }

        private static void ProxyBetween(IReceivingSocket from, IOutgoingSocket to, [CanBeNull] IOutgoingSocket control)
        {
            Msg msg = new Msg();
            msg.InitEmpty();

            Msg copy = new Msg();
            copy.InitEmpty();

            while (true)
            {
                from.Receive(ref msg);
                bool more = msg.HasMore;

                if (control != null)
                {
                    copy.Copy(ref msg);

                    control.Send(ref copy, more);
                }

                to.Send(ref msg, more);

                if (!more)
                {
                    break;
                }
            }

            copy.Close();
            msg.Close();
        }
    }
}