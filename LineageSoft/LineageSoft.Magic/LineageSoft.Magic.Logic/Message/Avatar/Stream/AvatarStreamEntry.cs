namespace LineageSoft.Magic.Logic.Message.Avatar.Stream
{
    using LineageSoft.Magic.Titan.DataStream;
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Math;

    public class AvatarStreamEntry
    {
        private LogicLong _id;
        private LogicLong _senderAvatarId;

        private string _senderName;

        private int _senderExpLevel;
        private int _senderLeagueType;
        private int _ageSeconds;

        private bool _new;
        private bool _dismiss;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarStreamEntry"/> class.
        /// </summary>
        public AvatarStreamEntry()
        {
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            this._id = null;
            this._senderAvatarId = null;
            this._senderName = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._id);

            if (this._senderAvatarId != null)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(this._senderAvatarId);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            encoder.WriteString(this._senderName);
            encoder.WriteInt(this._senderExpLevel);
            encoder.WriteInt(this._senderLeagueType);
            encoder.WriteInt(this._ageSeconds);
            encoder.WriteBoolean(this._dismiss);
            encoder.WriteBoolean(this._new);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode(ByteStream stream)
        {
            this._id = stream.ReadLong();

            if (stream.ReadBoolean())
            {
                this._senderAvatarId = stream.ReadLong();
            }

            this._senderName = stream.ReadString(900000);
            this._senderExpLevel = stream.ReadInt();
            this._senderLeagueType = stream.ReadInt();
            this._ageSeconds = stream.ReadInt();
            this._dismiss = stream.ReadBoolean();
            this._new = stream.ReadBoolean();
        }

        /// <summary>
        ///     Gets the avatar stream entry type.
        /// </summary>
        public virtual int GetAvatarStreamEntryType()
        {
            Debugger.Error("getAvatarStreamEntryType() must be overridden");
            return -1;
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public virtual void Save(LogicJSONObject jsonObject)
        {
            jsonObject.Put("id_hi", new LogicJSONNumber(this._id.GetHigherInt()));
            jsonObject.Put("id_lo", new LogicJSONNumber(this._id.GetLowerInt()));
            
            LogicJSONObject senderObject = new LogicJSONObject();

            if (this._senderAvatarId != null)
            {
                senderObject.Put("id_hi", new LogicJSONNumber(this._senderAvatarId.GetHigherInt()));
                senderObject.Put("id_lo", new LogicJSONNumber(this._senderAvatarId.GetLowerInt()));
            }

            senderObject.Put("name", new LogicJSONString(this._senderName));
            senderObject.Put("exp_lvl", new LogicJSONNumber(this._senderExpLevel));
            senderObject.Put("league_type", new LogicJSONNumber(this._senderLeagueType));
            senderObject.Put("age_secs", new LogicJSONNumber(this._ageSeconds));
            senderObject.Put("is_dismissed", new LogicJSONBoolean(this._dismiss));
            senderObject.Put("is_new", new LogicJSONBoolean(this._new));

            jsonObject.Put("sender", senderObject);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public virtual void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber idHigh = jsonObject.GetJSONNumber("id_hi");
            LogicJSONNumber idLow = jsonObject.GetJSONNumber("id_lo");

            if (idHigh != null)
            {
                if (idLow != null)
                {
                    this._id = new LogicLong(idHigh.GetIntValue(), idLow.GetIntValue());
                }
            }

            LogicJSONObject senderObject = jsonObject.GetJSONObject("sender");

            if (senderObject != null)
            {
                LogicJSONString nameObject = senderObject.GetJSONString("name");

                if (nameObject != null)
                {
                    this._senderName = nameObject.GetStringValue();
                }

                LogicJSONNumber expLevelObject = senderObject.GetJSONNumber("exp_lvl");

                if (expLevelObject != null)
                {
                    this._senderExpLevel = expLevelObject.GetIntValue();
                }

                LogicJSONNumber leagueTypeObject = senderObject.GetJSONNumber("league_type");

                if (leagueTypeObject != null)
                {
                    this._senderLeagueType = leagueTypeObject.GetIntValue();
                }

                LogicJSONNumber ageSecsObject = senderObject.GetJSONNumber("age_secs");

                if (ageSecsObject != null)
                {
                    this._ageSeconds = ageSecsObject.GetIntValue();
                }

                LogicJSONBoolean isDismissedObject = senderObject.GetJSONBoolean("is_dismissed");

                if (isDismissedObject != null)
                {
                    this._dismiss = isDismissedObject.IsTrue();
                }

                LogicJSONBoolean isNewObject = senderObject.GetJSONBoolean("is_new");

                if (isNewObject != null)
                {
                    this._new = isNewObject.IsTrue();
                }
            }
        }

        /// <summary>
        ///     Gets the instance id.
        /// </summary>
        public LogicLong GetId()
        {
            return this._id;
        }

        /// <summary>
        ///     Sets the instance id.
        /// </summary>
        public void SetId(LogicLong id)
        {
            this._id = id;
        }
    }
}