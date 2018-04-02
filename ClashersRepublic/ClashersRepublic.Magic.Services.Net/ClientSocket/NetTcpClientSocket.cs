namespace ClashersRepublic.Magic.Services.Net.ClientSocket
{
    using System;
    using System.Collections.Concurrent;
    using System.Net;
    using System.Net.Sockets;

    public class NetTcpClientSocket
    {
        protected readonly Socket _clientSocket;
        protected readonly IPEndPoint _remoteEndPoint;
        protected readonly ConcurrentQueue<SocketAsyncEventArgs> _sendQueue;
        protected readonly ConcurrentQueue<SocketAsyncEventArgs> _sendFailedQueue;

        protected bool _connected;
        protected bool _waitConnectCallback;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetTcpClientSocket"/> class.
        /// </summary>
        public NetTcpClientSocket(string host, int port)
        {
            this._clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._clientSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

            this._remoteEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
            this._sendQueue = new ConcurrentQueue<SocketAsyncEventArgs>();
            this._sendFailedQueue = new ConcurrentQueue<SocketAsyncEventArgs>();
        }

        /// <summary>
        ///     Connects the socket to the host.
        /// </summary>
        private void ConnectToServer()
        {
            if (!this._waitConnectCallback)
            {
                this._waitConnectCallback = true;

                SocketAsyncEventArgs connectionEventArgs = new SocketAsyncEventArgs();

                connectionEventArgs.RemoteEndPoint = this._remoteEndPoint;
                connectionEventArgs.Completed += this.ConnectCompleted;

                if (!this._clientSocket.ConnectAsync(connectionEventArgs))
                {
                    this.ConnectCompleted(this, connectionEventArgs);
                }
            }
        }

        /// <summary>
        ///     Called when the connect event is completed.
        /// </summary>
        protected void ConnectCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            this._waitConnectCallback = false;
            this._connected = socketAsyncEventArgs.SocketError == SocketError.Success;
            
            socketAsyncEventArgs.Dispose();
        }

        /// <summary>
        ///     Sends the buffer to the remote server.
        /// </summary>
        public void Send(byte[] buffer, int length)
        {
            SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();

            sendEventArgs.SetBuffer(buffer, 0, length);
            sendEventArgs.Completed += this.SendCompleted;

            this._sendQueue.Enqueue(sendEventArgs);
        }

        /// <summary>
        ///     Called when the socket is wakeup.
        /// </summary>
        public void Wakeup()
        {
            if (this._connected)
            {
                do
                {
                    if (!this._sendFailedQueue.TryDequeue(out SocketAsyncEventArgs sendEventArgs))
                    {
                        if (!this._sendQueue.TryDequeue(out sendEventArgs))
                        {
                            break;
                        }
                    }

                    try
                    {
                        if (!this._clientSocket.SendAsync(sendEventArgs))
                        {
                            this.SendCompleted(this, sendEventArgs);
                        }
                    }
                    catch (Exception)
                    {
                        this._sendFailedQueue.Enqueue(sendEventArgs);
                        break;
                    }
                } while (this._connected);
            }
            else
            {
                this.ConnectToServer();
            }
        }

        /// <summary>
        ///     Called when the send event is completed.
        /// </summary>
        private void SendCompleted(object sender, SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (socketAsyncEventArgs.SocketError != SocketError.Success)
            {
                this._sendFailedQueue.Enqueue(socketAsyncEventArgs);

                if (this._connected)
                {
                    this.Disconnected();
                }
            }
            else
            {
                socketAsyncEventArgs.Dispose();
            }
        }

        /// <summary>
        ///     Called when the socket is disconnected.
        /// </summary>
        private void Disconnected()
        {
            this._connected = false;
            this._clientSocket.Disconnect(true);
        }
    }
}