namespace ClashersRepublic.Magic.Services.Core.Network
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Util;

    internal class NetPacket
    {
        private readonly LogicArrayList<NetMessage> _messages;

        private byte _serviceNodeId;
        private byte _serviceNodeType;
        private byte _protocolVersion;
        private byte _sessionIdLength;
        private byte[] _sessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket"/> class.
        /// </summary>
        internal NetPacket()
        {
            this._protocolVersion = 1;
            this._messages = new LogicArrayList<NetMessage>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket"/> class.
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
            this._sessionId = null;
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
                this._serviceNodeType = stream.ReadByte();
                this._serviceNodeId = stream.ReadByte();
                this._sessionIdLength = stream.ReadByte();
                this._sessionId = stream.ReadBytes(this._sessionIdLength, 0xFF);

                if (!stream.IsAtEnd())
                {
                    int messageCount = stream.ReadVInt();

                    if (messageCount > 0)
                    {
                        this._messages.EnsureCapacity(messageCount);

                        for (int i = 0; i < messageCount; i++)
                        {
                            int messageType = stream.ReadVInt();
                            int encodingLength = stream.ReadVInt();
                            byte[] encodingByteArray = stream.ReadBytes(encodingLength, 0xFFFFFF);

                            NetMessage message = NetMessageFactory.CreateMessageByType(messageType);

                            if (message == null)
                            {
                                Logging.Warning(this, "NetPacket::decode ignoring message of unknown type " + messageType);
                                continue;
                            }

                            message.SetSessionId(this._sessionId, this._sessionIdLength);
                            message.SetServiceNodeType(this._serviceNodeType);
                            message.SetServiceNodeId(this._serviceNodeId);
                            message.GetByteStream().SetByteArray(encodingByteArray, encodingLength);

                            this._messages.Add(message);
                        }
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
            stream.WriteByte(this._serviceNodeType);
            stream.WriteByte(this._serviceNodeId);
            stream.WriteByte(this._sessionIdLength);
            stream.WriteBytesWithoutLength(this._sessionId, this._sessionIdLength);

            if (this._messages.Count != 0)
            {
                stream.WriteVInt(this._messages.Count);

                for (int i = 0; i < this._messages.Count; i++)
                {
                    NetMessage message = this._messages[i];

                    int encodingLength = message.GetEncodingLength();
                    byte[] encodingByteArray = message.GetMessageBytes();

                    stream.WriteVInt(message.GetMessageType());
                    stream.WriteVInt(encodingLength);
                    stream.WriteBytesWithoutLength(encodingByteArray, encodingLength);
                }
            }
        }

        /// <summary>
        ///     Gets the service node type.
        /// </summary>
        internal int GetServiceNodeType()
        {
            return this._serviceNodeType;
        }

        /// <summary>
        ///     Sets the service node type.
        /// </summary>
        internal void SetServiceNodeType(int value)
        {
            if (value > 0xFF)
            {
                Logging.Warning(this, "NetPacket::setServiceNodeType service node type too big (" + value + ")");
            }

            this._serviceNodeType = (byte) value;
        }

        /// <summary>
        ///     Gets the service node id.
        /// </summary>
        internal int GetServiceNodeId()
        {
            return this._serviceNodeId;
        }

        /// <summary>
        ///     Sets the service node id.
        /// </summary>
        internal void SetServiceNodeId(int value)
        {
            if (value > 0xFF)
            {
                Logging.Warning(this, "NetPacket::setServiceNodeId service node id too big (" + value + ")");
            }

            this._serviceNodeId = (byte) value;
        }

        /// <summary>
        ///     Gets the session id length.
        /// </summary>
        internal byte GetSessionIdLength()
        {
            return this._sessionIdLength;
        }

        /// <summary>
        ///     Removes the session id.
        /// </summary>
        internal byte[] RemoveSessionId()
        {
            byte[] tmp = this._sessionId;
            this._sessionId = null;
            this._sessionIdLength = 0;
            return tmp;
        }

        /// <summary>
        ///     Sets the session id.
        /// </summary>
        internal void SetSessionId(byte[] value, int length)
        {
            if (length > 0xFF)
            {
                Logging.Warning(this, "NetPacket::setSessionId session length too big (" + value + ")");
            }

            this._sessionId = value;
            this._sessionIdLength = (byte) length;
        }

        /// <summary>
        ///     Adds the specified <see cref="NetMessage"/>.
        /// </summary>
        internal void AddMessage(NetMessage message)
        {
            this._messages.Add(message);
        }

        /// <summary>
        ///     Gets received messages.
        /// </summary>
        internal LogicArrayList<NetMessage> GetNetMessages()
        {
            return this._messages;
        }

        /// <summary>
        ///     Gets the <see cref="NetMessage"/> count.
        /// </summary>
        internal int GetNetMessageCount()
        {
            return this._messages.Count;
        }
    }
}