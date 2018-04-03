/*
    Copyright (c) 2009-2011 250bpm s.r.o.
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

namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core
{
    using LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Utils;
    using JetBrains.Annotations;

    internal sealed class IOThread : ZObject, IMailboxEvent
    {
        /// <summary>
        ///     I/O thread accesses incoming commands via this mailbox.
        /// </summary>
        private readonly IOThreadMailbox m_mailbox;

        /// <summary>
        ///     I/O multiplexing is performed using a poller object.
        /// </summary>
        private readonly Proactor m_proactor;

        #if DEBUG
        /// <summary>
        ///     This gets set to "iothread-" plus the thread-id.
        /// </summary>
        private readonly string m_name;
        #endif

        /// <summary>
        ///     Create a new IOThread object within the given context (Ctx) and thread.
        /// </summary>
        /// <param name="ctx">the Ctx (context) for this thread to live within</param>
        /// <param name="threadId">the integer thread-id for this new IOThread</param>
        public IOThread([NotNull] Ctx ctx, int threadId)
            : base(ctx, threadId)
        {
            string name = "iothread-" + threadId;
            this.m_proactor = new Proactor(name);
            this.m_mailbox = new IOThreadMailbox(name, this.m_proactor, this);

            #if DEBUG
            this.m_name = name;
            #endif
        }

        [NotNull]
        internal Proactor Proactor
        {
            get
            {
                return this.m_proactor;
            }
        }

        public void Start()
        {
            this.m_proactor.Start();
        }

        public void Destroy()
        {
            this.m_proactor.Destroy();
            this.m_mailbox.Close();
        }

        public void Stop()
        {
            this.SendStop();
        }

        [NotNull]
        public IMailbox Mailbox
        {
            get
            {
                return this.m_mailbox;
            }
        }

        public int Load
        {
            get
            {
                return this.m_proactor.Load;
            }
        }

        protected override void ProcessStop()
        {
            this.m_proactor.Stop();
        }

        public void Ready()
        {
            // Process all available commands.
            Command command;
            while (this.m_mailbox.TryRecv(out command))
            {
                command.Destination.ProcessCommand(command);
            }
        }

        #if DEBUG
        public override string ToString()
        {
            return base.ToString() + "[" + this.m_name + "]";
        }
        #endif
    }
}