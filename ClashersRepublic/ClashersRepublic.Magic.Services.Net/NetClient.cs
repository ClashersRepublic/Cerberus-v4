namespace ClashersRepublic.Magic.Services.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;

    public class NetClient
    {
        private bool _connectState;
        private bool _connected;

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
            this._socket.NoDelay = true;
            
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
            this._connectState = false;

            if (socketAsyncEventArgs.SocketError == SocketError.Success)
            {
                this._connected = true;
                this.Wakeup();
            }

            socketAsyncEventArgs.Dispose();
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

            if (this._queue.TryDequeue(out byte[] firstPacket))
            {
                if (firstPacket.Length <= 0xffffff)
                {
                    byte[] nextPacket = new byte[tmp.Length + firstPacket.Length];
                    Array.Copy(firstPacket, 0, nextPacket, 0, firstPacket.Length);
                    Array.Copy(tmp, 0, nextPacket, firstPacket.Length, tmp.Length);
                    this._queue.Enqueue(nextPacket);
                }
                else
                {
                    this._queue.Enqueue(firstPacket);
                    this._queue.Enqueue(tmp);
                }
            }
            else
            {
                this._queue.Enqueue(tmp);
            }
        }

        /// <summary>
        ///     Sends all queue.
        /// </summary>
        public void Wakeup()
        {
            if (!this._socket.Connected || !this._connected)
            {
                if (!this._connectState)
                {
                    this.Init();
                }
            }
            else
            {
                int count = this._queue.Count;

                for (int i = 0; i < count; i++)
                {
                    if (this._queue.TryDequeue(out byte[] buffer))
                    {
                        SocketAsyncEventArgs sendEvent = new SocketAsyncEventArgs();

                        sendEvent.SetBuffer(buffer, 0, buffer.Length);
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
                            this._queue.Enqueue(buffer);
                        }
                    }
                }
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
                this._socket.Disconnect(true);

                this._connected = false;
            }

            socketAsyncEventArgs.Dispose();
        }
    }
}