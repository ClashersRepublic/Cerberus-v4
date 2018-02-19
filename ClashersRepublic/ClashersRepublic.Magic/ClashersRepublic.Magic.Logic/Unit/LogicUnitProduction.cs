namespace ClashersRepublic.Magic.Logic.Unit
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicUnitProduction
    {
        private LogicLevel _level;
        private LogicTimer _timer;

        private int _unitProductionType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitProduction"/> class.
        /// </summary>
        public LogicUnitProduction(LogicLevel level, int unitProductionType)
        {
            this._level = level;
            this._unitProductionType = unitProductionType;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._timer != null)
            {
                this._timer.Destruct();
                this._timer = null;
            }

            this._level = null;
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public void Tick()
        {

        }

        /// <summary>
        ///     Loads this instance to json.
        /// </summary>
        public void Load(LogicJSONObject root)
        {
            
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject root)
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            if (this._timer != null)
            {
                jsonObject.Put("t", new LogicJSONNumber(this._timer.GetRemainingSeconds(this._level.GetLogicTime())));
            }
        }
    }
}