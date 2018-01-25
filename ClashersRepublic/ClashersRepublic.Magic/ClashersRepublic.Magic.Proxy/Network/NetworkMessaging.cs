namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Factory;

    internal class NetworkMessaging
    {
        private readonly LogicMessageFactory _messageFactory;

        internal NetworkToken Token;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetworkMessaging" /> class.
        /// </summary>
        internal NetworkMessaging(NetworkToken token)
        {
            this.Token = token;
            this._messageFactory = LogicMagicMessageFactory.Instance;
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
                        byte[] messageBytes = new byte[messageLength];
                        Array.Copy(packet, 7, messageBytes, 0, messageLength);
                        
                        PiranhaMessage message = this._messageFactory.CreateMessageByType(messageType);

                        if (message != null)
                        {
                            message.SetMessageVersion((short) messageVersion);
                            message.GetByteStream().SetByteArray(messageBytes, messageLength);

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
                    message.Encode();

                    NetworkProcessor.EnqueueSentMessage(message, this);
                }
                else
                {
                    Logging.Error(this.GetType(), "Trying to send a client to server message.");
                }
            }
        }
    }
}