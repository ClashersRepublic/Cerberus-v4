namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core
{
    using System;
    using System.Runtime.InteropServices;
    using AsyncIO;
    using RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports;
    using JetBrains.Annotations;

    internal class MonitorEvent
    {
        private const int ValueInteger = 1;
        private const int ValueChannel = 2;

        private readonly int m_flag;

        private static readonly int s_sizeOfIntPtr;

        static MonitorEvent()
        {
            MonitorEvent.s_sizeOfIntPtr = Marshal.SizeOf(typeof(IntPtr));

            if (MonitorEvent.s_sizeOfIntPtr > 4)
            {
                MonitorEvent.s_sizeOfIntPtr = 8;
            }
        }

        public MonitorEvent(SocketEvents monitorEvent, [NotNull] string addr, ErrorCode arg)
            : this(monitorEvent, addr, (int) arg)
        {
        }

        public MonitorEvent(SocketEvents monitorEvent, [NotNull] string addr, int arg)
            : this(monitorEvent, addr, (object) arg)
        {
        }

        public MonitorEvent(SocketEvents monitorEvent, [NotNull] string addr, [NotNull] AsyncSocket arg)
            : this(monitorEvent, addr, (object) arg)
        {
        }

        private MonitorEvent(SocketEvents monitorEvent, [NotNull] string addr, [NotNull] object arg)
        {
            this.Event = monitorEvent;
            this.Addr = addr;
            this.Arg = arg;

            if (arg is int)
            {
                this.m_flag = MonitorEvent.ValueInteger;
            }
            else if (arg is AsyncSocket)
            {
                this.m_flag = MonitorEvent.ValueChannel;
            }
            else
            {
                this.m_flag = 0;
            }
        }

        [NotNull]
        public string Addr { get; }

        [NotNull]
        [CanBeNull]
        public object Arg { get; }

        public SocketEvents Event { get; }

        public void Write([NotNull] SocketBase s)
        {
            int size = 4 + 1 + (this.Addr?.Length ?? 0) + 1; // event + len(addr) + addr + flag

            if (this.m_flag == MonitorEvent.ValueInteger)
            {
                size += 4;
            }
            else if (this.m_flag == MonitorEvent.ValueChannel)
            {
                size += MonitorEvent.s_sizeOfIntPtr;
            }

            int pos = 0;

            ByteArraySegment buffer = new byte[size];
            buffer.PutInteger(Endianness.Little, (int) this.Event, pos);
            pos += 4;

            if (this.Addr != null)
            {
                buffer[pos++] = (byte) this.Addr.Length;

                // was not here originally

                buffer.PutString(this.Addr, pos);
                pos += this.Addr.Length;
            }
            else
            {
                buffer[pos++] = 0;
            }

            buffer[pos++] = (byte) this.m_flag;
            if (this.m_flag == MonitorEvent.ValueInteger)
            {
                buffer.PutInteger(Endianness.Little, (int) this.Arg, pos);
            }
            else if (this.m_flag == MonitorEvent.ValueChannel)
            {
                GCHandle handle = GCHandle.Alloc(this.Arg, GCHandleType.Weak);

                if (MonitorEvent.s_sizeOfIntPtr == 4)
                {
                    buffer.PutInteger(Endianness.Little, GCHandle.ToIntPtr(handle).ToInt32(), pos);
                }
                else
                {
                    buffer.PutLong(Endianness.Little, GCHandle.ToIntPtr(handle).ToInt64(), pos);
                }
            }

            Msg msg = new Msg();
            msg.InitGC((byte[]) buffer, buffer.Size);
            // An infinite timeout here can cause the IO thread to hang
            // see https://github.com/zeromq/netmq/issues/539
            s.TrySend(ref msg, TimeSpan.Zero, false);
        }

        [NotNull]
        public static MonitorEvent Read([NotNull] SocketBase s)
        {
            Msg msg = new Msg();
            msg.InitEmpty();

            s.TryRecv(ref msg, SendReceiveConstants.InfiniteTimeout);

            int pos = msg.Offset;
            ByteArraySegment data = msg.Data;

            SocketEvents @event = (SocketEvents) data.GetInteger(Endianness.Little, pos);
            pos += 4;
            int len = data[pos++];
            string addr = data.GetString(len, pos);
            pos += len;
            int flag = data[pos++];
            object arg = null;

            if (flag == MonitorEvent.ValueInteger)
            {
                arg = data.GetInteger(Endianness.Little, pos);
            }
            else if (flag == MonitorEvent.ValueChannel)
            {
                IntPtr value = MonitorEvent.s_sizeOfIntPtr == 4
                    ? new IntPtr(data.GetInteger(Endianness.Little, pos))
                    : new IntPtr(data.GetLong(Endianness.Little, pos));

                GCHandle handle = GCHandle.FromIntPtr(value);
                AsyncSocket socket = null;

                if (handle.IsAllocated)
                {
                    socket = handle.Target as AsyncSocket;
                }

                handle.Free();

                arg = socket;
            }

            return new MonitorEvent(@event, addr, arg);
        }
    }
}