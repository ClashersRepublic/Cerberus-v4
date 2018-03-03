namespace ClashersRepublic.Magic.Services.Core.Network
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.DataStream;

    internal class NetPacket
    {
        private NetMessage _message;
        private byte _protocolVersion;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket" /> class.
        /// </summary>
        internal NetPacket()
        {
            this._protocolVersion = 1;
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
            this._message = null;
            this._protocolVersion = 1;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        internal void Decode(byte[] buffer, int length)
        {
            ByteStream stream = new ByteStream(buffer, length);

            this._protocolVersion = stream.ReadByte();

            if (this._protocolVersion == 1)
            {
                if (!stream.IsAtEnd())
                {
                    int messageType = stream.ReadVInt();
                    int messageLength = stream.ReadVInt();
                    byte[] messageBytes = stream.ReadBytes(messageLength, 0x7FFFFFFF);

                    this._message = NetMessageFactory.CreateMessageByType(messageType);

                    if (this._message == null)
                    {
                        Logging.Warning(this, "NetPacket::decode ignoring message of unknown type " + messageType);
                    }
                    else
                    {
                        this._message.GetByteStream().SetByteArray(messageBytes, messageLength);
                    }
                }
            }
            else
            {
                Logging.Warning(this, "NetPacket::decode invalid protocol version");
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