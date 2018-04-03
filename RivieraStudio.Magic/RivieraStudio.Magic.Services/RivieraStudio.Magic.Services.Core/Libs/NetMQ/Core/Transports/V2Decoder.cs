namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    internal class V2Decoder : DecoderBase
    {
        private const int OneByteSizeReadyState = 0;
        private const int EightByteSizeReadyState = 1;
        private const int FlagsReadyState = 2;
        private const int MessageReadyState = 3;

        private readonly ByteArraySegment m_tmpbuf;
        private Msg m_inProgress;
        private IMsgSink m_msgSink;
        private readonly long m_maxmsgsize;
        private MsgFlags m_msgFlags;

        public V2Decoder(int bufsize, long maxmsgsize, IMsgSink session, Endianness endian)
            : base(bufsize, endian)
        {
            this.m_maxmsgsize = maxmsgsize;
            this.m_msgSink = session;

            this.m_tmpbuf = new byte[8];

            // At the beginning, read one byte and go to one_byte_size_ready state.
            this.NextStep(this.m_tmpbuf, 1, V2Decoder.FlagsReadyState);

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
                case V2Decoder.OneByteSizeReadyState:
                    return this.OneByteSizeReady();
                case V2Decoder.EightByteSizeReadyState:
                    return this.EightByteSizeReady();
                case V2Decoder.FlagsReadyState:
                    return this.FlagsReady();
                case V2Decoder.MessageReadyState:
                    return this.MessageReady();
                default:
                    return false;
            }
        }

        private bool OneByteSizeReady()
        {
            this.m_tmpbuf.Reset();

            // Message size must not exceed the maximum allowed size.
            if (this.m_maxmsgsize >= 0)
            {
                if (this.m_tmpbuf[0] > this.m_maxmsgsize)
                {
                    this.DecodingError();
                    return false;
                }
            }

            // in_progress is initialised at this point so in theory we should
            // close it before calling zmq_msg_init_size, however, it's a 0-byte
            // message and thus we can treat it as uninitialised...
            this.m_inProgress.InitPool(this.m_tmpbuf[0]);

            this.m_inProgress.SetFlags(this.m_msgFlags);
            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, V2Decoder.MessageReadyState);

            return true;
        }

        private bool EightByteSizeReady()
        {
            this.m_tmpbuf.Reset();

            // The payload size is encoded as 64-bit unsigned integer.
            // The most significant byte comes first.
            ulong msg_size = this.m_tmpbuf.GetUnsignedLong(this.Endian, 0);

            // Message size must not exceed the maximum allowed size.
            if (this.m_maxmsgsize >= 0)
            {
                if (msg_size > (ulong) this.m_maxmsgsize)
                {
                    this.DecodingError();
                    return false;
                }
            }

            // TODO: move this constant to a good place (0x7FFFFFC7)
            // Message size must fit within range of size_t data type.
            if (msg_size > 0x7FFFFFC7)
            {
                this.DecodingError();
                return false;
            }

            // in_progress is initialised at this point so in theory we should
            // close it before calling init_size, however, it's a 0-byte
            // message and thus we can treat it as uninitialised.
            this.m_inProgress.InitPool((int) msg_size);

            this.m_inProgress.SetFlags(this.m_msgFlags);
            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, V2Decoder.MessageReadyState);

            return true;
        }

        private bool FlagsReady()
        {
            this.m_tmpbuf.Reset();

            // Store the flags from the wire into the message structure.
            this.m_msgFlags = 0;
            int first = this.m_tmpbuf[0];
            if ((first & V2Protocol.MoreFlag) > 0)
            {
                this.m_msgFlags |= MsgFlags.More;
            }

            // The payload length is either one or eight bytes,
            // depending on whether the 'large' bit is set.
            if ((first & V2Protocol.LargeFlag) > 0)
            {
                this.NextStep(this.m_tmpbuf, 8, V2Decoder.EightByteSizeReadyState);
            }
            else
            {
                this.NextStep(this.m_tmpbuf, 1, V2Decoder.OneByteSizeReadyState);
            }

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
                    this.NextStep(this.m_tmpbuf, 1, V2Decoder.FlagsReadyState);
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