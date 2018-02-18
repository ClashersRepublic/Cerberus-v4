﻿namespace ClashersRepublic.Magic.Services.Logic.Message.Session
{
    public class ServiceNodeUnboundedToSessionMessage : ServiceMessage
    {
        public int ServiceNodeType;
        public int ServiceNodeId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceNodeUnboundedToSessionMessage"/> class.
        /// </summary>
        public ServiceNodeUnboundedToSessionMessage() : base()
        {
            // ServiceNodeUnboundedToSessionMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.ServiceNodeType = this.Stream.ReadVInt();
            this.ServiceNodeId = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteVInt(this.ServiceNodeType);
            this.Stream.WriteVInt(this.ServiceNodeId);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 10202;
        }
    }
}