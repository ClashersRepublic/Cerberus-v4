namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    /// <summary>
    ///     Encoder for 0MQ framing protocol. Converts messages into data stream.
    /// </summary>
    internal class V2Encoder : EncoderBase
    {
        private const int SizeReadyState = 0;
        private const int MessageReadyState = 1;

        private readonly ByteArraySegment m_tmpbuf = new byte[9];
        private Msg m_inProgress;

        private IMsgSource m_msgSource;

        public V2Encoder(int bufferSize, IMsgSource session, Endianness endian)
            : base(bufferSize, endian)
        {
            this.m_inProgress = new Msg();
            this.m_inProgress.InitEmpty();

            this.m_msgSource = session;

            // Write 0 bytes to the batch and go to message_ready state.
            this.NextStep(this.m_tmpbuf, 0, V2Encoder.MessageReadyState, true);
        }

        public override void SetMsgSource(IMsgSource msgSource)
        {
            this.m_msgSource = msgSource;
        }

        protected override bool Next()
        {
            switch (this.State)
            {
                case V2Encoder.SizeReadyState:
                    return this.SizeReady();
                case V2Encoder.MessageReadyState:
                    return this.MessageReady();
                default:
                    return false;
            }
        }

        private bool SizeReady()
        {
            // Write message body into the buffer.
            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, V2Encoder.MessageReadyState, !this.m_inProgress.HasMore);
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

            int protocolFlags = 0;
            if (this.m_inProgress.HasMore)
            {
                protocolFlags |= V2Protocol.MoreFlag;
            }

            if (this.m_inProgress.Size > 255)
            {
                protocolFlags |= V2Protocol.LargeFlag;
            }

            this.m_tmpbuf[0] = (byte) protocolFlags;

            // Encode the message length. For messages less then 256 bytes,
            // the length is encoded as 8-bit unsigned integer. For larger
            // messages, 64-bit unsigned integer in network byte order is used.
            int size = this.m_inProgress.Size;
            if (size > 255)
            {
                this.m_tmpbuf.PutLong(this.Endian, size, 1);
                this.NextStep(this.m_tmpbuf, 9, V2Encoder.SizeReadyState, false);
            }
            else
            {
                this.m_tmpbuf[1] = (byte) size;
                this.NextStep(this.m_tmpbuf, 2, V2Encoder.SizeReadyState, false);
            }

            return true;
        }
    }
}