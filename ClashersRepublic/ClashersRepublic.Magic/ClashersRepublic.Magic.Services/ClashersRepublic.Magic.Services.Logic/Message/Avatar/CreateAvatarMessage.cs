namespace ClashersRepublic.Magic.Services.Logic.Message.Avatar
{
    using ClashersRepublic.Magic.Titan.Math;

    public class CreateAvatarMessage : MagicServiceMessage
    {
        public LogicLong AccountId;

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAvatarMessage" /> class.
        /// </summary>
        public CreateAvatarMessage() : this(0)
        {
            // CreateAvatarMessage.
        }

        /// <summary>
        ///     Initializes a new instance <see cref="CreateAvatarMessage" /> class.
        /// </summary>
        public CreateAvatarMessage(short messageVersion) : base(messageVersion)
        {
            // CreateAvatarMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return 10200;
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 2;
        }
    }
}