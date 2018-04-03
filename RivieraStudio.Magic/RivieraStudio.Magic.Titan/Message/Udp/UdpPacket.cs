namespace RivieraStudio.Magic.Titan.Message.Udp
{
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

    public class UdpPacket
    {
        private readonly LogicArrayList<UdpMessage> _messages;

        private byte[] _ackMessageIds;
        private int _ackMessageIdCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UdpPacket" /> class.
        /// </summary>
        public UdpPacket()
        {
            this._messages = new LogicArrayList<UdpMessage>();
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(byte[] buffer, int length, LogicMessageFactory factory)
        {
            ByteStream stream = new ByteStream(buffer, length);

            if (!stream.IsAtEnd())
            {
                this._ackMessageIdCount = stream.ReadVInt();

                if (this._ackMessageIdCount <= 1400)
                {
                    this._ackMessageIds = stream.ReadBytes(this._ackMessageIdCount, 1400);

                    if (!stream.IsAtEnd())
                    {
                        int messageCount = stream.ReadVInt();

                        if (messageCount <= 1400)
                        {
                            this._messages.EnsureCapacity(messageCount);

                            for (int i = 0; i < messageCount; i++)
                            {
                                UdpMessage message = new UdpMessage();

                                message.Decode(stream, factory);

                                if (message.GetPiranhaMessage() == null)
                                {
                                    break;
                                }

                                this._messages.Add(message);
                            }
                        }
                    }
                }
            }

            stream.Destruct();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ByteStream byteStream)
        {
            if (this._ackMessageIdCount != 0 || this._messages.Count != 0)
            {
                byteStream.WriteVInt(this._ackMessageIdCount);
                byteStream.WriteBytes(this._ackMessageIds, this._ackMessageIdCount);

                ByteStream stream = new ByteStream(1400 - byteStream.GetOffset());

                if (this._messages.Count > 0)
                {
                    int streamLength = 0;
                    int encodedMessage = 0;

                    for (int i = this._messages.Count - 1; i >= 0; i--, encodedMessage++, streamLength = stream.GetLength())
                    {
                        this._messages[i].Encode(stream);

                        if (stream.GetLength() + byteStream.GetLength() > 1410)
                        {
                            Debugger.Warning("UdpPacket::encode over max size");
                            break;
                        }
                    }

                    if (encodedMessage > 0)
                    {
                        stream.WriteVInt(encodedMessage);
                        stream.WriteBytes(stream.GetByteArray(), streamLength);
                    }
                }

                stream.Destruct();

                if (byteStream.GetLength() > 1400)
                {
                    Debugger.Warning("UdpPacket::encode too big");
                }
            }
        }

        /// <summary>
        ///     Adds the specified message.
        /// </summary>
        public void AddMessage(UdpMessage message)
        {
            this._messages.Add(message);
        }

        /// <summary>
        ///     Gets messages.
        /// </summary>
        public LogicArrayList<UdpMessage> GetMessages()
        {
            return this._messages;
        }

        /// <summary>
        ///     Gets ack message ids.
        /// </summary>
        public byte[] GetAckMessageIds()
        {
            return this._ackMessageIds;
        }

        /// <summary>
        ///     Gets the ack message id count.
        /// </summary>
        public int GetAckMessageIdCount()
        {
            return this._ackMessageIdCount;
        }

        /// <summary>
        ///     Sets ack message ids.
        /// </summary>
        public void SetAckMessageIds(byte[] ackMessageIds, int count)
        {
            this._ackMessageIds = ackMessageIds;
            this._ackMessageIdCount = count;
        }
    }
}