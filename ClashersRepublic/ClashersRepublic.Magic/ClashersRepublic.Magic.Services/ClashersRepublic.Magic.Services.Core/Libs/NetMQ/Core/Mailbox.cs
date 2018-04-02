/*
    Copyright (c) 2010-2011 250bpm s.r.o.
    Copyright (c) 2007-2009 iMatix Corporation
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

namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core
{
    using System.Diagnostics;
    using System.Net.Sockets;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Utils;
    using JetBrains.Annotations;

    internal interface IMailbox
    {
        void Send([NotNull] Command command);

        void Close();
    }

    internal interface IMailboxEvent
    {
        void Ready();
    }

    internal class IOThreadMailbox : IMailbox
    {
        [NotNull] private readonly Proactor m_proactor;

        [NotNull] private readonly IMailboxEvent m_mailboxEvent;

        [NotNull] private readonly YPipe<Command> m_commandPipe = new YPipe<Command>(Config.CommandPipeGranularity, "mailbox");

        /// <summary>
        ///     There's only one thread receiving from the mailbox, but there
        ///     is arbitrary number of threads sending. Given that ypipe requires
        ///     synchronised access on both of its endpoints, we have to synchronize
        ///     the sending side.
        /// </summary>
        [NotNull] private readonly object m_sync = new object();

        #if DEBUG
        /// <summary>Mailbox name. Only used for debugging.</summary>
        [NotNull] private readonly string m_name;
        #endif

        private bool m_disposed;

        public IOThreadMailbox([NotNull] string name, [NotNull] Proactor proactor, [NotNull] IMailboxEvent mailboxEvent)
        {
            this.m_proactor = proactor;
            this.m_mailboxEvent = mailboxEvent;

            // Get the pipe into passive state. That way, if the users starts by
            // polling on the associated file descriptor it will get woken up when
            // new command is posted.
            Command cmd;
            bool ok = this.m_commandPipe.TryRead(out cmd);
            Debug.Assert(!ok);

            #if DEBUG
            this.m_name = name;
            #endif
        }

        public void Send(Command command)
        {
            bool ok;
            lock (this.m_sync)
            {
                this.m_commandPipe.Write(ref command, false);
                ok = this.m_commandPipe.Flush();
            }

            if (!ok)
            {
                this.m_proactor.SignalMailbox(this);
            }
        }

        public bool TryRecv(out Command command)
        {
            return this.m_commandPipe.TryRead(out command);
        }

        public void RaiseEvent()
        {
            if (!this.m_disposed)
            {
                this.m_mailboxEvent.Ready();
            }
        }

        public void Close()
        {
            this.m_disposed = true;
        }

        #if DEBUG
        public override string ToString()
        {
            return base.ToString() + "[" + this.m_name + "]";
        }
        #endif
    }

    internal class Mailbox : IMailbox
    {
        /// <summary>
        ///     The pipe to store actual commands.
        /// </summary>
        private readonly YPipe<Command> m_commandPipe = new YPipe<Command>(Config.CommandPipeGranularity, "mailbox");

        /// <summary>
        ///     Signaler to pass signals from writer thread to reader thread.
        /// </summary>
        private readonly Signaler m_signaler = new Signaler();

        /// <summary>
        ///     There's only one thread receiving from the mailbox, but there
        ///     is an arbitrary number of threads sending. Given that <see cref="YPipe{T}" /> requires
        ///     synchronised access on both of its endpoints, we have to synchronize
        ///     the sending side.
        /// </summary>
        private readonly object m_sync = new object();

        /// <summary>
        ///     True if the underlying pipe is active, ie. when we are allowed to
        ///     read commands from it.
        /// </summary>
        private bool m_active;

        #if DEBUG
        /// <summary>Mailbox name. Only used for debugging.</summary>
        [NotNull] private readonly string m_name;
        #endif

        /// <summary>
        ///     Create a new Mailbox with the given name.
        /// </summary>
        /// <param name="name">the name to give this new Mailbox</param>
        public Mailbox([NotNull] string name)
        {
            // Get the pipe into passive state. That way, if the users starts by
            // polling on the associated file descriptor it will get woken up when
            // new command is posted.

            Command cmd;
            bool ok = this.m_commandPipe.TryRead(out cmd);

            Debug.Assert(!ok);

            this.m_active = false;

            #if DEBUG
            this.m_name = name;
            #endif
        }

        /// <summary>
        ///     Get the socket-handle contained by the Signaler.
        /// </summary>
        [NotNull]
        public Socket Handle
        {
            get
            {
                return this.m_signaler.Handle;
            }
        }

        /// <summary>
        ///     Send the given Command out across the command-pipe.
        /// </summary>
        /// <param name="cmd">the Command to send</param>
        public void Send(Command cmd)
        {
            bool ok;
            lock (this.m_sync)
            {
                this.m_commandPipe.Write(ref cmd, false);
                ok = this.m_commandPipe.Flush();
            }

            //if (LOG.isDebugEnabled())
            //    LOG.debug( "{} -> {} / {} {}", new Object[] { Thread.currentThread().getName(), cmd_, cmd_.arg , !ok});

            if (!ok)
            {
                this.m_signaler.Send();
            }
        }

        /// <summary>
        ///     Receive and return a Command from the command-pipe.
        /// </summary>
        /// <param name="timeout">how long to wait for a command (in milliseconds) before returning</param>
        /// <param name="command"></param>
        public bool TryRecv(int timeout, out Command command)
        {
            // Try to get the command straight away.
            if (this.m_active)
            {
                if (this.m_commandPipe.TryRead(out command))
                {
                    return true;
                }

                // If there are no more commands available, switch into passive state.
                this.m_active = false;
                this.m_signaler.Recv();
            }

            // Wait for signal from the command sender.
            if (!this.m_signaler.WaitEvent(timeout))
            {
                command = default(Command);
                return false;
            }

            // We've got the signal. Now we can switch into active state.
            this.m_active = true;

            // Get a command.
            bool ok = this.m_commandPipe.TryRead(out command);
            Debug.Assert(ok);
            return ok;
        }

        /// <summary>
        ///     Close the contained Signaler.
        /// </summary>
        public void Close()
        {
            this.m_signaler.Close();
        }

        #if DEBUG
        /// <summary>
        ///     Override ToString to provide the type-name, plus the Mailbox name within brackets.
        /// </summary>
        /// <returns>a string of the form Mailbox[name]</returns>
        public override string ToString()
        {
            return base.ToString() + "[" + this.m_name + "]";
        }
        #endif
    }
}