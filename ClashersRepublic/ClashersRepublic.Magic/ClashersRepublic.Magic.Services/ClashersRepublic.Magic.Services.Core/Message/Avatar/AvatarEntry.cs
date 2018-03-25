namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Titan.DataStream;
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
            this._avatarId = entry._avatarId;
            this._avatarName = entry._avatarName;
            this._facebookId = entry._facebookId;
            this._expLevel = entry._expLevel;
            this._leagueType = entry._leagueType;
            this._allianceId = entry._allianceId;
            this._allianceExpLevel = entry._allianceExpLevel;
            this._allianceRole = entry._allianceRole;
            this._badgeId = entry._badgeId;
        }
    }
}