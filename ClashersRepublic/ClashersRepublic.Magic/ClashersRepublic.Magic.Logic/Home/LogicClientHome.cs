namespace ClashersRepublic.Magic.Logic.Home
{
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicClientHome
    {
        private LogicLong _homeId;

        private string _homeJson;
        private int _shieldDurationSeconds;
        private int _guardDurationSeconds;
        private int _nextMaintenanceSeconds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientHome"/> class.
        /// </summary>
        public LogicClientHome()
        {
            // LogicClientHome.
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

            if (this._homeJson != null)
            {
                
            }
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
            return this._homeJson;
        }

        /// <summary>
        ///     Sets the home json.
        /// </summary>
        public void SetHomeJSON(string json)
        {
            this._homeJson = json;
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
    }
}