namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicBuildingClassData : LogicData
    {
        private bool _townHall2Class;
        private bool _townHallClass;
        private bool _wallClass;
        private bool _worker2Class;
        private bool _workerClass;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuildingClassData" /> class.
        /// </summary>
        public LogicBuildingClassData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicBuildingClassData.
        }

        public string TID { get; protected set; }
        public bool CanBuy { get; protected set; }
        public bool ShopCategoryResource { get; protected set; }
        public bool ShopCategoryArmy { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            this._workerClass = string.Equals("Worker", this.GetName());

            if (!this._workerClass)
            {
                this._worker2Class = string.Equals("Worker2", this.GetName());
            }

            this._townHallClass = string.Equals("Town Hall", this.GetName());
            this._townHall2Class = string.Equals("Town Hall2", this.GetName());
            this._wallClass = string.Equals("Wall", this.GetName());
        }

        public bool IsWorker()
        {
            return this._workerClass;
        }

        public bool IsWorker2()
        {
            return this._worker2Class;
        }

        public bool IsTownHall()
        {
            return this._townHallClass;
        }

        public bool IsTownHall2()
        {
            return this._townHall2Class;
        }

        public bool IsWall()
        {
            return this._wallClass;
        }
    }
}