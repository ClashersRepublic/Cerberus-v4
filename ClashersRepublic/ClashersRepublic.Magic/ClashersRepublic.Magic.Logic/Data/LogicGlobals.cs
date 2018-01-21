namespace ClashersRepublic.Magic.Logic.Data
{
    public class LogicGlobals
    {
        private bool _moreAccurateTime;
        private int _startingDiamonds;
        private int _startingElixir;
        private int _startingElixir2;
        private int _startingGold;
        private int _startingGold2;
        private bool _useNewTraining;

        /// <summary>
        ///     Creates references.
        /// </summary>
        public void CreateReferences()
        {
            this._startingDiamonds = this.GetIntValue("STARTING_DIAMONDS");
            this._startingGold = this.GetIntValue("STARTING_GOLD");
            this._startingElixir = this.GetIntValue("STARTING_ELIXIR");
            this._startingGold2 = this.GetIntValue("STARTING_GOLD2");
            this._startingElixir2 = this.GetIntValue("STARTING_ELIXIR2");
            this._moreAccurateTime = this.GetBoolValue("MORE_ACCURATE_TIME");
            this._useNewTraining = this.GetBoolValue("USE_NEW_TRAINING");
        }

        /// <summary>
        ///     Gets the data instance by the name.
        /// </summary>
        private LogicGlobalData GetGlobalData(string name)
        {
            return LogicDataTables.GetGlobalByName(name);
        }

        /// <summary>
        ///     Gets the boolean value of specified data name.
        /// </summary>
        private bool GetBoolValue(string name)
        {
            return this.GetGlobalData(name).BooleanValue;
        }

        /// <summary>
        ///     Gets the integer value of specified data name.
        /// </summary>
        private int GetIntValue(string name)
        {
            return this.GetGlobalData(name).NumberValue;
        }

        /// <summary>
        ///     Gets the number of starting diamonds.
        /// </summary>
        public int GetStartingDiamonds()
        {
            return this._startingDiamonds;
        }

        /// <summary>
        ///     Gets the number of starting gold.
        /// </summary>
        public int GetStartingGold()
        {
            return this._startingGold;
        }

        /// <summary>
        ///     Gets the number of starting elixir.
        /// </summary>
        public int GetStartingElixir()
        {
            return this._startingElixir;
        }

        /// <summary>
        ///     Gets the number of starting gold2.
        /// </summary>
        public int GetStartingGold2()
        {
            return this._startingGold2;
        }

        /// <summary>
        ///     Gets the number of starting elixir2.
        /// </summary>
        public int GetStartingElixir2()
        {
            return this._startingElixir2;
        }

        /// <summary>
        ///     Gets a value indicating whether the time is more accurate.
        /// </summary>
        public bool MoreAccurateTime()
        {
            return this._moreAccurateTime;
        }

        /// <summary>
        ///     Gets a value indicating whether the new training method is used.
        /// </summary>
        public bool UseNewTraining()
        {
            return this._useNewTraining;
        }
    }
}