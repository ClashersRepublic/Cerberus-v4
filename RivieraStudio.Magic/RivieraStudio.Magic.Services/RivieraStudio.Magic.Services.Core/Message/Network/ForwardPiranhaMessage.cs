namespace RivieraStudio.Magic.Services.Core.Message.Network
{
    using RivieraStudio.Magic.Logic.Message;
    using RivieraStudio.Magic.Titan.Message;

    public class ForwardPiranhaMessage : NetMessage
    {
        private PiranhaMessage _message;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._message = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteVInt(this._message.GetMessageType());
            this.Stream.WriteVInt(this._message.GetEncodingLength());
            this.Stream.WriteBytesWithoutLength(this._message.GetMessageBytes(), this._message.GetEncodingLength());
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            int messageType = this.Stream.ReadVInt();
            int encodingLength = this.Stream.ReadVInt();
            byte[] messageBytes = this.Stream.ReadBytes(encodingLength, 0x7FFFFFFF);

            this._message = LogicMagicMessageFactory.Instance.CreateMessageByType(messageType);

            if (this._message != null)
            {
                this._message.GetByteStream().SetByteArray(messageBytes, encodingLength);
            }
            else
            {
                Logging.Warning("ForwardPiranhaMessage::decode ignoring message of unknown type " + messageType);
            }
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10400;
        }

        /// <summary>
        ///     Removes the <see cref="PiranhaMessage"/> instance.
        /// </summary>
        public PiranhaMessage RemovePiranhaMessage()
        {
            PiranhaMessage tmp = this._message;
            this._message = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="PiranhaMessage"/> instance.
        /// </summary>
        public void SetPiranhaMessage(PiranhaMessage message)
        {
            this._message = message;
        }
    }
}