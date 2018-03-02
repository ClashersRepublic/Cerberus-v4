namespace ClashersRepublic.Magic.Logic.Home
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home.Change;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicClientHome
    {
        private LogicLong _homeId;
        private LogicHomeChangeListener _listener;

        private int _shieldDurationSeconds;
        private int _guardDurationSeconds;
        private int _nextMaintenanceSeconds;

        private LogicCompressibleString _compressibleHomeJson;
        private LogicCompressibleString _compressibleGlobalJson;
        private LogicCompressibleString _compressibleCalendarJson;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientHome" /> class.
        /// </summary>
        public LogicClientHome()
        {
            this._compressibleHomeJson = new LogicCompressibleString();
            this._compressibleGlobalJson = new LogicCompressibleString();
            this._compressibleCalendarJson = new LogicCompressibleString();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._compressibleGlobalJson != null)
            {
                this._compressibleGlobalJson.Destruct();
                this._compressibleGlobalJson = null;
            }

            if (this._compressibleCalendarJson != null)
            {
                this._compressibleCalendarJson.Destruct();
                this._compressibleCalendarJson = null;
            }

            if (this._compressibleHomeJson != null)
            {
                this._compressibleHomeJson.Destruct();
                this._compressibleHomeJson = null;
            }

            if (this._listener != null)
            {
                this._listener.Destruct();
                this._listener = null;
            }

            this._homeId = null;
        }

        /// <summary>
        ///     Inits this instance.
        /// </summary>
        public void Init()
        {
            this._homeId = new LogicLong();
            this._listener = new LogicHomeChangeListener();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._homeId);

            encoder.WriteInt(this._shieldDurationSeconds);
            encoder.WriteInt(this._guardDurationSeconds);
            encoder.WriteInt(this._nextMaintenanceSeconds);

            this._compressibleHomeJson.Encode(encoder);
            this._compressibleCalendarJson.Encode(encoder);
            this._compressibleGlobalJson.Encode(encoder);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._homeId = stream.ReadLong();
            this._shieldDurationSeconds = stream.ReadInt();
            this._guardDurationSeconds = stream.ReadInt();
            this._nextMaintenanceSeconds = stream.ReadInt();

            this._compressibleHomeJson.Decode(stream);
            this._compressibleCalendarJson.Decode(stream);
            this._compressibleGlobalJson.Decode(stream);
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
        ///     Gets the total shield duration in secs.
        /// </summary>
        public int GetShieldDurationSeconds()
        {
            return this._shieldDurationSeconds;
        }

        /// <summary>
        ///     Gets the total guard duration in secs.
        /// </summary>
        public int GetGuardDurationSeconds()
        {
            return this._shieldDurationSeconds;
        }

        /// <summary>
        ///     Gets the time before the next maintenance.
        /// </summary>
        public int GetNextMaintenanceSeconds()
        {
            return this._nextMaintenanceSeconds;
        }

        /// <summary>
        ///     Gets the compressible calendar json.
        /// </summary>
        public LogicCompressibleString GetCompressibleCalendarJSON()
        {
            return this._compressibleCalendarJson;
        }

        /// <summary>
        ///     Gets the compressible global json.
        /// </summary>
        public LogicCompressibleString GetCompressibleGlobalJSON()
        {
            return this._compressibleGlobalJson;
        }

        /// <summary>
        ///     Gets the compressible home json.
        /// </summary>
        public LogicCompressibleString GetCompressibleHomeJSON()
        {
            return this._compressibleHomeJson;
        }

        /// <summary>
        ///     Gets the home json.
        /// </summary>
        public string GetHomeJSON()
        {
            return this._compressibleHomeJson.Get();
        }

        /// <summary>
        ///     Sets the home json.
        /// </summary>
        public void SetHomeJSON(string json)
        {
            this._compressibleHomeJson.Set(json);
        }

        /// <summary>
        ///     Gets the calendar json.
        /// </summary>
        public string GetCalendarJSON()
        {
            return this._compressibleCalendarJson.Get();
        }

        /// <summary>
        ///     Sets the calendar json.
        /// </summary>
        public void SetCalendarJSON(string json)
        {
            this._compressibleCalendarJson.Set(json);
        }

        /// <summary>
        ///     Gets the global json.
        /// </summary>
        public string GetGlobalJSON()
        {
            return this._compressibleGlobalJson.Get();
        }

        /// <summary>
        ///     Sets the global json.
        /// </summary>
        public void SetGlobalJSON(string json)
        {
            this._compressibleGlobalJson.Set(json);
        }

        /// <summary>
        ///     Sets the shield duration time.
        /// </summary>
        public void SetShieldDurationSeconds(int secs)
        {
            this._shieldDurationSeconds = secs;
        }

        /// <summary>
        ///     Sets the guard duration time.
        /// </summary>
        public void SetGuardDurationSeconds(int secs)
        {
            this._guardDurationSeconds = secs;
        }

        /// <summary>
        ///     Sets the next maintenance time.
        /// </summary>
        public void SetNextMaintenanceSeconds(int secs)
        {
            this._nextMaintenanceSeconds = secs;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject json)
        {
            this._homeId = new LogicLong(LogicJSONHelper.GetJSONNumber(json, "home_id_high"), LogicJSONHelper.GetJSONNumber(json, "home_id_low"));
            this._shieldDurationSeconds = LogicJSONHelper.GetJSONNumber(json, "shield_duration_secs");
            this._guardDurationSeconds = LogicJSONHelper.GetJSONNumber(json, "guard_duration_secs");
            this._nextMaintenanceSeconds = LogicJSONHelper.GetJSONNumber(json, "next_maintenance_secs");

            LogicJSONObject level = json.GetJSONObject("level");

            if (level != null)
            {
                this._compressibleHomeJson.Load(level);
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("home_id_high", new LogicJSONNumber(this._homeId.GetHigherInt()));
            jsonObject.Put("home_id_low", new LogicJSONNumber(this._homeId.GetLowerInt()));
            jsonObject.Put("shield_duration_secs", new LogicJSONNumber(this._shieldDurationSeconds));
            jsonObject.Put("guard_duration_secs", new LogicJSONNumber(this._guardDurationSeconds));
            jsonObject.Put("next_maintenance_secs", new LogicJSONNumber(this._nextMaintenanceSeconds));

            LogicJSONObject level = new LogicJSONObject();
            this._compressibleHomeJson.Save(level);
            jsonObject.Put("level", level);

            return jsonObject;
        }
    }
}