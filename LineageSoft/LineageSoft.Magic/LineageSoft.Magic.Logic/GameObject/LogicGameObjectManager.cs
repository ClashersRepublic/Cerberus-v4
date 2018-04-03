namespace LineageSoft.Magic.Logic.GameObject
{
    using System;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.GameObject.Component;
    using LineageSoft.Magic.Logic.GameObject.Listener;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Logic.Time;
    using LineageSoft.Magic.Logic.Unit;
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Util;

    public sealed class LogicGameObjectManager
    {
        private readonly int _villageType;
        private readonly int[] _gameObjectIds;

        private LogicLevel _level;
        private LogicTileMap _tileMap;
        private LogicUnitProduction _unitProduction;
        private LogicUnitProduction _spellProduction;
        private LogicGameObjectManagerListener _listener;
        private LogicRandom _obstacleRespawnRandom;
        private LogicRandom _tallGrassRespawnRandom;
        private readonly LogicComponentManager _componentManager;
        private readonly LogicArrayList<LogicGameObject>[] _gameObjects;
        private readonly LogicArrayList<LogicBuilding> _barracks;
        private readonly LogicArrayList<LogicBuilding> _darkBarracks;
        private readonly LogicArrayList<int> _obstacleDiamondsReward;
        private readonly LogicArrayList<int> _obstacleDiamondsRewardVillage2;

        private LogicBuilding _allianceCastle;
        private LogicBuilding _clockTower;
        private LogicBuilding _townHall;
        private LogicBuilding _laboratory;
        private LogicBuilding _spellForge;
        private LogicBuilding _darkSpellForge;

        //private LogicAlliancePortal _alliancePortal;
        private LogicObstacle _lootCart;
        private LogicVillageObject _shipyard;
        private LogicVillageObject _rowBoat;
        private LogicVillageObject _clanGate;
        private LogicObstacleData _bonusGemboxData;
        private LogicObstacleData _specialObstacleData;

        private int _secondsFromLastRespawn;
        private int _secondsFromLastTallGrassRespawn;
        private int _specialObstacleDropSecs;
        private int _specialObstaclePeriodSecs;
        private int _obstacleClearCounter;
        private int _gemBoxDropSecs;
        private int _gemBoxPeriodSecs;
        private int _unitProductionCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObjectManager" /> class.
        /// </summary>
        public LogicGameObjectManager(LogicTileMap tileMap, LogicLevel level, int villageType)
        {
            this._level = level;
            this._tileMap = tileMap;
            this._villageType = villageType;

            this._gameObjectIds = new int[9];
            this._gameObjects = new LogicArrayList<LogicGameObject>[9];

            for (int i = 0; i < 9; i++)
            {
                this._gameObjects[i] = new LogicArrayList<LogicGameObject>(32);
            }

            this._obstacleDiamondsReward = new LogicArrayList<int>(20);
            this._obstacleDiamondsRewardVillage2 = new LogicArrayList<int>(20);

            for (int i = 0; i < 20; i++)
            {
                this._obstacleDiamondsReward.Add(0);
                this._obstacleDiamondsRewardVillage2.Add(0);
            }

            this._obstacleDiamondsReward[1] = 3;
            this._obstacleDiamondsReward[3] = 1;
            this._obstacleDiamondsReward[4] = 2;
            this._obstacleDiamondsReward[6] = 1;
            this._obstacleDiamondsReward[7] = 1;
            this._obstacleDiamondsReward[10] = 3;
            this._obstacleDiamondsReward[11] = 1;
            this._obstacleDiamondsReward[13] = 2;
            this._obstacleDiamondsReward[14] = 2;
            this._obstacleDiamondsReward[17] = 3;
            this._obstacleDiamondsReward[19] = 1;
            this._obstacleDiamondsRewardVillage2[17] = -1;
            this._obstacleDiamondsRewardVillage2[10] = -1;
            this._obstacleDiamondsRewardVillage2[11] = -1;
            this._obstacleDiamondsRewardVillage2[3] = 2;
            this._obstacleDiamondsRewardVillage2[4] = 1;
            this._obstacleDiamondsRewardVillage2[5] = 1;
            this._obstacleDiamondsRewardVillage2[6] = 1;
            this._obstacleDiamondsRewardVillage2[13] = -1;
            this._obstacleDiamondsRewardVillage2[19] = -1;

            this._barracks = new LogicArrayList<LogicBuilding>();
            this._darkBarracks = new LogicArrayList<LogicBuilding>();
            this._bonusGemboxData = LogicDataTables.GetObstacleByName("Bonus Gembox");
            this._componentManager = new LogicComponentManager(level);
            this._listener = new LogicGameObjectManagerListener();
            this._obstacleRespawnRandom = new LogicRandom();
            this._tallGrassRespawnRandom = new LogicRandom();

            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                this._unitProduction = new LogicUnitProduction(level, 2);
                this._spellProduction = new LogicUnitProduction(level, 25);
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            for (int i = 0; i < 9; i++)
            {
                if (this._gameObjects[i].Count != 0)
                {
                    do
                    {
                        this._gameObjects[i][0].Destruct();
                        this._gameObjects[i].Remove(0);
                    } while (this._gameObjects[i].Count != 0);
                }
            }

            if (this._barracks.Count != 0)
            {
                do
                {
                    this._barracks.Remove(0);
                } while (this._barracks.Count != 0);
            }

            if (this._darkBarracks.Count != 0)
            {
                do
                {
                    this._darkBarracks.Remove(0);
                } while (this._darkBarracks.Count != 0);
            }

            if (this._unitProduction != null)
            {
                this._unitProduction.Destruct();
                this._unitProduction = null;
            }

            if (this._spellProduction != null)
            {
                this._spellProduction.Destruct();
                this._spellProduction = null;
            }

            this._listener = null;
            this._obstacleRespawnRandom = null;
            this._tallGrassRespawnRandom = null;
            this._level = null;
            this._tileMap = null;
            this._allianceCastle = null;
            this._clockTower = null;
            this._townHall = null;
            this._laboratory = null;
            this._spellForge = null;
            this._darkSpellForge = null;
            this._lootCart = null;
            this._shipyard = null;
            this._rowBoat = null;
            this._clanGate = null;
            this._bonusGemboxData = null;
            this._specialObstacleData = null;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Init(LogicLevel level, int villageType)
        {
            // Init.
        }

        /// <summary>
        ///     Adds the specified gameobject to this instance.
        /// </summary>
        public void AddGameObject(LogicGameObject gameObject)
        {
            int globalId = gameObject.GetGlobalID();
            int gameObjectType = gameObject.GetGameObjectType();

            if (gameObject.GetData().GetVillageType() != this._villageType)
            {
                Debugger.Error(string.Format("Invalid item in level for villageType {0} DataId: {1}", this._villageType, gameObject.GetData().GetGlobalID()));
            }

            if (globalId == -1)
            {
                globalId = this.GenerateGameObjectGlobalID(gameObject);
            }
            else
            {
                int table = GlobalID.GetClassID(globalId);
                int idx = GlobalID.GetInstanceID(globalId);

                if (table - 500 != gameObjectType)
                {
                    Debugger.Error(string.Format("LogicGameObjectManager::addGameObject with global ID {0}, doesn't have right index", globalId));
                }

                if (this.GetGameObjectByID(globalId) != null)
                {
                    Debugger.Error(string.Format("LogicGameObjectManager::addGameObject with global ID {0}, global ID already taken", globalId));
                }

                if (this._gameObjectIds[gameObjectType] <= idx)
                {
                    this._gameObjectIds[gameObjectType] = idx + 1;
                }
            }

            gameObject.SetGlobalID(globalId);

            LogicRandom random = new LogicRandom(this._level.GetGameMode().GetCurrentTimestamp() + globalId);
            random.Rand(0x7fffffff);
            random.Rand(0x7fffffff);
            random.Rand(0x7fffffff);
            gameObject.SetSeed(random.Rand(0x7fffffff));

            if (gameObjectType == 0)
            {
                LogicBuilding building = (LogicBuilding) gameObject;
                LogicBuildingData buildingData = building.GetBuildingData();

                if (buildingData.IsAllianceCastle())
                {
                    this._allianceCastle = building;
                }

                if (buildingData.GetUnitProduction(0) >= 1)
                {
                    if (!buildingData.ForgesSpells)
                    {
                        if (buildingData.GetProducesUnitsOfType() == 1)
                        {
                            this._barracks.Add(building);
                        }
                        else if (buildingData.GetProducesUnitsOfType() == 2)
                        {
                            this._darkBarracks.Add(building);
                        }
                    }
                }

                if (buildingData.IsClockTower())
                {
                    this._clockTower = building;
                }

                if (buildingData.IsTownHall() || buildingData.IsTownHallVillage2())
                {
                    this._townHall = building;
                }

                if (buildingData.IsWorkerBuilding() || buildingData.IsTownHallVillage2())
                {
                    this._level.GetWorkerManagerAt(this._villageType).IncreaseWorkerCount();
                }

                if (buildingData.IsLaboratory())
                {
                    this._laboratory = building;
                }

                if (buildingData.GetUnitProduction(0) >= 1)
                {
                    this._unitProductionCount += 1;
                }

                if (buildingData.ForgesSpells)
                {
                    int unitsOfType = buildingData.GetProducesUnitsOfType();

                    if (unitsOfType == 1)
                    {
                        this._darkSpellForge = building;
                    }
                    else
                    {
                        this._spellForge = building;
                    }
                }
            }
            else if (gameObjectType == 3)
            {
                LogicObstacle obstacleObject = (LogicObstacle) gameObject;
                LogicObstacleData obstacleObjectData = obstacleObject.GetObstacleData();

                if (obstacleObjectData.IsLootCart())
                {
                    this._lootCart = obstacleObject;
                }
            }
            else if (gameObjectType == 5)
            {
                //this._alliancePortal = (LogicAlliancePortal)gameObject;
            }
            else if (gameObjectType == 8)
            {
                LogicVillageObject villageObject = (LogicVillageObject) gameObject;
                LogicVillageObjectData villageObjectData = villageObject.GetVillageObjectData();

                if (villageObjectData.IsShipyard())
                {
                    this._shipyard = villageObject;
                }

                if (villageObjectData.IsRowBoat())
                {
                    this._rowBoat = villageObject;
                }

                if (villageObjectData.IsClanGate())
                {
                    this._clanGate = villageObject;
                }
            }

            this._gameObjects[gameObject.GetGameObjectType()].Add(gameObject);

            if (this._level.GetVillageType() == this._villageType)
            {
                this._level.GetTileMap().AddGameObject(gameObject);
            }
        }

        /// <summary>
        ///     Removes the specified gameobject.
        /// </summary>
        public void RemoveGameObject(LogicGameObject gameObject)
        {
            int index = -1;
            int gameObjectType = gameObject.GetGameObjectType();

            LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[gameObjectType];

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].GetGlobalID() == gameObject.GetGlobalID())
                {
                    index = i;
                    break;
                }
            }

            gameObjects.Remove(index);

            if (this._townHall == gameObject)
            {
                this._townHall = null;
            }

            if (this._clockTower == gameObject)
            {
                this._clockTower = null;
            }

            if (gameObjectType == 0)
            {
                LogicBuildingData buildingData = ((LogicBuilding) gameObject).GetBuildingData();

                if (buildingData.IsWorkerBuilding() || buildingData.IsTownHallVillage2())
                {
                    this._level.GetWorkerManagerAt(this._villageType).DecreaseWorkerCount();
                }

                if (buildingData.GetUnitProduction(0) > 0)
                {
                    this._unitProductionCount -= 1;

                    if (!buildingData.ForgesSpells)
                    {
                        if (buildingData.GetProducesUnitsOfType() == 1)
                        {
                            for (int i = 0; i < this._barracks.Count; i++)
                            {
                                if (this._barracks[i] == gameObject)
                                {
                                    this._barracks.Remove(i);
                                    break;
                                }
                            }
                        }
                        else if (buildingData.GetProducesUnitsOfType() == 2)
                        {
                            for (int i = 0; i < this._darkBarracks.Count; i++)
                            {
                                if (this._darkBarracks[i] == gameObject)
                                {
                                    this._darkBarracks.Remove(i);
                                    break;
                                }
                            }
                        }
                    }
                }

                if (this._allianceCastle == gameObject)
                {
                    this._allianceCastle = null;
                }

                if (this._laboratory == gameObject)
                {
                    this._laboratory = null;
                }

                if (this._spellForge == gameObject)
                {
                    this._spellForge = null;
                }

                if (this._darkSpellForge == gameObject)
                {
                    this._darkSpellForge = null;
                }

                gameObject.Destruct();

                this.RemoveGameObjectReferences(gameObject);
            }
        }

        /// <summary>
        ///     Removes all references on the specified gameobject.
        /// </summary>
        private void RemoveGameObjectReferences(LogicGameObject gameObject)
        {
            for (int i = 0; i < 9; i++)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[i];

                for (int j = 0; j < gameObjects.Count; j++)
                {
                    gameObjects[j].RemoveGameObjectReferences(gameObject);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                this._level.GetWorkerManagerAt(i).RemoveGameObjectReference(gameObject);
            }

            this._componentManager.RemoveGameObjectReferences(gameObject);

            if (this._lootCart == gameObject)
            {
                this._lootCart = null;
            }
        }

        /// <summary>
        ///     Generates a global id for the gameobject.
        /// </summary>
        public int GenerateGameObjectGlobalID(LogicGameObject gameObject)
        {
            if (gameObject.GetGameObjectType() >= 9)
            {
                Debugger.Error("LogicGameObjectManager::generateGameObjectGlobalID(). Index is out of bounds.");
            }

            return GlobalID.CreateGlobalID(gameObject.GetGameObjectType() + 500, this._gameObjectIds[gameObject.GetGameObjectType()]++);
        }

        /// <summary>
        ///     Gets the specified gameobject list.
        /// </summary>
        public LogicArrayList<LogicGameObject> GetGameObjects(int index)
        {
            return this._gameObjects[index];
        }

        /// <summary>
        ///     Gets the gameobject correspoding at specified id.
        /// </summary>
        public LogicGameObject GetGameObjectByID(int globalId)
        {
            int classId = GlobalID.GetClassID(globalId) - 500;

            for (int i = 0; i < this._gameObjects[classId].Count; i++)
            {
                if (this._gameObjects[classId][i].GetGlobalID() == globalId)
                {
                    return this._gameObjects[classId][i];
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the gameobject count by data.
        /// </summary>
        public int GetGameObjectCountByData(LogicData data)
        {
            int cnt = 0;

            for (int i = 0; i < 9; i++)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[i];

                if (gameObjects.Count > 0)
                {
                    if (gameObjects[0].GetData().GetDataType() == data.GetDataType())
                    {
                        for (int j = 0; j < gameObjects.Count; j++)
                        {
                            LogicGameObject gameObject = gameObjects[j];

                            if (gameObject.GetData() == data)
                            {
                                ++cnt;
                            }
                        }
                    }
                }
            }

            return cnt;
        }

        /// <summary>
        ///     Gets the gear up building count.
        /// </summary>
        public int GetGearUpBuildingCount()
        {
            LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[0];
            int cnt = 0;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                cnt += ((LogicBuilding) gameObjects[i]).GetGearLevel() > 0 ? 1 : 0;
            }

            return cnt;
        }

        /// <summary>
        ///     Gets the highest building level.
        /// </summary>
        public int GetHighestBuildingLevel(LogicBuildingData data)
        {
            return this.GetHighestBuildingLevel(data, true);
        }

        /// <summary>
        ///     Gets the barrack count.
        /// </summary>
        public int GetBarrackCount()
        {
            if (this._barracks != null)
            {
                return this._barracks.Count;
            }

            return 0;
        }

        /// <summary>
        ///     Gets the barrack count.
        /// </summary>
        public int GetDarkBarrackCount()
        {
            if (this._darkBarracks != null)
            {
                return this._darkBarracks.Count;
            }

            return 0;
        }

        /// <summary>
        ///     Gets the barrack at specified index.
        /// </summary>
        public LogicGameObject GetBarrack(int idx)
        {
            return this._barracks[idx];
        }

        /// <summary>
        ///     Gets the dark barrack at specified index.
        /// </summary>
        public LogicGameObject GetDarkBarrack(int idx)
        {
            return this._barracks[idx];
        }

        /// <summary>
        ///     Gets the highest building level.
        /// </summary>
        public int GetHighestBuildingLevel(LogicBuildingData data, bool includeConstruction)
        {
            LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[0];
            int maxLevel = -1;

            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (gameObjects[i].GetData() == data)
                {
                    LogicBuilding building = (LogicBuilding) gameObjects[i];

                    if (building.IsConstructing())
                    {
                        if (building.IsUpgrading())
                        {
                            continue;
                        }
                    }

                    if (!building.IsLocked())
                    {
                        int upgLevel = building.GetUpgradeLevel();

                        if (includeConstruction && building.IsConstructing())
                        {
                            ++upgLevel;
                        }

                        maxLevel = LogicMath.Max(maxLevel, upgLevel);
                    }
                }
            }

            return maxLevel;
        }

        /// <summary>
        ///     Gets the component manager instance.
        /// </summary>
        public LogicComponentManager GetComponentManager()
        {
            return this._componentManager;
        }

        /// <summary>
        ///     Gets the town hall building.
        /// </summary>
        public LogicBuilding GetTownHall()
        {
            return this._townHall;
        }

        /// <summary>
        ///     Gets the checksum for this instance.
        /// </summary>
        public void GetChecksum(ChecksumHelper checksum, bool includeGameObjects)
        {
            checksum.StartObject("LogicGameObjectManager");

            int num = 0;

            for (int i = 0; i < 9; i++)
            {
                num += this._gameObjects[i].Count;
            }

            checksum.WriteValue("numGameObjects", num);

            if (includeGameObjects)
            {
                for (int i = 0; i < 9; i++)
                {
                    checksum.StartArray("type" + i);

                    for (int j = 0; j < this._gameObjects[i].Count; j++)
                    {
                        this._gameObjects[i][j].GetChecksum(checksum, includeGameObjects);
                    }

                    checksum.EndArray();
                }
            }
            else
            {
                checksum.StartArray("type0");

                for (int i = 0; i < this._gameObjects[0].Count; i++)
                {
                    this._gameObjects[0][i].GetChecksum(checksum, false);
                }

                checksum.EndArray();
            }

            checksum.EndObject();
        }

        /// <summary>
        ///     Sets the <see cref="LogicGameObjectManagerListener"/> instance.
        /// </summary>
        public void SetListener(LogicGameObjectManagerListener listener)
        {
            this._listener = listener;
        }

        /// <summary>
        ///     Called when the town hall of the village 2 is fixed.
        /// </summary>
        public void Village2TownHallFixed()
        {
            LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[0];

            for (int i = 0; i < gameObjects.Count; i++)
            {
                LogicBuilding building = (LogicBuilding) gameObjects[i];

                if (building.IsLocked())
                {
                    if (building.GetBuildingData().GetRequiredTownHallLevel(0) <= 1)
                    {
                        building.FinishConstruction(true);

                        if (building.GetComponent(15) != null)
                        {
                            LogicVillage2UnitComponent village2UnitComponent = (LogicVillage2UnitComponent) building.GetComponent(15);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Loads village objects.
        /// </summary>
        public void LoadVillageObjects()
        {
            LogicDataTable dataTable = LogicDataTables.GetTable(38);

            for (int i = 0; i < dataTable.GetItemCount(); i++)
            {
                LogicVillageObjectData data = (LogicVillageObjectData) dataTable.GetItemAt(i);

                if (data.GetVillageType() == this._villageType && !data.Disabled && this.GetGameObjectCountByData(data) == 0)
                {
                    LogicVillageObject villageObject = (LogicVillageObject) LogicGameObjectFactory.CreateGameObject(data, this._level, this._villageType);
                    villageObject.SetInitialPosition((data.TileX100 << 9) / 100, (data.TileY100 << 9) / 100);
                    this.AddGameObject(villageObject);
                }
            }
        }

        /// <summary>
        ///     Called when the loading is finished.
        /// </summary>
        public void LoadingFinished()
        {
            this.RespawnObstacles();

            if (this._gameObjects[0].Count > 0)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[0];

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].LoadingFinished();
                }
            }

            if (this._gameObjects[1].Count > 0)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[1];

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].LoadingFinished();
                }
            }

            if (this._gameObjects[2].Count > 0)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[2];

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].LoadingFinished();
                }
            }

            if (this._gameObjects[4].Count > 0)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[4];

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].LoadingFinished();
                }
            }

            if (this._gameObjects[6].Count > 0)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[6];

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    gameObjects[i].LoadingFinished();
                }
            }

            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardTime(int secs)
        {
            this._secondsFromLastRespawn += secs;
            this._secondsFromLastTallGrassRespawn += secs;
            this._gemBoxDropSecs -= secs;
            this._specialObstacleDropSecs -= secs;

            if (secs > 0)
            {
                int idx = 0;

                do
                {
                    int maxSecs = secs;

                    if (idx == 999)
                    {
                        Debugger.Warning("LogicGameObjectManager::fastForwardTime - Pass limit reached");
                    }
                    else
                    {
                        for (int i = 0; i < 9; i++)
                        {
                            LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[i];

                            for (int j = 0; j < gameObjects.Count; j++)
                            {
                                int maxFastForwardTime = gameObjects[j].GetMaxFastForwardTime();

                                if (maxFastForwardTime >= 0)
                                {
                                    maxSecs = LogicMath.Min(maxFastForwardTime, maxSecs);
                                }
                            }
                        }
                    }

                    for (int i = 0; i < 9; i++)
                    {
                        LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[i];

                        for (int j = 0; j < gameObjects.Count; j++)
                        {
                            gameObjects[j].FastForwardTime(maxSecs);
                        }
                    }

                    if (LogicDataTables.GetGlobals().UseNewTraining())
                    {
                        this._unitProduction.FastForwardTime(maxSecs);
                        this._spellProduction.FastForwardTime(maxSecs);
                    }

                    secs -= maxSecs;
                } while (secs > 0);
            }

            this.RespawnObstacles();
        }

        /// <summary>
        ///     Increases the obstacle clear counter.
        /// </summary>
        public int IncreaseObstacleClearCounter(int lootMultiplier)
        {
            this._obstacleClearCounter = (this._obstacleClearCounter + 1) % this._obstacleDiamondsReward.Count;
            int diamondsReward = this._obstacleDiamondsReward[this._obstacleClearCounter];

            if (lootMultiplier >= 2)
            {
                diamondsReward = this._obstacleDiamondsRewardVillage2[this._obstacleClearCounter] + lootMultiplier * diamondsReward;
            }

            return diamondsReward;
        }

        /// <summary>
        ///     Generates the next gembox drop time.
        /// </summary>
        public void GenerateNextGemboxDropTime(bool clamp)
        {
            int random = this._obstacleRespawnRandom.Rand(this._bonusGemboxData.AppearancePeriodHours);
            int timeToGemboxDrop = this._gemBoxPeriodSecs + 3600 * random;

            if (clamp)
            {
                int minTime = 3600 * this._bonusGemboxData.MinRespawnTimeHours;

                if (timeToGemboxDrop < minTime)
                {
                    timeToGemboxDrop = minTime;
                }
            }

            this._gemBoxDropSecs = timeToGemboxDrop;
            this._gemBoxPeriodSecs = 3600 * (this._bonusGemboxData.AppearancePeriodHours - random);
        }

        /// <summary>
        ///     Respawnes obstacles.
        /// </summary>
        public void RespawnObstacles()
        {
            int villageType = this._level.GetVillageType();

            if (villageType == this._villageType && this._level.GetMatchType() != 3)
            {
                if (villageType == 0)
                {
                    if (this._level.GetMatchType() == 0)
                    {
                        this.Village1RespawnObstacle();

                        if (this._bonusGemboxData != null && this._gemBoxDropSecs <= 0 && this._bonusGemboxData.GetLootCount() > 0)
                        {
                            this.CreateSpecialObstacle(this._bonusGemboxData, true);
                            this.GenerateNextGemboxDropTime(true);
                        }

                        if (this._specialObstacleData != null && this._specialObstacleDropSecs <= 0 && this._specialObstacleData.GetLootCount() > 0)
                        {
                            this.CreateSpecialObstacle(this._specialObstacleData, false);

                            int rnd = this._obstacleRespawnRandom.Rand(this._specialObstacleData.AppearancePeriodHours);

                            this._specialObstacleDropSecs = 3600 * rnd + this._specialObstaclePeriodSecs;
                            this._specialObstaclePeriodSecs = 3600 * (this._specialObstaclePeriodSecs - rnd);
                        }
                    }
                }
                else if (villageType == 1 && this._level.GetMatchType() != 5)
                {
                    this.Village2RespawnObstacles();
                }
            }
        }

        /// <summary>
        ///     Respawn obstacles of village 1.
        /// </summary>
        public void Village1RespawnObstacle()
        {
            if (this._villageType == 0 && this._level.GetVillageType() == 0)
            {
                int obstacleRespawnTime = LogicDataTables.GetGlobals().GetObstacleRespawnSecs();
                int obstacleMaxCount = LogicDataTables.GetGlobals().GetObstacleMaxCount();

                int tombStoneCount = this._level.GetTombStoneCount();
                int tallGrassCount = this._level.GetTallGrassCount();

                if (this._secondsFromLastRespawn > obstacleRespawnTime)
                {
                    int ignoreCount = tombStoneCount + tallGrassCount;

                    do
                    {
                        int count = this._gameObjects[3].Count - ignoreCount;

                        if (count >= obstacleMaxCount)
                        {
                            this._secondsFromLastRespawn = 0;
                            break;
                        }

                        this.CreateVillage1Obstacle();
                        this._secondsFromLastRespawn -= obstacleRespawnTime;
                    } while (this._secondsFromLastRespawn > obstacleRespawnTime);
                }
            }
        }

        /// <summary>
        ///     Respawn obstacles of village 2.
        /// </summary>
        public void Village2RespawnObstacles()
        {
        }

        /// <summary>
        ///     Respawn the bonus gembox.
        /// </summary>
        public bool CreateSpecialObstacle(LogicObstacleData data, bool oneOnly)
        {
            if (oneOnly && this._gameObjects[3].Count > 0)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[3];

                for (int i = 0; i < gameObjects.Count; i++)
                {
                    if (gameObjects[i].GetData() == data)
                    {
                        return false;
                    }
                }
            }

            bool created = this.RandomlyPlaceObstacle(data);

            if (!created)
            {
                created = this.RandomlyPlaceSpecialObstacle(data, data.Width, data.Height);
            }

            return created;
        }

        /// <summary>
        ///     Creates a new obstacle.
        /// </summary>
        public void CreateVillage1Obstacle()
        {
            if (this._villageType != 0)
            {
                Debugger.Warning("invalid village type home!");
            }

            if (this._level.GetVillageType() != 0)
            {
                Debugger.Warning("invalid village type home (2)!");
            }

            LogicDataTable table = LogicDataTables.GetTable(7);
            Int32 respawnWeights = 0;

            for (int i = 0; i < table.GetItemCount(); i++)
            {
                LogicObstacleData obstacleData = (LogicObstacleData) table.GetItemAt(i);

                if (obstacleData.GetVillageType() == this._villageType)
                {
                    respawnWeights += obstacleData.GetRespawnWeight();
                }
            }

            Int32 rnd = this._obstacleRespawnRandom.Rand(respawnWeights);
            LogicObstacleData respawnObstacleData = null;

            for (int i = 0, weights = 0; i < table.GetItemCount(); i++)
            {
                LogicObstacleData obstacleData = (LogicObstacleData) table.GetItemAt(i);

                if (obstacleData.GetVillageType() == this._villageType)
                {
                    weights += obstacleData.GetRespawnWeight();

                    if (weights > rnd)
                    {
                        respawnObstacleData = obstacleData;
                        break;
                    }
                }
            }

            if (respawnObstacleData != null)
            {
                if (respawnObstacleData.GetVillageType() == this._villageType)
                {
                    this.RandomlyPlaceObstacle(respawnObstacleData);
                }
            }
        }

        /// <summary>
        ///     Places randomly the obstacle.
        /// </summary>
        public bool RandomlyPlaceObstacle(LogicObstacleData data)
        {
            if (data.GetVillageType() == this._villageType)
            {
                if (this._level.GetVillageType() != this._villageType && data.LootDefensePercentage == 0)
                {
                    Debugger.Warning("invalid village type for randomlyPlaceObstacle");
                }

                for (int i = 0; i < 20; i++)
                {
                    int widthInTiles = this._level.GetWidthInTiles();
                    int heightInTiles = this._level.GetHeightInTiles();
                    int x = this._obstacleRespawnRandom.Rand(widthInTiles - data.Width + 1);
                    int y = this._obstacleRespawnRandom.Rand(heightInTiles - data.Height + 1);

                    if (this._level.IsValidPlaceForObstacle(x, y, data.Width, data.Height, true, true))
                    {
                        LogicObstacle obstacle = (LogicObstacle) LogicGameObjectFactory.CreateGameObject(data, this._level, this._villageType);

                        if (data.GetLootCount() > 0)
                        {
                            obstacle.SetLootMultiplyVersion(2);
                        }

                        obstacle.SetInitialPosition(x << 9, y << 9);

                        this.AddGameObject(obstacle);

                        return true;
                    }
                }
            }
            else
            {
                Debugger.Warning("randomlyPlaceObstacle; trying to place obstacle in wrong village");
            }

            return false;
        }

        /// <summary>
        ///     Places randomly the obstacle.
        /// </summary>
        public bool RandomlyPlaceSpecialObstacle(LogicObstacleData data, int width, int height)
        {
            int levelWidth = this._level.GetWidthInTiles();
            int levelHeight = this._level.GetHeightInTiles();
            int possibility = levelWidth * levelHeight;
            int x = this._obstacleRespawnRandom.Rand(levelWidth);
            int y = this._obstacleRespawnRandom.Rand(levelHeight);

            if (possibility > 0)
            {
                do
                {
                    if (!this._level.IsValidPlaceForObstacle(x, y, levelWidth, levelHeight, false, true))
                    {
                        if (++x + width > levelWidth)
                        {
                            if (++y + height > levelHeight)
                            {
                                y = 0;
                            }

                            x = 0;
                        }
                    }
                    else
                    {
                        LogicObstacle obstacle = (LogicObstacle) LogicGameObjectFactory.CreateGameObject(data, this._level, this._villageType);
                        obstacle.SetInitialPosition(x << 9, y << 9);
                        this.AddGameObject(obstacle);

                        return true;
                    }
                } while (--possibility > 1);
            }

            return false;
        }

        /// <summary>
        ///     Enables the gameobject manager.
        /// </summary>
        public void ChangeVillageType(bool isEnabled)
        {
            for (int i = 0; i < 9; i++)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[i];

                for (int j = 0; j < gameObjects.Count; j++)
                {
                    LogicGameObject gameObject = gameObjects[j];

                    if (isEnabled)
                    {
                        this._tileMap.AddGameObject(gameObject);

                        if (LogicDataTables.GetGlobals().UseTeslaTriggerCommand())
                        {
                            if (gameObject.GetGameObjectType() == 0)
                            {
                                // TODO: Implement this.
                            }
                        }

                        if (LogicDataTables.GetGlobals().UseTrapTriggerCommand())
                        {
                            if (gameObject.GetGameObjectType() == 4)
                            {
                                // TODO: Implement this.
                            }
                        }
                    }
                    else
                    {
                        this._tileMap.RemoveGameObject(gameObject);
                    }
                }
            }
        }

        /// <summary>
        ///     Destructes gameobjects which must be destroyed.
        /// </summary>
        public void DoDestucting()
        {
            bool projectileDestructed = false;

            for (int type = 0; type < 9; type++)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[type];

                if (gameObjects.Count > 0)
                {
                    int idx = 0;

                    do
                    {

                        LogicGameObject gameObject = gameObjects[idx];

                        if (gameObject.ShouldDestruct())
                        {
                            gameObjects.Remove(idx--);

                            this.RemoveGameObjectReferences(gameObject);
                            this._componentManager.RemoveGameObjectReferences(gameObject);

                            if (gameObject != null)
                            {
                                gameObject.Destruct();
                            }

                            if (type == 2)
                            {
                                projectileDestructed = true;
                            }
                        }
                    } while (++idx < gameObjects.Count);
                }
            }

            if (this._level.GetGameMode().GetConfiguration().GetBatteWaitForProjectileDestruction())
            {
                if (projectileDestructed)
                {
                    if (this._gameObjects[2].Count == 0)
                    {
                        this._level.UpdateBattleStatus();
                    }
                }
            }
        }

        /// <summary>
        ///     Ticks for update this instance. Called before Tick method.
        /// </summary>
        public void SubTick()
        {
            this._componentManager.SubTick();

            LogicArrayList<LogicGameObject> buildings = this._gameObjects[0];
            LogicArrayList<LogicGameObject> characters = this._gameObjects[1];
            LogicArrayList<LogicGameObject> projectiles = this._gameObjects[2];
            LogicArrayList<LogicGameObject> spells = this._gameObjects[7];
            LogicArrayList<LogicGameObject> vObjects = this._gameObjects[8];

            for (int i = 0; i < characters.Count; i++)
            {
                characters[i].SubTick();
            }

            for (int i = 0; i < spells.Count; i++)
            {
                spells[i].SubTick();
            }

            for (int i = 0; i < buildings.Count; i++)
            {
                buildings[i].SubTick();
            }

            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].SubTick();
            }

            for (int i = 0; i < vObjects.Count; i++)
            {
                vObjects[i].SubTick();
            }
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public void Tick()
        {
            this.DoDestucting();

            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                this._unitProduction.Tick();
                this._spellProduction.Tick();
            }

            this._componentManager.Tick();

            for (int i = 0; i < 9; i++)
            {
                LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[i];

                for (int j = 0; j < gameObjects.Count; j++)
                {
                    gameObjects[j].Tick();
                }
            }
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            this.Load(jsonObject, false, true);
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject, bool ignoreRespawnVars, bool loadObstacle)
        {
            this._specialObstacleData = this._level.GetGameMode().GetConfiguration().GetSpecialObstacleData();

            for (int i = 0; i < 9; i++)
            {
                if (this._gameObjects[i].Count != 0)
                {
                    Debugger.Error("LogicGameObjectManager::load - numGameObjects is non zero!");
                    return;
                }
            }

            if (this._villageType == 1)
            {
                LogicJSONArray buildingArray = jsonObject.GetJSONArray("buildings2");
                LogicJSONArray trapArray = jsonObject.GetJSONArray("traps2");
                LogicJSONArray decoArray = jsonObject.GetJSONArray("decos2");

                if (buildingArray != null)
                {
                    this.LoadGameObjectsJsonArray(buildingArray);
                }

                if (loadObstacle)
                {
                    LogicJSONArray vObjArray = jsonObject.GetJSONArray("vobjs2");
                    LogicJSONArray obstacleArray = jsonObject.GetJSONArray("obstacles2");

                    if (obstacleArray != null)
                    {
                        this.LoadGameObjectsJsonArray(obstacleArray);
                    }

                    if (vObjArray != null)
                    {
                        this.LoadGameObjectsJsonArray(vObjArray);
                    }
                }

                if (trapArray != null)
                {
                    this.LoadGameObjectsJsonArray(trapArray);
                }

                if (decoArray != null)
                {
                    this.LoadGameObjectsJsonArray(decoArray);
                }

                if (!ignoreRespawnVars)
                {
                    LogicJSONNumber respawnSecondsObject = jsonObject.GetJSONNumber("v2rs");

                    if (respawnSecondsObject != null)
                    {
                        this._secondsFromLastRespawn = respawnSecondsObject.GetIntValue();
                    }

                    LogicJSONNumber respawnSeedObject = jsonObject.GetJSONNumber("v2rseed");

                    if (respawnSeedObject != null)
                    {
                        this._obstacleRespawnRandom.SetIteratedRandomSeed(respawnSeedObject.GetIntValue());
                    }
                    else
                    {
                        this._obstacleRespawnRandom.SetIteratedRandomSeed(112);
                    }

                    LogicJSONNumber respawnClearCounterObject = jsonObject.GetJSONNumber("v2ccounter");

                    if (respawnClearCounterObject != null)
                    {
                        this._obstacleClearCounter = respawnClearCounterObject.GetIntValue();
                    }

                    LogicJSONNumber respawnTallGrassSecondsObject = jsonObject.GetJSONNumber("tgsec");

                    if (respawnTallGrassSecondsObject != null)
                    {
                        this._secondsFromLastTallGrassRespawn = respawnTallGrassSecondsObject.GetIntValue();
                    }

                    LogicJSONNumber respawnTallGrassSeedObject = jsonObject.GetJSONNumber("tgseed");

                    if (respawnTallGrassSeedObject != null)
                    {
                        this._tallGrassRespawnRandom.SetIteratedRandomSeed(respawnTallGrassSeedObject.GetIntValue());
                    }
                }
            }
            else
            {
                LogicJSONArray buildingArray = jsonObject.GetJSONArray("buildings");
                LogicJSONArray trapArray = jsonObject.GetJSONArray("traps");
                LogicJSONArray decoArray = jsonObject.GetJSONArray("decos");

                if (buildingArray != null)
                {
                    this.LoadGameObjectsJsonArray(buildingArray);
                }
                else
                {
                    Debugger.Error("LogicGameObjectManager::load - Building array is NULL!");
                }

                if (loadObstacle)
                {
                    LogicJSONArray obstacleArray = jsonObject.GetJSONArray("obstacles");
                    LogicJSONArray vObjArray = jsonObject.GetJSONArray("vobjs");

                    if (obstacleArray != null)
                    {
                        this.LoadGameObjectsJsonArray(obstacleArray);
                    }

                    if (vObjArray != null)
                    {
                        this.LoadGameObjectsJsonArray(vObjArray);
                    }
                }

                if (trapArray != null)
                {
                    this.LoadGameObjectsJsonArray(trapArray);
                }

                if (decoArray != null)
                {
                    this.LoadGameObjectsJsonArray(decoArray);
                }

                if (!ignoreRespawnVars)
                {
                    LogicJSONObject respawnVarsObject = jsonObject.GetJSONObject("respawnVars");

                    if (respawnVarsObject != null)
                    {
                        this._secondsFromLastRespawn = respawnVarsObject.GetJSONNumber("secondsFromLastRespawn").GetIntValue();
                        this._obstacleRespawnRandom.SetIteratedRandomSeed(respawnVarsObject.GetJSONNumber("respawnSeed").GetIntValue());
                        this._obstacleClearCounter = respawnVarsObject.GetJSONNumber("obstacleClearCounter").GetIntValue();

                        LogicJSONNumber timeToGemboxDropObject = respawnVarsObject.GetJSONNumber("time_to_gembox_drop");

                        if (timeToGemboxDropObject != null)
                        {
                            this._gemBoxDropSecs = timeToGemboxDropObject.GetIntValue();
                        }
                        else
                        {
                            if (this._bonusGemboxData != null)
                            {
                                int random = this._obstacleRespawnRandom.Rand(this._bonusGemboxData.AppearancePeriodHours);

                                this._gemBoxDropSecs = 3600 * random + this._gemBoxPeriodSecs;
                                this._gemBoxPeriodSecs = 3600 * (this._bonusGemboxData.AppearancePeriodHours - random);
                            }
                        }

                        LogicJSONNumber timeToGemboxPeriodObject = respawnVarsObject.GetJSONNumber("time_in_gembox_period");

                        if (timeToGemboxPeriodObject != null)
                        {
                            this._gemBoxPeriodSecs = timeToGemboxPeriodObject.GetIntValue();
                        }

                        if (this._specialObstacleData != null)
                        {
                            LogicJSONNumber timeToSpecialDropObject = respawnVarsObject.GetJSONNumber("time_to_special_drop");

                            if (timeToSpecialDropObject != null)
                            {
                                this._specialObstacleDropSecs = timeToSpecialDropObject.GetIntValue();
                            }
                            else
                            {
                                int random = this._obstacleRespawnRandom.Rand(this._specialObstacleData.AppearancePeriodHours);

                                this._specialObstacleDropSecs = 3600 * random + this._specialObstaclePeriodSecs;
                                this._specialObstaclePeriodSecs = 3600 * (this._specialObstacleData.AppearancePeriodHours - random);
                            }

                            LogicJSONNumber timeToSpecialPeriodObject = respawnVarsObject.GetJSONNumber("time_to_special_period");

                            if (timeToSpecialPeriodObject != null)
                            {
                                this._specialObstaclePeriodSecs = timeToSpecialPeriodObject.GetIntValue();
                            }
                        }
                    }
                    else
                    {
                        Debugger.Warning("Can't find respawn variables");

                        this._obstacleRespawnRandom.SetIteratedRandomSeed(112);
                        this._gemBoxDropSecs = 604800;

                        if (this._bonusGemboxData != null)
                        {
                            this._gemBoxDropSecs = 3600 * this._bonusGemboxData.AppearancePeriodHours;
                        }

                        this._specialObstacleDropSecs = 604800;

                        if (this._specialObstacleData != null)
                        {
                            this._specialObstacleDropSecs = 3600 * this._specialObstacleData.AppearancePeriodHours;
                        }
                    }
                }

                if (LogicDataTables.GetGlobals().UseNewTraining())
                {
                    LogicJSONObject unitsObject = jsonObject.GetJSONObject("units");
                    LogicJSONObject spellsObjects = jsonObject.GetJSONObject("spells");

                    if (unitsObject != null)
                    {
                        this._unitProduction.Load(unitsObject);
                    }

                    if (spellsObjects != null)
                    {
                        this._spellProduction.Load(spellsObjects);
                    }
                }
            }

            this._tileMap.EnableRoomIndices(false); // TRUE !!!!
        }

        /// <summary>
        ///     Loads gameobjects from json array.
        /// </summary>
        public void LoadGameObjectsJsonArray(LogicJSONArray jsonArray)
        {
            int size = jsonArray.Size();

            for (int i = 0; i < size; i++)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) jsonArray[i];

                if (jsonObject != null)
                {
                    LogicJSONNumber jsonData = jsonObject.GetJSONNumber("data");

                    if (jsonData != null)
                    {
                        LogicData data = LogicDataTables.GetDataById(jsonData.GetIntValue());

                        if (data != null)
                        {
                            int dataType = data.GetDataType();

                            if (dataType != 0 &&
                                dataType != 7 &&
                                dataType != 11 &&
                                dataType != 17 &&
                                dataType != 38)
                            {
                                return;
                            }

                            LogicGameObject gameObject = LogicGameObjectFactory.CreateGameObject(data, this._level, this._villageType);

                            if (gameObject != null)
                            {
                                LogicJSONNumber jsonId = jsonObject.GetJSONNumber("id");

                                if (jsonId != null)
                                {
                                    gameObject.SetGlobalID(jsonId.GetIntValue());
                                }

                                gameObject.Load(jsonObject);

                                this.AddGameObject(gameObject);
                            }
                        }
                        else
                        {
                            Debugger.Error("LogicGameObjectManager::load - Data is NULL!");
                        }
                    }
                    else
                    {
                        Debugger.Error("LogicGameObjectManager::load - Data id was not found!");
                    }
                }
                else
                {
                    Debugger.Error("LogicGameObjectManager::load - Building is NULL!");
                }
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject jsonObject)
        {
            if (this._villageType == 1)
            {
                jsonObject.Put("buildings2", this.SaveGameObjects(0));
                jsonObject.Put("obstacles2", this.SaveGameObjects(3));
                jsonObject.Put("traps2", this.SaveGameObjects(4));
                jsonObject.Put("decos2", this.SaveGameObjects(6));

                if (!this._level.IsNpcVillage())
                {
                    if (LogicDataTables.GetGlobals().SaveVillageObjects())
                    {
                        jsonObject.Put("vobjs2", this.SaveGameObjects(8));
                    }

                    int passedSecs = LogicTime.GetTicksInSeconds(this._level.GetLogicTime());

                    jsonObject.Put("v2rs", new LogicJSONNumber(passedSecs + this._secondsFromLastRespawn));
                    jsonObject.Put("v2rseed", new LogicJSONNumber(this._obstacleRespawnRandom.GetIteratedRandomSeed()));
                    jsonObject.Put("v2ccounter", new LogicJSONNumber(this._obstacleClearCounter));
                    jsonObject.Put("tgsec", new LogicJSONNumber(this._secondsFromLastTallGrassRespawn));
                    jsonObject.Put("tgseed", new LogicJSONNumber(this._tallGrassRespawnRandom.GetIteratedRandomSeed()));
                }
            }
            else
            {
                jsonObject.Put("buildings", this.SaveGameObjects(0));
                jsonObject.Put("obstacles", this.SaveGameObjects(3));
                jsonObject.Put("traps", this.SaveGameObjects(4));
                jsonObject.Put("decos", this.SaveGameObjects(6));

                if (!this._level.IsNpcVillage())
                {
                    if (LogicDataTables.GetGlobals().SaveVillageObjects())
                    {
                        jsonObject.Put("vobjs", this.SaveGameObjects(8));
                    }

                    LogicJSONObject respawnObject = new LogicJSONObject();

                    int passedSecs = LogicTime.GetTicksInSeconds(this._level.GetLogicTime());

                    respawnObject.Put("secondsFromLastRespawn", new LogicJSONNumber(passedSecs + this._secondsFromLastRespawn));
                    respawnObject.Put("respawnSeed", new LogicJSONNumber(this._obstacleRespawnRandom.GetIteratedRandomSeed()));
                    respawnObject.Put("obstacleClearCounter", new LogicJSONNumber(this._obstacleClearCounter));

                    int maxGemBoxRespawnSecs = this._bonusGemboxData != null ? 7200 * this._bonusGemboxData.AppearancePeriodHours : 1209600;

                    if (this._gemBoxDropSecs > maxGemBoxRespawnSecs)
                    {
                        this._gemBoxDropSecs = 0;
                        this._gemBoxPeriodSecs = 0;
                    }

                    respawnObject.Put("time_to_gembox_drop", new LogicJSONNumber(this._gemBoxDropSecs - passedSecs));
                    respawnObject.Put("time_in_gembox_period", new LogicJSONNumber(this._gemBoxPeriodSecs - passedSecs));

                    jsonObject.Put("respawnVars", respawnObject);

                    if (LogicDataTables.GetGlobals().UseNewTraining())
                    {
                        LogicJSONObject unitObject = new LogicJSONObject();
                        LogicJSONObject spellObject = new LogicJSONObject();
                        this._unitProduction.Save(unitObject);
                        this._spellProduction.Save(spellObject);
                        jsonObject.Put("units", unitObject);
                        jsonObject.Put("spells", spellObject);
                    }
                }
            }
        }

        /// <summary>
        ///     Saves the <see cref="LogicArrayList{T}"/> to json array.
        /// </summary>
        public LogicJSONArray SaveGameObjects(int gameObjectType)
        {
            LogicArrayList<LogicGameObject> gameObjects = this._gameObjects[gameObjectType];
            LogicJSONArray jsonArray = new LogicJSONArray(gameObjects.Count);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                LogicGameObject gameObject = gameObjects[i];
                LogicJSONObject jsonObject = new LogicJSONObject();
                jsonObject.Put("data", new LogicJSONNumber(gameObject.GetData().GetGlobalID()));
                jsonObject.Put("id", new LogicJSONNumber(gameObject.GetGlobalID()));
                gameObject.Save(jsonObject);
                jsonArray.Add(jsonObject);
            }

            return jsonArray;
        }

        /// <summary>
        ///     Gets the clock tower.
        /// </summary>
        public LogicBuilding GetClockTower()
        {
            return this._clockTower;
        }

        /// <summary>
        ///     Gets the alliance castle.
        /// </summary>
        public LogicBuilding GetAllianceCastle()
        {
            return this._allianceCastle;
        }

        /// <summary>
        ///     Gets the shipyard.
        /// </summary>
        public LogicVillageObject GetShipyard()
        {
            return this._shipyard;
        }

        /// <summary>
        ///     Gets the laboratory.
        /// </summary>
        /// <returns></returns>
        public LogicBuilding GetLaboratory()
        {
            return this._laboratory;
        }
    }
}