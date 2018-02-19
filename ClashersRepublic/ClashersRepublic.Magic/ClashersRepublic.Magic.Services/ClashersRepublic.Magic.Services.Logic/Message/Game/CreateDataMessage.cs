namespace ClashersRepublic.Magic.Services.Logic.Message.Game
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateDataMessage : ServiceMessage
    {
        public LogicLong Id;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateDataMessage"/> class.
        /// </summary>
        public CreateDataMessage() : base()
        {
            // CreateDataMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.Id = this.Stream.ReadLong();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this.Id);
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
            return 10100;
        }
    }
}