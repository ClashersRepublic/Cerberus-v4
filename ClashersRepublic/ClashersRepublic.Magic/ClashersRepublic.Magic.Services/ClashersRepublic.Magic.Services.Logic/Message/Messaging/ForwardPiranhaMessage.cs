namespace ClashersRepublic.Magic.Services.Logic.Message.Messaging
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Message;

    public class ForwardPiranhaMessage : ServiceMessage
    {
        public PiranhaMessage PiranhaMessage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardPiranhaMessage"/> class.
        /// </summary>
        public ForwardPiranhaMessage() : base()
        {
            // ForwardLogicMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            if (this.Stream.ReadBoolean())
            {
                short messageType = this.Stream.ReadShort();
                short messageVersion = this.Stream.ReadShort();
                int messageLength = this.Stream.ReadVInt();
                byte[] messageEncoding = this.Stream.ReadBytes(messageLength, 0xFFFFFF);

                this.PiranhaMessage = LogicMagicMessageFactory.CreateMessageByType(messageType);

                if (this.PiranhaMessage == null)
                {
                    Debugger.Error("ForwardLogicMessage::decode message is NULL");
                }
                else
                {
                    this.PiranhaMessage.SetMessageVersion(messageVersion);
                    this.PiranhaMessage.GetByteStream().SetByteArray(messageEncoding, messageLength);
                    this.PiranhaMessage.GetByteStream().SetOffset(messageLength);
                }
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            if (this.PiranhaMessage == null)
            {
                this.Stream.WriteBoolean(false);
            }
            else
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteShort(this.PiranhaMessage.GetMessageType());
                this.Stream.WriteShort(this.PiranhaMessage.GetMessageVersion());
                this.Stream.WriteVInt(this.PiranhaMessage.GetEncodingLength());
                this.Stream.WriteBytesWithoutLength(this.PiranhaMessage.GetByteStream().GetByteArray(), this.PiranhaMessage.GetEncodingLength());
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this.PiranhaMessage = null;
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10300;
        }
    }
}