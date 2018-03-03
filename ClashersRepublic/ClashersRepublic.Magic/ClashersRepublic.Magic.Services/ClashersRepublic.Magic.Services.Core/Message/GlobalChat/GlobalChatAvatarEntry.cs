namespace ClashersRepublic.Magic.Services.Core.Message.GlobalChat
{
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Math;

    public class GlobalChatAvatarEntry
    {
        private LogicLong _avatarId;
        private LogicLong _homeId;
        private LogicLong _allianceId;

        private string _avatarName;
        private string _allianceName;

        private int _avatarExpLevel;
        private int _avatarLeagueType;
        private int _allianceBadgeId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GlobalChatAvatarEntry"/> class.
        /// </summary>
        public GlobalChatAvatarEntry()
        {
            this._avatarId = new LogicLong();
            this._homeId = new LogicLong();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._avatarId = null;
            this._homeId = null;
            this._allianceId = null;
            this._avatarName = null;
            this._allianceName = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._avatarId);
            encoder.WriteLong(this._homeId);
            encoder.WriteString(this._avatarName);
            encoder.WriteVInt(this._avatarExpLevel);
            encoder.WriteVInt(this._avatarLeagueType);

            if (this._allianceId != null)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(this._allianceId);
                encoder.WriteString(this._allianceName);
                encoder.WriteVInt(this._allianceBadgeId);
            }
            else
            {
                encoder.WriteBoolean(false);
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._avatarId = stream.ReadLong();
            this._homeId = stream.ReadLong();
            this._avatarName = stream.ReadString(900000);
            this._avatarExpLevel = stream.ReadVInt();
            this._avatarLeagueType = stream.ReadVInt();

            if (stream.ReadBoolean())
            {
                this._allianceId = stream.ReadLong();
                this._allianceName = stream.ReadString(900000);
                this._allianceBadgeId = stream.ReadVInt();
            }
        }

        /// <summary>
        ///     Gets if the avatar is in an alliance.
        /// </summary>
        public bool InAlliance()
        {
            return this._allianceId != null;
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
        public void SetAvatarId(LogicLong value)
        {
            this._avatarId = value;
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
        public void SetHomeId(LogicLong value)
        {
            this._homeId = value;
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
        public void SetAvatarName(string value)
        {
            this._avatarName = value;
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
        public void SetAvatarExpLevel(int value)
        {
            this._avatarExpLevel = value;
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
        public void SetAvatarLeagueType(int value)
        {
            this._avatarLeagueType = value;
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
        public void SetAllianceId(LogicLong value)
        {
            this._allianceId = value;
        }

        /// <summary>
        ///     Gets the alliance name.
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
        ///     Gets the alliance badge id.
        /// </summary>
        public int GetAllianceBadgeId()
        {
            return this._allianceBadgeId;
        }

        /// <summary>
        ///     Sets the alliance badge id.
        /// </summary>
        public void SetAllianceBadgeId(int value)
        {
            this._allianceBadgeId = value;
        }
    }
}