namespace RivieraStudio.Magic.Logic.Battle
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Json;

    public class LogicReplay
    {
        private LogicLevel _level;
        private LogicJSONObject _replayObject;
        private LogicJSONNumber _endTickNumber;
        private LogicJSONNumber _preparationSkipNumber;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicReplay"/> class.
        /// </summary>
        public LogicReplay(LogicLevel level)
        {
            this._level = level;
            this._replayObject = new LogicJSONObject();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {

        }

        /// <summary>
        ///     Starts the record.
        /// </summary>
        public void StartRecord()
        {
            this._replayObject = new LogicJSONObject();
            this._endTickNumber = new LogicJSONNumber();

            LogicJSONObject levelObject = new LogicJSONObject();
            LogicJSONObject visitorObject = new LogicJSONObject();
            LogicJSONObject attackerObject = new LogicJSONObject();

            this._level.SaveToJSON(levelObject);
            this._level.GetVisitorAvatar().SaveToReplay(visitorObject);
            this._level.GetHomeOwnerAvatar().SaveToReplay(attackerObject);

            this._replayObject.Put("level", levelObject);
            this._replayObject.Put("attacker", visitorObject);
            this._replayObject.Put("defender", attackerObject);
            this._replayObject.Put("end_tick", this._endTickNumber);
            this._replayObject.Put("timestamp", new LogicJSONNumber(this._level.GetGameMode().GetCurrentTimestamp()));
            this._replayObject.Put("calendar", new LogicJSONObject());
            this._replayObject.Put("cmd", new LogicJSONArray());

            if (this._level.GetGameMode().GetConfiguration().GetJson() != null)
            {
                this._replayObject.Put("globals", this._level.GetGameMode().GetConfiguration().GetJson());
            }
        }

        /// <summary>
        ///     Records the preparation skip time.
        /// </summary>
        public void RecordPreparationSkipTime(int secs)
        {
            if (secs > 0)
            {
                if (this._preparationSkipNumber == null)
                {
                    this._preparationSkipNumber = new LogicJSONNumber();
                    this._replayObject.Put("prep_skip", this._preparationSkipNumber);
                }

                this._preparationSkipNumber.SetIntValue(secs);
            }
        }
    }
}