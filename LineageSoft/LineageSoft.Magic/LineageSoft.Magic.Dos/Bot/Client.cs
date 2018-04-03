namespace LineageSoft.Magic.Dos.Bot
{
    using System;
    using LineageSoft.Magic.Dos.Debug;
    using LineageSoft.Magic.Dos.Network;
    using LineageSoft.Magic.Logic;
    using LineageSoft.Magic.Logic.Message;
    using LineageSoft.Magic.Logic.Message.Account;
    using LineageSoft.Magic.Logic.Message.Security;
    using LineageSoft.Magic.Titan;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Message;

    internal class Client
    {
        private NetworkGateway _networkGateway;
        private StreamEncrypter _sendEncrypter;
        private StreamEncrypter _receiveEncrypter;

        internal MessageManager MessageManager { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Client"/> class.
        /// </summary>
        internal Client()
        {
            this._networkGateway = new NetworkGateway(this);
            this.MessageManager = new MessageManager(this);
        }

        /// <summary>
        ///     Gets a value indicating whether the client is connected.
        /// </summary>
        internal bool IsConnected()
        {
            return this._networkGateway.IsConnected();
        }

        /// <summary>
        ///     Initializes the instance of encrypters.
        /// </summary>
        internal void InitEncrypters(string nonce)
        {
            this._sendEncrypter = new RC4Encrypter(LogicMessagingConstants.RC4_KEY, nonce);
            this._receiveEncrypter = new RC4Encrypter(LogicMessagingConstants.RC4_KEY, nonce);
        }

        /// <summary>
        ///     Disconnects the client.
        /// </summary>
        internal void Disconnect()
        {
            this._networkGateway.Disconnect();
        }

        /// <summary>
        ///     Connects the client to specified host.
        /// </summary>
        internal void ConnectTo(string host, int port)
        {
            this.InitEncrypters("nonce");
            this._networkGateway.Connect(host, port);
        }

        /// <summary>
        ///     Connects the bot to prod server.
        /// </summary>
        internal void ConnectToProd()
        {
            this.ConnectTo("game.clashofclans.com", 9339);
        }

        /// <summary>
        ///     Connects the bot to private server.
        /// </summary>
        internal void ConnectToPrivateServer()
        {
            this.ConnectTo("163.172.251.148", 9339);
        }

        /// <summary>
        ///     Connects the bot to the local server.
        /// </summary>
        internal void ConnectToLocalServer()
        {
            this.ConnectTo("127.0.0.1", 9339);
        }

        /// <summary>
        ///     Called when the gateway has been connected to server.
        /// </summary>
        internal void OnConnect()
        {
            LoginMessage loginMessage = new LoginMessage
            {
                AccountId = new LogicLong(),
                ClientMajorVersion = LogicVersion.MajorVersion,
                ClientBuildVersion = LogicVersion.BuildVersion,
                ResourceSha = "de7eb7684e08fece56a23abb3da256363597f85c"
            };

            this.SendMessage(loginMessage);
        }

        /// <summary>
        ///     Called when the gateway receive a packet.
        /// </summary>
        internal int OnReceive(byte[] packet, int length)
        {
            if (length >= 7)
            {
                length -= 7;

                int messageType = packet[1] | packet[0] << 8;
                int messageLength = packet[4] | packet[3] << 8 | packet[2] << 16;
                int messageVersion = packet[6] | packet[5] << 8;

                if (length >= messageLength)
                {
                    int encodingLength = messageLength;
                    byte[] encodingByteArray = new byte[messageLength];
                    Array.Copy(packet, 7, encodingByteArray, 0, messageLength);

                    if (this._receiveEncrypter != null)
                    {
                        byte[] encryptedByteArray = encodingByteArray;
                        byte[] decryptedByteArray = new byte[messageLength - this._receiveEncrypter.GetOverheadEncryption()];

                        this._receiveEncrypter.Decrypt(encryptedByteArray, decryptedByteArray, messageLength);

                        encodingByteArray = decryptedByteArray;
                        encodingLength -= this._receiveEncrypter.GetOverheadEncryption();
                    }

                    PiranhaMessage message = LogicMagicMessageFactory.Instance.CreateMessageByType(messageType);

                    if (message != null)
                    {
                        message.SetMessageVersion((short) messageVersion);
                        message.GetByteStream().SetByteArray(encodingByteArray, encodingLength);

                        try
                        {
                            message.Decode();

                            if (message.GetMessageType() == 20000)
                            {
                                ExtendedSetEncryptionMessage extendedSetEncryptionMessage = (ExtendedSetEncryptionMessage) message;
                                LogicMersenneTwisterRandom scrambler = new LogicMersenneTwisterRandom(0);

                                byte byte100 = 0;

                                for (int i = 0; i < 100; i++)
                                {
                                    byte100 = (byte) scrambler.NextInt();
                                }

                                byte[] nonce = extendedSetEncryptionMessage.RemoveNonce();
                                string scrambledNonce = null;

                                for (int i = 0; i < nonce.Length; i++)
                                {
                                    scrambledNonce += (char) (nonce[i] ^ (byte) (scrambler.NextInt() & byte100));
                                }

                                if (this._receiveEncrypter != null)
                                {
                                    this._receiveEncrypter.Destruct();
                                    this._receiveEncrypter = null;
                                }

                                if (this._sendEncrypter != null)
                                {
                                    this._sendEncrypter.Destruct();
                                    this._sendEncrypter = null;
                                }

                                this._receiveEncrypter = new RC4Encrypter(LogicMessagingConstants.RC4_KEY, scrambledNonce);
                                this._sendEncrypter = new RC4Encrypter(LogicMessagingConstants.RC4_KEY, scrambledNonce);
                            }
                            else
                            {
                                this.MessageManager.ReceiveMessage(message);
                            }
                        }
                        catch (Exception exception)
                        {
                            Logging.Error(this, "Client::onReceive message decode exception, trace: " + exception);
                        }

                        Logging.Debug(this, "Client::sendMessage message " + message.GetType().Name + " received");
                    }
                    else
                    {
                        Logging.Warning(this, "NetworkMessaging::onReceive Ignoring message of unknown type " + messageType);
                    }

                    return messageLength + 7;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Called when the gateway cannot connect to server.
        /// </summary>
        internal void OnConnectFailed()
        {
            Logging.Log(this, "Client::onConnectFailed unable to connect to server");
        }

        /// <summary>
        ///     Called when the gateway is disconnected from the server.
        /// </summary>
        internal void OnDisconnect()
        {
            // RIP.
        }

        /// <summary>
        ///     Sends the specified message to server.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            message.Encode();

            byte[] encodingByteArray = message.GetByteStream().RemoveByteArray();
            int encodingLength = message.GetEncodingLength();

            if (this._sendEncrypter != null)
            {
                byte[] decryptedByteArray = encodingByteArray;
                byte[] encryptedByteArray = new byte[encodingLength + this._sendEncrypter.GetOverheadEncryption()];

                this._sendEncrypter.Encrypt(decryptedByteArray, encryptedByteArray, encodingLength);

                encodingLength += this._sendEncrypter.GetOverheadEncryption();
                encodingByteArray = encryptedByteArray;
            }

            byte[] packet = new byte[7 + encodingLength];

            Array.Copy(encodingByteArray, 0, packet, 7, encodingLength);
            this.WriteHeader(message, packet, encodingLength);
            this._networkGateway.Send(packet, encodingLength + 7);

            Logging.Debug(this, "Client::sendMessage message " + message.GetType().Name + " sent");
        }

        /// <summary>
        ///     Writes the message header
        /// </summary>
        private void WriteHeader(PiranhaMessage message, byte[] packet, int encodingLength)
        {
            int messageType = message.GetMessageType();
            int messageVersion = message.GetMessageVersion();

            packet[1] = (byte) (messageType);
            packet[0] = (byte) (messageType >> 8);
            packet[4] = (byte) (encodingLength);
            packet[3] = (byte) (encodingLength >> 8);
            packet[2] = (byte) (encodingLength >> 16);
            packet[6] = (byte) (messageVersion);
            packet[5] = (byte) (messageVersion >> 8);
        }
    }
}