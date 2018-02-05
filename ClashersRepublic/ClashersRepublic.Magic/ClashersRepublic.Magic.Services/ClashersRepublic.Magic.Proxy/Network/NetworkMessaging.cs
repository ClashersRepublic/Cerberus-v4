﻿namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Security;
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Proxy.Message;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.User;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Titan;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Message;

    internal class NetworkMessaging
    {
        private StreamEncrypter _receiveEncrypter;
        private StreamEncrypter _sendEncrypter;
        private readonly Client _client;
        
        /// <summary>
        ///     Gets the token instance.
        /// </summary>
        internal NetworkToken Token { get; }

        /// <summary>
        ///     Gets the message manager instance.
        /// </summary>
        internal MessageManager MessageManager { get; }

        /// <summary>
        ///     Gets the pepper step.
        /// </summary>
        internal int PepperStep { get; private set; }

        /// <summary>
        ///     Gets or Sets the scrambler seed.
        /// </summary>
        internal int ScramblerSeed { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkMessaging" /> class.
        /// </summary>
        internal NetworkMessaging(Client client, NetworkToken token)
        {
            this._client = client;
            this.Token = token;
            this.MessageManager = new MessageManager(this._client, this);
        }

        /// <summary>
        ///     Initializes the encrypters.
        /// </summary>
        internal void InitEncrypters(string nonce)
        {
            this.SetEncrypters(new RC4Encrypter(LogicMagicMessageFactory.RC4_KEY, nonce), new RC4Encrypter(LogicMagicMessageFactory.RC4_KEY, nonce));
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
        ///     Called for handle received data.
        /// </summary>
        internal int OnReceive(byte[] buffer, int length)
        {
            if (length >= 7)
            {
                length -= 7;

                int messageType = buffer[1] | (buffer[0] << 8);
                int messageVersion = buffer[5] | (buffer[6] << 8);
                int messageLength = buffer[4] | (buffer[3] << 8) | (buffer[2] << 16);

                if (length >= messageLength)
                {
                    byte[] messageByteArray = new byte[messageLength];
                    Array.Copy(buffer, 7, messageByteArray, 0, messageLength);

                    PiranhaMessage message = LogicMagicMessageFactory.CreateMessageByType(messageType);

                    if (message != null)
                    {
                        message.SetMessageVersion((short) messageVersion);
                        message.GetByteStream().SetByteArray(messageByteArray, length);
                        
                        if (this._receiveEncrypter == null)
                        {
                            if (this.PepperStep != -1)
                            {
                                if (this.PepperStep == 0)
                                {
                                    if (messageType == 10101)
                                    {
                                        this.InitEncrypters("nonce");

                                        byte[] encryptedByteArray = message.GetByteStream().RemoveByteArray();
                                        byte[] decryptedByteArray = new byte[encryptedByteArray.Length - this._receiveEncrypter.GetOverheadEncryption()];

                                        int encryptionResult = this._receiveEncrypter.Decrypt(encryptedByteArray, decryptedByteArray, messageLength);

                                        if (encryptionResult != 0)
                                        {
                                            Logging.Error(this, "NetworkMessaging::onReceive encryption failure, code: " + encryptionResult);
                                        }

                                        message.GetByteStream().SetByteArray(decryptedByteArray, decryptedByteArray.Length);
                                    }
                                    else if (messageType == 10100)
                                    {
                                        this.HandlePepperAuthentificationMessage(message);
                                    }
                                }
                                else if (this.PepperStep == 1)
                                {
                                    if (messageType == 10101)
                                    {
                                        this.HandlePepperLoginMessage(message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            byte[] encryptedByteArray = message.GetByteStream().RemoveByteArray();
                            byte[] decryptedByteArray = new byte[encryptedByteArray.Length - this._receiveEncrypter.GetOverheadEncryption()];

                            int encryptionResult = this._receiveEncrypter.Decrypt(encryptedByteArray, decryptedByteArray, messageLength);

                            if (encryptionResult != 0)
                            {
                                Logging.Error(this, "NetworkMessaging::onReceive encryption failure, code: " + encryptionResult);
                            }

                            message.GetByteStream().SetByteArray(decryptedByteArray, decryptedByteArray.Length);
                        }

                        try
                        {
                            message.Decode();

                            if (!message.IsServerToClientMessage())
                            {
                                if (message.GetServiceNodeType() == 1)
                                {
                                    NetworkManager.ReceiveMessage(message, this);
                                }
                                else
                                {
                                    if (this._client.State == 6)
                                    {
                                        ServiceMessageManager.SendMessage(new ForwardClientMessage {Message = message},
                                            ServiceExchangeName.ServiceNodeTypeToServiceName(message.GetServiceNodeType()),
                                            Config.ServerId,
                                            this._client.GameSession.SessionId);
                                    }
                                }
                            }
                        }
                        catch (Exception exception)
                        {
                            Logging.Error(this, "NetworkMessaging::onReceive Message decode exception, trace: " + exception);
                        }

                        Logging.Debug(this, "NetworkMessaging::onReceive message " + message.GetType().Name + " received");
                    }
                    else
                    {
                        Logging.Warning(this, "NetworkMessaging::onReceive Ignoring message of unknown type " + messageType);
                    }

                    return 7 + messageLength;
                }

                if (messageLength > 0x3F0000)
                {
                    return -1;
                }
            }

            return 0;
        }

        /// <summary>
        ///     Sends the specified <see cref="PiranhaMessage" />.
        /// </summary>
        internal void Send(PiranhaMessage message)
        {
            if (message.IsServerToClientMessage())
            {
                NetworkManager.SendMessage(message, this);
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
                int rnd = Resources.Random.NextInt();

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
        ///     Internal method for send the specified <see cref="PiranhaMessage" /> to client.
        /// </summary>
        internal void OnWakeup(PiranhaMessage message)
        {
            if (message.GetEncodingLength() == 0)
            {
                message.Encode();
            }

            int encodingLength = message.GetEncodingLength();
            byte[] messageByteArray = message.GetByteStream().GetByteArray();

            if (this._sendEncrypter != null)
            {
                if (message.GetMessageType() == 20104)
                {
                    if (this.PepperStep == 0)
                    {
                        byte[] nonce = this.GenerateNonce();

                        this.OnWakeup(new EncryptionMessage
                        {
                            ServerNonce = nonce,
                            Version = 1
                        });

                        this.ScrambleEncrypters(nonce);
                    }
                }

                byte[] decryptedByteArray = messageByteArray;
                byte[] encryptedByteArray = new byte[this._sendEncrypter.GetOverheadEncryption() + encodingLength];

                int encryptionResult = this._sendEncrypter.Encrypt(decryptedByteArray, encryptedByteArray, encodingLength);

                if (encryptionResult != 0)
                {
                    Logging.Error(this, "NetworkMessaging::onWakeup encryption failure, code: " + encryptionResult);
                }

                messageByteArray = encryptedByteArray;
                encodingLength += this._sendEncrypter.GetOverheadEncryption();
            }
            
            byte[] packet = new byte[7 + encodingLength];
            Array.Copy(messageByteArray, 0, packet, 7, encodingLength);
            this.WriteHeader(message, packet, encodingLength);

            this.Token.WriteData(packet);

            Logging.Debug(this, "NetworkMessaging::onWakeup message " + message.GetType().Name + " sent");
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
                Logging.Error(this, "NetworkMessaging::writeHeader trying to send too big message, type " + messageType);
            }
        }

        /// <summary>
        ///     Handles the specified pepper authentification message.
        /// </summary>
        internal void HandlePepperAuthentificationMessage(PiranhaMessage message)
        {
            this.PepperStep = 1;
        }

        /// <summary>
        ///     Handles the specified pepper long message.
        /// </summary>
        internal void HandlePepperLoginMessage(PiranhaMessage message)
        {
            this.PepperStep = 2;
        }

        /// <summary>
        ///     Handles the specified pepper login response message.
        /// </summary>
        internal void SendPepperLoginResponseMessage(PiranhaMessage message)
        {
            this.PepperStep = 3;
        }
    }
}