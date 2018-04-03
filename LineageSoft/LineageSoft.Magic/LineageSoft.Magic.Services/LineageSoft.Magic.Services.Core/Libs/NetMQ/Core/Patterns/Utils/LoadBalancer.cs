/*
    Copyright (c) 2010-2011 250bpm s.r.o.
    Copyright (c) 2007-2009 iMatix Corporation
    Copyright (c) 2011 VMware, Inc.
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

namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Patterns.Utils
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;

    internal class LoadBalancer
    {
        /// <summary>
        ///     List of outbound pipes.
        /// </summary>
        private readonly List<Pipe> m_pipes = new List<Pipe>();

        /// <summary>
        ///     Number of active pipes. All the active pipes are located at the
        ///     beginning of the pipes array.
        /// </summary>
        private int m_active;

        /// <summary>
        ///     Points to the last pipe that the most recent message was sent to.
        /// </summary>
        private int m_current;

        /// <summary>
        ///     True if last we are in the middle of a multipart message.
        /// </summary>
        private bool m_more;

        /// <summary>
        ///     True if we are dropping current message.
        /// </summary>
        private bool m_dropping;

        public void Attach([NotNull] Pipe pipe)
        {
            this.m_pipes.Add(pipe);
            this.Activated(pipe);
        }

        /// <summary>
        ///     This gets called by ProcessPipeTermAck or XTerminated to respond to the termination of the given pipe.
        /// </summary>
        /// <param name="pipe">the pipe that was terminated</param>
        public void Terminated([NotNull] Pipe pipe)
        {
            int index = this.m_pipes.IndexOf(pipe);

            // If we are in the middle of multipart message and current pipe
            // have disconnected, we have to drop the remainder of the message.
            if (index == this.m_current && this.m_more)
            {
                this.m_dropping = true;
            }

            // Remove the pipe from the list; adjust number of active pipes
            // accordingly.
            if (index < this.m_active)
            {
                this.m_active--;
                this.m_pipes.Swap(index, this.m_active);
                if (this.m_current == this.m_active)
                {
                    this.m_current = 0;
                }
            }

            this.m_pipes.Remove(pipe);
        }

        public void Activated([NotNull] Pipe pipe)
        {
            // Move the pipe to the list of active pipes.
            this.m_pipes.Swap(this.m_pipes.IndexOf(pipe), this.m_active);
            this.m_active++;
        }

        public bool Send(ref Msg msg)
        {
            // Drop the message if required. If we are at the end of the message
            // switch back to non-dropping mode.
            if (this.m_dropping)
            {
                this.m_more = msg.HasMore;
                this.m_dropping = this.m_more;

                msg.Close();
                msg.InitEmpty();
                return true;
            }

            while (this.m_active > 0)
            {
                if (this.m_pipes[this.m_current].Write(ref msg))
                {
                    break;
                }

                Debug.Assert(!this.m_more);
                this.m_active--;
                if (this.m_current < this.m_active)
                {
                    this.m_pipes.Swap(this.m_current, this.m_active);
                }
                else
                {
                    this.m_current = 0;
                }
            }

            // If there are no pipes we cannot send the message.
            if (this.m_active == 0)
            {
                return false;
            }

            // If it's part of the message we can flush it downstream and
            // continue round-robinning (load balance).
            this.m_more = msg.HasMore;
            if (!this.m_more)
            {
                this.m_pipes[this.m_current].Flush();
                if (this.m_active > 1)
                {
                    this.m_current = (this.m_current + 1) % this.m_active;
                }
            }

            // Detach the message from the data buffer.
            msg.InitEmpty();

            return true;
        }

        public bool HasOut()
        {
            // If one part of the message was already written we can definitely
            // write the rest of the message.
            if (this.m_more)
            {
                return true;
            }

            while (this.m_active > 0)
            {
                // Check whether a pipe has room for another message.
                if (this.m_pipes[this.m_current].CheckWrite())
                {
                    return true;
                }

                // Deactivate the pipe.
                this.m_active--;
                this.m_pipes.Swap(this.m_current, this.m_active);
                if (this.m_current == this.m_active)
                {
                    this.m_current = 0;
                }
            }

            return false;
        }
    }
}