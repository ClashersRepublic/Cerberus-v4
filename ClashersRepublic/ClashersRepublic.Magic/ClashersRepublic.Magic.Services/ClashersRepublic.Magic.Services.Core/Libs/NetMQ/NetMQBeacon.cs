namespace ClashersRepublic.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Sockets;
    using JetBrains.Annotations;

    /// <summary>
    ///     A NetMQBeaconEventArgs is an EventArgs that provides a property that holds a NetMQBeacon.
    /// </summary>
    public class NetMQBeaconEventArgs : EventArgs
    {
        /// <summary>
        ///     Create a new NetMQBeaconEventArgs object containing the given NetMQBeacon.
        /// </summary>
        /// <param name="beacon">the NetMQBeacon object to hold a reference to</param>
        public NetMQBeaconEventArgs([NotNull] NetMQBeacon beacon)
        {
            this.Beacon = beacon;
        }

        /// <summary>
        ///     Get the NetMQBeacon object that this holds.
        /// </summary>
        [NotNull]
        public NetMQBeacon Beacon { get; }
    }

    /// <summary>
    /// </summary>
    public sealed class NetMQBeacon : IDisposable, ISocketPollable
    {
        /// <summary>
        /// </summary>
        public const int UdpFrameMax = 255;

        private const string ConfigureCommand = "CONFIGURE";
        private const string PublishCommand = "PUBLISH";
        private const string SilenceCommand = "SILENCE";
        private const string SubscribeCommand = "SUBSCRIBE";
        private const string UnsubscribeCommand = "UNSUBSCRIBE";

        #region Nested class: Shim

        private sealed class Shim : IShimHandler
        {
            private NetMQSocket m_pipe;
            private Socket m_udpSocket;
            private int m_udpPort;

            private EndPoint m_broadcastAddress;

            private NetMQFrame m_transmit;
            private NetMQFrame m_filter;
            private NetMQTimer m_pingTimer;
            private NetMQPoller m_poller;

            private void Configure([NotNull] string interfaceName, int port)
            {
                // In case the beacon was configured twice
                if (this.m_udpSocket != null)
                {
                    this.m_poller.Remove(this.m_udpSocket);

                    #if NET35
                    m_udpSocket.Close();
                                        #else
                    this.m_udpSocket.Dispose();
                    #endif
                }

                this.m_udpPort = port;
                this.m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                this.m_poller.Add(this.m_udpSocket, this.OnUdpReady);

                // Ask operating system for broadcast permissions on socket
                this.m_udpSocket.EnableBroadcast = true;

                // Allow multiple owners to bind to socket; incoming
                // messages will replicate to each owner
                this.m_udpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                IPAddress bindTo = null;
                IPAddress sendTo = null;

                if (interfaceName == "*")
                {
                    bindTo = IPAddress.Any;
                    sendTo = IPAddress.Broadcast;
                }
                else if (interfaceName == "loopback")
                {
                    bindTo = IPAddress.Loopback;
                    sendTo = IPAddress.Broadcast;
                }
                else
                {
                    InterfaceCollection interfaceCollection = new InterfaceCollection();

                    IPAddress interfaceAddress = !string.IsNullOrEmpty(interfaceName)
                        ? IPAddress.Parse(interfaceName)
                        : null;

                    foreach (InterfaceItem @interface in interfaceCollection)
                    {
                        if (interfaceAddress == null || @interface.Address.Equals(interfaceAddress))
                        {
                            sendTo = @interface.BroadcastAddress;
                            bindTo = @interface.Address;
                            break;
                        }
                    }
                }

                if (bindTo != null)
                {
                    this.m_broadcastAddress = new IPEndPoint(sendTo, this.m_udpPort);
                    this.m_udpSocket.Bind(new IPEndPoint(bindTo, this.m_udpPort));
                }

                this.m_pipe.SendFrame(bindTo?.ToString() ?? "");
            }

            private static bool Compare([NotNull] NetMQFrame a, [NotNull] NetMQFrame b, int size)
            {
                for (int i = 0; i < size; i++)
                {
                    if (a.Buffer[i] != b.Buffer[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public void Run(PairSocket shim)
            {
                this.m_pipe = shim;

                shim.SignalOK();

                this.m_pipe.ReceiveReady += this.OnPipeReady;

                this.m_pingTimer = new NetMQTimer(TimeSpan.Zero);
                this.m_pingTimer.Elapsed += this.PingElapsed;
                this.m_pingTimer.Enable = false;

                using (this.m_poller = new NetMQPoller {this.m_pipe, this.m_pingTimer})
                {
                    this.m_poller.Run();
                }

                // the beacon might never been configured
                #if NET35
                m_udpSocket?.Close();
                                #else
                this.m_udpSocket?.Dispose();
                #endif
            }

            private void PingElapsed(object sender, NetMQTimerEventArgs e)
            {
                this.SendUdpFrame(this.m_transmit);
            }

            private void OnUdpReady(Socket socket)
            {
                string peerName;
                NetMQFrame frame = this.ReceiveUdpFrame(out peerName);

                // If filter is set, check that beacon matches it
                bool isValid = frame.MessageSize >= this.m_filter?.MessageSize && Shim.Compare(frame, this.m_filter, this.m_filter.MessageSize);

                // If valid, discard our own broadcasts, which UDP echoes to us
                if (isValid && this.m_transmit != null)
                {
                    if (frame.MessageSize == this.m_transmit.MessageSize && Shim.Compare(frame, this.m_transmit, this.m_transmit.MessageSize))
                    {
                        isValid = false;
                    }
                }

                // If still a valid beacon, send on to the API
                if (isValid)
                {
                    this.m_pipe.SendMoreFrame(peerName).SendFrame(frame.Buffer, frame.MessageSize);
                }
            }

            private void OnPipeReady(object sender, NetMQSocketEventArgs e)
            {
                NetMQMessage message = this.m_pipe.ReceiveMultipartMessage();

                string command = message.Pop().ConvertToString();

                switch (command)
                {
                    case NetMQBeacon.ConfigureCommand:
                        string interfaceName = message.Pop().ConvertToString();
                        int port = message.Pop().ConvertToInt32();
                        this.Configure(interfaceName, port);
                        break;
                    case NetMQBeacon.PublishCommand:
                        this.m_transmit = message.Pop();
                        this.m_pingTimer.Interval = message.Pop().ConvertToInt32();
                        this.m_pingTimer.Enable = true;
                        this.SendUdpFrame(this.m_transmit);
                        break;
                    case NetMQBeacon.SilenceCommand:
                        this.m_transmit = null;
                        this.m_pingTimer.Enable = false;
                        break;
                    case NetMQBeacon.SubscribeCommand:
                        this.m_filter = message.Pop();
                        break;
                    case NetMQBeacon.UnsubscribeCommand:
                        this.m_filter = null;
                        break;
                    case NetMQActor.EndShimMessage:
                        this.m_poller.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            private void SendUdpFrame(NetMQFrame frame)
            {
                this.m_udpSocket.SendTo(frame.Buffer, 0, frame.MessageSize, SocketFlags.None, this.m_broadcastAddress);
            }

            private NetMQFrame ReceiveUdpFrame(out string peerName)
            {
                byte[] buffer = new byte[NetMQBeacon.UdpFrameMax];
                EndPoint peer = new IPEndPoint(IPAddress.Any, 0);

                int bytesRead = this.m_udpSocket.ReceiveFrom(buffer, ref peer);
                peerName = peer.ToString();

                return new NetMQFrame(buffer, bytesRead);
            }
        }

        #endregion

        private readonly NetMQActor m_actor;

        private readonly EventDelegator<NetMQBeaconEventArgs> m_receiveEvent;

        [CanBeNull] private string m_hostName;

        /// <summary>
        ///     Create a new NetMQBeacon.
        /// </summary>
        public NetMQBeacon()
        {
            this.m_actor = NetMQActor.Create(new Shim());

            EventHandler<NetMQActorEventArgs> onReceive = (sender, e) => this.m_receiveEvent.Fire(this, new NetMQBeaconEventArgs(this));

            this.m_receiveEvent = new EventDelegator<NetMQBeaconEventArgs>(
                () => this.m_actor.ReceiveReady += onReceive,
                () => this.m_actor.ReceiveReady -= onReceive);
        }

        /// <summary>
        ///     Get the host name this beacon is bound to.
        /// </summary>
        /// <remarks>
        ///     This may involve a reverse DNS lookup which can take a second or two.
        ///     <para />
        ///     An empty string is returned if:
        ///     <list type="bullet">
        ///         <item>the beacon is not bound,</item>
        ///         <item>the beacon is bound to all interfaces,</item>
        ///         <item>an error occurred during reverse DNS lookup.</item>
        ///     </list>
        /// </remarks>
        [CanBeNull]
        public string HostName
        {
            get
            {
                if (this.m_hostName != null)
                {
                    return this.m_hostName;
                }

                // create a copy for thread safety
                string boundTo = this.BoundTo;

                if (boundTo == null)
                {
                    return null;
                }

                if (IPAddress.Any.ToString() == boundTo || IPAddress.IPv6Any.ToString() == boundTo)
                {
                    return this.m_hostName = string.Empty;
                }

                try
                {
                    #if NETSTANDARD1_3
                    return m_hostName = Dns.GetHostEntryAsync(boundTo).Result.HostName;
                                        #else
                    return this.m_hostName = Dns.GetHostEntry(boundTo).HostName;
                    #endif
                }
                catch
                {
                    return this.m_hostName = string.Empty;
                }
            }
        }

        /// <summary>
        ///     Get the IP address this beacon is bound to.
        /// </summary>
        [CanBeNull]
        public string BoundTo { get; private set; }

        /// <summary>
        ///     Get the socket of the contained actor.
        /// </summary>
        NetMQSocket ISocketPollable.Socket
        {
            get
            {
                return ((ISocketPollable) this.m_actor).Socket;
            }
        }

        /// <summary>
        ///     This event occurs when at least one message may be received from the socket without blocking.
        /// </summary>
        public event EventHandler<NetMQBeaconEventArgs> ReceiveReady
        {
            add
            {
                this.m_receiveEvent.Event += value;
            }
            remove
            {
                this.m_receiveEvent.Event -= value;
            }
        }

        /// <summary>
        ///     Configure beacon for the specified port on all interfaces.
        /// </summary>
        /// <remarks>Blocks until the bind operation completes.</remarks>
        /// <param name="port">The UDP port to bind to.</param>
        public void ConfigureAllInterfaces(int port)
        {
            this.Configure(port, "*");
        }

        /// <summary>
        ///     Configure beacon for the specified port and, optionally, to a specific interface.
        /// </summary>
        /// <remarks>Blocks until the bind operation completes.</remarks>
        /// <param name="port">The UDP port to bind to.</param>
        /// <param name="interfaceName">
        ///     IP address of the interface to bind to. Pass empty string (the default value) to use the
        ///     default interface.
        /// </param>
        public void Configure(int port, [NotNull] string interfaceName = "")
        {
            NetMQMessage message = new NetMQMessage();
            message.Append(NetMQBeacon.ConfigureCommand);
            message.Append(interfaceName);
            message.Append(port);

            this.m_actor.SendMultipartMessage(message);

            this.BoundTo = this.m_actor.ReceiveFrameString();
            this.m_hostName = null;
        }

        /// <summary>
        ///     Publish beacon immediately and continue to publish when interval elapsed
        /// </summary>
        /// <param name="transmit">Beacon to transmit.</param>
        /// <param name="interval">Interval to transmit beacon</param>
        /// <param name="encoding">Encoding for <paramref name="transmit" />. Defaults to <see cref="Encoding.UTF8" />.</param>
        public void Publish([NotNull] string transmit, TimeSpan interval, Encoding encoding = null)
        {
            this.Publish((encoding ?? Encoding.UTF8).GetBytes(transmit), interval);
        }

        /// <summary>
        ///     Publish beacon immediately and continue to publish when interval elapsed
        /// </summary>
        /// <param name="transmit">Beacon to transmit</param>
        /// <param name="interval">Interval to transmit beacon</param>
        public void Publish([NotNull] byte[] transmit, TimeSpan interval)
        {
            NetMQMessage message = new NetMQMessage();
            message.Append(NetMQBeacon.PublishCommand);
            message.Append(transmit);
            message.Append((int) interval.TotalMilliseconds);

            this.m_actor.SendMultipartMessage(message);
        }

        /// <summary>
        ///     Publish beacon immediately and continue to publish every second
        /// </summary>
        /// <param name="transmit">Beacon to transmit</param>
        /// <param name="encoding">Encoding for <paramref name="transmit" />. Defaults to <see cref="Encoding.UTF8" />.</param>
        public void Publish([NotNull] string transmit, Encoding encoding = null)
        {
            this.Publish(transmit, TimeSpan.FromSeconds(1), encoding);
        }

        /// <summary>
        ///     Publish beacon immediately and continue to publish every second
        /// </summary>
        /// <param name="transmit">Beacon to transmit</param>
        public void Publish([NotNull] byte[] transmit)
        {
            this.Publish(transmit, TimeSpan.FromSeconds(1));
        }

        /// <summary>
        ///     Stop publishing beacons.
        /// </summary>
        public void Silence()
        {
            this.m_actor.SendFrame(NetMQBeacon.SilenceCommand);
        }

        /// <summary>
        ///     Subscribe to beacon messages that match the specified filter.
        /// </summary>
        /// <remarks>
        ///     Beacons must prefix-match with <paramref name="filter" />.
        ///     Any previous subscription is replaced by this one.
        /// </remarks>
        /// <param name="filter">Beacon will be filtered by this</param>
        public void Subscribe([NotNull] string filter)
        {
            this.m_actor.SendMoreFrame(NetMQBeacon.SubscribeCommand).SendFrame(filter);
        }

        /// <summary>
        ///     Unsubscribe to beacon messages
        /// </summary>
        public void Unsubscribe()
        {
            this.m_actor.SendFrame(NetMQBeacon.UnsubscribeCommand);
        }

        /// <summary>
        ///     Receives the next beacon message, blocking until it arrives.
        /// </summary>
        public BeaconMessage Receive()
        {
            string peerName = this.m_actor.ReceiveFrameString();
            byte[] bytes = this.m_actor.ReceiveFrameBytes();

            return new BeaconMessage(bytes, peerName);
        }

        /// <summary>
        ///     Receives the next beacon message if one is available before <paramref name="timeout" /> expires.
        /// </summary>
        /// <param name="timeout">The maximum amount of time to wait for the next beacon message.</param>
        /// <param name="message">The received beacon message.</param>
        /// <returns><c>true</c> if a beacon message was received, otherwise <c>false</c>.</returns>
        public bool TryReceive(TimeSpan timeout, out BeaconMessage message)
        {
            string peerName;
            if (!this.m_actor.TryReceiveFrameString(timeout, out peerName))
            {
                message = default(BeaconMessage);
                return false;
            }

            byte[] bytes = this.m_actor.ReceiveFrameBytes();

            message = new BeaconMessage(bytes, peerName);
            return true;
        }

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            this.m_actor.Dispose();
            this.m_receiveEvent.Dispose();
        }
    }

    /// <summary>
    ///     Contents of a message received from a beacon.
    /// </summary>
    public struct BeaconMessage
    {
        /// <summary>
        ///     THe beacon content as a byte array.
        /// </summary>
        public byte[] Bytes { get; }

        /// <summary>
        ///     The address of the peer that sent this message. Includes host name and port number.
        /// </summary>
        public string PeerAddress { get; }

        internal BeaconMessage(byte[] bytes, string peerAddress) : this()
        {
            this.Bytes = bytes;
            this.PeerAddress = peerAddress;
        }

        /// <summary>
        ///     The beacon content as a string.
        /// </summary>
        /// <remarks>Decoded using <see cref="Encoding.UTF8" />. Other encodings may be used with <see cref="Bytes" /> directly.</remarks>
        public string String
        {
            get
            {
                return Encoding.UTF8.GetString(this.Bytes);
            }
        }

        /// <summary>
        ///     The host name of the peer that sent this message.
        /// </summary>
        /// <remarks>This is simply the value of <see cref="PeerAddress" /> without the port number.</remarks>
        public string PeerHost
        {
            get
            {
                int i = this.PeerAddress.IndexOf(':');
                return i == -1 ? this.PeerAddress : this.PeerAddress.Substring(0, i);
            }
        }
    }
}