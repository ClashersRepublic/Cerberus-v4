namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Util;

    internal class NetPacket
    {
        private readonly LogicArrayList<NetMessage> _messages;
        private byte _protocolVersion;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetPacket" /> class.
        /// </summary>
        internal NetPacket()
        {
            this._protocolVersion = 1;
            this._messages = new LogicArrayList<NetMessage>();
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
            if (this._messages.Count != 0)
            {
                do
                {
                    this._messages[0].Destruct();
                    this._messages.Remove(0);
                } while (this._messages.Count != 0);
            }
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

            if (this._messages.Count != 0)
            {
                stream.WriteVInt(this._messages.Count);
                stream.EnsureCapacity(15 * this._messages.Count);

                for (int i = 0; i < this._messages.Count; i++)
                {
                    NetMessage message = this._messages[i];

                    int encodingLength = message.GetEncodingLength();
                    byte[] encodingByteArray = message.GetMessageBytes();

                    stream.EnsureCapacity(5 + encodingLength);
                    stream.WriteVInt(message.GetMessageType());
                    stream.WriteVInt(encodingLength);
                    stream.WriteBytesWithoutLength(encodingByteArray, encodingLength);
                }
            }
        }
        
        /// <summary>
        ///     Adds the specified <see cref="NetMessage" />.
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
        ///     Gets the <see cref="NetMessage" /> count.
        /// </summary>
        internal int GetNetMessageCount()
        {
            return this._messages.Count;
        }
    }
}