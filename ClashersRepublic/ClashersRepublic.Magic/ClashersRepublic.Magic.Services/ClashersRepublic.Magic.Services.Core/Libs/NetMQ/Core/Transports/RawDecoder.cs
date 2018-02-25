namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    using System.Diagnostics;

    internal class RawDecoder : DecoderBase
    {
        private readonly long m_maxMsgSize;

        private IMsgSink m_msgSink;
        private Msg m_inProgress;

        private const int RawMessageReadyState = 1;

        public RawDecoder(int bufferSize, long maxMsgSize, IMsgSink msgSink,
            Endianness endianness)
            : base(bufferSize, endianness)
        {
            this.m_msgSink = msgSink;
            this.m_maxMsgSize = maxMsgSize;
        }

        public override void SetMsgSink(IMsgSink msgSink)
        {
            this.m_msgSink = msgSink;
        }

        public override bool Stalled()
        {
            return false;
        }

        public override bool MessageReadySize(int msgSize)
        {
            this.m_inProgress = new Msg();
            this.m_inProgress.InitPool(msgSize);

            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, RawDecoder.RawMessageReadyState);

            return true;
        }

        protected override bool Next()
        {
            if (this.State == RawDecoder.RawMessageReadyState)
            {
                return this.RawMessageReady();
            }

            return false;
        }

        private bool RawMessageReady()
        {
            Debug.Assert(this.m_inProgress.Size != 0);

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
                    // NOTE: This is just to break out of process_buffer
                    // raw_message_ready should never get called in state machine w/o
                    // message_ready_size from stream_engine.    
                    this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset),
                        1, RawDecoder.RawMessageReadyState);
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