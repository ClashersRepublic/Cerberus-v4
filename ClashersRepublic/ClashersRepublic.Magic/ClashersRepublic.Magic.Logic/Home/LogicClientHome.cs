namespace ClashersRepublic.Magic.Logic.Home
{
    using ClashersRepublic.Magic.Logic.Helper;
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
        private string _eventJSON;
        private string _globalJSON;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientHome"/> class.
        /// </summary>
        public LogicClientHome()
        {
            this._homeJSON = string.Empty;
            this._eventJSON = string.Empty;
            this._globalJSON = string.Empty;
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

            if (this._eventJSON != null)
            {
                ZLibHelper.ConpressInZLibFormat(LogicStringUtil.GetBytes(this._eventJSON), out byte[] compressed);

                encoder.WriteBoolean(true);
                encoder.WriteBytes(compressed, compressed.Length);
            }
            else
            {
                encoder.WriteBoolean(false);
                encoder.WriteString(this._eventJSON);
            }

            if (this._globalJSON != null)
            {
                ZLibHelper.ConpressInZLibFormat(LogicStringUtil.GetBytes(this._globalJSON), out byte[] compressed);

                encoder.WriteBoolean(true);
                encoder.WriteBytes(compressed, compressed.Length);
            }
            else
            {
                encoder.WriteBoolean(false);
                encoder.WriteString(this._globalJSON);
            }
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

            if (stream.ReadBoolean())
            {
                int byteArrayLength = stream.ReadBytesLength();
                byte[] byteArray = stream.ReadBytes(byteArrayLength, 900000);

                ZLibHelper.DecompressInMySQLFormat(byteArray, byteArrayLength, out byte[] decompressed);

                this._eventJSON = LogicStringUtil.CreateString(decompressed, 0, decompressed.Length);
            }
            else
            {
                this._eventJSON = stream.ReadString(900000);
            }

            if (stream.ReadBoolean())
            {
                int byteArrayLength = stream.ReadBytesLength();
                byte[] byteArray = stream.ReadBytes(byteArrayLength, 900000);

                ZLibHelper.DecompressInMySQLFormat(byteArray, byteArrayLength, out byte[] decompressed);

                this._globalJSON = LogicStringUtil.CreateString(decompressed, 0, decompressed.Length);
            }
            else
            {
                this._globalJSON = stream.ReadString(900000);
            }
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
        ///     Gets the event json.
        /// </summary>
        public string GetEventJSON()
        {
            return this._eventJSON;
        }

        /// <summary>
        ///     Sets the event json.
        /// </summary>
        public void SetEventJSON(string json)
        {
            this._eventJSON = json;
        }

        /// <summary>
        ///     Gets the global json.
        /// </summary>
        public string GetGlobalJSON()
        {
            return this._globalJSON;
        }

        /// <summary>
        ///     Sets the global json.
        /// </summary>
        public void SetGlobalJSON(string json)
        {
            this._globalJSON = json;
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
            this._eventJSON = null;
            this._globalJSON = null;
            this._homeJSON = null;
        }
    }
}