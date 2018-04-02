namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Core.Transports
{
    using JetBrains.Annotations;

    internal class RawEncoder : EncoderBase
    {
        private IMsgSource m_msgSource;
        private Msg m_inProgress;

        private const int RawMessageSizeReadyState = 1;
        private const int RawMessageReadyState = 2;

        public RawEncoder(int bufferSize, [NotNull] IMsgSource msgSource, Endianness endianness) :
            base(bufferSize, endianness)
        {
            this.m_msgSource = msgSource;

            this.m_inProgress = new Msg();
            this.m_inProgress.InitEmpty();

            this.NextStep(null, 0, RawEncoder.RawMessageReadyState, true);
        }

        public override void SetMsgSource(IMsgSource msgSource)
        {
            this.m_msgSource = msgSource;
        }

        protected override bool Next()
        {
            switch (this.State)
            {
                case RawEncoder.RawMessageSizeReadyState:
                    return this.RawMessageSizeReady();
                case RawEncoder.RawMessageReadyState:
                    return this.RawMessageReady();
                default:
                    return false;
            }
        }

        private bool RawMessageSizeReady()
        {
            // Write message body into the buffer.
            this.NextStep(new ByteArraySegment(this.m_inProgress.Data, this.m_inProgress.Offset), this.m_inProgress.Size, RawEncoder.RawMessageReadyState, !this.m_inProgress.HasMore);
            return true;
        }

        private bool RawMessageReady()
        {
            // Destroy content of the old message.
            this.m_inProgress.Close();

            // Read new message. If there is none, return false.
            // Note that new state is set only if write is successful. That way
            // unsuccessful write will cause retry on the next state machine
            // invocation.
            if (this.m_msgSource == null)
            {
                return false;
            }

            bool result = this.m_msgSource.PullMsg(ref this.m_inProgress);

            if (!result)
            {
                this.m_inProgress.InitEmpty();
                return false;
            }

            this.m_inProgress.ResetFlags(MsgFlags.Shared | MsgFlags.More | MsgFlags.Identity);

            this.NextStep(null, 0, RawEncoder.RawMessageSizeReadyState, true);

            return true;
        }
    }
}