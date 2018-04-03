namespace LineageSoft.Magic.Services.Core.Network
{
    using LineageSoft.Magic.Services.Core.Message;
    using LineageSoft.Magic.Titan.DataStream;

    internal class NetPacket
    {
        private const byte PROTOCOL_VERSION = 2;
        private byte _protocolVersion;
        private NetMessage _message;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket" /> class.
        /// </summary>
        internal NetPacket()
        {
            this._protocolVersion = NetPacket.PROTOCOL_VERSION;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket" /> class.
        /// </summary>
        internal NetPacket(NetMessage message) : this()
        {
            this._message = message;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket" /> class.
        /// </summary>
        internal NetPacket(byte[] buffer, int length) : this()
        {
            this.Decode(buffer, length);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            if (this._message != null)
            {
                this._message.Destruct();
                this._message = null;
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        internal void Decode(byte[] buffer, int length)
        {
            ByteStream stream = new ByteStream(buffer, length);

            this._protocolVersion = stream.ReadByte();

            if (this._protocolVersion <= 2)
            {
                if (!stream.IsAtEnd())
                {
                    int messageType = stream.ReadVInt();
                    int messageLength = stream.ReadVInt();
                    byte[] messageBytes = stream.ReadBytes(messageLength, 0x7FFFFFFF);

                    this._message = NetMessageFactory.CreateMessageByType(messageType);

                    if (this._message == null)
                    {
                        Logging.Error("NetPacket::decode unknown message received, type: " + messageType);
                        return;
                    }

                    this._message.GetByteStream().SetByteArray(messageBytes, messageLength);
                }
            }
            else
            {
                Logging.Warning("NetPacket::decode invalid protocol version");
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        internal void Encode(ByteStream stream)
        {
            stream.WriteByte(this._protocolVersion);

            if (this._message != null)
            {
                int messageType = this._message.GetMessageType();
                int messageLength = this._message.GetEncodingLength();
                byte[] messageBytes = this._message.GetMessageBytes();

                stream.EnsureCapacity(5 + messageLength);
                stream.WriteVInt(messageType);
                stream.WriteVInt(messageLength);
                stream.WriteBytesWithoutLength(messageBytes, messageLength);
            }
        }

        /// <summary>
        ///     Sets the <see cref="NetMessage"/> instance.
        /// </summary>
        internal void SetNetMessage(NetMessage message)
        {
            this._message = message;
        }

        /// <summary>
        ///     Gets received messages.
        /// </summary>
        internal NetMessage GetNetMessage()
        {
            return this._message;
        }
    }
}