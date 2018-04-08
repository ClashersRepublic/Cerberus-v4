namespace RivieraStudio.Magic.Logic.Util
{
    using RivieraStudio.Magic.Logic.Data;

    public class LogicUnitProductionSlot
    {
        private LogicData _data;

        private int _count;
        private bool _terminate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitSlot" /> class.
        /// </summary>
        public LogicUnitProductionSlot(LogicData data, int count, bool terminate)
        {
            this._data = data;
            this._count = count;
            this._terminate = terminate;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._data = null;
            this._count = 0;
        }

        /// <summary>
        ///     Gets the data instance.
        /// </summary>
        public LogicData GetData()
        {
            return this._data;
        }

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public int GetCount()
        {
            return this._count;
        }

        /// <summary>
        ///     Sets the count.
        /// </summary>
        public void SetCount(int count)
        {
            this._count = count;
        }

        /// <summary>
        ///     Gets if the unit prod was finished.
        /// </summary>
        public bool IsTerminate()
        {
            return this._terminate;
        }

        /// <summary>
        ///     Sets if the unit prod was finished.
        /// </summary>
        public void SetTerminate(bool terminate)
        {
            this._terminate = terminate;
        }
    }
}