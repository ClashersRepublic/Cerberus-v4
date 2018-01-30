namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;

    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Titan;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;

    internal class NetworkMessaging
    {
        private readonly LogicMessageFactory _messageFactory;

        internal StreamEncrypter SendEncrypter;
        internal StreamEncrypter ReceiveEncrypter;
        internal NetworkToken Token;

        internal byte[] PepperSessionKey;

        internal int ScramblerSeed;
        internal int EncryptionState;

        internal bool UsePepper;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkMessaging" /> class.
        /// </summary>
        internal NetworkMessaging(NetworkToken token)
        {
            this.Token = token;
            this._messageFactory = LogicMagicMessageFactory.Instance;

            this.InitializeEncryption("nonce");
        }

        /// <summary>
        ///     Initializes the encrypters instance.
        /// </summary>
        internal void InitializeEncryption(string nonce)
        {
            this.SetEncrypters(new RC4Encrypter(LogicMagicMessageFactory.RC4_KEY, nonce), new RC4Encrypter(LogicMagicMessageFactory.RC4_KEY, nonce));
        }

        /// <summary>
        ///     Called when a packet has been received.
        /// </summary>
        internal void OnReceiveMessage(byte[] packet)
        {
            int length = packet.Length;

            if (length >= 7)
            {
                int messageType = packet[1] | (packet[0] << 8);
                int messageLength = packet[4] | (packet[3] << 8) | (packet[2] << 16);
                int messageVersion = packet[6] | (packet[5] << 8);

                if (length - 7 >= messageLength)
                {
                    if (messageLength < 0x3F0000)
                    {
                        PiranhaMessage message = this._messageFactory.CreateMessageByType(messageType);

                        if (message != null)
                        {
                            message.SetMessageVersion((short) messageVersion);
                            byte[] messageBytes = new byte[messageLength];
                            Array.Copy(packet, 7, messageBytes, 0, messageLength);
                            message.GetByteStream().SetByteArray(messageBytes, messageLength);

                            if (this.ReceiveEncrypter == null)
                            {
                                if (messageType == 10100)
                                {
                                    if (this.EncryptionState == 0)
                                    {
                                        this.HandlePepperAuthentificationMessage(message);
                                    }
                                }
                                else
                                {
                                    if (messageType == 10101)
                                    {
                                        if (this.EncryptionState == 2)
                                        {
                                            this.HandlePepperLoginMessage((LoginMessage) message);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                byte[] encryptedBytes = message.GetByteStream().RemoveByteArray();
                                byte[] decryptedBytes = new byte[encryptedBytes.Length - this.ReceiveEncrypter.GetOverheadEncryption()];
                                int result = this.ReceiveEncrypter.Decrypt(encryptedBytes, decryptedBytes, encryptedBytes.Length);

                                if (result != 0)
                                {
                                    Logging.Error(this.GetType(), "Encryption failure for message type " + messageType + ", length: " + messageLength + ", encrypter: " + this.ReceiveEncrypter.GetType().Name);
                                }

                                message.GetByteStream().SetByteArray(decryptedBytes, decryptedBytes.Length);
                            }
                            
                            NetworkProcessor.EnqueueReceivedMessage(message, this);
                        }
                        else
                        {
                            Logging.Warning(this.GetType(), "A unknown message has been received. type: " + messageType);
                        }

                        this.Token.RemoveData(messageLength + 7);

                        if (packet.Length - 7 - messageLength >= 7)
                        {
                            byte[] tmp = new byte[packet.Length - 7 - messageLength];
                            Array.Copy(packet, 7 + messageLength, tmp, 0, tmp.Length);
                            this.OnReceiveMessage(tmp);
                        }
                    }
                    else
                    {
                        NetworkGateway.Disconnect(this.Token.AsyncEvent);
                    }
                }
                else
                {
                    NetworkGateway.Disconnect(this.Token.AsyncEvent);
                }
            }
        }

        /// <summary>
        ///     Sends the specified message to client.
        /// </summary>
        internal void SendMessage(PiranhaMessage message)
        {
            if (this.Token.IsConnected())
            {
                if (message.IsServerToClientMessage())
                {
                    int messageType = message.GetMessageType();

                    if (this.SendEncrypter == null)
                    {
                        if (this.EncryptionState == 1)
                        {
                            this.SendPepperAuthentificationResponseMessage(message);
                        }
                        else
                        {
                            if (this.EncryptionState == 3)
                            {
                                this.SendPepperLoginResponseMessage(message);
                            }
                        }
                    }
                    else
                    {
                        byte[] decryptedBytes = message.GetByteStream().RemoveByteArray();
                        byte[] encryptedBytes = new byte[decryptedBytes.Length - this.SendEncrypter.GetOverheadEncryption()];
                        int result = this.SendEncrypter.Encrypt(decryptedBytes, encryptedBytes, decryptedBytes.Length);

                        if (result != 0)
                        {
                            Logging.Error(this.GetType(), "Encryption failure for message type " + messageType + ", encrypter: " + this.SendEncrypter.GetType().Name);
                        }

                        message.GetByteStream().SetByteArray(encryptedBytes, encryptedBytes.Length);
                    }

                    NetworkProcessor.EnqueueSentMessage(message, this);
                }
                else
                {
                    Logging.Error(this.GetType(), "Trying to send a client to server message.");
                }
            }
        }

        /// <summary>
        ///     Handles the pepper authentification message.
        /// </summary>
        internal void HandlePepperAuthentificationMessage(PiranhaMessage message)
        {
            this.SendEncrypter = null;
            this.ReceiveEncrypter = null;
            this.EncryptionState = 1;
        }

        /// <summary>
        ///     Handles the pepper login message.
        /// </summary>
        internal void HandlePepperLoginMessage(LoginMessage message)
        {
            this.EncryptionState = 3;
        }

        /// <summary>
        ///     Sends the pepper authentification response message to client.
        /// </summary>
        internal void SendPepperAuthentificationResponseMessage(PiranhaMessage message)
        {
            this.EncryptionState = 2;
        }

        /// <summary>
        ///     Sends the pepper login response message to client.
        /// </summary>
        internal void SendPepperLoginResponseMessage(PiranhaMessage message)
        {
            this.EncryptionState = 4;
        }

        /// <summary>
        ///     Sets the encrypters instance.
        /// </summary>
        internal void SetEncrypters(StreamEncrypter receiveEncrypter, StreamEncrypter sendEncrypter)
        {
            this.SendEncrypter = sendEncrypter;
            this.ReceiveEncrypter = receiveEncrypter;
        }

        /// <summary>
        ///     Writes the message header.
        /// </summary>
        internal static void WriteHeader(PiranhaMessage message, byte[] packet, int length)
        {
            int messageType = message.GetMessageType();
            int messageVersion = message.GetMessageVersion();

            packet[0] = (byte) (messageType >> 8);
            packet[1] = (byte) (messageType);
            packet[2] = (byte) (length >> 16);
            packet[3] = (byte) (length >> 8);
            packet[4] = (byte) (length);
            packet[5] = (byte) (messageVersion >> 8);
            packet[6] = (byte) (messageVersion);

            if (length >= 0x1000000)
            {
                Logging.Error(typeof(NetworkMessaging), "Trying to send too big message, type " + messageType);
            }
        }
    }
}