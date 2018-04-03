namespace RivieraStudio.Magic.Logic.Message.Chat
{
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Message;

    public class GlobalChatLineMessage : PiranhaMessage
    {
        private string _message;
        private string _avatarName;
        private string _allianceName;
            
        private int _avatarExpLevel;
        private int _avatarLeagueType;
        private int _allianceBadgeId;

        private LogicLong _avatarId;
        private LogicLong _homeId;
        private LogicLong _allianceId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GlobalChatLineMessage" /> class.
        /// </summary>
        public GlobalChatLineMessage() : this(0)
        {
            // GlobalChatLineMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GlobalChatLineMessage" /> class.
        /// </summary>
        public GlobalChatLineMessage(short messageVersion) : base(messageVersion)
        {
            // GlobalChatLineMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._message = this.Stream.ReadString(900000);
            this._avatarName = this.Stream.ReadString(900000);
            this._avatarExpLevel = this.Stream.ReadInt();
            this._avatarLeagueType = this.Stream.ReadInt();
            this._avatarId = this.Stream.ReadLong();
            this._homeId = this.Stream.ReadLong();

            if (this.Stream.ReadBoolean())
            {
                this._allianceId = this.Stream.ReadLong();
                this._allianceName = this.Stream.ReadString(900000);
                this._allianceBadgeId = this.Stream.ReadInt();
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteString(this._message);
            this.Stream.WriteString(this._avatarName);
            this.Stream.WriteInt(this._avatarExpLevel);
            this.Stream.WriteInt(this._avatarLeagueType);
            this.Stream.WriteLong(this._avatarId);
            this.Stream.WriteLong(this._homeId);

            if (this._allianceId != null)
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteLong(this._allianceId);
                this.Stream.WriteString(this._allianceName);
                this.Stream.WriteInt(this._allianceBadgeId);
            }
            else
            {
                this.Stream.WriteBoolean(false);
            }
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24715;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 6;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._message = null;
            this._avatarName = null;
            this._allianceName = null;
            this._avatarId = null;
            this._homeId = null;
            this._allianceId = null;
        }

        /// <summary>
        ///     Removes the message.
        /// </summary>
        public string RemoveMessage(string message)
        {
            string tmp = this._message;
            this._message = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the message.
        /// </summary>
        public void SetMessage(string message)
        {
            this._message = message;
        }

        /// <summary>
        ///     Gets the avatar name.
        /// </summary>
        public string GetAvatarName()
        {
            return this._avatarName;
        }

        /// <summary>
        ///     Sets the avatar name.
        /// </summary>
        public void SetAvatarName(string name)
        {
            this._avatarName = name;
        }

        /// <summary>
        ///     Gets the alliance name;
        /// </summary>
        public string GetAllianceName()
        {
            return this._allianceName;
        }

        /// <summary>
        ///     Sets the alliance name.
        /// </summary>
        public void SetAllianceName(string name)
        {
            this._allianceName = name;
        }

        /// <summary>
        ///     Gets the avatar exp level.
        /// </summary>
        public int GetAvatarExpLevel()
        {
            return this._avatarExpLevel;
        }

        /// <summary>
        ///     Sets the avatar exp level.
        /// </summary>
        public void SetAvatarExpLevel(int lvl)
        {
            this._avatarExpLevel = lvl;
        }

        /// <summary>
        ///     Gets the avatar league type.
        /// </summary>
        public int GetAvatarLeagueType()
        {
            return this._avatarLeagueType;
        }

        /// <summary>
        ///     Sets the avatar league type.
        /// </summary>
        public void SetAvatarLeagueType(int leagueType)
        {
            this._avatarLeagueType = leagueType;
        }

        /// <summary>
        ///     Gets the avatar id.
        /// </summary>
        public LogicLong GetAvatarId()
        {
            return this._avatarId;
        }

        /// <summary>
        ///     Sets the avatar id.
        /// </summary>
        public void SetAvatarId(LogicLong id)
        {
            this._avatarId = id;
        }

        /// <summary>
        ///     Gets the home id.
        /// </summary>
        public LogicLong GetHomeId()
        {
            return this._homeId;
        }

        /// <summary>
        ///     Sets the home id.
        /// </summary>
        public void SetHomeId(LogicLong id)
        {
            this._homeId = id;
        }

        /// <summary>
        ///     Gets the alliance id.
        /// </summary>
        public LogicLong GetAllianceId()
        {
            return this._allianceId;
        }

        /// <summary>
        ///     Sets the alliance id.
        /// </summary>
        public void SetAllianceId(LogicLong id)
        {
            this._allianceId = id;
        }

        /// <summary>
        ///     Gets the alliance badge id.
        /// </summary>
        public int GetAllianceBadgeId()
        {
            return this._allianceBadgeId;
        }

        /// <summary>
        ///     Sets the alliance badge id.
        /// </summary>
        public void SetAllianceBadgeId(int id)
        {
            this._allianceBadgeId = id;
        }
    }
}