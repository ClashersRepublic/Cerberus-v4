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

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    internal class V1Encoder : EncoderBase
    {
        private const int SizeReadyState = 0;
        private const int MessageReadyState = 1;

        private readonly ByteArraySegment m_tmpbuf = new byte[10];
        private Msg m_inProgress;

        private IMsgSource m_msgSource;

        public V1Encoder(int bufferSize, Endianness endian)
            : base(bufferSize, endian)
        {
            this.m_inProgress = new Msg();
            this.m_inProgress.InitEmpty();

            // Write 0 bytes to the batch and go to message_ready state.
            this.NextStep(this.m_tmpbuf, 0, V1Encoder.MessageReadyState, true);
        }

        public override void SetMsgSource(IMsgSource msgSource)
        {
            this.m_msgSource = msgSource;
        }

        protected override bool Next()
        {
            switch (this.State)
            {
                case V1Encoder.SizeReadyState:
                    return this.SizeReady();
                case V1Encoder.MessageReadyState:
                    return this.MessageReady();
                default:
                    return false;
            }
        }

        private bool SizeReady()
        {
            // Write message body into the buffer.
            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, V1Encoder.MessageReadyState, !this.m_inProgress.HasMore);
            return true;
        }

        private bool MessageReady()
        {
            // Release the content of the old message.
            this.m_inProgress.Close();

            this.m_tmpbuf.Reset();

            // Read new message. If there is none, return false.
            // Note that new state is set only if write is successful. That way
            // unsuccessful write will cause retry on the next state machine
            // invocation.

            if (this.m_msgSource == null)
            {
                this.m_inProgress.InitEmpty();
                return false;
            }

            bool messagedPulled = this.m_msgSource.PullMsg(ref this.m_inProgress);
            if (!messagedPulled)
            {
                this.m_inProgress.InitEmpty();
                return false;
            }

            // Get the message size.
            int size = this.m_inProgress.Size;

            // Account for the 'flags' byte.
            size++;

            // For messages less than 255 bytes long, write one byte of message size.
            // For longer messages write 0xff escape character followed by 8-byte
            // message size. In both cases 'flags' field follows.

            if (size < 255)
            {
                this.m_tmpbuf[0] = (byte) size;
                this.m_tmpbuf[1] = (byte) (this.m_inProgress.Flags & MsgFlags.More);
                this.NextStep(this.m_tmpbuf, 2, V1Encoder.SizeReadyState, false);
            }
            else
            {
                this.m_tmpbuf[0] = 0xff;
                this.m_tmpbuf.PutLong(this.Endian, size, 1);
                this.m_tmpbuf[9] = (byte) (this.m_inProgress.Flags & MsgFlags.More);
                this.NextStep(this.m_tmpbuf, 10, V1Encoder.SizeReadyState, false);
            }

            return true;
        }
    }
}