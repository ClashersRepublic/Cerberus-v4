namespace ClashersRepublic.Magic.Client.Game.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using ClashersRepublic.Magic.Client.Game.Network.Listener;
    using ClashersRepublic.Magic.Titan;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;

    internal class Messaging : IConnectionListener
    {
        private string _serverUrl;

        private Queue<int> _serverPorts;
        private Connection _connection;

        private ConcurrentQueue<PiranhaMessage> _sendMessageQueue;
        private ConcurrentQueue<PiranhaMessage> _receiveMessageQueue;

        private StreamEncrypter _sendEncrypter;
        private StreamEncrypter _receiveEncrypter;
        private MessageManager _messageManager;
        private PiranhaMessage _pepperLoginMessage;

        private int _pepperState;

        private bool _inConnecting;
        private bool _inDisconnecting;
        private bool _connectFailed;
        private bool _connected;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Messaging"/> class.
        /// </summary>
        internal Messaging(ServerConnection serverConnection)
        {
            this._serverPorts = new Queue<int>();
            this._sendMessageQueue = new ConcurrentQueue<PiranhaMessage>();
            this._receiveMessageQueue = new ConcurrentQueue<PiranhaMessage>();
            this._messageManager = new MessageManager(this, serverConnection);
        }

        /// <summary>
        ///     Adds the specified connection port.
        /// </summary>
        internal void AddPort(int port)
        {
            this._serverPorts.Enqueue(port);
        }

        /// <summary>
        ///     Connects the client to specified host.
        /// </summary>
        internal void Connect(string serverUrl, int port)
        {
            if (!this._connection.IsConnected())
            {
                if (!this._inConnecting)
                {
                    if (!this._inDisconnecting)
                    {
                        this.AddPort(port);

                        this._serverUrl = serverUrl;
                        this._inConnecting = true;
                    }
                    else
                    {
                        Debugger.Warning("Messaging::connect while disconnecting");
                    }
                }
                else
                {
                    Debugger.Warning("Messaging::connect while connecting");
                }
            }
            else
            {
                Debugger.Warning("Messaging::connect while connected");
            }
        }

        /// <summary>
        ///     Connects the connection to next port.
        /// </summary>
        internal void ConnectToNextPort()
        {
            if (!string.IsNullOrEmpty(this._serverUrl))
            {
                if (this._serverPorts.Count != 0)
                {
                    this._connection.Connect(this._serverUrl, this._serverPorts.Dequeue());
                }
            }
        }

        /// <summary>
        ///     Disconnects the client.
        /// </summary>
        internal void Disconnect()
        {
            this._inDisconnecting = true;
        }

        /// <summary>
        ///     Gets a value indicating whether the client is connected to server.
        /// </summary>
        internal bool IsConnected()
        {
            return this._connection.IsConnected();
        }

        /// <summary>
        ///     Gets a value indicating whether a connection error has occurred.
        /// </summary>
        internal bool HasConnectFailed()
        {
            return this._connectFailed;
        }

        /// <summary>
        ///     Sets the encrypter instances.
        /// </summary>
        internal void SetEncrypters(StreamEncrypter sendEncrypter, StreamEncrypter receiveEncrypter)
        {
            this._sendEncrypter = sendEncrypter;
            this._receiveEncrypter = receiveEncrypter;
        }

        /// <summary>
        ///     Sends the specified message to server.
        /// </summary>
        internal void Send(PiranhaMessage message)
        {
            if (this._connection.IsConnected())
            {
                if (message.IsServerToClientMessage())
                {
                    this._sendMessageQueue.Enqueue(message);
                }
            }
        }

        /// <summary>
        ///     Sends the pepper authentification message to server.
        /// </summary>
        internal void SendPepperAuthentification(ClientHelloMessage authMessage, PiranhaMessage pepperloginMessage, byte[] serverPublicKey)
        {
            this._pepperState = 1;

            this._sendEncrypter = null;
            this._receiveEncrypter = null;

            this._pepperLoginMessage = pepperloginMessage;

            this.Send(authMessage);
        }

        /// <summary>
        ///     Writes the message header to buffer.
        /// </summary>
        internal static void WriteHeader(PiranhaMessage message, byte[] buffer, int length)
        {
            int messageType = message.GetMessageType();
            int messageVersion = message.GetMessageVersion();

            buffer[1] = (byte)(messageType);
            buffer[0] = (byte)(messageType >> 8);
            buffer[4] = (byte)(length);
            buffer[3] = (byte)(length >> 8);
            buffer[2] = (byte)(length >> 16);
            buffer[6] = (byte)(messageVersion);
            buffer[5] = (byte)(messageVersion >> 8);

            if (length > 0xFFFFFF)
            {
                Debugger.Error("Trying to send too big message, type " + messageType);
            }
        }

        /// <summary>
        ///     Called when the connection failed to connect to server.
        /// </summary>
        public void OnConnectionFailed(Connection connection)
        {
            if (this._inConnecting)
            {
                if (this._serverPorts.Count != 0)
                {
                    this.ConnectToNextPort();
                }
                else
                {
                    this._connectFailed = false;
                }
            }
        }

        /// <summary>
        ///     Called when the connection has been started.
        /// </summary>
        public void OnStart(Connection connection)
        {
            if (this._inDisconnecting)
            {
                Debugger.Warning("Messaging::onStart while disconnecting");
            }
            else
            {
                if (!this._connection.IsConnected())
                {
                    if (this._inConnecting)
                    {
                        if (this._serverPorts.Count != 0)
                        {
                            this.ConnectToNextPort();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the connection has been connected to server.
        /// </summary>
        public void OnConnect(Connection connection)
        {
            this._serverUrl = null;
            this._inConnecting = false;
            this._connected = true;
        }

        /// <summary>
        ///     Called when the connection has been diconnected.
        /// </summary>
        public void OnDisconnect(Connection connection)
        {
            this._inConnecting = false;
        }

        /// <summary>
        ///     Called when the connection is wakeup.
        /// </summary>
        public void OnWakeup(Connection connection)
        {
            while (this._sendMessageQueue.TryDequeue(out PiranhaMessage message))
            {
                int encodingLength = message.GetEncodingLength();
                byte[] encodingByteArray = message.GetByteStream().GetByteArray();

                if (this._sendEncrypter != null)
                {
                    int overheadLength = this._sendEncrypter.GetOverheadEncryption();

                    byte[] decryptedByteArray = encodingByteArray;
                    byte[] encryptedByteArray = new byte[encodingLength + overheadLength];

                    this._sendEncrypter.Encrypt(decryptedByteArray, encryptedByteArray, encodingLength);

                    encodingLength += overheadLength;
                    encodingByteArray = encryptedByteArray;
                }

                byte[] packetByteArray = new byte[encodingLength + 7];

                Messaging.WriteHeader(message, packetByteArray, encodingLength);
                Array.Copy(encodingByteArray, 0, packetByteArray, 7, encodingLength);

                this._connection.WriteBlocking(packetByteArray, encodingLength + 7);
            }
        }

        /// <summary>
        ///     Called when the connection receive a block of bytes.
        /// </summary>
        public void OnReceive(Connection connection)
        {
            byte[] buffer = this._connection.Buffer;
            int length = buffer.Length;

            if (length >= 7)
            {
                length -= 7;

                int messageType = buffer[1] | buffer[0] << 8;
                int messageLength = buffer[4] | buffer[3] << 8 | buffer[2] << 16;
                int messageVersion = buffer[6] | buffer[5] << 8;

                if (length >= messageLength)
                {
                    this._connection.RemoveBlocking(messageLength + 7);

                    if (this._pepperState != 0)
                    {
                        if (this._pepperState == 1)
                        {
                            if (messageType == 20100)
                            {

                            }
                        }
                        else if (this._pepperState == 2)
                        {

                        }
                    }
                }
            }
        }
    }
}