namespace RivieraStudio.Magic.Logic.Message.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Home;
    using RivieraStudio.Magic.Titan.Message;

    public class NpcDataMessage : PiranhaMessage
    {
        private LogicClientHome _clientHome;
        private LogicNpcAvatar _npcAvatar;
        private LogicClientAvatar _clientAvatar;

        private int _secondsSinceLastSave;
        private int _currentTimestamp;
        private bool _npcDuel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NpcDataMessage" /> class.
        /// </summary>
        public NpcDataMessage() : this(0)
        {
            // NpcDataMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="NpcDataMessage" /> class
        /// </summary>
        public NpcDataMessage(short messageVersion) : base(messageVersion)
        {
            // NpcDataMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._secondsSinceLastSave = this.Stream.ReadInt();
            this._currentTimestamp = this.Stream.ReadInt();
            this._clientHome = new LogicClientHome();
            this._clientHome.Decode(this.Stream);
            this._clientAvatar = new LogicClientAvatar();
            this._clientAvatar.Decode(this.Stream);
            this._npcAvatar = new LogicNpcAvatar();
            this._npcAvatar.Decode(this.Stream);
            this._npcDuel = this.Stream.ReadBoolean();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this._secondsSinceLastSave);
            this.Stream.WriteInt(this._currentTimestamp);
            this._clientHome.Encode(this.Stream);
            this._clientAvatar.Encode(this.Stream);
            this._npcAvatar.Encode(this.Stream);
            this.Stream.WriteBoolean(this._npcDuel);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24133;
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
            this._clientHome = null;
            this._clientAvatar = null;
            this._npcAvatar = null;
        }

        /// <summary>
        ///     Removes the <see cref="LogicClientHome"/> instance.
        /// </summary>
        public LogicClientHome RemoveLogicClientHome()
        {
            LogicClientHome tmp = this._clientHome;
            this._clientHome = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public void SetLogicClientHome(LogicClientHome instance)
        {
            this._clientHome = instance;
        }

        /// <summary>
        ///     Removes the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public LogicClientAvatar RemoveLogicClientAvatar()
        {
            LogicClientAvatar tmp = this._clientAvatar;
            this._clientAvatar = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public void SetLogicClientAvatar(LogicClientAvatar instance)
        {
            this._clientAvatar = instance;
        }

        /// <summary>
        ///     Removes the <see cref="LogicNpcAvatar"/> instance.
        /// </summary>
        public LogicNpcAvatar RemoveLogicNpcAvatar()
        {
            LogicNpcAvatar tmp = this._npcAvatar;
            this._npcAvatar = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicNpcAvatar"/> instance.
        /// </summary>
        public void SetLogicNpcAvatar(LogicNpcAvatar instance)
        {
            this._npcAvatar = instance;
        }

        /// <summary>
        ///     Gets the current timestamp.
        /// </summary>
        public int GetCurrentTimestamp()
        {
            return this._currentTimestamp;
        }

        /// <summary>
        ///     Sets the current timestamp.
        /// </summary>
        public void SetCurrentTimestamp(int value)
        {
            this._currentTimestamp = value;
        }
    }
}