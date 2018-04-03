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

// Helper base class for decoders that know the amount of data to read
//  in advance at any moment. Knowing the amount in advance is a property
//  of the protocol used. 0MQ framing protocol is based size-prefixed
//  paradigm, which qualifies it to be parsed by this class.
//  On the other hand, XML-based transports (like XMPP or SOAP) don't allow
//  for knowing the size of data to read in advance and should use different
//  decoding algorithms.
//
//  This class , the state machine that parses the incoming buffer.
//  Derived class should implement individual state machine actions.

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    internal class V1Decoder : DecoderBase
    {
        private const int OneByteSizeReadyState = 0;
        private const int EightByteSizeReadyState = 1;
        private const int FlagsReadyState = 2;
        private const int MessageReadyState = 3;

        private readonly ByteArraySegment m_tmpbuf;

        private Msg m_inProgress;

        /// <summary>
        ///     The maximum message-size. If this is -1 then there is no maximum.
        /// </summary>
        private readonly long m_maxMessageSize;

        private IMsgSink m_msgSink;

        /// <summary>
        ///     Create a new V1Decoder with the given buffer-size, maximum-message-size and Endian-ness.
        /// </summary>
        /// <param name="bufsize">the buffer-size to give the contained buffer</param>
        /// <param name="maxMessageSize">the maximum message size. -1 indicates no limit.</param>
        /// <param name="endian">the Endianness to specify for it - either Big or Little</param>
        public V1Decoder(int bufsize, long maxMessageSize, Endianness endian)
            : base(bufsize, endian)
        {
            this.m_maxMessageSize = maxMessageSize;
            this.m_tmpbuf = new ByteArraySegment(new byte[8]);

            // At the beginning, read one byte and go to one_byte_size_ready state.
            this.NextStep(this.m_tmpbuf, 1, V1Decoder.OneByteSizeReadyState);

            this.m_inProgress = new Msg();
            this.m_inProgress.InitEmpty();
        }

        /// <summary>
        ///     Set the receiver of decoded messages.
        /// </summary>
        public override void SetMsgSink(IMsgSink msgSink)
        {
            this.m_msgSink = msgSink;
        }

        protected override bool Next()
        {
            switch (this.State)
            {
                case V1Decoder.OneByteSizeReadyState:
                    return this.OneByteSizeReady();
                case V1Decoder.EightByteSizeReadyState:
                    return this.EightByteSizeReady();
                case V1Decoder.FlagsReadyState:
                    return this.FlagsReady();
                case V1Decoder.MessageReadyState:
                    return this.MessageReady();
                default:
                    return false;
            }
        }

        private bool OneByteSizeReady()
        {
            this.m_tmpbuf.Reset();

            // First byte of size is read. If it is 0xff read 8-byte size.
            // Otherwise allocate the buffer for message data and read the
            // message data into it.
            byte first = this.m_tmpbuf[0];
            if (first == 0xff)
            {
                this.NextStep(this.m_tmpbuf, 8, V1Decoder.EightByteSizeReadyState);
            }
            else
            {
                // There has to be at least one byte (the flags) in the message).
                if (first == 0)
                {
                    this.DecodingError();
                    return false;
                }

                // in_progress is initialised at this point so in theory we should
                // close it before calling zmq_msg_init_size, however, it's a 0-byte
                // message and thus we can treat it as uninitialised...
                if (this.m_maxMessageSize >= 0 && first - 1 > this.m_maxMessageSize)
                {
                    this.DecodingError();
                    return false;
                }

                this.m_inProgress.InitPool(first - 1);

                this.NextStep(this.m_tmpbuf, 1, V1Decoder.FlagsReadyState);
            }

            return true;
        }

        private bool EightByteSizeReady()
        {
            this.m_tmpbuf.Reset();

            //  8-byte payload length is read. Allocate the buffer
            // for message body and read the message data into it.
            long payloadLength = this.m_tmpbuf.GetLong(this.Endian, 0);

            // There has to be at least one byte (the flags) in the message).
            if (payloadLength == 0)
            {
                this.DecodingError();
                return false;
            }

            // Message size must not exceed the maximum allowed size.
            if (this.m_maxMessageSize >= 0 && payloadLength - 1 > this.m_maxMessageSize)
            {
                this.DecodingError();
                return false;
            }

            // Message size must fit within range of size_t data type.
            if (payloadLength - 1 > int.MaxValue)
            {
                this.DecodingError();
                return false;
            }

            int msgSize = (int) (payloadLength - 1);
            // in_progress is initialised at this point so in theory we should
            // close it before calling init_size, however, it's a 0-byte
            // message and thus we can treat it as uninitialised...
            this.m_inProgress.InitPool(msgSize);

            this.NextStep(this.m_tmpbuf, 1, V1Decoder.FlagsReadyState);

            return true;
        }

        private bool FlagsReady()
        {
            this.m_tmpbuf.Reset();

            // Store the flags from the wire into the message structure.

            int first = this.m_tmpbuf[0];

            this.m_inProgress.SetFlags((MsgFlags) first & MsgFlags.More);

            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, V1Decoder.MessageReadyState);

            return true;
        }

        private bool MessageReady()
        {
            this.m_tmpbuf.Reset();

            // Message is completely read. Push it further and start reading
            // new message. (in_progress is a 0-byte message after this point.)

            if (this.m_msgSink == null)
            {
                return false;
            }

            try
            {
                bool isMessagedPushed = this.m_msgSink.PushMsg(ref this.m_inProgress);

                if (isMessagedPushed)
                {
                    this.NextStep(this.m_tmpbuf, 1, V1Decoder.OneByteSizeReadyState);
                }

                return isMessagedPushed;
            }
            catch (NetMQException)
            {
                this.DecodingError();
                return false;
            }
        }
    }
}