namespace RivieraStudio.Magic.Titan.Message.Udp
{
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;

    public class UdpMessage
    {
        private int _messageId;
        private PiranhaMessage _piranhaMessage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UdpMessage" /> class.
        /// </summary>
        public UdpMessage()
        {
            // UdpMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="UdpMessage" /> class.
        /// </summary>
        public UdpMessage(byte messageId)
        {
            this._messageId = messageId;
        }

        /// <summary>
        ///     Gets the message id.
        /// </summary>
        public int GetMessageId()
        {
            return this._messageId;
        }

        /// <summary>
        ///     Gets the piranha message.
        /// </summary>
        public PiranhaMessage GetPiranhaMessage()
        {
            return this._piranhaMessage;
        }

        /// <summary>
        ///     Removes the piranha message.
        /// </summary>
        public PiranhaMessage RemovePiranhaMessage()
        {
            PiranhaMessage message = this._piranhaMessage;
            this._piranhaMessage = null;
            return message;
        }

        /// <summary>
        ///     Sets the piranha message.
        /// </summary>
        public void SetPiranhaMessage(PiranhaMessage message)
        {
            this._piranhaMessage = message;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream, LogicMessageFactory factory)
        {
            this._messageId = stream.ReadByte();
            int messageType = stream.ReadVInt();
            this._piranhaMessage = factory.CreateMessageByType(messageType);

            if (this._piranhaMessage != null)
            {
                int encodingLength = stream.ReadVInt();
                this._piranhaMessage.GetByteStream().SetByteArray(stream.ReadBytes(encodingLength, 900000), encodingLength);
            }
            else
            {
                Debugger.Warning("UdpMessage::decode unable to read message type " + messageType);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ByteStream stream)
        {
            int encodingLength = this._piranhaMessage.GetEncodingLength();

            stream.WriteByte((byte) this._messageId);
            stream.WriteVInt(this._piranhaMessage.GetMessageType());
            stream.WriteVInt(encodingLength);
            stream.WriteBytesWithoutLength(this._piranhaMessage.GetByteStream().GetByteArray(), encodingLength);
        }

        /// <summary>
        ///     Gets if this message is more recent than the specified message.
        /// </summary>
        public bool IsMoreRecent(char messageId)
        {
            return this._messageId > messageId;
        }
    }
}