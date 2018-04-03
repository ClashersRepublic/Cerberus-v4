namespace LineageSoft.Magic.Services.Core.Libs.NetMQ.Core.Utils
{
    using System.Threading;

    internal class Switch
    {
        private readonly object m_sync;
        private volatile bool m_status;

        public Switch(bool status)
        {
            this.m_sync = new object();
            this.m_status = status;
        }

        public bool Status
        {
            get
            {
                lock (this.m_sync)
                {
                    return this.m_status;
                }
            }
        }

        public void WaitForOff()
        {
            lock (this.m_sync)
            {
                // while the status is on
                while (this.m_status)
                {
                    Monitor.Wait(this.m_sync);
                }
            }
        }

        public void WaitForOn()
        {
            lock (this.m_sync)
            {
                // while the status is off
                while (!this.m_status)
                {
                    Monitor.Wait(this.m_sync);
                }
            }
        }

        public void SwitchOn()
        {
            lock (this.m_sync)
            {
                if (!this.m_status)
                {
                    this.m_status = true;
                    Monitor.PulseAll(this.m_sync);
                }
            }
        }

        public void SwitchOff()
        {
            lock (this.m_sync)
            {
                if (this.m_status)
                {
                    this.m_status = false;
                    Monitor.PulseAll(this.m_sync);
                }
            }
        }
    }
}