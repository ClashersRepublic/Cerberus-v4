namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.Achievement;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Battle;
    using ClashersRepublic.Magic.Logic.Cooldown;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Logic.Offer;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Logic.Worker;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicLevel
    {
        private LogicGameMode _gameMode;
        private LogicClientHome _clientHome;
        private LogicAvatar _homeOwnerAvatar;
        private LogicAvatar _visitorAvatar;
        private LogicTileMap _tileMap;
        private LogicRect _map;

        private readonly LogicGameObjectManager[] _gameObjectManagers;
        private readonly LogicWorkerManager[] _workerManagers;
        private LogicOfferManager _offerManager;
        private LogicAchievementManager _achievementManager;
        private LogicCooldownManager _cooldownManager;
        private LogicBattleLog _battleLog;
        private LogicGameListener _gameListener;
        private LogicJSONObject _levelJSON;

        private readonly LogicArrayList<int> _layoutState;
        private readonly LogicArrayList<int> _layoutCooldown;
        private readonly LogicArrayList<int> _layoutStateVillage2;
        private readonly LogicArrayList<string> _armyNames;
        private LogicArrayList<LogicDataSlot> _unplacedObjects;

        private LogicArrayList<int> _newShopBuildings;
        private LogicArrayList<int> _newShopTraps;
        private LogicArrayList<int> _newShopDecos;

        private int _liveReplayUpdateFrequency;

        private int _villageType;
        private int _warLayout;
        private int _activeLayout;
        private int _activeLayoutVillage2;
        private int _lastLeagueRank;
        private int _lastAllianceLevel;
        private int _lastLeagueShuffle;
        private int _lastSeasonSeen;
        private int _lastNewsSeen;
        private int _waveNumber;
        private int _experienceVersion;
        private int _warTutorialsSeen;
        private int _matchType;
        private int _remainingClockTowerBoostTime;

        private bool _helpOpened;
        private bool _warBase;
        private bool _editModeShown;
        private bool _npcVillage;
        private bool _androidClient;
        private bool _battleStarted;
        private bool _battleEndPending;
        private bool _isWarLevel;
        private bool _isDirectLevel;
        private bool _isDirectVillage2Level;
        private bool _invulverabilityEnabled;

        private string _warRequestMessage;
        private string _troopRequestMessage;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLevel" /> class.
        /// </summary>
        public LogicLevel(LogicGameMode gameMode)
        {
            this._gameMode = gameMode;

            this._gameListener = new LogicGameListener();
            this._achievementManager = new LogicAchievementManager(this);
            this._layoutState = new LogicArrayList<int>();
            this._armyNames = new LogicArrayList<string>();
            this._gameObjectManagers = new LogicGameObjectManager[2];
            this._workerManagers = new LogicWorkerManager[2];
            this._tileMap = new LogicTileMap(50, 50);

            for (int i = 0; i < 2; i++)
            {
                this._workerManagers[i] = new LogicWorkerManager(this);
                this._gameObjectManagers[i] = new LogicGameObjectManager(this._tileMap, this, i);
            }

            this._offerManager = new LogicOfferManager();
            this._map = new LogicRect(3, 3, 47, 47);
            this._cooldownManager = new LogicCooldownManager();
            this._battleLog = new LogicBattleLog(this);
            this._layoutState = new LogicArrayList<int>(8);
            this._layoutCooldown = new LogicArrayList<int>(8);
            this._layoutStateVillage2 = new LogicArrayList<int>(8);
            this._unplacedObjects = new LogicArrayList<LogicDataSlot>();
            this._newShopBuildings = new LogicArrayList<int>();
            this._newShopTraps = new LogicArrayList<int>();
            this._newShopDecos = new LogicArrayList<int>();

            LogicDataTable buildingTable = LogicDataTables.GetTable(0);
            LogicDataTable trapTable = LogicDataTables.GetTable(11);
            LogicDataTable decoTable = LogicDataTables.GetTable(17);

            this._newShopBuildings.EnsureCapacity(buildingTable.GetItemCount());

            for (int i = 0; i < buildingTable.GetItemCount(); i++)
            {
                this._newShopBuildings.Add(0);
            }

            this._newShopBuildings.EnsureCapacity(trapTable.GetItemCount());

            for (int i = 0; i < trapTable.GetItemCount(); i++)
            {
                this._newShopTraps.Add(0);
            }

            this._newShopBuildings.EnsureCapacity(decoTable.GetItemCount());

            for (int i = 0; i < decoTable.GetItemCount(); i++)
            {
                this._newShopDecos.Add(0);
            }

            LogicGlobals globalsInstance = LogicDataTables.GetGlobals();

            if (globalsInstance.LiveReplayEnabled())
            {
                this._liveReplayUpdateFrequency = LogicTime.GetSecondsInTicks(globalsInstance.GetLiveReplayUpdateFrequencySecs());
            }
            else
            {
                this._liveReplayUpdateFrequency = 0;
            }

            for (int i = 0; i < 8; i++)
            {
                this._layoutState.Add(0);
            }

            this._layoutCooldown.EnsureCapacity(8);

            for (int i = 0; i < 8; i++)
            {
                this._layoutCooldown.Add(0);
            }

            this._layoutStateVillage2.EnsureCapacity(8);

            for (int i = 0; i < 8; i++)
            {
                this._layoutStateVillage2.Add(0);
            }

            this._armyNames.EnsureCapacity(4);

            for (int i = 0; i < 4; i++)
            {
                this._armyNames.Add(string.Empty);
            }
        }

        /// <summary>
        ///     Gets the game listener.
        /// </summary>
        public LogicGameListener GetGameListener()
        {
            return this._gameListener;
        }

        /// <summary>
        ///     Sets the game listener.
        /// </summary>
        public void SetGameListener(LogicGameListener listener)
        {
            this._gameListener = listener;
        }

        /// <summary>
        ///     Gets if the invulnerability is enabled.
        /// </summary>
        public bool InvulnerabilityEnabled()
        {
            return this._invulverabilityEnabled;
        }

        /// <summary>
        ///     Sets the invulnerability state.
        /// </summary>
        public void SetInvulnerability(bool state)
        {
            this._invulverabilityEnabled = state;
        }

        /// <summary>
        ///     Gets the state.
        /// </summary>
        public int GetState()
        {
            if (this._gameMode != null)
            {
                return this._gameMode.GetState();
            }

            return 0;
        }

        /// <summary>
        ///     Gets the remaining clock tower boost time.
        /// </summary>
        public int GetRemainingClockTowerBoostTime()
        {
            return this._remainingClockTowerBoostTime;
        }

        /// <summary>
        ///     Gets a value indicating whether the battle end is pending.
        /// </summary>
        public bool GetBattleEndPending()
        {
            return this._battleEndPending;
        }

        /// <summary>
        ///     Gets the logic time.
        /// </summary>
        public LogicTime GetLogicTime()
        {
            if (this._gameMode != null)
            {
                return this._gameMode.GetLogicTime();
            }

            return null;
        }

        /// <summary>
        ///     Gets the worker manager instance.
        /// </summary>
        public LogicWorkerManager GetWorkerManager()
        {
            return this._workerManagers[this._villageType];
        }

        /// <summary>
        ///     Gets the worker manager instance at specified index.
        /// </summary>
        public LogicWorkerManager GetWorkerManagerAt(int index)
        {
            return this._workerManagers[index];
        }

        /// <summary>
        ///     Gets the gameobject manager instance.
        /// </summary>
        public LogicGameObjectManager GetGameObjectManager()
        {
            return this._gameObjectManagers[this._villageType];
        }

        /// <summary>
        ///     Gets the gameobject manager instance at specified index.
        /// </summary>
        public LogicGameObjectManager GetGameObjectManagerAt(int index)
        {
            return this._gameObjectManagers[index];
        }

        /// <summary>
        ///     Gets the component manager instance.
        /// </summary>
        public LogicComponentManager GetComponentManager(int villageType)
        {
            return this._gameObjectManagers[villageType].GetComponentManager();
        }

        /// <summary>
        ///     Gets the tile map instance.
        /// </summary>
        public LogicTileMap GetTileMap()
        {
            return this._tileMap;
        }

        /// <summary>
        ///     Gets the player avatar instance.
        /// </summary>
        public LogicAvatar GetPlayerAvatar()
        {
            if (this.GetState() == 1 || this.GetState() == 3)
            {
                return this._homeOwnerAvatar;
            }

            return this._visitorAvatar;
        }

        /// <summary>
        ///     Gets the home owner avatar.
        /// </summary>
        public LogicAvatar GetHomeOwnerAvatar()
        {
            return this._homeOwnerAvatar;
        }

        /// <summary>
        ///     Gets the visitor45- avatar.
        /// </summary>
        public LogicAvatar GetVisitorAvatar()
        {
            return this._visitorAvatar;
        }

        /// <summary>
        ///     Gets the home.
        /// </summary>
        public LogicClientHome GetHome()
        {
            return this._clientHome;
        }

        /// <summary>
        ///     Sets the home instance.
        /// </summary>
        public void SetHome(LogicClientHome home, bool isAndroidClient)
        {
            this._clientHome = home;

            this._levelJSON = (LogicJSONObject) LogicJSONParser.Parse(home.GetHomeJSON());

            this._androidClient = LogicJSONHelper.GetJSONBoolean(this._levelJSON, "android_client");
            this._warBase = LogicJSONHelper.GetJSONBoolean(this._levelJSON, "war_base");
            this._activeLayout = LogicJSONHelper.GetJSONNumber(this._levelJSON, "active_layout");
            this._activeLayoutVillage2 = LogicJSONHelper.GetJSONNumber(this._levelJSON, "act_l2");

            if (this._activeLayout < 0)
            {
                this._activeLayout = 0;
            }

            if (this._activeLayoutVillage2 < 0)
            {
                this._activeLayoutVillage2 = 0;
            }

            LogicJSONNumber warLayoutNumber = this._levelJSON.GetJSONNumber("war_layout");

            if (warLayoutNumber != null)
            {
                this._warLayout = warLayoutNumber.GetIntValue();
            }
            else if (this._warBase)
            {
                this._warLayout = 1;
            }

            if (this._warLayout < 0)
            {
                this._warLayout = 0;
            }

            if (this._layoutState.Count > 0)
            {
                for (int i = 0; i < this._layoutState.Count; i++)
                {
                    this._layoutState[i] = 0;
                }
            }

            LogicJSONArray layoutStateArray = this._levelJSON.GetJSONArray("layout_state");

            if (layoutStateArray != null)
            {
                int arraySize = layoutStateArray.Size();

                for (int i = 0; i < this._layoutState.Count; i++)
                {
                    if (i >= arraySize)
                    {
                        break;
                    }

                    LogicJSONNumber numObject = layoutStateArray.GetJSONNumber(i);

                    if (numObject != null)
                    {
                        int num = numObject.GetIntValue();

                        if (num > -1)
                        {
                            this._layoutState[i] = num;
                        }
                    }
                }
            }

            if (this._layoutStateVillage2.Count > 0)
            {
                for (int i = 0; i < this._layoutStateVillage2.Count; i++)
                {
                    this._layoutStateVillage2[i] = 0;
                }
            }

            LogicJSONArray layoutState2Array = this._levelJSON.GetJSONArray("layout_state2");

            if (layoutState2Array != null)
            {
                int arraySize = layoutState2Array.Size();

                for (int i = 0; i < this._layoutStateVillage2.Count; i++)
                {
                    if (i >= arraySize)
                    {
                        break;
                    }

                    LogicJSONNumber numObject = layoutState2Array.GetJSONNumber(i);

                    if (numObject != null)
                    {
                        int num = numObject.GetIntValue();

                        if (num > -1)
                        {
                            this._layoutStateVillage2[i] = num;
                        }
                    }
                }
            }

            if (this._layoutCooldown.Count > 0)
            {
                for (int i = 0; i < this._layoutCooldown.Count; i++)
                {
                    this._layoutCooldown[i] = 0;
                }
            }

            LogicJSONArray layoutCooldownArray = this._levelJSON.GetJSONArray("layout_state2");

            if (layoutCooldownArray != null)
            {
                int arraySize = layoutCooldownArray.Size();

                for (int i = 0; i < this._layoutCooldown.Count; i++)
                {
                    if (i >= arraySize)
                    {
                        break;
                    }

                    LogicJSONNumber numObject = layoutCooldownArray.GetJSONNumber(i);

                    if (numObject != null)
                    {
                        int num = LogicMath.Min(numObject.GetIntValue(), 15 * LogicDataTables.GetGlobals().GetChallengeBaseSaveCooldown());

                        if (num > -1)
                        {
                            this._layoutCooldown[i] = num;
                        }
                    }
                }
            }

            if (this._unplacedObjects != null)
            {
                if (this._unplacedObjects.Count != 0)
                {
                    do
                    {
                        this._unplacedObjects.Remove(0);
                    } while (this._unplacedObjects.Count != 0);
                }
            }

            LogicJSONArray unplacedArray = this._levelJSON.GetJSONArray("unplaced");

            if (unplacedArray != null)
            {
                int arraySize = unplacedArray.Size();

                for (int i = 0; i < arraySize; i++)
                {
                    LogicDataSlot dataSlot = new LogicDataSlot(null, 0);
                    dataSlot.ReadFromJSON(unplacedArray.GetJSONObject(i));
                    this.AddUnplacedObject(dataSlot);
                }
            }

            LogicJSONNumber waveNumObject = this._levelJSON.GetJSONNumber("wave_num");

            if (waveNumObject != null)
            {
                if (this.GetState() != 1)
                {
                    this._waveNumber = waveNumObject.GetIntValue();
                }
            }

            LogicJSONBoolean directObject = this._levelJSON.GetJSONBoolean("direct");

            if (directObject != null)
            {
                this._isDirectLevel = !directObject.IsTrue();
            }
            else
            {
                this._isDirectLevel = true;
            }

            LogicJSONBoolean direct2Object = this._levelJSON.GetJSONBoolean("direct2");

            if (direct2Object != null)
            {
                if (direct2Object.IsTrue())
                {
                }
            }

            if (!this._npcVillage)
            {
                this._experienceVersion = LogicJSONHelper.GetJSONNumber(this._levelJSON, "exp_ver");

                if (this._gameMode.GetState() != 5)
                {
                    if (this._experienceVersion <= 0)
                    {
                        do
                        {
                            // TODO: Implement this.
                        } while (++this._experienceVersion <= 0);
                    }
                }

                if (false)
                {
                }

                for (int i = 0; i < 2; i++)
                {
                    this._gameObjectManagers[i].Load(this._levelJSON);
                }

                this._cooldownManager.Load(this._levelJSON);
                this._offerManager.Load(this._levelJSON);
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void SaveToJSON(LogicJSONObject jsonObject)
        {
            if (!this._npcVillage)
            {
                if (this._waveNumber > 0)
                {
                    LogicJSONHelper.SetJSONNumber(jsonObject, "wave_num", this._waveNumber);
                }

                if (this._experienceVersion > 0)
                {
                    LogicJSONHelper.SetJSONNumber(jsonObject, "exp_ver", this._experienceVersion);
                }

                if (this._androidClient)
                {
                    LogicJSONHelper.SetJSONBoolean(jsonObject, "android_client", this._androidClient);
                }

                if (this._matchType == 3)
                {
                    LogicJSONHelper.SetJSONBoolean(jsonObject, "direct", true);
                }
                else if (this._matchType == 5)
                {
                    LogicJSONHelper.SetJSONBoolean(jsonObject, "war", true);
                }
                else if (this._matchType == 8)
                {
                    LogicJSONHelper.SetJSONBoolean(jsonObject, "direct2", true);
                }

                LogicJSONHelper.SetJSONNumber(jsonObject, "active_layout", this._activeLayout);
                LogicJSONHelper.SetJSONNumber(jsonObject, "act_l2", this._activeLayoutVillage2);

                if (this._warBase)
                {
                    if (LogicDataTables.GetGlobals().RevertBrokenWarLayouts())
                    {
                        /* if ( sub_1E436C(v22) != 1 )
                           {
                               this._warBase = false;
                           }
                        */
                    }

                    LogicJSONHelper.SetJSONNumber(jsonObject, "war_layout", this._warLayout);
                }

                LogicJSONArray layoutStateArray = new LogicJSONArray();

                for (int i = 0; i < this._layoutState.Count; i++)
                {
                    layoutStateArray.Add(new LogicJSONNumber(this._layoutState[i]));
                }

                jsonObject.Put("layout_state", layoutStateArray);

                LogicJSONArray layoutState2Array = new LogicJSONArray();

                for (int i = 0; i < this._layoutStateVillage2.Count; i++)
                {
                    layoutState2Array.Add(new LogicJSONNumber(this._layoutStateVillage2[i]));
                }

                jsonObject.Put("layout_state2", layoutState2Array);

                LogicJSONArray layoutCooldownArray = new LogicJSONArray();

                for (int i = 0; i < this._layoutCooldown.Count; i++)
                {
                    layoutCooldownArray.Add(new LogicJSONNumber(this._layoutCooldown[i]));
                }

                jsonObject.Put("layout_cooldown", layoutCooldownArray);
            }

            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].Save(jsonObject);
            }

            if (!this._npcVillage)
            {
                this._cooldownManager.Save(jsonObject);
                this.SaveShopNewItems(jsonObject);

                LogicJSONHelper.SetJSONNumber(jsonObject, "last_league_rank", this._lastLeagueRank);
                LogicJSONHelper.SetJSONNumber(jsonObject, "last_alliance_level", this._lastAllianceLevel);
                LogicJSONHelper.SetJSONNumber(jsonObject, "last_league_shuffle", this._lastLeagueShuffle);
                LogicJSONHelper.SetJSONNumber(jsonObject, "last_season_seen", this._lastSeasonSeen);
                LogicJSONHelper.SetJSONNumber(jsonObject, "last_news_seen", this._lastNewsSeen);

                if (this._troopRequestMessage.Length > 0)
                {
                    LogicJSONHelper.SetJSONString(jsonObject, "troop_req_msg", this._troopRequestMessage);
                }

                if (this._warRequestMessage.Length > 0)
                {
                    LogicJSONHelper.SetJSONString(jsonObject, "war_req_msg", this._warRequestMessage);
                }

                LogicJSONHelper.SetJSONNumber(jsonObject, "war_tutorials_seen", this._warTutorialsSeen);
                LogicJSONHelper.SetJSONBoolean(jsonObject, "war_base", this._warBase);

                LogicJSONArray armyNameArray = new LogicJSONArray();

                for (int i = 0; i < this._armyNames.Count; i++)
                {
                    armyNameArray.Add(new LogicJSONString(this._armyNames[i]));
                }

                jsonObject.Put("army_names", armyNameArray);

                if (this._unplacedObjects != null)
                {
                    if (this._unplacedObjects.Count > 0)
                    {
                        LogicJSONArray unplacedArray = new LogicJSONArray();

                        for (int i = 0; i < this._unplacedObjects.Count; i++)
                        {
                            LogicJSONObject obj = new LogicJSONObject();
                            this._unplacedObjects[i].WriteToJSON(obj);
                            unplacedArray.Add(obj);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Saves the shop new items.
        /// </summary>
        private void SaveShopNewItems(LogicJSONObject jsonObject)
        {
            LogicDataTable buildingTable = LogicDataTables.GetTable(0);
            LogicDataTable trapTable = LogicDataTables.GetTable(11);
            LogicDataTable decoTable = LogicDataTables.GetTable(17);

            int townHallLevel = this._homeOwnerAvatar.GetTownHallLevel();
            int expLevel = this._homeOwnerAvatar.GetExpLevel();

            LogicJSONArray newShopBuildingArray = new LogicJSONArray();

            for (int i = 0; i < this._newShopBuildings.Count; i++)
            {
                int currentNewItemCount = this._newShopBuildings[i];
                int unlockedShopItemCount = this.GetShopUnlockCount(buildingTable.GetItemAt(i), townHallLevel);

                newShopBuildingArray.Add(new LogicJSONNumber(unlockedShopItemCount - currentNewItemCount));
            }

            jsonObject.Put("newShopBuildings", newShopBuildingArray);

            LogicJSONArray newShopTrapArray = new LogicJSONArray();

            for (int i = 0; i < this._newShopTraps.Count; i++)
            {
                int currentNewItemCount = this._newShopTraps[i];
                int unlockedShopItemCount = this.GetShopUnlockCount(trapTable.GetItemAt(i), townHallLevel);

                newShopTrapArray.Add(new LogicJSONNumber(unlockedShopItemCount - currentNewItemCount));
            }

            jsonObject.Put("newShopTraps", newShopTrapArray);

            LogicJSONArray newShopDecoArray = new LogicJSONArray();

            for (int i = 0; i < this._newShopDecos.Count; i++)
            {
                int currentNewItemCount = this._newShopDecos[i];
                int unlockedShopItemCount = this.GetShopUnlockCount(decoTable.GetItemAt(i), expLevel);

                newShopDecoArray.Add(new LogicJSONNumber(unlockedShopItemCount - currentNewItemCount));
            }

            jsonObject.Put("newShopDecos", newShopDecoArray);
        }

        /// <summary>
        ///     Loads the shop new items.
        /// </summary>
        public void LoadShopNewItems()
        {
            // TODO: Implement LogicLevel::loadShopNewItems();
        }

        /// <summary>
        ///     Refreshes the new shop unlocks with exp.
        /// </summary>
        public void RefreshNewShopUnlocksExp()
        {
            int expLevel = this._homeOwnerAvatar.GetExpLevel();

            if (this._homeOwnerAvatar.GetExpLevel() > 0)
            {
                LogicDataTable table = LogicDataTables.GetTable(17);

                for (int i = 0; i < this._newShopDecos.Count; i++)
                {
                    LogicData data = table.GetItemAt(i);

                    int totalShopUnlock = this.GetShopUnlockCount(data, expLevel);
                    int shopUnlockCount = totalShopUnlock - this.GetShopUnlockCount(data, expLevel - 1);

                    if (shopUnlockCount > 0)
                    {
                        this._newShopDecos[i] += shopUnlockCount;
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the home owner avatar instance.
        /// </summary>
        public void SetHomeOwnerAvatar(LogicAvatar avatar)
        {
            this._homeOwnerAvatar = avatar;

            if (avatar != null)
            {
                avatar.SetLevel(this);

                if (avatar.IsClientAvatar())
                {
                    this._lastLeagueRank = ((LogicClientAvatar) avatar).GetLeagueType();
                }
            }
        }

        /// <summary>
        ///     Adds the unplaced object.
        /// </summary>
        public void AddUnplacedObject(LogicDataSlot obj)
        {
            if (this._unplacedObjects == null)
            {
                this._unplacedObjects = new LogicArrayList<LogicDataSlot>();
            }

            this._unplacedObjects.Add(obj);
        }
        
        /// <summary>
        ///     Gets the shop unlock count for specified data.
        /// </summary>
        /// <param name="arg">Can be the town hall level index or exp level.</param>
        public int GetShopUnlockCount(LogicData data, int arg)
        {
            int unlock = 0;

            switch (data.GetDataType())
            {
                case 0:
                {
                    LogicBuildingData buildingData = (LogicBuildingData) data;

                    if (!buildingData.IsLocked())
                    {
                        unlock = LogicDataTables.GetTownHallLevel(arg).GetUnlockedBuildingCount(buildingData);
                    }

                    break;
                }

                case 11:
                {
                    unlock = LogicDataTables.GetTownHallLevel(arg).GetUnlockedTrapCount((LogicTrapData) data);
                    break;
                }

                case 17:
                {
                    LogicDecoData decoData = (LogicDecoData) data;

                    if (decoData.GetRequiredExpLevel() <= arg)
                    {
                        unlock = decoData.GetMaxCount();
                    }

                    break;
                }
            }

            return unlock;
        }

        /// <summary>
        ///     Called when the battle has started.
        /// </summary>
        public void BattleStarted()
        {
            this._battleStarted = true;

            if (this._matchType == 4 && !LogicDataTables.GetGlobals().RemoveRevengeWhenBattleIsLoaded())
            {
            }

            if (this.GetState() != 5)
            {
                if (this._matchType <= 8)
                {
                    if (this._matchType == 1 ||
                        this._matchType == 3 ||
                        this._matchType == 4 ||
                        this._matchType == 7)
                    {
                    }
                    else if (this._matchType == 5 ||
                             this._matchType == 8)
                    {
                    }
                }
            }

            // TODO: Implement LogicLevel::battleStarted();
        }

        /// <summary>
        ///     Gets a value indicating whether the level is in combat state.
        /// </summary>
        public bool IsInCombatState()
        {
            int state = this.GetState();
            return state == 2 || state == 3 || state == 5;
        }

        /// <summary>
        ///     Called when the loading is finished.
        /// </summary>
        public void LoadingFinished()
        {
            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].GetComponentManager().DevideAvatarResourcesToStorages();
            }

            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].GetComponentManager().CalculateLoot(true);
            }

            if (this._battleLog != null)
            {
                // TODO: LogicBattleLog::calculateAvailableResources(this, this._matchType);
                // TODO: LogicLevel::setOwnerInformationToBattleLog();
            }

            if (this._gameMode.GetState() == 2)
            {
                if (this._matchType == 1)
                {
                    Debugger.Log("matchmaking");
                }
                else if (this._matchType == 8)
                {
                    Debugger.Log("matchmakingv2");
                }
            }

            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].LoadingFinished();
            }

            // TODO: LogicMissionManager::refreshOpenMissions();
            this.LoadShopNewItems();

            if (this._levelJSON != null)
            {
                this._lastLeagueRank = LogicJSONHelper.GetJSONNumber(this._levelJSON, "last_league_rank");
                this._lastAllianceLevel = LogicJSONHelper.GetJSONNumber(this._levelJSON, "last_alliance_level");
                this._lastLeagueShuffle = LogicJSONHelper.GetJSONNumber(this._levelJSON, "last_league_shuffle");
                this._lastSeasonSeen = LogicJSONHelper.GetJSONNumber(this._levelJSON, "last_season_seen");
                this._lastNewsSeen = LogicJSONHelper.GetJSONNumber(this._levelJSON, "last_news_seen");
                this._editModeShown = LogicJSONHelper.GetJSONBoolean(this._levelJSON, "edit_mode_shown");
                this._troopRequestMessage = LogicJSONHelper.GetJSONString(this._levelJSON, "troop_req_msg");
                this._warRequestMessage = LogicJSONHelper.GetJSONString(this._levelJSON, "war_req_msg");
                this._warTutorialsSeen = LogicJSONHelper.GetJSONNumber(this._levelJSON, "war_tutorials_seen");

                LogicJSONArray armyNameArray = this._levelJSON.GetJSONArray("army_names");

                if (armyNameArray != null)
                {
                    int size = LogicMath.Min(armyNameArray.Size(), this._armyNames.Count);

                    for (int i = 0; i < size; i++)
                    {
                        this._armyNames[i] = armyNameArray.GetJSONString(i).GetStringValue();
                    }
                }

                this._helpOpened = LogicJSONHelper.GetJSONBoolean(this._levelJSON, "help_opened");
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardTime(int totalSecs)
        {
            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].FastForwardTime(totalSecs);
            }
        }

        /// <summary>
        ///     Sub ticks this instance.
        /// </summary>
        public void SubTick()
        {
            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].SubTick();
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            int state = this.GetState();

            if (state == 2 && !this._battleStarted && this._battleLog.GetBattleStarted())
            {
                this.BattleStarted();
            }

            if (state == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    this._gameObjectManagers[i].Tick();
                }
            }
            else
            {
                this._gameObjectManagers[this._villageType].Tick();
            }

            // TODO: LogicMissionManager::tick();
            this._achievementManager.Tick();
            this._offerManager.Tick();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._tileMap != null)
            {
                this._tileMap.Destruct();
                this._tileMap = null;
            }

            if (this._map != null)
            {
                this._map.Destruct();
                this._map = null;
            }

            for (int i = 0; i < 2; i++)
            {
                if (this._gameObjectManagers[i] != null)
                {
                    this._gameObjectManagers[i].Destruct();
                    this._gameObjectManagers[i] = null;
                }

                if (this._workerManagers[i] != null)
                {
                    this._workerManagers[i].Destruct();
                    this._workerManagers[i] = null;
                }
            }

            if (this._offerManager != null)
            {
                this._offerManager.Destruct();
                this._offerManager = null;
            }

            if (this._achievementManager != null)
            {
                this._achievementManager.Destruct();
                this._achievementManager = null;
            }

            if (this._cooldownManager != null)
            {
                this._cooldownManager.Destruct();
                this._cooldownManager = null;
            }

            if (this._battleLog != null)
            {
                this._battleLog.Destruct();
                this._battleLog = null;
            }

            if (this._gameListener != null)
            {
                this._gameListener.Destruct();
                this._gameListener = null;
            }

            if (this._newShopBuildings != null)
            {
                if (this._newShopBuildings.Count != 0)
                {
                    do
                    {
                        this._newShopBuildings.Remove(0);
                    } while (this._newShopBuildings.Count != 0);
                }

                this._newShopBuildings = null;
            }

            if (this._newShopTraps != null)
            {
                if (this._newShopTraps.Count != 0)
                {
                    do
                    {
                        this._newShopTraps.Remove(0);
                    } while (this._newShopTraps.Count != 0);
                }

                this._newShopTraps = null;
            }

            if (this._newShopDecos != null)
            {
                if (this._newShopDecos.Count != 0)
                {
                    do
                    {
                        this._newShopDecos.Remove(0);
                    } while (this._newShopDecos.Count != 0);
                }

                this._newShopDecos = null;
            }

            if (this._layoutState != null)
            {
                if (this._layoutState.Count != 0)
                {
                    do
                    {
                        this._layoutState.Remove(0);
                    } while (this._layoutState.Count != 0);
                }
            }

            if (this._layoutStateVillage2 != null)
            {
                if (this._layoutStateVillage2.Count != 0)
                {
                    do
                    {
                        this._layoutStateVillage2.Remove(0);
                    } while (this._layoutStateVillage2.Count != 0);
                }
            }

            if (this._layoutCooldown != null)
            {
                if (this._layoutCooldown.Count != 0)
                {
                    do
                    {
                        this._layoutCooldown.Remove(0);
                    } while (this._layoutCooldown.Count != 0);
                }
            }

            if (this._armyNames != null)
            {
                if (this._armyNames.Count != 0)
                {
                    do
                    {
                        this._armyNames.Remove(0);
                    } while (this._armyNames.Count != 0);
                }
            }

            if (this._unplacedObjects != null)
            {
                if (this._unplacedObjects.Count != 0)
                {
                    do
                    {
                        this._unplacedObjects[0].Destruct();
                        this._unplacedObjects.Remove(0);
                    } while (this._unplacedObjects.Count != 0);
                }
            }

            this._gameMode = null;
            this._clientHome = null;
            this._homeOwnerAvatar = null;
            this._visitorAvatar = null;
        }
    }
}