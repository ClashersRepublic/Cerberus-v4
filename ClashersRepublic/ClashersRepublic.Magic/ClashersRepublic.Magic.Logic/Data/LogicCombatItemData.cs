namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Titan.CSV;

    public class LogicCombatItemData : LogicData
    {
        private LogicResourceData[] _upgradeResourceData;
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
        protected int[] UpgradeTimeH { get; set; }
        protected int[] UpgradeTimeM { get; set; }
        protected int[] UpgradeCost { get; set; }
        protected int[] TrainingTime { get; set; }
        protected int[] TrainingCost { get; set; }
        protected int[] LaboratoryLevel { get; set; }
        protected string[] UpgradeResource { get; set; }

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
            this._upgradeTime = new int[this.UpgradeTimeH.Length];

            for (int i = 0; i < this.UpgradeTimeH.Length; i++)
            {
                this._upgradeTime[i] = 3600 * this.UpgradeTimeH[i] + 60 * this.UpgradeTimeM[i];
            }

            this._upgradeResourceData = new LogicResourceData[this.UpgradeResource.Length];

            for (int i = 0; i < this.UpgradeResource.Length; i++)
            {
                this._upgradeResourceData[i] = LogicDataTables.GetResourceByName(this.UpgradeResource[i]);
            }
        }

        public int GetLaboratoryLevel(int index)
        {
            return this.LaboratoryLevel[index];
        }

        public int GetUpgradeTime(int index)
        {
            return this._upgradeTime[index];
        }

        public int GetTrainingCost(int index)
        {
            return this.TrainingCost[index];
        }

        public int GetUpgradeCost(int index)
        {
            return this.UpgradeCost[index];
        }

        public int GetTrainingTime(int index)
        {
            return this.TrainingTime[index];
        }
    }
}