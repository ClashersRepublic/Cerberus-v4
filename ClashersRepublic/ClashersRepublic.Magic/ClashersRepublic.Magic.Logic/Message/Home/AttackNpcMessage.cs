namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Message;

    public class AttackNpcMessage : PiranhaMessage
    {
        private LogicNpcData _npcData;

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
            this._npcData = (LogicNpcData) this.Stream.ReadDataReference(16);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteDataReference(this._npcData);
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
            this._npcData = null;
        }

        /// <summary>
        ///     Gets the <see cref="LogicNpcData" /> data.
        /// </summary>
        public LogicNpcData GetNpcData()
        {
            return this._npcData;
        }

        /// <summary>
        ///     Sets the <see cref="LogicNpcData" /> data.
        /// </summary>
        public void SetNpcData(LogicNpcData data)
        {
            this._npcData = data;
        }
    }
}