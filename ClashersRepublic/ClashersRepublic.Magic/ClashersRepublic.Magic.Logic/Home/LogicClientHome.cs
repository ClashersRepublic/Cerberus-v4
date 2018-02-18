namespace ClashersRepublic.Magic.Logic.Home
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Utils;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicClientHome
    {
        private LogicLong _homeId;
        
        private int _shieldDurationSeconds;
        private int _guardDurationSeconds;
        private int _nextMaintenanceSeconds;

        private string _homeJSON;

        private LogicCompressibleString _compressibleGlobalJson;
        private LogicCompressibleString _compressibleCalandarJson;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientHome"/> class.
        /// </summary>
        public LogicClientHome()
        {
            this._homeJSON = string.Empty;
            this._compressibleGlobalJson = new LogicCompressibleString();
            this._compressibleCalandarJson = new LogicCompressibleString();
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

            if (this._homeJSON != null)
            {
                ZLibHelper.ConpressInZLibFormat(LogicStringUtil.GetBytes(this._homeJSON), out byte[] compressed);

                encoder.WriteBoolean(true);
                encoder.WriteBytes(compressed, compressed.Length);
            }
            else
            {
                encoder.WriteBoolean(false);
                encoder.WriteString(this._homeJSON);
            }

            this._compressibleCalandarJson.Encode(encoder);
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

            if (stream.ReadBoolean())
            {
                int byteArrayLength = stream.ReadBytesLength();
                byte[] byteArray = stream.ReadBytes(byteArrayLength, 900000);

                ZLibHelper.DecompressInMySQLFormat(byteArray, byteArrayLength, out byte[] decompressed);

                this._homeJSON = LogicStringUtil.CreateString(decompressed, 0, decompressed.Length);
            }
            else
            {
                this._homeJSON = stream.ReadString(900000);
            }

            this._compressibleCalandarJson.Decode(stream);
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
        ///     Gets the compressible calandar json.
        /// </summary>
        public LogicCompressibleString GetCompressibleCalandarJSON()
        {
            return this._compressibleCalandarJson;
        }

        /// <summary>
        ///     Gets the compressible global json.
        /// </summary>
        public LogicCompressibleString GetCompressibleGlobalJSON()
        {
            return this._compressibleGlobalJson;
        }

        /// <summary>
        ///     Gets the home json.
        /// </summary>
        public string GetHomeJSON()
        {
            return this._homeJSON;
        }

        /// <summary>
        ///     Sets the home json.
        /// </summary>
        public void SetHomeJSON(string json)
        {
            this._homeJSON = json;
        }

        /// <summary>
        ///     Gets the calandar json.
        /// </summary>
        public string GetCalandarJSON()
        {
            return this._compressibleCalandarJson.Get();
        }

        /// <summary>
        ///     Sets the calandar json.
        /// </summary>
        public void SetCalandarJSON(string json)
        {
            this._compressibleCalandarJson.Set(json);
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
            this._homeJSON = LogicJSONHelper.GetJSONString(json, "level");
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
            jsonObject.Put("level", new LogicJSONString(this._homeJSON));

            return jsonObject;
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

            if (this._compressibleCalandarJson != null)
            {
                this._compressibleCalandarJson.Destruct();
                this._compressibleCalandarJson = null;
            }
            
            this._homeJSON = null;
        }
    }
}