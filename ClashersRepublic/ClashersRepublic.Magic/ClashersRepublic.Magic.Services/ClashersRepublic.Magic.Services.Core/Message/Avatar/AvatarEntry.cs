namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public class AvatarEntry
    {
        private LogicLong _avatarId;
        private LogicLong _homeId;
        private LogicLong _allianceId;

        private string _avatarName;
        private string _allianceName;
        private string _facebookId;

        private int _expLevel;
        private int _leagueType;
        private int _allianceExpLevel;
        private int _allianceRole;
        private int _badgeId;
        private int _nameChangeState;

        private bool _nameSetByUser;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarEntry"/> class.
        /// </summary>
        public AvatarEntry()
        {
            this._avatarId = new LogicLong();
            this._homeId = new LogicLong();
        }
        
        /// <summary>
        ///     Sets the data.
        /// </summary>
        public void SetData(LogicClientAvatar avatar)
        {
            this._avatarId = avatar.GetId();
            this._homeId = avatar.GetCurrentHomeId();
            this._avatarName = avatar.GetName();
            this._facebookId = avatar.GetFacebookId();
            this._expLevel = avatar.GetExpLevel();
            this._leagueType = avatar.GetLeagueType();
            this._nameChangeState = avatar.GetNameChangeState();
            this._nameSetByUser = avatar.GetNameSetByUser();

            if (avatar.IsInAlliance())
            {
                this._allianceId = avatar.GetAllianceId();
                this._allianceName = avatar.GetAllianceName();
                this._allianceExpLevel = avatar.GetAllianceExpLevel();
                this._allianceRole = avatar.GetAllianceRole();
                this._badgeId = avatar.GetAllianceBadge();
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
            this._facebookId = stream.ReadString(900000);
            this._expLevel = stream.ReadVInt();
            this._leagueType = stream.ReadVInt();
            this._nameChangeState = stream.ReadVInt();
            this._nameSetByUser = stream.ReadBoolean();

            if (stream.ReadBoolean())
            {
                this._allianceId = stream.ReadLong();
                this._allianceName = stream.ReadString(900000);
                this._allianceExpLevel = stream.ReadVInt();
                this._allianceRole = stream.ReadVInt();
                this._badgeId = stream.ReadVInt();
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._avatarId);
            encoder.WriteLong(this._homeId);
            encoder.WriteString(this._avatarName);
            encoder.WriteString(this._facebookId);
            encoder.WriteVInt(this._expLevel);
            encoder.WriteVInt(this._leagueType);
            encoder.WriteVInt(this._nameChangeState);
            encoder.WriteBoolean(this._nameSetByUser);

            if (this._allianceId != null)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(this._allianceId);
                encoder.WriteString(this._allianceName);
                encoder.WriteVInt(this._allianceExpLevel);
                encoder.WriteVInt(this._allianceRole);
                encoder.WriteVInt(this._badgeId);
            }
            else
            {
                encoder.WriteBoolean(false);
            }
        }

        /// <summary>
        ///     Copies all data of this entry to specified entry.
        /// </summary>
        public void CopyTo(AvatarEntry entry)
        {
            entry._avatarId = this._avatarId;
            entry._homeId = this._homeId;
            entry._avatarName = this._avatarName;
            entry._allianceName = this._allianceName;
            entry._facebookId = this._facebookId;
            entry._expLevel = this._expLevel;
            entry._leagueType = this._leagueType;
            entry._nameChangeState = this._nameChangeState;
            entry._nameSetByUser = this._nameSetByUser;
            entry._allianceId = this._allianceId;
            entry._allianceExpLevel = this._allianceExpLevel;
            entry._allianceRole = this._allianceRole;
            entry._badgeId = this._badgeId;
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
            return this._expLevel;
        }

        /// <summary>
        ///     Sets the avatar exp level.
        /// </summary>
        public void SetAvatarExpLevel(int lvl)
        {
            this._expLevel = lvl;
        }

        /// <summary>
        ///     Gets the avatar league type.
        /// </summary>
        public int GetAvatarLeagueType()
        {
            return this._leagueType;
        }

        /// <summary>
        ///     Sets the avatar league type.
        /// </summary>
        public void SetAvatarLeagueType(int leagueType)
        {
            this._leagueType = leagueType;
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
            return this._badgeId;
        }

        /// <summary>
        ///     Sets the alliance badge id.
        /// </summary>
        public void SetAllianceBadgeId(int id)
        {
            this._badgeId = id;
        }

        /// <summary>
        ///     Gets if the name is set by user.
        /// </summary>
        public bool GetNameSetByUser()
        {
            return this._nameSetByUser;
        }

        /// <summary>
        ///     Sets if the name is set by user.
        /// </summary>
        public void SetNameSetByUser(bool set)
        {
            this._nameSetByUser = set;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            this._avatarId = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "avatar_id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "avatar_id_lo"));
            this._homeId = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "home_id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "home_id_lo"));
            this._avatarName = LogicJSONHelper.GetJSONString(jsonObject, "name");
            this._facebookId = LogicJSONHelper.GetJSONString(jsonObject, "facebook_id");
            this._expLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "exp_lvl");
            this._leagueType = LogicJSONHelper.GetJSONNumber(jsonObject, "league_type");
            this._nameChangeState = LogicJSONHelper.GetJSONNumber(jsonObject, "name_change_state");
            this._nameSetByUser = LogicJSONHelper.GetJSONBoolean(jsonObject, "name_set");

            LogicJSONNumber allianceIdHigh = jsonObject.GetJSONNumber("alliance_id_hi");
            LogicJSONNumber allianceIdLow = jsonObject.GetJSONNumber("alliance_id_lo");

            if (allianceIdHigh != null && allianceIdLow != null)
            {
                this._allianceId = new LogicLong(allianceIdHigh.GetIntValue(), allianceIdLow.GetIntValue());
                this._allianceName = LogicJSONHelper.GetJSONString(jsonObject, "alliance_name");
                this._allianceExpLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "alliance_exp_lvl");
                this._allianceRole = LogicJSONHelper.GetJSONNumber(jsonObject, "alliance_role");
                this._badgeId = LogicJSONHelper.GetJSONNumber(jsonObject, "badge_id");
            }
            else
            {
                this._badgeId = -1;
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public LogicJSONObject Save()
        {
            LogicJSONObject jsonObject  = new LogicJSONObject();

            jsonObject.Put("avatar_id_hi", new LogicJSONNumber(this._avatarId.GetHigherInt()));
            jsonObject.Put("avatar_id_lo", new LogicJSONNumber(this._avatarId.GetLowerInt()));
            jsonObject.Put("home_id_hi", new LogicJSONNumber(this._homeId.GetHigherInt()));
            jsonObject.Put("home_id_lo", new LogicJSONNumber(this._homeId.GetLowerInt()));
            jsonObject.Put("name", new LogicJSONString(this._avatarName));
            jsonObject.Put("facebook_id", new LogicJSONString(this._facebookId));
            jsonObject.Put("exp_lvl", new LogicJSONNumber(this._expLevel));
            jsonObject.Put("league_type", new LogicJSONNumber(this._leagueType));
            jsonObject.Put("name_change_state", new LogicJSONNumber(this._nameChangeState));
            jsonObject.Put("name_set", new LogicJSONBoolean(this._nameSetByUser));

            if (this._allianceId != null)
            {
                jsonObject.Put("alliance_id_hi", new LogicJSONNumber(this._allianceId.GetHigherInt()));
                jsonObject.Put("alliance_id_lo", new LogicJSONNumber(this._allianceId.GetLowerInt()));
                jsonObject.Put("alliance_name", new LogicJSONString(this._allianceName));
                jsonObject.Put("alliance_exp_lvl", new LogicJSONNumber(this._allianceExpLevel));
                jsonObject.Put("alliance_role", new LogicJSONNumber(this._allianceRole));
                jsonObject.Put("badge_id", new LogicJSONNumber(this._badgeId));
            }

            return jsonObject;
        }
    }
}