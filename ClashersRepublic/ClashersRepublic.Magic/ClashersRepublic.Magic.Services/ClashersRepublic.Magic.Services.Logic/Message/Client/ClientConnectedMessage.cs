namespace ClashersRepublic.Magic.Services.Logic.Message.Client
{
    using ClashersRepublic.Magic.Titan.Math;

    public class ClientConnectedMessage : ServiceMessage
    {
        public LogicLong AccountId;
        public bool IsNewClient;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClientConnectedMessage"/> class.
        /// </summary>
        public ClientConnectedMessage() : base(0)
        {
            // ClientConnectedMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.AccountId = this.Stream.ReadLong();
            this.IsNewClient = this.Stream.ReadBoolean();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteLong(this.AccountId);
            this.Stream.WriteBoolean(this.IsNewClient);
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
            return 10198;
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