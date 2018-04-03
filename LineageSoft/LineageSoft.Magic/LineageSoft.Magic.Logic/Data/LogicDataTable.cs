namespace LineageSoft.Magic.Logic.Data
{
    using LineageSoft.Magic.Titan.CSV;
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Util;

    public class LogicDataTable
    {
        private readonly CSVTable _table;
        private readonly int _tableIndex;
        private LogicArrayList<LogicData> _items;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDataTable" /> class.
        /// </summary>
        public LogicDataTable(CSVTable table, int index)
        {
            this._tableIndex = index;
            this._table = table;

            this.LoadTable();
        }

        /// <summary>
        ///     Loads the data table.
        /// </summary>
        public void LoadTable()
        {
            this._items = new LogicArrayList<LogicData>();

            for (int i = 0; i < this._table.GetRowCount(); i++)
            {
                LogicData data = this.CreateItem(this._table.GetRowAt(i));

                if (data == null)
                {
                    break;
                }

                this._items.Add(data);
            }
        }

        /// <summary>
        ///     Creates a new data item.
        /// </summary>
        public LogicData CreateItem(CSVRow row)
        {
            LogicData data = null;

            switch (this._tableIndex)
            {
                case 0:
                {
                    data = new LogicBuildingData(row, this);
                    break;
                }

                case 1:
                {
                    data = new LogicLocaleData(row, this);
                    break;
                }

                case 2:
                {
                    data = new LogicResourceData(row, this);
                    break;
                }

                case 3:
                {
                    data = new LogicCharacterData(row, this);
                    break;
                }

                case 5:
                {
                    data = new LogicProjectileData(row, this);
                    break;
                }

                case 6:
                {
                    data = new LogicBuildingClassData(row, this);
                    break;
                }

                case 7:
                {
                    data = new LogicObstacleData(row, this);
                    break;
                }

                case 8:
                {
                    data = new LogicEffectData(row, this);
                    break;
                }

                case 9:
                {
                    data = new LogicParticleEmitterData(row, this);
                    break;
                }

                case 10:
                {
                    data = new LogicExperienceLevelData(row, this);
                    break;
                }

                case 11:
                {
                    data = new LogicTrapData(row, this);
                    break;
                }

                case 12:
                {
                    data = new LogicAllianceBadgeData(row, this);
                    break;
                }

                case 13:
                {
                    data = new LogicGlobalData(row, this);
                    break;
                }

                case 14:
                {
                    data = new LogicTownhallLevelData(row, this);
                    break;
                }

                case 15:
                {
                    data = new LogicAlliancePortalData(row, this);
                    break;
                }

                case 16:
                {
                    data = new LogicNpcData(row, this);
                    break;
                }

                case 17:
                {
                    data = new LogicDecoData(row, this);
                    break;
                }

                case 18:
                {
                    data = new LogicResourcePackData(row, this);
                    break;
                }

                case 19:
                {
                    data = new LogicShieldData(row, this);
                    break;
                }

                case 20:
                {
                    data = new LogicMissionData(row, this);
                    break;
                }

                case 21:
                {
                    data = new LogicBillingPackageData(row, this);
                    break;
                }

                case 22:
                {
                    data = new LogicAchievementData(row, this);
                    break;
                }

                case 23:
                {
                    data = new LogicFaqData(row, this);
                    break;
                }

                case 25:
                {
                    data = new LogicSpellData(row, this);
                    break;
                }

                case 26:
                {
                    data = new LogicHintData(row, this);
                    break;
                }

                case 27:
                {
                    data = new LogicHeroData(row, this);
                    break;
                }

                case 28:
                {
                    data = new LogicLeagueData(row, this);
                    break;
                }

                case 29:
                {
                    data = new LogicNewsData(row, this);
                    break;
                }

                case 30:
                {
                    data = new LogicWarData(row, this);
                    break;
                }

                case 31:
                {
                    data = new LogicRegionData(row, this);
                    break;
                }

                case 32:
                {
                    data = new LogicGlobalData(row, this);
                    break;
                }

                case 33:
                {
                    data = new LogicAllianceBadgeLayerData(row, this);
                    break;
                }

                case 34:
                {
                    data = new LogicAllianceLevelData(row, this);
                    break;
                }

                case 36:
                {
                    data = new LogicVariableData(row, this);
                    break;
                }

                case 37:
                {
                    data = new LogicGemBundleData(row, this);
                    break;
                }

                case 38:
                {
                    data = new LogicVillageObjectData(row, this);
                    break;
                }

                case 39:
                {
                    data = new LogicCalendarEventFunctionData(row, this);
                    break;
                }

                case 40:
                {
                    data = new LogicBoomboxData(row, this);
                    break;
                }

                case 41:
                {
                    data = new LogicEventEntryData(row, this);
                    break;
                }

                case 42:
                {
                    data = new LogicDeeplinkData(row, this);
                    break;
                }

                case 43:
                {
                    data = new LogicLeague2Data(row, this);
                    break;
                }

                default:
                {
                    Debugger.Error("Invalid data table id: " + this._tableIndex);
                    break;
                }
            }

            return data;
        }

        /// <summary>
        ///     Called for initialize datas.
        /// </summary>
        public void CreateReferences()
        {
            for (int i = 0; i < this._items.Count; i++)
            {
                this._items[i].AutoLoadData();
            }

            for (int i = 0; i < this._items.Count; i++)
            {
                this._items[i].CreateReferences();
            }
        }

        /// <summary>
        ///     Gets the item at specified index.
        /// </summary>
        public LogicData GetItemAt(int index)
        {
            return this._items[index];
        }

        /// <summary>
        ///     Gets the item at specified index.
        /// </summary>
        public LogicData GetDataByName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                for (int i = 0; i < this._items.Count; i++)
                {
                    if (this._items[i].GetName().Equals(name))
                    {
                        return this._items[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets a item by id.
        /// </summary>
        public LogicData GetItemById(int globalId)
        {
            int instanceId = GlobalID.GetInstanceID(globalId);

            if (instanceId < 0 || instanceId >= this._items.Count)
            {
                Debugger.Warning("LogicDataTable::getItemById() - Instance id out of bounds! " + (instanceId + 1) + "/" + this._items.Count);
                return null;
            }

            return this._items[instanceId];
        }

        /// <summary>
        ///     Gets the number of items.
        /// </summary>
        public int GetItemCount()
        {
            return this._items.Count;
        }

        /// <summary>
        ///     Gets the index of this data table.
        /// </summary>
        public int GetTableIndex()
        {
            return this._tableIndex;
        }

        /// <summary>
        ///     Gets the name of this data table.
        /// </summary>
        public string GetTableName()
        {
            return this._table.GetFileName();
        }
    }
}