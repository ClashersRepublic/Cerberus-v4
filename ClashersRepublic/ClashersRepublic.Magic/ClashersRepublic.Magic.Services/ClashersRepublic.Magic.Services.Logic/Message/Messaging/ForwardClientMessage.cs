﻿namespace ClashersRepublic.Magic.Services.Logic.Message.Messaging
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Message;

    public class ForwardClientMessage : ServiceMessage
    {
        public PiranhaMessage Message;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ForwardClientMessage"/> class.
        /// </summary>
        public ForwardClientMessage() : base(0)
        {
            // ForwardClientMessage.
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
                int messageEncodingLength = this.Stream.ReadVInt();
                byte[] messageBytes = this.Stream.ReadBytes(this.Stream.ReadBytesLength(), 0xFFFFFF);

                this.Message = LogicMagicMessageFactory.CreateMessageByType(messageType);

                if (this.Message == null)
                {
                    Debugger.Error("ForwardClientMessage::decode message is NULL");
                }
                else
                {
                    this.Message.SetMessageVersion(messageVersion);
                    this.Message.GetByteStream().SetByteArray(messageBytes, messageEncodingLength);
                    this.Message.GetByteStream().SetOffset(messageEncodingLength);
                }
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            if (this.Message == null)
            {
                this.Stream.WriteBoolean(false);
            }
            else
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteShort(this.Message.GetMessageType());
                this.Stream.WriteShort(this.Message.GetMessageVersion());
                this.Stream.WriteVInt(this.Message.GetEncodingLength());
                this.Stream.WriteBytes(this.Message.GetByteStream().GetByteArray(), this.Message.GetByteStream().GetOffset());
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this.Message = null;
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10140;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 0;
        }
    }
}