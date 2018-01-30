namespace ClashersRepublic.Magic.Services.Logic.Message.Session
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Titan.Message;

    public class ForwardClientMessage : MagicServiceMessage
    {
        public PiranhaMessage Message;

        /// <summary>
        ///     Initializes a new instance <see cref="ForwardClientMessage" /> class.
        /// </summary>
        public ForwardClientMessage() : this(0)
        {
            // ForwardClientMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="ForwardClientMessage" /> class.
        /// </summary>
        public ForwardClientMessage(short messageVersion) : base(messageVersion)
        {
            // ForwardClientMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            short messageType = this.Stream.ReadShort();
            short messageVersion = this.Stream.ReadShort();
            byte[] messageBytes = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 0xFFFFFF);

            if (messageType != 0)
            {
                this.Message = LogicMagicMessageFactory.Instance.CreateMessageByType(messageType);
                this.Message.SetMessageVersion(messageVersion);
                this.Message.GetByteStream().SetByteArray(messageBytes, messageBytes.Length);
                this.Message.GetByteStream().SetOffset(messageBytes.Length);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            if (this.Message != null)
            {
                this.Stream.WriteShort(this.Message.GetMessageType());
                this.Stream.WriteShort(this.Message.GetMessageVersion());

                byte[] messageBytes = this.Message.GetByteStream().GetBytes();

                this.Stream.WriteBytes(messageBytes, messageBytes.Length);
            }
            else
            {
                this.Stream.WriteShort(0);
                this.Stream.WriteShort(0);
                this.Stream.WriteBytes(null, 0);
            }
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10501;
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 15;
        }
    }
}