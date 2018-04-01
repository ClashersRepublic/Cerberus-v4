namespace ClashersRepublic.Magic.Services.Net
{
    using System.Net;
    using System.Net.Sockets;
    using ClashersRepublic.Magic.Services.Net.ServerSocket;

    internal class NetConnection
    {
        private bool _destructed;
        private NetListener _listener;
        private NetBuffer _buffer;

        /// <summary>
        ///     Gets the client socket instance.
        /// </summary>
        internal Socket Socket { get; }

        /// <summary>
        ///     Gets the client socket read event.
        /// </summary>
        internal SocketAsyncEventArgs ReadEvent { get; private set; }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetConnection"/> class.
        /// </summary>
        internal NetConnection(Socket socket, NetListener listener, SocketAsyncEventArgs readEvent)
        {
            this.Socket = socket;
            this._listener = listener;
            this.ReadEvent = readEvent;

            this._buffer = new NetBuffer();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this._destructed = true;
            
            if (this._buffer != null)
            {
                this._buffer.Destruct();
                this._buffer = null;
            }

            if (this.Socket != null)
            {
                this.Socket.Close();
            }

            this.ReadEvent = null;
            this._listener = null;
        }

        /// <summary>
        ///     Gets if the connection is destructed.
        /// </summary>
        internal bool IsDestructed()
        {
            return this._destructed;
        }

        /// <summary>
        ///     Tries to receive the available data on the socket.
        /// </summary>
        internal bool TryReceive()
        {
            int rcv = this.ReadEvent.BytesTransferred;
            this._buffer.Write(this.ReadEvent.Buffer, rcv);
            return true;
        }

        /// <summary>
        ///     Gets if the client connected.
        /// </summary>
        internal bool IsConnected()
        {
            return this.Socket.Connected;
        }

        /// <summary>
        ///     Gets the client address.
        /// </summary>
        internal string GetAddress()
        {
            return ((IPEndPoint) this.Socket.RemoteEndPoint).Address.ToString();
        }

        /// <summary>
        ///     Processes the available data.
        /// </summary>
        internal bool Process()
        {
            int read = this._listener.OnReceive(this._buffer.GetBuffer(), this._buffer.GetLength());

            if (read != -1)
            {
                if (read != 0)
                {
                    this._buffer.Remove(read);

                    if (this._buffer.GetLength() > 0)
                    {
                        return this.Process();
                    }
                }

                return true;
            }

            return false;
        }
    }
}