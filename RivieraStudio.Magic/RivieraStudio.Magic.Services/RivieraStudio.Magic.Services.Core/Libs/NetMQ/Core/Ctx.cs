/*
    Copyright (c) 2007-2012 iMatix Corporation
    Copyright (c) 2009-2011 250bpm s.r.o.
    Copyright (c) 2007-2015 Other contributors as noted in the AUTHORS file

    This file is part of 0MQ.

    0MQ is free software; you can redistribute it and/or modify it under
    the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation; either version 3 of the License, or
    (at your option) any later version.

    0MQ is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>
    ///     Objects of class Ctx are intended to encapsulate all of the global state
    ///     associated with the NetMQ library. This contains the sockets, and manages interaction
    ///     between them.
    /// </summary>
    /// <remarks>Internal analog of the public <see cref="NetMQContext" /> class.</remarks>
    internal sealed class Ctx
    {
        internal const int DefaultIOThreads = 1;
        internal const int DefaultMaxSockets = 1024;

        #region Nested class: Endpoint

        /// <summary>
        ///     Information associated with inproc endpoint. Note that endpoint options
        ///     are registered as well so that the peer can access them without a need
        ///     for synchronisation, handshaking or similar.
        /// </summary>
        public class Endpoint
        {
            /// <summary>
            ///     Create a new Endpoint with the given socket.
            /// </summary>
            /// <param name="socket">the socket for this new Endpoint</param>
            /// <param name="options">the Options to assign to this new Endpoint</param>
            public Endpoint([NotNull] SocketBase socket, [NotNull] Options options)
            {
                this.Socket = socket;
                this.Options = options;
            }

            /// <summary>
            ///     Get the socket associated with this Endpoint.
            /// </summary>
            [NotNull]
            public SocketBase Socket { get; }

            /// <summary>
            ///     Get the Options of this Endpoint.
            /// </summary>
            [NotNull]
            public Options Options { get; }
        }

        #endregion

        private bool m_disposed;

        /// <summary>
        ///     Sockets belonging to this context. We need the list so that
        ///     we can notify the sockets when zmq_term() is called. The sockets
        ///     will return ETERM then.
        /// </summary>
        private readonly List<SocketBase> m_sockets = new List<SocketBase>();

        /// <summary>
        ///     List of unused thread slots.
        /// </summary>
        private readonly Stack<int> m_emptySlots = new Stack<int>();

        /// <summary>
        ///     If true, zmq_init has been called but no socket has been created
        ///     yet. Launching of I/O threads is delayed.
        /// </summary>
        private volatile bool m_starting = true;

        /// <summary>
        ///     If true, zmq_term was already called.
        /// </summary>
        private bool m_terminating;

        /// <summary>
        ///     This object is for synchronisation of accesses to global slot-related data:
        ///     sockets, empty_slots, terminating. It also synchronises
        ///     access to zombie sockets as such (as opposed to slots) and provides
        ///     a memory barrier to ensure that all CPU cores see the same data.
        /// </summary>
        private readonly object m_slotSync = new object();

        /// <summary>
        ///     The reaper thread.
        /// </summary>
        [CanBeNull] private Reaper m_reaper;

        /// <summary>
        ///     List of I/O threads.
        /// </summary>
        private readonly List<IOThread> m_ioThreads = new List<IOThread>();

        /// <summary>
        ///     Length of the mailbox-array.
        /// </summary>
        private int m_slotCount;

        /// <summary>
        ///     Array of pointers to mailboxes for both application and I/O threads.
        /// </summary>
        [CanBeNull] private IMailbox[] m_slots;

        /// <summary>
        ///     Mailbox for zmq_term thread.
        /// </summary>
        private readonly Mailbox m_termMailbox = new Mailbox("terminator");

        /// <summary>
        ///     Dictionary containing the inproc endpoints within this context.
        /// </summary>
        private readonly Dictionary<string, Endpoint> m_endpoints = new Dictionary<string, Endpoint>();

        /// <summary>
        ///     This object provides synchronisation of access to the list of inproc endpoints.
        /// </summary>
        private readonly object m_endpointsSync = new object();

        /// <summary>
        ///     The highest socket-id that has been assigned thus far.
        /// </summary>
        private static int s_maxSocketId;

        /// <summary>
        ///     The maximum number of sockets that can be opened at the same time.
        /// </summary>
        private int m_maxSockets = Ctx.DefaultMaxSockets;

        /// <summary>
        ///     The number of I/O threads to launch.
        /// </summary>
        private int m_ioThreadCount = Ctx.DefaultIOThreads;

        /// <summary>
        ///     This object is used to synchronize access to context options.
        /// </summary>
        private readonly object m_optSync = new object();

        /// <summary>
        ///     The thread-id for the termination (the equivalent of the zmq_term) thread.
        /// </summary>
        public const int TermTid = 0;

        /// <summary>
        ///     This is the thread-id to assign to the Reaper (value is 1).
        /// </summary>
        public const int ReaperTid = 1;

        /// <summary>Throws <see cref="ObjectDisposedException" /> if this is already disposed.</summary>
        /// <exception cref="ObjectDisposedException">This object has already been disposed.</exception>
        public void CheckDisposed()
        {
            if (this.m_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        ///     This function is called when user invokes zmq_term. If there are
        ///     no more sockets open it'll cause all the infrastructure to be shut
        ///     down. If there are open sockets still, the deallocation happens
        ///     after the last one is closed.
        /// </summary>
        public void Terminate(bool block)
        {
            this.m_disposed = true;

            Monitor.Enter(this.m_slotSync);

            if (!this.m_starting)
            {
                // Check whether termination was already underway, but interrupted and now
                // restarted.
                bool restarted = this.m_terminating;
                this.m_terminating = true;
                Monitor.Exit(this.m_slotSync);

                // First attempt to terminate the context.
                if (!restarted)
                {
                    // First send stop command to sockets so that any blocking calls
                    // can be interrupted. If there are no sockets we can ask reaper
                    // thread to stop.
                    Monitor.Enter(this.m_slotSync);
                    try
                    {
                        foreach (SocketBase socket in this.m_sockets)
                        {
                            socket.Stop();
                        }

                        if (!block)
                        {
                            this.m_reaper.ForceStop();
                        }
                        else if (this.m_sockets.Count == 0)
                        {
                            this.m_reaper.Stop();
                        }
                    }
                    finally
                    {
                        Monitor.Exit(this.m_slotSync);
                    }
                }

                // Wait till reaper thread closes all the sockets.
                Command command;
                bool found = this.m_termMailbox.TryRecv(-1, out command);

                Debug.Assert(found);
                Debug.Assert(command.CommandType == CommandType.Done);
                Monitor.Enter(this.m_slotSync);
            }

            Monitor.Exit(this.m_slotSync);

            // Deallocate the resources.
            foreach (IOThread it in this.m_ioThreads)
            {
                it.Stop();
            }

            foreach (IOThread it in this.m_ioThreads)
            {
                it.Destroy();
            }

            this.m_reaper?.Destroy();

            this.m_termMailbox.Close();

            this.m_disposed = true;
        }

        public int IOThreadCount
        {
            get
            {
                return this.m_ioThreadCount;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Must be zero or greater");
                }

                lock (this.m_optSync)
                {
                    this.m_ioThreadCount = value;
                }
            }
        }

        public int MaxSockets
        {
            get
            {
                return this.m_maxSockets;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "Must be greater than zero");
                }

                lock (this.m_optSync)
                {
                    this.m_maxSockets = value;
                }
            }
        }

        /// <summary>
        ///     Create and return a new socket of the given type, and initialize this Ctx if this is the first one.
        /// </summary>
        /// <param name="type">the type of socket to create</param>
        /// <returns>the newly-created socket</returns>
        /// <exception cref="TerminatingException">Cannot create new socket while terminating.</exception>
        /// <exception cref="NetMQException">Maximum number of sockets reached.</exception>
        /// <exception cref="TerminatingException">The context (Ctx) must not be already terminating.</exception>
        [NotNull]
        public SocketBase CreateSocket(ZmqSocketType type)
        {
            lock (this.m_slotSync)
            {
                if (this.m_starting)
                {
                    this.m_starting = false;
                    // Initialise the array of mailboxes. Additional three slots are for
                    // zmq_term thread and reaper thread.

                    int ios;
                    int mazmq;

                    lock (this.m_optSync)
                    {
                        mazmq = this.m_maxSockets;
                        ios = this.m_ioThreadCount;
                    }

                    this.m_slotCount = mazmq + ios + 2;
                    this.m_slots = new IMailbox[this.m_slotCount];
                    //alloc_Debug.Assert(slots);

                    // Initialise the infrastructure for zmq_term thread.
                    this.m_slots[Ctx.TermTid] = this.m_termMailbox;

                    // Create the reaper thread.
                    this.m_reaper = new Reaper(this, Ctx.ReaperTid);
                    //alloc_Debug.Assert(reaper);
                    this.m_slots[Ctx.ReaperTid] = this.m_reaper.Mailbox;
                    this.m_reaper.Start();

                    // Create I/O thread objects and launch them.
                    for (int i = 2; i != ios + 2; i++)
                    {
                        IOThread ioThread = new IOThread(this, i);
                        //alloc_Debug.Assert(io_thread);
                        this.m_ioThreads.Add(ioThread);
                        this.m_slots[i] = ioThread.Mailbox;
                        ioThread.Start();
                    }

                    // In the unused part of the slot array, create a list of empty slots.
                    for (int i = this.m_slotCount - 1; i >= ios + 2; i--)
                    {
                        this.m_emptySlots.Push(i);
                        this.m_slots[i] = null;
                    }
                }

                // Once zmq_term() was called, we can't create new sockets.
                if (this.m_terminating)
                {
                    string xMsg = $"Ctx.CreateSocket({type}), cannot create new socket while terminating.";
                    throw new TerminatingException(null, xMsg);
                }

                // If max_sockets limit was reached, return error.
                if (this.m_emptySlots.Count == 0)
                {
                    #if DEBUG
                    string xMsg = $"Ctx.CreateSocket({type}), max number of sockets {this.m_maxSockets} reached.";
                    throw NetMQException.Create(xMsg, ErrorCode.TooManyOpenSockets);
                    #else
                    throw NetMQException.Create(ErrorCode.TooManyOpenSockets);
                                        #endif
                }

                // Choose a slot for the socket.
                int slot = this.m_emptySlots.Pop();

                // Generate new unique socket ID.
                int socketId = Interlocked.Increment(ref Ctx.s_maxSocketId);

                // Create the socket and register its mailbox.
                SocketBase s = SocketBase.Create(type, this, slot, socketId);

                this.m_sockets.Add(s);
                this.m_slots[slot] = s.Mailbox;

                //LOG.debug("NEW Slot [" + slot + "] " + s);

                return s;
            }
        }

        /// <summary>
        ///     Destroy the given socket - which means to remove it from the list of active sockets,
        ///     and add it to the list of unused sockets to be terminated.
        /// </summary>
        /// <param name="socket">the socket to destroy</param>
        /// <remarks>
        ///     If this was the last socket, then stop the reaper.
        /// </remarks>
        public void DestroySocket([NotNull] SocketBase socket)
        {
            // Free the associated thread slot.
            lock (this.m_slotSync)
            {
                int threadId = socket.ThreadId;
                this.m_emptySlots.Push(threadId);
                this.m_slots[threadId].Close();
                this.m_slots[threadId] = null;

                // Remove the socket from the list of sockets.
                this.m_sockets.Remove(socket);

                // If zmq_term() was already called and there are no more socket
                // we can ask reaper thread to terminate.
                if (this.m_terminating && this.m_sockets.Count == 0)
                {
                    this.m_reaper.Stop();
                }
            }

            //LOG.debug("Released Slot [" + socket_ + "] ");
        }

        /// <summary>
        ///     Returns reaper thread object.
        /// </summary>
        public ZObject GetReaper()
        {
            return this.m_reaper;
        }

        /// <summary>
        ///     Send a command to the given destination thread.
        /// </summary>
        public void SendCommand(int threadId, [NotNull] Command command)
        {
            this.m_slots[threadId].Send(command);
        }

        /// <summary>
        ///     Returns the <see cref="IOThread" /> that is the least busy at the moment.
        /// </summary>
        /// <paramref name="affinity">Which threads are eligible (0 = all).</paramref>
        /// <returns>The least busy thread, or <c>null</c> if none is available.</returns>
        [CanBeNull]
        public IOThread ChooseIOThread(long affinity)
        {
            if (this.m_ioThreads.Count == 0)
            {
                return null;
            }

            // Find the I/O thread with minimum load.
            int minLoad = -1;
            IOThread selectedIOThread = null;

            for (int i = 0; i != this.m_ioThreads.Count; i++)
            {
                IOThread ioThread = this.m_ioThreads[i];

                if (affinity == 0 || (affinity & (1L << i)) > 0)
                {
                    if (selectedIOThread == null || ioThread.Load < minLoad)
                    {
                        minLoad = ioThread.Load;
                        selectedIOThread = ioThread;
                    }
                }
            }

            return selectedIOThread;
        }

        /// <summary>
        ///     Save the given address and Endpoint within our internal list.
        ///     This is used for management of inproc endpoints.
        /// </summary>
        /// <param name="address">the textual name to give this endpoint</param>
        /// <param name="endpoint">the Endpoint to remember</param>
        /// <returns>true if the given address was NOT already registered</returns>
        public bool RegisterEndpoint([NotNull] string address, [NotNull] Endpoint endpoint)
        {
            lock (this.m_endpointsSync)
            {
                if (this.m_endpoints.ContainsKey(address))
                {
                    return false;
                }

                this.m_endpoints[address] = endpoint;
                return true;
            }
        }

        /// <summary>
        ///     Un-register the given address/socket, by removing it from the contained list of endpoints.
        /// </summary>
        /// <param name="address">the (string) address denoting the endpoint to unregister</param>
        /// <param name="socket">the socket associated with that endpoint</param>
        /// <returns>true if the endpoint having this address and socket is found, false otherwise</returns>
        public bool UnregisterEndpoint([NotNull] string address, [NotNull] SocketBase socket)
        {
            lock (this.m_endpointsSync)
            {
                Endpoint endpoint;

                if (!this.m_endpoints.TryGetValue(address, out endpoint))
                {
                    return false;
                }

                if (socket != endpoint.Socket)
                {
                    return false;
                }

                this.m_endpoints.Remove(address);
                return true;
            }
        }

        /// <summary>
        ///     Remove from the list of endpoints, all endpoints that reference the given socket.
        /// </summary>
        /// <param name="socket">the socket to remove all references to</param>
        public void UnregisterEndpoints([NotNull] SocketBase socket)
        {
            lock (this.m_endpointsSync)
            {
                IList<string> removeList = this.m_endpoints.Where(e => e.Value.Socket == socket).Select(e => e.Key).ToList();

                foreach (string item in removeList)
                {
                    this.m_endpoints.Remove(item);
                }
            }
        }

        /// <summary>
        ///     Return the EndPoint that has the given address, and increments the seqnum of the associated socket.
        /// </summary>
        /// <param name="addr">the (string) address to match against the endpoints</param>
        /// <returns>the Endpoint that was found</returns>
        /// <exception cref="EndpointNotFoundException">The given address was not found in the list of endpoints.</exception>
        /// <remarks>
        ///     By calling this method, the socket associated with that returned EndPoint has it's Seqnum incremented,
        ///     in order to prevent it from being de-allocated before a command can be sent to it.
        /// </remarks>
        [NotNull]
        public Endpoint FindEndpoint([NotNull] string addr)
        {
            Debug.Assert(addr != null);

            lock (this.m_endpointsSync)
            {
                if (!this.m_endpoints.ContainsKey(addr))
                {
                    throw new EndpointNotFoundException();
                }

                Endpoint endpoint = this.m_endpoints[addr];

                if (endpoint == null)
                {
                    throw new EndpointNotFoundException();
                }

                // Increment the command sequence number of the peer so that it won't
                // get deallocated until "bind" command is issued by the caller.
                // The subsequent 'bind' has to be called with inc_seqnum parameter
                // set to false, so that the seqnum isn't incremented twice.
                endpoint.Socket.IncSeqnum();

                return endpoint;
            }
        }
    }
}