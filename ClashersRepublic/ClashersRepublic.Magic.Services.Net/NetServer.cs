namespace ClashersRepublic.Magic.Services.Net
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    public class NetServer
    {
        private Socket _listener;
        private List<Client> _clients;

        private int _count;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetServer"/> class.
        /// </summary>
        public NetServer(int port)
        {
            this._clients = new List<Client>();
            this._listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(new IPEndPoint(IPAddress.Any, port));
            this._listener.Listen(150);

            SocketAsyncEventArgs acceptEvent = new SocketAsyncEventArgs();
            acceptEvent.Completed += this.OnAcceptCompleted;
            acceptEvent.DisconnectReuseSocket = true;

            this.StartAccept(acceptEvent);
        }

        /// <summary>
        ///     Accepts a TCP Request.
        /// </summary>
        /// <param name="acceptEvent">The <see cref="SocketAsyncEventArgs" /> instance containing the event data.</param>
        private void StartAccept(SocketAsyncEventArgs acceptEvent)
        {
            acceptEvent.AcceptSocket = null;

            if (!this._listener.AcceptAsync(acceptEvent))
            {
                this.OnAcceptCompleted(null, acceptEvent);
            }
        }

        /// <summary>
        ///     Called when the a is accepted.
        /// </summary>
        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                this.ProcessAccept(asyncEvent);
            }

            this.StartAccept(asyncEvent);
        }

        /// <summary>
        ///     Accept the new client and store it in memory.
        /// </summary>
        private void ProcessAccept(SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.AcceptSocket.Connected)
            {
                Socket socket = asyncEvent.AcceptSocket;
                SocketAsyncEventArgs readEvent = new SocketAsyncEventArgs();
                Client client = new Client(socket);

                readEvent.SetBuffer(new byte[2048], 0, 2048);
                readEvent.Completed += this.OnReceiveCompleted;
                readEvent.UserToken = client;

                this._clients.Add(client);

                if (!socket.ReceiveAsync(readEvent))
                {
                    this.ProcessReceive(readEvent);
                }
            }
        }

        /// <summary>
        ///     Processes the received data.
        /// </summary>
        private void ProcessReceive(SocketAsyncEventArgs asyncEvent)
        {
            Client client = (Client) asyncEvent.UserToken;

            if (asyncEvent.BytesTransferred > 0)
            {
                client.Write(asyncEvent.Buffer, asyncEvent.BytesTransferred);

                if (client.Socket.Connected)
                {
                    try
                    {
                        if (!client.Socket.ReceiveAsync(asyncEvent))
                        {
                            this.OnReceiveCompleted(null, asyncEvent);
                        }
                    }
                    catch (Exception)
                    {
                        client.Socket.Close();
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a receive event is completed.
        /// </summary>
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs asyncEvent)
        {
            if (asyncEvent.SocketError == SocketError.Success)
            {
                this.ProcessReceive(asyncEvent);
            }
            else
            {
                if (asyncEvent.UserToken != null)
                {
                    this._clients.Remove((Client) asyncEvent.UserToken);
                }
            }
        }

        /// <summary>
        ///     Reads available bytes.
        /// </summary>
        public bool Read(ref Queue<byte[]> messages)
        {
            for (int i = 0, count = this._clients.Count; i < count; i++)
            {
                Client client = this._clients[i];

                while (true)
                {
                    byte[] message = client.RemoveMessages();

                    if (message == null)
                    {
                        break;
                    }
                    
                    messages.Enqueue(message);
                }
            }

            return messages.Count != 0;
        }

        private class Client
        {
            internal Socket Socket { get; }

            private readonly NetBuffer _buffer;
            private readonly ConcurrentQueue<byte[]> _messages;
            
            /// <summary>
            ///     Initializes a new instance of the <see cref="Client"/> class.
            /// </summary>
            internal Client(Socket socket)
            {
                this.Socket = socket;
                this._buffer = new NetBuffer();
                this._messages = new ConcurrentQueue<byte[]>();
            }

            /// <summary>
            ///     Writes the received bytes.
            /// </summary>
            internal void Write(byte[] buffer, int length)
            {
                this._buffer.Write(buffer, length);
                this.Decode();
            }

            /// <summary>
            ///     Decodes the received packet.
            /// </summary>
            private void Decode()
            {
                if (this._buffer.GetLength() >= 4)
                {
                    byte[] tmp = this._buffer.GetBuffer();
                    int messageLength = tmp[3] << 24 | tmp[2] << 16 | tmp[1] << 8 | tmp[0];

                    if (this._buffer.GetLength() - 4 >= messageLength)
                    {
                        byte[] messageBytes = new byte[messageLength];

                        Array.Copy(tmp, 4, messageBytes, 0, messageLength);

                        this._messages.Enqueue(messageBytes);
                        this._buffer.Remove(4 + messageLength);

                        if (this._buffer.GetLength() >= 4)
                        {
                            this.Decode();
                        }
                    }
                }
            }

            /// <summary>
            ///     Removes the first available message.
            /// </summary>
            internal byte[] RemoveMessages()
            {
                this._messages.TryDequeue(out byte[] buffer);
                return buffer;
            }
        }
    }
}