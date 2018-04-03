namespace LineageSoft.Magic.Services.Proxy.Network
{
    using System;
    using System.Collections.Concurrent;

    using LineageSoft.Magic.Logic.Message;
    using LineageSoft.Magic.Logic.Message.Security;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Core.Message;
    using LineageSoft.Magic.Services.Core.Message.Network;
    using LineageSoft.Magic.Services.Core.Network;
    using LineageSoft.Magic.Services.Proxy.Game.Message;
    using LineageSoft.Magic.Services.Proxy.Network.Message;
    using LineageSoft.Magic.Services.Proxy.Network.Session;
    using LineageSoft.Magic.Titan;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Message;

    internal class NetworkMessaging
    {
        private bool _destructed;
        private bool _scrambled;
        private int _pepperState;

        private StreamEncrypter _sendEncrypter;
        private StreamEncrypter _receiveEncrypter;
        private LogicMessageFactory _messageFactory;
        private ConcurrentQueue<PiranhaMessage> _sendMessageQueue;
        private ConcurrentQueue<PiranhaMessage> _receiveMessageQueue;

        /// <summary>
        ///     Gets the <see cref="NetworkConnection"/> instance.
        /// </summary>
        internal NetworkConnection Connection { get; }

        /// <summary>
        ///     Gets the <see cref="NetworkClient"/> instance.
        /// </summary>
        internal NetworkClient Client { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Game.Message.MessageManager"/> instance.
        /// </summary>
        internal MessageManager MessageManager { get; }

        /// <summary>
        ///     Gets or Sets the scrambler seed.
        /// </summary>
        internal int ScramblerSeed { get; set; }

        /// <summary>
        ///     Gets of Sets the messaging id.
        /// </summary>
        internal long MessagingId { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkMessaging"/> class.
        /// </summary>
        internal NetworkMessaging(NetworkConnection connection)
        {
            this._pepperState = -1;
            this.MessagingId = -1;
            this.Connection = connection;
            this.Client = new NetworkClient(this);
            this.MessageManager = new MessageManager(this);
            this._sendMessageQueue = new ConcurrentQueue<PiranhaMessage>();
            this._receiveMessageQueue = new ConcurrentQueue<PiranhaMessage>();
            this._messageFactory = LogicMagicMessageFactory.Instance;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this._destructed = true;
            
            if (this.Client != null)
            {
                this.Client.Destruct();
                this.Client = null;
            }

            while (this._sendMessageQueue.TryDequeue(out PiranhaMessage message))
            {
                message.Destruct();
            }

            while (this._receiveMessageQueue.TryDequeue(out PiranhaMessage message))
            {
                message.Destruct();
            }

            this._messageFactory = null;
        }

        /// <summary>
        ///     Initializes the encrypters.
        /// </summary>
        internal void InitEncrypters(string nonce)
        {
            this.SetEncrypters(new RC4Encrypter(LogicMessagingConstants.RC4_KEY, nonce), new RC4Encrypter(LogicMessagingConstants.RC4_KEY, nonce));
        }

        /// <summary>
        ///     Sets the encrypters.
        /// </summary>
        internal void SetEncrypters(StreamEncrypter receiveEncrypter, StreamEncrypter sendEncrypter)
        {
            this._receiveEncrypter = receiveEncrypter;
            this._sendEncrypter = sendEncrypter;
        }

        /// <summary>
        ///     Receives the received data.
        /// </summary>
        internal int OnReceive(byte[] buffer, int length)
        {
            if (length >= 7)
            {
                length -= 7;

                int messageType = buffer[0] << 8 | buffer[1];
                int messageLength = buffer[2] << 16 | buffer[3] << 8 | buffer[4];
                int messageVersion = buffer[5] << 8 | buffer[6];

                if (messageLength <= length)
                {
                    int encodingLength = messageLength;
                    byte[] encodingByteArray = new byte[encodingLength];

                    Array.Copy(buffer, 7, encodingByteArray, 0, encodingLength);

                    if (this._receiveEncrypter == null)
                    {
                        if (this._pepperState == -1)
                        {
                            if (messageType == 10101)
                            {
                                this.InitEncrypters("nonce");

                                int overheadEncryption = this._receiveEncrypter.GetOverheadEncryption();

                                byte[] encryptedData = encodingByteArray;
                                byte[] decryptedData = new byte[encodingLength - overheadEncryption];

                                this._receiveEncrypter.Decrypt(encryptedData, decryptedData, encodingLength);

                                encodingByteArray = decryptedData;
                                encodingLength -= overheadEncryption;
                            }
                            else if (messageType == 10100)
                            {
                                this.ReceivePepperAuthentification(ref encodingByteArray, ref encodingLength);
                            }
                        }
                    }
                    else
                    {
                        int overheadEncryption = this._receiveEncrypter.GetOverheadEncryption();

                        byte[] encryptedData = encodingByteArray;
                        byte[] decryptedData = new byte[encodingLength - overheadEncryption];

                        this._receiveEncrypter.Decrypt(encryptedData, decryptedData, encodingLength);

                        encodingByteArray = decryptedData;
                        encodingLength -= overheadEncryption;
                    }

                    PiranhaMessage message = this._messageFactory.CreateMessageByType(messageType);

                    if (message != null)
                    {
                        message.SetMessageVersion(messageVersion);
                        message.GetByteStream().SetByteArray(encodingByteArray, encodingLength);

                        try
                        {
                            message.Decode();

                            if (!message.IsServerToClientMessage())
                            {
                                this._receiveMessageQueue.Enqueue(message);
                            }
                        }
                        catch (Exception exception)
                        {
                            Logging.Warning("NetworkMessaging::receive exception while the decode of message type " + messageType + ", trace: " + exception);
                        }
                        finally
                        {
                            Logging.Debug("NetworkMessaging::receive message " + message.GetType().Name + " received");
                        }
                    }
                    else
                    {
                        Logging.Warning("NetworkMessaging::receive ignoring message of unknown type " + messageType);
                    }

                    return messageLength + 7;
                }
                else
                {
                    if (messageLength >= 0x3F0000)
                    {
                        return -1;
                    }
                }
            }

            return 0;
        }

        /// <summary>
        ///     Receives the pepper authentification.
        /// </summary>
        internal void ReceivePepperAuthentification(ref byte[] buffer, ref int length)
        {
            this._pepperState = 1;
        }

        /// <summary>
        ///     Adds the specified <see cref="PiranhaMessage"/> to the send queue.
        /// </summary>
        internal void Send(PiranhaMessage message)
        {
            if (message.IsServerToClientMessage())
            {
                if (this.Connection.IsConnected())
                {
                    this._sendMessageQueue.Enqueue(message);
                }
            }
        }

        /// <summary>
        ///     Generates a nonce.
        /// </summary>
        private byte[] GenerateNonce()
        {
            byte[] nonce = new byte[24];

            for (int i = 0; i < 24; i += 4)
            {
                int rnd = ServiceProxy.Random.Rand(0x7fffffff);

                nonce[i] = (byte) (rnd);
                nonce[i + 1] = (byte) (rnd >> 8);
                nonce[i + 2] = (byte) (rnd >> 16);
                nonce[i + 3] = (byte) (rnd >> 24);
            }

            return nonce;
        }

        /// <summary>
        ///     Scrambles the encrypters.
        /// </summary>
        private void ScrambleEncrypters(byte[] nonce)
        {
            LogicMersenneTwisterRandom scrambler = new LogicMersenneTwisterRandom(this.ScramblerSeed);

            string scrambledNonce = null;
            byte byte100 = 0;

            for (int i = 0; i < 100; i++)
            {
                byte100 = (byte) scrambler.NextInt();
            }

            for (int i = 0; i < nonce.Length; i++)
            {
                scrambledNonce += (char) (nonce[i] ^ (byte) (scrambler.NextInt() & byte100));
            }

            this.InitEncrypters(scrambledNonce);
        }

        /// <summary>
        ///     Called for send all messages in send queue.
        /// </summary>
        internal void OnWakeup()
        {
            while (this._sendMessageQueue.TryDequeue(out PiranhaMessage message))
            {
                this.InternalSend(message);
            }
        }

        /// <summary>
        ///     Sends the specified <see cref="PiranhaMessage"/> to client.
        /// </summary>
        internal void InternalSend(PiranhaMessage message)
        {
            if (message.GetServiceNodeType() == 1)
            {
                message.Encode();
            }

            int encodingLength = message.GetEncodingLength();
            byte[] messageByteArray = message.GetMessageBytes();

            if (this._sendEncrypter != null)
            {
                if (!this._scrambled && this._pepperState == -1)
                {
                    if (message.GetMessageType() == 20104)
                    {
                        byte[] nonce = this.GenerateNonce();

                        ExtendedSetEncryptionMessage encryptionMessage = new ExtendedSetEncryptionMessage();

                        encryptionMessage.SetNonce(nonce);
                        encryptionMessage.SetNonceMethod(1);

                        this.InternalSend(encryptionMessage);
                        this.ScrambleEncrypters(nonce);
                    }
                }

                byte[] decryptedByteArray = messageByteArray;
                byte[] encryptedByteArray = new byte[this._sendEncrypter.GetOverheadEncryption() + encodingLength];

                int encryptionResult = this._sendEncrypter.Encrypt(decryptedByteArray, encryptedByteArray, encodingLength);

                if (encryptionResult != 0)
                {
                    Logging.Error("NetworkMessaging::onWakeup encryption failure, code: " + encryptionResult);
                }

                messageByteArray = encryptedByteArray;
                encodingLength += this._sendEncrypter.GetOverheadEncryption();
            }

            byte[] packet = new byte[encodingLength + 7];
            Array.Copy(messageByteArray, 0, packet, 7, encodingLength);
            this.WriteHeader(message, packet, encodingLength);
            this.Connection.SendData(packet, encodingLength + 7);

            Logging.Debug("NetworkMessaging::onWakeup message " + message.GetType().Name + " sent");
        }

        /// <summary>
        ///     Writes the message header.
        /// </summary>
        private void WriteHeader(PiranhaMessage message, byte[] stream, int length)
        {
            int messageType = message.GetMessageType();
            int messageVersion = message.GetMessageVersion();

            stream[1] = (byte) messageType;
            stream[0] = (byte) (messageType >> 8);
            stream[4] = (byte) length;
            stream[3] = (byte) (length >> 8);
            stream[2] = (byte) (length >> 16);
            stream[6] = (byte) messageVersion;
            stream[5] = (byte) (messageVersion >> 8);

            if (length > 0xFFFFFF)
            {
                Logging.Error("NetworkMessaging::writeHeader trying to send too big message, type " + messageType);
            }
        }

        /// <summary>
        ///     Gets the next <see cref="PiranhaMessage"/> in receive queue.
        /// </summary>
        internal bool NextMessage(out PiranhaMessage message)
        {
            return this._receiveMessageQueue.TryDequeue(out message);
        }

        /// <summary>
        ///     Gets if this instance is destructed.
        /// </summary>
        internal bool IsDestructed()
        {
            return this._destructed;
        }
    }
}