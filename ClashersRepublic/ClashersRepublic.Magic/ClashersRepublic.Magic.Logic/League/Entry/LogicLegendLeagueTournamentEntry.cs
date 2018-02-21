namespace ClashersRepublic.Magic.Logic.League.Entry
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicLegendLeagueTournamentEntry
    {
        private int _bestSeasonState;
        private int _bestSeasonMonth;
        private int _bestSeasonYear;
        private int _bestSeasonRank;
        private int _bestSeasonScore;

        private int _lastSeasonState;
        private int _lastSeasonMonth;
        private int _lastSeasonYear;
        private int _lastSeasonRank;
        private int _lastSeasonScore;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._bestSeasonState = stream.ReadInt();
            this._bestSeasonYear = stream.ReadInt();
            this._bestSeasonMonth = stream.ReadInt();
            this._bestSeasonRank = stream.ReadInt();
            this._bestSeasonScore = stream.ReadInt();
            this._lastSeasonState = stream.ReadInt();
            this._lastSeasonYear = stream.ReadInt();
            this._lastSeasonMonth = stream.ReadInt();
            this._lastSeasonRank = stream.ReadInt();
            this._lastSeasonScore = stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._bestSeasonState);
            encoder.WriteInt(this._bestSeasonYear);
            encoder.WriteInt(this._bestSeasonMonth);
            encoder.WriteInt(this._bestSeasonRank);
            encoder.WriteInt(this._bestSeasonScore);
            encoder.WriteInt(this._lastSeasonState);
            encoder.WriteInt(this._lastSeasonYear);
            encoder.WriteInt(this._lastSeasonMonth);
            encoder.WriteInt(this._lastSeasonRank);
            encoder.WriteInt(this._lastSeasonScore);
        }

        /// <summary>
        ///     Reads this instance from json.
        /// </summary>
        public void ReadFromJSON(LogicJSONObject jsonObject)
        {
            this._bestSeasonState = LogicJSONHelper.GetJSONNumber(jsonObject, "best_season_state");
            this._bestSeasonYear = LogicJSONHelper.GetJSONNumber(jsonObject, "best_season_year");
            this._bestSeasonMonth = LogicJSONHelper.GetJSONNumber(jsonObject, "best_season_month");
            this._bestSeasonRank = LogicJSONHelper.GetJSONNumber(jsonObject, "best_season_rank");
            this._bestSeasonScore = LogicJSONHelper.GetJSONNumber(jsonObject, "best_season_score");
            this._lastSeasonState = LogicJSONHelper.GetJSONNumber(jsonObject, "last_season_state");
            this._lastSeasonYear = LogicJSONHelper.GetJSONNumber(jsonObject, "last_season_year");
            this._lastSeasonMonth = LogicJSONHelper.GetJSONNumber(jsonObject, "last_season_month");
            this._lastSeasonRank = LogicJSONHelper.GetJSONNumber(jsonObject, "last_season_rank");
            this._lastSeasonScore = LogicJSONHelper.GetJSONNumber(jsonObject, "last_season_score");
        }

        /// <summary>
        ///     Writes this instance from json.
        /// </summary>
        public void WriteToJSON(LogicJSONObject jsonObject)
        {
            jsonObject.Put("best_season_state", new LogicJSONNumber(this._bestSeasonState));
            jsonObject.Put("best_season_year", new LogicJSONNumber(this._bestSeasonYear));
            jsonObject.Put("best_season_month", new LogicJSONNumber(this._bestSeasonMonth));
            jsonObject.Put("best_season_rank", new LogicJSONNumber(this._bestSeasonRank));
            jsonObject.Put("best_season_score", new LogicJSONNumber(this._bestSeasonScore));
            jsonObject.Put("last_season_state", new LogicJSONNumber(this._lastSeasonState));
            jsonObject.Put("last_season_year", new LogicJSONNumber(this._lastSeasonYear));
            jsonObject.Put("last_season_month", new LogicJSONNumber(this._lastSeasonMonth));
            jsonObject.Put("last_season_rank", new LogicJSONNumber(this._lastSeasonRank));
            jsonObject.Put("last_season_score", new LogicJSONNumber(this._lastSeasonScore));
        }
    }
}