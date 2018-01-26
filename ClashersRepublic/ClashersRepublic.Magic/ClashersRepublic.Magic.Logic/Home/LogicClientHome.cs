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
    }
}