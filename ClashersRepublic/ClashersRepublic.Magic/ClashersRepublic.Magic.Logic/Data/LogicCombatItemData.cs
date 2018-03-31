namespace ClashersRepublic.Magic.Logic.Data
{
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicCombatItemData : LogicData
    {
        private LogicResourceData[] _upgradeResourceData;
        private LogicResourceData _trainingResourceData;

        private int _upgradeLevelCount;

        private int[] _upgradeTime;
        private int[] _upgradeCost;
        private int[] _trainingTime;
        private int[] _trainingCost;
        private int[] _laboratoryLevel;
        private int[] _upgradeLevelByTownHall;

        private int _housingSpace;
        private int _unitType;
        private int _donateCost;

        private bool _productionEnabled;
        private bool _enabledByCalendar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCombatItemData" /> class.
        /// </summary>
        public LogicCombatItemData(CSVRow row, LogicDataTable table) : base(row, table)
        {
            // LogicCombatItemData.
        }

        /// <summary>
        ///     Called when all instances has been loaded for initialized members in instance.
        /// </summary>
        public override void CreateReferences()
        {
            int size = this._upgradeLevelCount = this._row.GetBiggestArraySize();

            this._upgradeLevelByTownHall = new int[size];
            this._upgradeTime = new int[size];
            this._upgradeCost = new int[size];
            this._trainingTime = new int[size];
            this._trainingCost = new int[size];
            this._laboratoryLevel = new int[size];

            this._upgradeResourceData = new LogicResourceData[size];

            for (int i = 0; i < size; i++)
            {
                this._upgradeLevelByTownHall[i] = this.GetClampedIntegerValue("UpgradeLevelByTH", i);
                this._upgradeTime[i] = 3600 * this.GetClampedIntegerValue("UpgradeTimeH", i) + 60 * this.GetClampedIntegerValue("UpgradeTimeM", i);
                this._upgradeCost[i] = this.GetClampedIntegerValue("UpgradeCost", i);
                this._trainingTime[i] = this.GetClampedIntegerValue("TrainingTime", i);
                this._trainingCost[i] = this.GetClampedIntegerValue("TrainingCost", i);
                this._laboratoryLevel[i] = this.GetClampedIntegerValue("LaboratoryLevel", i) - 1;

                this._upgradeResourceData[i] = LogicDataTables.GetResourceByName(this.GetClampedValue("UpgradeResource", i));

                if (this._upgradeResourceData[i] == null && this.GetCombatItemType() != 2)
                {
                    Debugger.Error("UpgradeResource is not defined for " + this.GetName());
                }
            }

            if (this.GetName().Equals("Barbarian2"))
            {
                if (this._trainingTime[0] == 0)
                {
                    this._trainingTime[0] = 30;
                }
            }

            this._trainingResourceData = LogicDataTables.GetResourceByName(this.GetValue("TrainingResource", 0));
            this._housingSpace = this.GetIntegerValue("HousingSpace", 0);
            this._productionEnabled = !this.GetBooleanValue("DisableProduction", 0);
            this._enabledByCalendar = this.GetBooleanValue("EnabledByCalendar", 0);
            this._unitType = this.GetIntegerValue("UnitOfType", 0);
            this._donateCost = this.GetIntegerValue("DonateCost", 0);

            if (this._trainingResourceData == null && this.GetCombatItemType() != 2)
            {
                Debugger.Error("TrainingResource is not defined for " + this.GetName());
            }
        }

        /// <summary>
        ///     Gets the upgrade level count.
        /// </summary>
        public int GetUpgradeLevelCount()
        {
            return this._upgradeLevelCount;
        }

        /// <summary>
        ///     Gets the upgrade time.
        /// </summary>
        public int GetUpgradeTime(int idx)
        {
            return this._upgradeTime[idx];
        }

        /// <summary>
        ///     Gets the upgrade resource data.
        /// </summary>
        public LogicResourceData GetUpgradeResource(int idx)
        {
            return this._upgradeResourceData[idx];
        }

        /// <summary>
        ///     Gets the upgrade cost.
        /// </summary>
        public int GetUpgradeCost(int idx)
        {
            return this._upgradeCost[idx];
        }

        /// <summary>
        ///     Gets the training resource data.
        /// </summary>
        /// <returns></returns>
        public LogicResourceData GetTrainingResource()
        {
            return this._trainingResourceData;
        }

        /// <summary>
        ///     Gets the training cost.
        /// </summary>
        public int GetTrainingCost(int idx)
        {
            return this._trainingCost[idx];
        }

        /// <summary>
        ///     Gets the unit of type.
        /// </summary>
        public int GetUnitOfType()
        {
            return this._unitType;
        }

        /// <summary>
        ///     Gets the required laboratory level.
        /// </summary>
        public int GetRequiredLaboratoryLevel(int idx)
        {
            return this._laboratoryLevel[idx];
        }

        /// <summary>
        ///     Gets the required production level.
        /// </summary>
        public virtual int GetRequiredProductionLevel()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the housing space.
        /// </summary>
        public int GetHousingSpace()
        {
            return this._housingSpace;
        }

        /// <summary>
        ///     Gets the upgrade level by town hall.
        /// </summary>
        public int GetUpgradeLevelByTownHall(int townHallLevel)
        {
            int levelCount = this._upgradeLevelCount;

            if (levelCount >= 2)
            {
                int index = 1;
                
                while (townHallLevel + 1 >= this._upgradeLevelByTownHall[index])
                {
                    if (++index >= levelCount)
                    {
                        return levelCount - 1;
                    }
                }
                
                levelCount = index;
            }

            return levelCount - 1;
        }

        /// <summary>
        ///     Gets if the upgrade level is by town hall level.
        /// </summary>
        public bool UseUpgradeLevelByTownHall()
        {
            return this._upgradeLevelByTownHall[0] > 0;
        }

        public int GetTrainingTime(int index, LogicLevel level, int additionalBarrackCount)
        {
            int trainingTime = this._trainingTime[index];

            if (this.GetVillageType() != 1 &&
                LogicDataTables.GetGlobals().UseNewTraining() &&
                this.GetCombatItemType() == 0)
            {
                if (level != null)
                {
                    LogicGameObjectManager gameObjectManager = level.GetGameObjectManagerAt(0);

                    if (this._unitType != 0)
                    {
                        if (this._unitType == 1)
                        {
                            int barrackCount = gameObjectManager.GetBarrackCount();
                            int barrackFound = 0;

                            if (barrackCount > 0)
                            {
                                int productionLevel = this.GetRequiredProductionLevel();
                                int idx = 0;

                                do
                                {
                                    LogicBuilding barrack = (LogicBuilding) gameObjectManager.GetBarrack(idx);

                                    if (barrack != null)
                                    {
                                        if (barrack.GetBuildingData().GetProducesUnitsOfType() == this.GetCombatItemType())
                                        {
                                            if (barrack.GetUpgradeLevel() >= productionLevel)
                                            {
                                                if (!barrack.IsConstructing())
                                                {
                                                    barrackFound += 1;
                                                }
                                            }
                                        }
                                    }
                                } while (++idx != barrackCount);
                            }

                            if (barrackCount + additionalBarrackCount <= 0)
                            {
                                return trainingTime;
                            }

                            int[] barrackDivisor = LogicDataTables.GetGlobals().GetBarrackReduceTrainingDevisor();
                            int divisor = barrackDivisor[LogicMath.Min(barrackDivisor.Length - 1, barrackCount + additionalBarrackCount - 1)];

                            if (divisor > 0)
                            {
                                return trainingTime / divisor;
                            }

                            return trainingTime;
                        }

                        if (this.GetUnitOfType() == 2)
                        {
                            int barrackCount = gameObjectManager.GetDarkBarrackCount();
                            int barrackFound = 0;

                            if (barrackCount > 0)
                            {
                                int productionLevel = this.GetRequiredProductionLevel();
                                int idx = 0;

                                do
                                {
                                    LogicBuilding barrack = (LogicBuilding) gameObjectManager.GetDarkBarrack(idx);

                                    if (barrack != null)
                                    {
                                        if (barrack.GetBuildingData().GetProducesUnitsOfType() == this.GetCombatItemType())
                                        {
                                            if (barrack.GetUpgradeLevel() >= productionLevel)
                                            {
                                                if (!barrack.IsConstructing())
                                                {
                                                    barrackFound += 1;
                                                }
                                            }
                                        }
                                    }
                                } while (++idx != barrackCount);
                            }

                            if (barrackCount + additionalBarrackCount <= 0)
                            {
                                return trainingTime;
                            }

                            int[] barrackDivisor = LogicDataTables.GetGlobals().GetDarkBarrackReduceTrainingDevisor();
                            int divisor = barrackDivisor[LogicMath.Min(barrackDivisor.Length - 1, barrackCount + additionalBarrackCount - 1)];

                            if (divisor > 0)
                            {
                                return trainingTime / divisor;
                            }

                            return trainingTime;
                        }
                    }

                    Debugger.Error("invalid type for unit");
                }
                else
                {
                    Debugger.Error("level was null in getTrainingTime()");
                }
            }

            return trainingTime;
        }

        public virtual int GetCombatItemType()
        {
            return -1;
        }
    }
}