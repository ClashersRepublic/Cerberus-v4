/*
    Copyright (c) 2007-2012 iMatix Corporation
    Copyright (c) 2009-2011 250bpm s.r.o.
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

namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    using System;

    internal abstract class EncoderBase : IEncoder
    {
        /// <summary>
        ///     Where to get the data to write from.
        /// </summary>
        private ByteArraySegment m_writePos;

        /// <summary>
        ///     If true, first byte of the message is being written.
        /// </summary>
        private bool m_beginning;

        /// <summary>
        ///     How much data to write before the next step should be executed.
        /// </summary>
        private int m_toWrite;

        /// <summary>
        ///     The buffer for encoded data.
        /// </summary>
        private readonly byte[] m_buffer;

        /// <summary>
        ///     The size of the encoded-data buffer
        /// </summary>
        private readonly int m_bufferSize;

        /// <summary>
        ///     This flag indicates whether there has been an encoder error.
        /// </summary>
        private bool m_error;

        /// <summary>
        ///     Create a new EncoderBase with a buffer of the given size.
        /// </summary>
        /// <param name="bufferSize">how big of an internal buffer to allocate (in bytes)</param>
        /// <param name="endian">the <see cref="Endianness" /> to set this EncoderBase to</param>
        protected EncoderBase(int bufferSize, Endianness endian)
        {
            this.Endian = endian;
            this.m_bufferSize = bufferSize;
            this.m_buffer = new byte[bufferSize];
        }

        /// <summary>
        ///     Get the Endianness (Big or Little) that this EncoderBase uses.
        /// </summary>
        public Endianness Endian { get; }

        public abstract void SetMsgSource(IMsgSource msgSource);

        /// <summary>
        ///     This returns a batch of binary data. The data
        ///     are filled to a supplied buffer. If no buffer is supplied (data_
        ///     points to NULL) decoder object will provide buffer of its own.
        /// </summary>
        public void GetData(ref ByteArraySegment data, ref int size)
        {
            int offset = -1;

            this.GetData(ref data, ref size, ref offset);
        }

        public void GetData(ref ByteArraySegment data, ref int size, ref int offset)
        {
            ByteArraySegment buffer = data ?? new ByteArraySegment(this.m_buffer);
            int bufferSize = data == null ? this.m_bufferSize : size;

            int pos = 0;

            while (pos < bufferSize)
            {
                // If there are no more data to return, run the state machine.
                // If there are still no data, return what we already have
                // in the buffer.
                if (this.m_toWrite == 0)
                {
                    // If we are to encode the beginning of a new message,
                    // adjust the message offset.

                    if (this.m_beginning)
                    {
                        if (offset == -1)
                        {
                            offset = pos;
                        }
                    }

                    if (!this.Next())
                    {
                        break;
                    }
                }

                // If there are no data in the buffer yet and we are able to
                // fill whole buffer in a single go, let's use zero-copy.
                // There's no disadvantage to it as we cannot stuck multiple
                // messages into the buffer anyway. Note that subsequent
                // write(s) are non-blocking, thus each single write writes
                // at most SO_SNDBUF bytes at once not depending on how large
                // is the chunk returned from here.
                // As a consequence, large messages being sent won't block
                // other engines running in the same I/O thread for excessive
                // amounts of time.
                if (pos == 0 && data == null && this.m_toWrite >= bufferSize)
                {
                    data = this.m_writePos;
                    size = this.m_toWrite;

                    this.m_writePos = null;
                    this.m_toWrite = 0;
                    return;
                }

                // Copy data to the buffer. If the buffer is full, return.
                int toCopy = Math.Min(this.m_toWrite, bufferSize - pos);

                if (toCopy != 0)
                {
                    this.m_writePos.CopyTo(0, buffer, pos, toCopy);
                    pos += toCopy;
                    this.m_writePos.AdvanceOffset(toCopy);
                    this.m_toWrite -= toCopy;
                }
            }

            data = buffer;
            size = pos;
        }

        protected int State { get; private set; }

        /// <summary>
        ///     Set a flag that indicates that there has been an encoding-error.
        /// </summary>
        protected void EncodingError()
        {
            this.m_error = true;
        }

        /// <summary>
        ///     Return true if there has been an encoding error.
        /// </summary>
        /// <returns>the state of the error-flag</returns>
        public bool IsError()
        {
            return this.m_error;
        }

        protected abstract bool Next();

        //protected void next_step (Msg msg_, int state_, bool beginning_) {
        //    if (msg_ == null)
        //        next_step((ByteBuffer) null, 0, state_, beginning_);
        //    else
        //        next_step(msg_.data(), msg_.size(), state_, beginning_);
        //}

        protected void NextStep(ByteArraySegment writePos, int toWrite, int state, bool beginning)
        {
            this.m_writePos = writePos;
            this.m_toWrite = toWrite;
            this.State = state;
            this.m_beginning = beginning;
        }

        //protected void next_step (byte[] buf_, int to_write_,
        //        int next_, bool beginning_)
        //{
        //    write_buf = null;
        //    write_array = buf_;
        //    write_pos = 0;
        //    to_write = to_write_;
        //    next = next_;
        //    beginning = beginning_;
        //}
    }
}