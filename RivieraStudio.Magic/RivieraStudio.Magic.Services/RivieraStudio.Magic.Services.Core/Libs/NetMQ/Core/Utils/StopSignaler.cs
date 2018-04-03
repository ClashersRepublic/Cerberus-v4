namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Utils
{
    using System;
    using RivieraStudio.Magic.Services.Core.Libs.NetMQ.Sockets;

    internal class StopSignaler : ISocketPollable, IDisposable
    {
        private readonly PairSocket m_writer;
        private readonly PairSocket m_reader;

        public StopSignaler()
        {
            PairSocket.CreateSocketPair(out this.m_writer, out this.m_reader);
            this.m_reader.ReceiveReady += delegate
            {
                this.m_reader.SkipFrame();
                this.IsStopRequested = true;
            };

            this.IsStopRequested = false;
        }

        public bool IsStopRequested { get; private set; }

        NetMQSocket ISocketPollable.Socket
        {
            get
            {
                return this.m_reader;
            }
        }

        public void Dispose()
        {
            this.m_reader.Dispose();
            this.m_writer.Dispose();
        }

        public void Reset()
        {
            this.IsStopRequested = false;
        }

        public void RequestStop()
        {
            lock (this.m_writer)
            {
                this.m_writer.SignalOK();
            }
        }
    }
}