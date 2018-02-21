namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Unit;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;
    
    public sealed class LogicGameObjectManager
    {
        private int _villageType;
        private int[] _gameObjectIds;

        private LogicLevel _level;
        private LogicTileMap _tileMap;
        private LogicUnitProduction _unitProduction;
        private LogicUnitProduction _spellProduction;
        private LogicComponentManager _componentManager;
        private LogicArrayList<LogicGameObject>[] _gameObjects;

        private LogicBuilding _townHall;
        private LogicBuilding _townHallVillage2;
        private LogicBuilding _allianceCastle;
        private LogicBuilding _laboratory;

        private LogicData _specialObstacleData;

        private int _specialObstacleDropTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObjectManager"/> class.
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
                this._gameObjects[i] = new LogicArrayList<LogicGameObject>();
            }

            this._componentManager = new LogicComponentManager(level);

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

            this._level = null;
            this._tileMap = null;
            this._townHall = null;
            this._townHallVillage2 = null;
            this._allianceCastle = null;
            this._laboratory = null;
            this._specialObstacleData = null;
        }

        /// <summary>
        ///     Adds the specified gameobject to this instance.
        /// </summary>
        public void AddGameObject(LogicGameObject gameObject)
        {
            if (gameObject.GetGameObjectType() > 8)
            {
                Debugger.Error("LogicGameObjectManager::generateGameObjectGlobalID(). Index is out of bounds.");
            }

            if (gameObject.GetGlobalID() == -1)
            {
                gameObject.SetGlobalID(this.GenerateGameObjectGlobalID(gameObject));
            }

            if (gameObject.GetGameObjectType() == 0)
            {
                LogicBuilding building = (LogicBuilding) gameObject;
                LogicBuildingData buildingData = building.GetBuildingData();
                
                if (buildingData.IsWorkerBuilding() || buildingData.IsWorker2Building())
                {
                    this._level.GetWorkerManagerAt(this._villageType).IncreaseWorkerCount();
                }

                if (buildingData.IsTownHall())
                {
                    this._townHall = building;
                }

                if (buildingData.IsTownHall2())
                {
                    this._townHallVillage2 = building;
                }

                if (buildingData.IsLaboratory())
                {
                    this._laboratory = building;
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
            if (gameObject.GetGameObjectType() > 8)
            {
                Debugger.Error("LogicGameObjectManager::generateGameObjectGlobalID(). Index is out of bounds.");
            }

            return GlobalID.CreateGlobalID(gameObject.GetGameObjectType() + 500, this._gameObjectIds[gameObject.GetGameObjectType()]++);
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
        ///     Gets the component manager instance.
        /// </summary>
        public LogicComponentManager GetComponentManager()
        {
            return this._componentManager;
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

            this._tileMap.EnableRoomIndices(true);
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
            // Save.
        }
    }
}