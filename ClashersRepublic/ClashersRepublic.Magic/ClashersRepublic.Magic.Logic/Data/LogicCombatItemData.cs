namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicCombatItemData : LogicData
    {
        private LogicResourceData _upgradeResourceData;
        private LogicResourceData _trainingResourceData;
        private int _upgradeLevelCount;
        private int[] _upgradeTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCombatItemData" /> class.
        /// </summary>
        public LogicCombatItemData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicCombatItemData.
        }

        protected int[] UpgradeLevelByTH { get; set; }
        protected string[] IconExportName { get; set; }
        protected string[] BigPicture { get; set; }
        protected int[] UpgradeCost { get; set; }
        protected int[] TrainingTime { get; set; }
        protected int[] TrainingCost { get; set; }
        protected int[] LaboratoryLevel { get; set; }
        protected string UpgradeResource { get; set; }

        public string TrainingResource { get; protected set; }
        public int HousingSpace { get; protected set; }
        public bool DisableProduction { get; protected set; }
        public bool EnabledByCalendar { get; protected set; }
        public int UnitOfType { get; protected set; }
        public int DonateCost { get; protected set; }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void LoadingFinished()
        {
            this._upgradeLevelCount = this._row.GetBiggestArraySize();
            this._upgradeTime = new int[this._upgradeLevelCount];

            for (int i = 0; i < this._upgradeLevelCount; i++)
            {
                this._upgradeTime[i] = 3600 * this.GetIntegerValue("UpgradeTimeH", i) + 60 * this.GetIntegerValue("UpgradeTimeM", i);
            }

            this._upgradeResourceData = LogicDataTables.GetResourceByName(this.UpgradeResource);
            this._trainingResourceData = LogicDataTables.GetResourceByName(this.TrainingResource);
        }

        public int GetLaboratoryLevel(int index)
        {
            return this.LaboratoryLevel[index];
        }

        public int GetUpgradeTime(int index)
        {
            return this._upgradeTime[index];
        }

        public LogicResourceData GetUpgradeResource()
        {
            return this._upgradeResourceData;
        }

        public int GetUpgradeCost(int index)
        {
            return this.UpgradeCost[index];
        }

        public LogicResourceData GetTrainingResource()
        {
            return this._trainingResourceData;
        }

        public int GetTrainingCost(int index)
        {
            return this.TrainingCost[index];
        }

        public int GetTrainingTime(int index)
        {
            return this.TrainingTime[index];
        }

        public virtual int GetCombatItemType()
        {
            return -1;
        }
    }
}