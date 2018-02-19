namespace ClashersRepublic.Magic.Logic.Message.Battle
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Message;

    public class AttackNpcMessage : PiranhaMessage
    {
        public LogicData LogicNpcData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AttackNpcMessage" /> class.
        /// </summary>
        public AttackNpcMessage() : this(0)
        {
            // AttackNpcMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AttackNpcMessage" /> class
        /// </summary>
        public AttackNpcMessage(short messageVersion) : base(messageVersion)
        {
            // AttackNpcMessage.
        }
        
        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this.LogicNpcData =  this.Stream.ReadDataReference(16);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteDataReference(this.LogicNpcData);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 14134;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this.LogicNpcData = null;
        }
    }
}
