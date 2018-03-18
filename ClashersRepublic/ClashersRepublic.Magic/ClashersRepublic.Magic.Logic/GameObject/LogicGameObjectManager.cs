namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.GameObject.Listener;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Unit;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

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
        private LogicRandom _tileGrassRespawnRandom;
        private readonly LogicComponentManager _componentManager;
        private readonly LogicArrayList<LogicGameObject>[] _gameObjects;

        private LogicBuilding _allianceCastle;
        private LogicBuilding _clockTower;
        private LogicBuilding _townHall;
        private LogicBuilding _laboratory;
        //private LogicAlliancePortal _alliancePortal;
        private LogicObstacle _lootCart;
        private LogicVillageObject _shipyard;
        private LogicVillageObject _rowBoat;
        private LogicVillageObject _clanGate;
        private LogicObstacleData _gemBoxData;
        private LogicData _specialObstacleData;

        private int _fastForwardRespawnSecs;
        private int _specialObstacleDropTime;
        private int _obstacleClearCounter;
        private int _gemBoxDropSecs;
        private int _gemBoxPeriodSecs;
        private int _passedRespawnSecs;
        private int _passedTimeGrassRespawnSecs;

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
                this._gameObjects[i] = new LogicArrayList<LogicGameObject>(128);
            }

            this._gemBoxData = LogicDataTables.GetObstacleByName("Bonus Gembox");
            this._componentManager = new LogicComponentManager(level);
            this._listener = new LogicGameObjectManagerListener();
            this._obstacleRespawnRandom = new LogicRandom();
            this._tileGrassRespawnRandom = new LogicRandom();

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
            this._tileGrassRespawnRandom = null;
            this._level = null;
            this._tileMap = null;
            this._allianceCastle = null;
            this._clockTower = null;
            this._townHall = null;
            this._laboratory = null;
            this._lootCart = null;
            this._shipyard = null;
            this._rowBoat = null;
            this._clanGate = null;
            this._gemBoxData = null;
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
                gameObject.SetGlobalID(this.GenerateGameObjectGlobalID(gameObject));
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
                            /*v32 = *(_DWORD*)(v3 + 200)
                            sub_238DCC(v32, (int)&v52);*/
                        }
                        else if (buildingData.GetProducesUnitsOfType() == 2)
                        {
                            /*v32 = *(_DWORD*)(v3 + 204);
                            sub_238DCC(v32, (int)&v52);*/
                        }
                    }
                }

                if (buildingData.IsClockTower())
                {
                    this._clockTower = building;
                }

                if (buildingData.IsTownHall() || buildingData.IsTownHall2())
                {
                    this._townHall = building;
                }

                if (buildingData.IsWorkerBuilding() || buildingData.IsTownHall2())
                {
                    this._level.GetWorkerManagerAt(this._villageType).IncreaseWorkerCount();
                }

                if (buildingData.IsLaboratory())
                {
                    this._laboratory = building;
                }

                if (buildingData.GetUnitProduction(0) >= 1)
                {
                    //++*(_DWORD*)(v3 + 292);
                }

                if (buildingData.ForgesSpells)
                {
                    int unitsOfType = buildingData.GetProducesUnitsOfType();
                    /*v49 = v3 + 192;
                    if (unitsOfType == 1)
                        v49 = v3 + 188;
                    *(_DWORD*)v49 = v52;*/
                }
            }
            else if (gameObjectType == 3)
            {
                LogicObstacle obstacleObject = (LogicObstacle)gameObject;
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
                LogicVillageObject villageObject = (LogicVillageObject)gameObject;
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

            this._level.GetTileMap().AddGameObject(gameObject);
            this._gameObjects[gameObject.GetGameObjectType()].Add(gameObject);
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
        ///     Gets the highest building level.
        /// </summary>
        public int GetHighestBuildingLevel(LogicBuildingData data)
        {
            return this.GetHighestBuildingLevel(data, true);
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
        public void GetChecksum(ChecksumHelper checksum, int mode)
        {
            checksum.StartObject("LogicGameObjectManager");

            int num = 0;

            for (int i = 0; i < 9; i++)
            {
                num += this._gameObjects[i].Count;
            }

            checksum.WriteValue("numGameObjects", num);

            if (mode == 1)
            {
                for (int i = 0; i < 9; i++)
                {
                    checksum.StartArray("type" + i);

                    for (int j = 0; j < this._gameObjects[i].Count; j++)
                    {
                        this._gameObjects[i][j].GetChecksum(checksum);
                    }

                    checksum.EndArray();
                }
            }
            else
            {
                checksum.StartArray("type0");

                for (int i = 0; i < this._gameObjects[0].Count; i++)
                {
                    this._gameObjects[0][i].GetChecksum(checksum);
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
        ///     Called when the loading is finished.
        /// </summary>
        public void LoadingFinished()
        {
            // TODO: Implement LogicGameObjectManager::loadingFinished();
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardTime(int totalSecs)
        {
            if (totalSecs > 0)
            {
                Debugger.Log("LogicGameObjectManager::fastForward: " + totalSecs);

                this._passedRespawnSecs += totalSecs;
                this._passedTimeGrassRespawnSecs += totalSecs;

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < this._gameObjects[i].Count; j++)
                    {
                        this._gameObjects[i][j].FastForwardTime(totalSecs);
                    }
                }
            }
        }

        /// <summary>
        ///     Destructes gameobjects which must be destroyed.
        /// </summary>
        public void DoDestucting()
        {
            // TODO Implement LogicGameObjectManager::doDestructing.
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
                    jsonObject.GetJSONNumber("v2rs");
                    jsonObject.GetJSONNumber("v2rseed");
                    jsonObject.GetJSONNumber("v2ccounter");
                    jsonObject.GetJSONNumber("tgsec");
                    jsonObject.GetJSONNumber("tgseed");
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
                        respawnVarsObject.GetJSONNumber("secondsFromLastRespawn");
                        respawnVarsObject.GetJSONNumber("respawnSeed");
                        respawnVarsObject.GetJSONNumber("obstacleClearCounter");
                        respawnVarsObject.GetJSONNumber("time_to_gembox_drop");
                        respawnVarsObject.GetJSONNumber("time_in_gembox_period");

                        if (this._specialObstacleData != null)
                        {
                            respawnVarsObject.GetJSONNumber("time_to_special_drop");
                            respawnVarsObject.GetJSONNumber("time_to_special_period");
                        }
                    }
                    else
                    {
                        Debugger.Warning("Can't find respawn variables");
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

                    if (unitsObject != null)
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

                    jsonObject.Put("v2rs", new LogicJSONNumber(LogicTime.GetTicksInSeconds(this._passedRespawnSecs) + this._fastForwardRespawnSecs));
                    jsonObject.Put("v2rseed", new LogicJSONNumber(this._obstacleRespawnRandom.GetIteratedRandomSeed()));
                    jsonObject.Put("v2ccounter", new LogicJSONNumber(this._obstacleClearCounter));
                    jsonObject.Put("tgsec", new LogicJSONNumber(LogicTime.GetTicksInSeconds(this._passedTimeGrassRespawnSecs)));
                    jsonObject.Put("tgseed", new LogicJSONNumber(this._tileGrassRespawnRandom.GetIteratedRandomSeed()));
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

                    int passedRespawnTime = LogicTime.GetTicksInSeconds(this._passedRespawnSecs);

                    respawnObject.Put("secondsFromLastRespawn", new LogicJSONNumber(passedRespawnTime + this._fastForwardRespawnSecs));
                    respawnObject.Put("respawnSeed", new LogicJSONNumber(this._obstacleRespawnRandom.GetIteratedRandomSeed()));
                    respawnObject.Put("obstacleClearCounter", new LogicJSONNumber(this._obstacleClearCounter));

                    int maxGemBoxRespawnSecs = this._gemBoxData != null ? 7200 * this._gemBoxData.AppearancePeriodHours : 1209600;

                    if (this._gemBoxDropSecs > maxGemBoxRespawnSecs)
                    {
                        this._gemBoxDropSecs = 0;
                        this._gemBoxPeriodSecs = 0;
                    }

                    respawnObject.Put("time_to_gembox_drop", new LogicJSONNumber(this._gemBoxDropSecs - passedRespawnTime));
                    respawnObject.Put("time_in_gembox_period", new LogicJSONNumber(this._gemBoxPeriodSecs - passedRespawnTime));

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
    }
}