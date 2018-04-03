#if DEBUG
#endif

namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ.Core.Transports.Pgm
{
    using System;
    using System.Diagnostics;
    using System.Net.Sockets;
    using System.Text;
    using AsyncIO;
    using JetBrains.Annotations;

    /// <summary>
    ///     This enum-type denotes the type of Pragmatic General Multicast (PGM) socket.
    ///     Publisher, Receiver, or Listener.
    /// </summary>
    internal enum PgmSocketType
    {
        Publisher,
        Receiver,
        Listener
    }

//    internal struct RM_SEND_WINDOW
//    {
//        public uint RateKbitsPerSec; // Send rate
//        public uint WindowSizeInMSecs;
//        public uint WindowSizeInBytes;
//    }

    /// <summary>
    ///     A PgmSocket utilizes the Pragmatic General Multicast (PGM) multicast protocol, which is also referred to as
    ///     "reliable multicast".
    ///     This is only supported on Windows when Microsoft Message Queueing (MSMQ) is installed.
    ///     See RFC 3208.
    /// </summary>
    internal sealed class PgmSocket : IDisposable
    {
        public const int ProtocolTypeNumber = 113;
        public const ProtocolType PgmProtocolType = (ProtocolType) 113;
        public const SocketOptionLevel PgmLevel = (SocketOptionLevel) PgmSocket.ProtocolTypeNumber;

        public const int RmOptionsbase = 1000;

        /// <summary>
        ///     Set/Query rate (Kb/Sec) + window size (Kb and/or MSec) -- described by RM_SEND_WINDOW below
        /// </summary>
        public const SocketOptionName RM_RATE_WINDOW_SIZE = (SocketOptionName) (PgmSocket.RmOptionsbase + 1);

        /// <summary>
        ///     set IP multicast outgoing interface
        /// </summary>
        public const SocketOptionName RM_SET_SEND_IF = (SocketOptionName) (PgmSocket.RmOptionsbase + 7);

        /// <summary>
        ///     add IP multicast incoming interface
        /// </summary>
        public const SocketOptionName RM_ADD_RECEIVE_IF = (SocketOptionName) (PgmSocket.RmOptionsbase + 8);

//        /// <summary>
//        /// delete IP multicast incoming interface
//        /// </summary>
//        public const SocketOptionName RM_DEL_RECEIVE_IF = (SocketOptionName)(RmOptionsbase + 9);

        /// <summary>
        ///     Set the Time-To-Live (TTL) of the MCast packets -- (ULONG)
        /// </summary>
        public const SocketOptionName RM_SET_MCAST_TTL = (SocketOptionName) (PgmSocket.RmOptionsbase + 12);

        public const SocketOptionName EnableGigabitOption = (SocketOptionName) 1014;

        private readonly Options m_options;
        private readonly PgmSocketType m_pgmSocketType;
        private readonly PgmAddress m_pgmAddress;

        public PgmSocket([NotNull] Options options, PgmSocketType pgmSocketType, [NotNull] PgmAddress pgmAddress)
        {
            this.m_options = options;
            this.m_pgmSocketType = pgmSocketType;
            this.m_pgmAddress = pgmAddress;
        }

        /// <summary>
        ///     Perform initialization of this PgmSocket, including creating the socket handle.
        /// </summary>
        internal void Init()
        {
            #if DEBUG
            // Don't want to bloat the code with excessive debugging information, unless this is a DEBUG build.  jh
            try
            {
                #endif
                this.Handle = AsyncSocket.Create(AddressFamily.InterNetwork, SocketType.Rdm, PgmSocket.PgmProtocolType);
                #if DEBUG
            }
            catch (SocketException x)
            {
                string xMsg = $"SocketException with SocketErrorCode={x.SocketErrorCode}, Message={x.Message}, in PgmSocket.Init, within AsyncSocket.Create(AddressFamily.InterNetwork, SocketType.Rdm, PGM_PROTOCOL_TYPE), {this}";
                Debug.WriteLine(xMsg);
                // If running on Microsoft Windows, suggest to the developer that he may need to install MSMQ in order to get PGM socket support.

                #if NETSTANDARD1_3
                bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                                #else
                PlatformID p = Environment.OSVersion.Platform;
                bool isWindows = true;
                switch (p)
                {
                    case PlatformID.Win32NT:
                        break;
                    case PlatformID.Win32S:
                        break;
                    case PlatformID.Win32Windows:
                        break;
                    default:
                        isWindows = false;
                        break;
                }
                #endif
                if (isWindows)
                {
                    Debug.WriteLine("For Microsoft Windows, you may want to check to see whether you have installed MSMQ on this host, to get PGM socket support.");
                }

                throw new FaultException(x, xMsg);
            }
            #endif
            this.Handle.ExclusiveAddressUse = false;
            this.Handle.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        }

        internal void InitReceiver()
        {
            this.Handle = AsyncSocket.Create(AddressFamily.InterNetwork, SocketType.Rdm, PgmSocket.PgmProtocolType);
        }

        internal void InitOptions()
        {
            // Enable gigabit on the socket
            try
            {
                this.Handle.SetSocketOption(PgmSocket.PgmLevel, PgmSocket.EnableGigabitOption, BitConverter.GetBytes((uint) 1));
            }
            catch (Exception)
            {
                // If gigabit is not supported don't throw.
            }

            // set the receive buffer size for receiver and listener
            if (this.m_options.ReceiveBuffer > 0 && (this.m_pgmSocketType == PgmSocketType.Receiver || this.m_pgmSocketType == PgmSocketType.Listener))
            {
                this.Handle.ReceiveBufferSize = this.m_options.ReceiveBuffer;
            }

            // set the send buffer for the publisher
            if (this.m_options.SendBuffer > 0 && this.m_pgmSocketType == PgmSocketType.Publisher)
            {
                this.Handle.SendBufferSize = this.m_options.SendBuffer;
            }

            // set the receive interface on the listener and receiver
            if (this.m_pgmSocketType == PgmSocketType.Listener || this.m_pgmSocketType == PgmSocketType.Receiver)
            {
                if (this.m_pgmAddress.InterfaceAddress != null)
                {
                    this.Handle.SetSocketOption(PgmSocket.PgmLevel, PgmSocket.RM_ADD_RECEIVE_IF, this.m_pgmAddress.InterfaceAddress.GetAddressBytes());
                }
            }
            else if (this.m_pgmSocketType == PgmSocketType.Publisher)
            {
                // set multicast hops for the publisher
                this.Handle.SetSocketOption(PgmSocket.PgmLevel, PgmSocket.RM_SET_MCAST_TTL, this.m_options.MulticastHops);

                // set the publisher send interface
                if (this.m_pgmAddress.InterfaceAddress != null)
                {
                    this.Handle.SetSocketOption(PgmSocket.PgmLevel, PgmSocket.RM_SET_SEND_IF, this.m_pgmAddress.InterfaceAddress.GetAddressBytes());
                }

                // instead of using the struct _RM_SEND_WINDOW we are using byte array of size 12 (the size of the original struct and the size of three ints)
                // typedef struct _RM_SEND_WINDOW {
                // ULONG RateKbitsPerSec;
                // ULONG WindowSizeInMSecs;
                // ULONG WindowSizeInBytes;
                //} RM_SEND_WINDOW;
                byte[] sendWindow = new byte[12];

                // setting the rate of the transmission in Kilobits per second
                uint rate = (uint) this.m_options.Rate;
                Array.Copy(BitConverter.GetBytes(rate), 0, sendWindow, 0, 4);

                // setting the recovery interval
                uint sizeInMS = (uint) this.m_options.RecoveryIvl;
                Array.Copy(BitConverter.GetBytes(sizeInMS), 0, sendWindow, 4, 4);

                // we are not setting the size in bytes because it get filled automatically, if we want to set it we would just uncomment the following lines
                //uint sizeInBytes = (uint)((rate / 8.0) * sizeInMS);
                //Array.Copy(BitConverter.GetBytes(sizeInBytes), 0, sendWindow, 8, 4);

                this.Handle.SetSocketOption(PgmSocket.PgmLevel, PgmSocket.RM_RATE_WINDOW_SIZE, sendWindow);
            }
        }

        public AsyncSocket Handle { get; private set; }

        /// <summary>
        ///     Override the ToString method to produce a more descriptive, useful description.
        /// </summary>
        /// <returns>a useful description of this object's state</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("PgmSocket(pgmSocketType=");
            sb.Append(this.m_pgmSocketType);
            sb.Append(", pgmAddress=");
            sb.Append(this.m_pgmAddress);
            sb.Append(", m_options=");
            sb.Append(this.m_options).Append(")");
            return sb.ToString();
        }

        public void Dispose()
        {
            this.Handle.Dispose();
        }
    }
}