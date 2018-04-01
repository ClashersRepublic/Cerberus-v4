namespace ClashersRepublic.Magic.Services.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Sockets;

    public class NetClient
    {
        private bool _connectState;
        private string _host;
        private int _port;

        private readonly Socket _socket;
        private readonly ConcurrentQueue<byte[]> _queue;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetClient"/> class.
        /// </summary>
        public NetClient(string host, int port)
        {
            this._host = host;
            this._port = port;

            this._queue = new ConcurrentQueue<byte[]>();
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            
            this.Init();
        }

        /// <summary>
        ///     Initializes the connection
        /// </summary>
        private void Init()
        {
            if (!string.IsNullOrEmpty(this._host))
            {
                if (this._port > 0 && this._port < ushort.MaxValue)
                {
                    this._connectState = true;

                    SocketAsyncEventArgs connectEvent = new SocketAsyncEventArgs();

                    connectEvent.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(this._host), this._port);
                    connectEvent.Completed += this.ConnectCompleted;

                    if (!this._socket.ConnectAsync(connectEvent))
                    {
                        this.ConnectCompleted(this, connectEvent);
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the connect event is completed.
        /// </summary>
        private void ConnectCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                socketAsyncEventArgs.Dispose();

                this._connectState = false;
                this.Wakeup();
            }
            else
            {
                if (!this._socket.ConnectAsync(socketAsyncEventArgs))
                {
                    this.ConnectCompleted(this, socketAsyncEventArgs);
                }
            }
        }

        /// <summary>
        ///     Sends the specified packet to the server.
        /// </summary>
        public void Send(byte[] packet, int length)
        {
            byte[] tmp = new byte[length + 4];

            tmp[3] = (byte) (length >> 24);
            tmp[2] = (byte) (length >> 16);
            tmp[1] = (byte) (length >> 8);
            tmp[0] = (byte) (length);

            Array.Copy(packet, 0, tmp, 4, length);

            this._queue.Enqueue(tmp);

            if (!this._socket.Connected)
            {
                if (!this._connectState)
                {
                    this.Init();
                }

                return;
            }

            this.Wakeup();
        }

        /// <summary>
        ///     Sends all queue.
        /// </summary>
        internal void Wakeup()
        {
            int count = this._queue.Count;
            int offset = 0;

            byte[] packet = new byte[2048 * count];

            while (this._queue.TryDequeue(out byte[] buffer))
            {
                int length = buffer.Length;

                if (offset + length > packet.Length)
                {
                    byte[] tmp = packet;
                    packet = new byte[(tmp.Length + buffer.Length) << 1];
                    Array.Copy(packet, tmp, offset);
                }

                Array.Copy(buffer, 0, packet, offset, length);
                offset += length;
            }

            SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();

            sendEvent.SetBuffer(packet, 0, offset);
            sendEvent.Completed += this.SendCompleted;

            try
            {
                if (!this._socket.SendAsync(sendEvent))
                {
                    this.SendCompleted(null, sendEvent);
                }
            }
            catch (Exception)
            {
                this._queue.Enqueue(packet);
            }
        }

        /// <summary>
        ///     Called when the send event is completed.
        /// </summary>
        private void SendCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.SocketError != SocketError.Success)
            {
                this._queue.Enqueue(socketAsyncEventArgs.Buffer);
            }

            socketAsyncEventArgs.Dispose();
        }
    }
}