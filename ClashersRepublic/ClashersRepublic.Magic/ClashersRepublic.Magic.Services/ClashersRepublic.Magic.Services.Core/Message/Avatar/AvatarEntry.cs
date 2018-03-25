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
        private LogicLong _allianceId;

        private string _avatarName;
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
        }
        
        /// <summary>
        ///     Sets the data.
        /// </summary>
        public void SetData(LogicClientAvatar avatar)
        {
            this._avatarId = avatar.GetId();
            this._avatarName = avatar.GetName();
            this._facebookId = avatar.GetFacebookId();
            this._expLevel = avatar.GetExpLevel();
            this._leagueType = avatar.GetLeagueType();
            this._nameChangeState = avatar.GetNameChangeState();
            this._nameSetByUser = avatar.GetNameSetByUser();

            if (avatar.IsInAlliance())
            {
                this._allianceId = avatar.GetAllianceId();
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
            this._avatarName = stream.ReadString(900000);
            this._facebookId = stream.ReadString(900000);
            this._expLevel = stream.ReadVInt();
            this._leagueType = stream.ReadVInt();
            this._nameChangeState = stream.ReadVInt();
            this._nameSetByUser = stream.ReadBoolean();

            if (stream.ReadBoolean())
            {
                this._allianceId = stream.ReadLong();
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
            entry._avatarName = this._avatarName;
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
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            this._avatarId = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "avatar_id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "avatar_id_lo"));
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
                jsonObject.Put("alliance_exp_lvl", new LogicJSONNumber(this._allianceExpLevel));
                jsonObject.Put("alliance_role", new LogicJSONNumber(this._allianceRole));
                jsonObject.Put("badge_id", new LogicJSONNumber(this._badgeId));
            }

            return jsonObject;
        }
    }
}