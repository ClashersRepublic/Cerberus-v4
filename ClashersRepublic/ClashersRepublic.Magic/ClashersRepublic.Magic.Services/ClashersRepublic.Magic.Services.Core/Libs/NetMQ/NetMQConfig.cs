namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core;

    /// <summary>
    /// </summary>
    public static class NetMQConfig
    {
        private static TimeSpan s_linger;

        private static Ctx s_ctx;
        private static int s_threadPoolSize = Ctx.DefaultIOThreads;
        private static int s_maxSockets = Ctx.DefaultMaxSockets;
        private static readonly object s_sync;

        static NetMQConfig()
        {
            NetMQConfig.s_sync = new object();
            NetMQConfig.s_linger = TimeSpan.Zero;
        }

        internal static Ctx Context
        {
            get
            {
                lock (NetMQConfig.s_sync)
                {
                    if (NetMQConfig.s_ctx == null)
                    {
                        NetMQConfig.s_ctx = new Ctx();
                        NetMQConfig.s_ctx.IOThreadCount = NetMQConfig.s_threadPoolSize;
                        NetMQConfig.s_ctx.MaxSockets = NetMQConfig.s_maxSockets;
                    }

                    return NetMQConfig.s_ctx;
                }
            }
        }

        /// <summary>
        ///     Cleanup library resources, call this method when your process is shutting-down.
        /// </summary>
        /// <param name="block">Set to true when you want to make sure sockets send all pending messages</param>
        public static void Cleanup(bool block = true)
        {
            lock (NetMQConfig.s_sync)
            {
                if (NetMQConfig.s_ctx != null)
                {
                    NetMQConfig.s_ctx.Terminate(block);
                    NetMQConfig.s_ctx = null;
                }
            }
        }

        /// <summary>
        ///     Get or set the default linger period for the all sockets,
        ///     which determines how long pending messages which have yet to be sent to a peer
        ///     shall linger in memory after a socket is closed.
        /// </summary>
        /// <remarks>
        ///     This also affects the termination of the socket's context.
        ///     -1: Specifies infinite linger period. Pending messages shall not be discarded after the socket is closed;
        ///     attempting to terminate the socket's context shall block until all pending messages have been sent to a peer.
        ///     0: The default value of 0 specifies an no linger period. Pending messages shall be discarded immediately when the
        ///     socket is closed.
        ///     Positive values specify an upper bound for the linger period. Pending messages shall not be discarded after the
        ///     socket is closed;
        ///     attempting to terminate the socket's context shall block until either all pending messages have been sent to a
        ///     peer,
        ///     or the linger period expires, after which any pending messages shall be discarded.
        /// </remarks>
        public static TimeSpan Linger
        {
            get
            {
                lock (NetMQConfig.s_sync)
                {
                    return NetMQConfig.s_linger;
                }
            }
            set
            {
                lock (NetMQConfig.s_sync)
                {
                    NetMQConfig.s_linger = value;
                }
            }
        }

        /// <summary>
        ///     Get or set the number of IO Threads NetMQ will create, default is 1.
        ///     1 is good for most cases.
        /// </summary>
        public static int ThreadPoolSize
        {
            get
            {
                lock (NetMQConfig.s_sync)
                {
                    return NetMQConfig.s_threadPoolSize;
                }
            }
            set
            {
                lock (NetMQConfig.s_sync)
                {
                    NetMQConfig.s_threadPoolSize = value;

                    if (NetMQConfig.s_ctx != null)
                    {
                        NetMQConfig.s_ctx.IOThreadCount = value;
                    }
                }
            }
        }

        /// <summary>
        ///     Get or set the maximum number of sockets.
        /// </summary>
        public static int MaxSockets
        {
            get
            {
                lock (NetMQConfig.s_sync)
                {
                    return NetMQConfig.s_maxSockets;
                }
            }
            set
            {
                lock (NetMQConfig.s_sync)
                {
                    NetMQConfig.s_maxSockets = value;

                    if (NetMQConfig.s_ctx != null)
                    {
                        NetMQConfig.s_ctx.MaxSockets = value;
                    }
                }
            }
        }

        #region Obsolete

        /// <summary>
        ///     Method is obsolete, call Cleanup instead
        /// </summary>
        [Obsolete("Use Cleanup method")]
        public static void ManualTerminationTakeOver()
        {
        }

        /// <summary>
        ///     Method is obsolete, call Cleanup instead
        /// </summary>
        [Obsolete("Use Cleanup method")]
        internal static void DisableManualTermination()
        {
        }

        /// <summary>
        ///     Method is obsolete, call Cleanup instead
        /// </summary>
        /// <param name="block">Should the context block the thread while terminating.</param>
        [Obsolete("Use Cleanup method")]
        public static void ContextTerminate(bool block = true)
        {
        }

        /// <summary>
        ///     /// Method is obsolete, context created automatically
        /// </summary>
        [Obsolete("Context is created automatically")]
        public static void ContextCreate(bool block = false)
        {
            NetMQConfig.Cleanup(block);
        }

        #endregion
    }
}