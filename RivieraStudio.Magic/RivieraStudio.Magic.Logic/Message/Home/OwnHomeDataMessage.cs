namespace RivieraStudio.Magic.Logic.Message.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Home;
    using RivieraStudio.Magic.Titan.Message;

    public class OwnHomeDataMessage : PiranhaMessage
    {
        private int _elapsedSeconds;
        private int _currentTimestamp;
        private int _secondsSinceLastSave;

        private LogicClientAvatar _logicClientAvatar;
        private LogicClientHome _logicClientHome;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OwnHomeDataMessage" /> class.
        /// </summary>
        public OwnHomeDataMessage() : this(0)
        {
            // OwnHomeDataMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OwnHomeDataMessage" /> class.
        /// </summary>
        public OwnHomeDataMessage(short messageVersion) : base(messageVersion)
        {
            // OwnHomeDataMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._secondsSinceLastSave = this.Stream.ReadInt();
            this._elapsedSeconds = this.Stream.ReadInt();
            this._currentTimestamp = this.Stream.ReadInt();

            this._logicClientHome = new LogicClientHome();
            this._logicClientHome.Decode(this.Stream);
            this._logicClientAvatar = new LogicClientAvatar();
            this._logicClientAvatar.Decode(this.Stream);

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            /* sub_36BCBC - START */

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            this.Stream.ReadInt();
            this.Stream.ReadInt();

            /* sub_36BCBC - END */

            this.Stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this._secondsSinceLastSave);
            this.Stream.WriteInt(this._elapsedSeconds);
            this.Stream.WriteInt(this._currentTimestamp);

            this._logicClientHome.Encode(this.Stream);
            this._logicClientAvatar.Encode(this.Stream);

            this.Stream.WriteInt(0);
            this.Stream.WriteInt(0);

            this.Stream.WriteInt(352);
            this.Stream.WriteInt(1190797808);

            this.Stream.WriteInt(352);
            this.Stream.WriteInt(1192597808);

            this.Stream.WriteInt(352);
            this.Stream.WriteInt(1192597808);

            this.Stream.WriteInt(0);
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24101;
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

            this._logicClientHome = null;
            this._logicClientAvatar = null;
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

        /// <summary>
        ///     Gets the seconds since last save.
        /// </summary>
        public int GetSecondsSinceLastSave()
        {
            return this._secondsSinceLastSave;
        }

        /// <summary>
        ///     Sets the seconds since last save.
        /// </summary>
        public void SetSecondsSinceLastSave(int value)
        {
            this._secondsSinceLastSave = value;
        }

        /// <summary>
        ///     Gets the random seed
        /// </summary>
        public int GetElapsedSecs()
        {
            return this._elapsedSeconds;
        }

        /// <summary>
        ///     Sets the elapsed secs.
        /// </summary>
        public void SetElapsedSecs(int value)
        {
            this._elapsedSeconds = value;
        }

        /// <summary>
        ///     Removes the <see cref="LogicClientHome" /> instance.
        /// </summary>
        public LogicClientHome RemoveLogicClientHome()
        {
            LogicClientHome tmp = this._logicClientHome;
            this._logicClientHome = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientHome" /> instance.
        /// </summary>
        public void SetLogicClientHome(LogicClientHome logicClientHome)
        {
            this._logicClientHome = logicClientHome;
        }

        /// <summary>
        ///     Removes the <see cref="LogicClientAvatar" /> instance.
        /// </summary>
        public LogicClientAvatar RemoveLogicClientAvatar()
        {
            LogicClientAvatar tmp = this._logicClientAvatar;
            this._logicClientAvatar = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar" /> instance.
        /// </summary>
        public void SetLogicClientAvatar(LogicClientAvatar logicClientAvatar)
        {
            this._logicClientAvatar = logicClientAvatar;
        }
    }
}
 
 