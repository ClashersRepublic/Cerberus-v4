namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicVillageObject : LogicGameObject
    {
        private LogicTimer _constructionTimer;

        private bool _isLocked;
        private bool _isUpgrading;
        private int _upgLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicObstacle" /> class.
        /// </summary>
        public LogicVillageObject(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicVillageObject.
        }

        /// <summary>
        ///     Gets the <see cref="LogicObstacleData"/> instance.
        /// </summary>
        public LogicVillageObjectData GetVillageObjectData()
        {
            return (LogicVillageObjectData) this._data;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }
        }
        
        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
            base.FastForwardTime(time);
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public override void Tick()
        {
            base.Tick();
        }
        
        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            if (this._upgLevel != 0 || this._constructionTimer == null || this._isUpgrading)
            {
                jsonObject.Put("lvl", new LogicJSONNumber(this._upgLevel));
            }
            else
            {
                jsonObject.Put("lvl", new LogicJSONNumber(-1));
            }

            if (this._constructionTimer != null)
            {
                jsonObject.Put("const_t", new LogicJSONNumber(this._constructionTimer.GetRemainingSeconds(this._level.GetLogicTime())));

                if (this._constructionTimer.GetEndTimestamp() != -1)
                {
                    jsonObject.Put("const_t_end", new LogicJSONNumber(this._constructionTimer.GetEndTimestamp()));
                }

                if (this._constructionTimer.GetFastForward() != -1)
                {
                    jsonObject.Put("con_ff", new LogicJSONNumber(this._constructionTimer.GetFastForward()));
                }
            }

            if (this._isLocked)
            {
                jsonObject.Put("locked", new LogicJSONBoolean(true));
            }

            base.Save(jsonObject);
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber lvlObject = jsonObject.GetJSONNumber("lvl");

            if (lvlObject != null)
            {
                this._upgLevel = lvlObject.GetIntValue();
                int maxLvl = this.GetVillageObjectData().GetUpgradeLevelCount();

                if (this._upgLevel >= maxLvl)
                {
                    Debugger.Warning(string.Format("LogicVillageObject::load() - Loaded upgrade level {0} is over max! (max = {1}) id {2} data id {3}",
                        lvlObject.GetIntValue(),
                        maxLvl,
                        this._globalId,
                        this._data.GetGlobalID()));
                    this._upgLevel = maxLvl - 1;
                }
                else
                {
                    if (this._upgLevel < -1)
                    {
                        Debugger.Error("LogicVillageObject::load() - Loaded an illegal upgrade level!");
                    }
                }
            }
            else
            {
                Debugger.Error("LogicVillageObject::load - Upgrade level was not found!");
            }

            if (this.GetVillageObjectData().RequiresBuilder)
            {
                this._level.GetWorkerManagerAt(this._villageType).DeallocateWorker(this);
            }

            LogicJSONBoolean lockedObject = jsonObject.GetJSONBoolean("locked");

            if (lockedObject != null)
            {
                this._isLocked = lockedObject.IsTrue();
            }

            if (this._constructionTimer != null)
            {
                this._constructionTimer.Destruct();
                this._constructionTimer = null;
            }

            LogicJSONNumber constTimeObject = jsonObject.GetJSONNumber("const_t");

            if (constTimeObject != null)
            {
                int constTime = constTimeObject.GetIntValue();

                if (!LogicDataTables.GetGlobals().ClampBuildingTimes())
                {
                    if (this._upgLevel < this.GetVillageObjectData().GetUpgradeLevelCount() - 1)
                    {
                        constTime = LogicMath.Min(constTime, this.GetVillageObjectData().GetBuildTime(this._upgLevel + 1));
                    }
                }

                this._constructionTimer = new LogicTimer();
                this._constructionTimer.StartTimer(constTime, this._level.GetLogicTime(), false, -1);

                LogicJSONNumber constTimeEndObject = jsonObject.GetJSONNumber("const_t_end");

                if (constTimeEndObject != null)
                {
                    this._constructionTimer.SetEndTimestamp(constTimeEndObject.GetIntValue());
                }

                LogicJSONNumber conffObject = jsonObject.GetJSONNumber("con_ff");

                if (conffObject != null)
                {
                    this._constructionTimer.SetFastForward(conffObject.GetIntValue());
                }

                LogicVillageObjectData villageObjectData = this.GetVillageObjectData();

                if (villageObjectData.RequiresBuilder && !villageObjectData.AutomaticUpgrades)
                {
                    this._level.GetWorkerManagerAt(this._villageType).AllocateWorker(this);
                }

                this._isUpgrading = this._upgLevel != -1;
            }

            this._upgLevel = LogicMath.Min(this._upgLevel, this.GetVillageObjectData().GetUpgradeLevelCount());

            base.Load(jsonObject);

            this.SetPositionXY((this.GetVillageObjectData().TileX100 << 9) / 100,
                               (this.GetVillageObjectData().TileY100 << 9) / 100);
        }

        /// <summary>
        ///     Called when the loading of this <see cref="LogicObstacle"/> instance is finished.
        /// </summary>
        public override void LoadingFinished()
        {
            base.LoadingFinished();

            if (this._listener != null)
            {
                this._listener.LoadedFromJSON();
            }
        }

        /// <summary>
        ///     Gets the checksum of this <see cref="LogicObstacle"/> instance.
        /// </summary>
        public override void GetChecksum(ChecksumHelper checksum)
        {
            base.GetChecksum(checksum);
        }

        /// <summary>
        ///     Gets the <see cref="LogicGameObject"/> type.
        /// </summary>
        public override int GetGameObjectType()
        {
            return 8;
        }

        /// <summary>
        ///     Gets the upgrade level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgLevel;
        }
    }
}