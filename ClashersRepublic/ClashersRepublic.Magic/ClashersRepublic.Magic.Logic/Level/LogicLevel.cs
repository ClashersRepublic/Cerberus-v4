namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.Achievement;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Logic.Offer;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Worker;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;
    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Battle;
    using ClashersRepublic.Magic.Logic.Cooldown;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;

    public class LogicLevel
    {
        private LogicGameMode _gameMode;
        private LogicClientHome _clientHome;
        private LogicAvatar _homeOwnerAvatar;
        private LogicAvatar _visitorAvatar;
        private LogicTileMap _tileMap;
        private LogicRect _map;

        private LogicGameObjectManager[] _gameObjectManagers;
        private LogicWorkerManager[] _workerManagers;
        private LogicOfferManager _offerManager;
        private LogicAchievementManager _achievementManager;
        private LogicCooldownManager _cooldownManager;
        private LogicBattleLog _battleLog;
        private LogicGameListener _gameListener;

        private LogicArrayList<int> _layoutState;
        private LogicArrayList<int> _layoutCooldown;
        private LogicArrayList<int> _layoutStateVillage2;
        private LogicArrayList<string> _armyNames;
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
        private int _matchType;

        private bool _warBase;
        private bool _npcVillage;
        private bool _androidClient;
        private bool _battleStarted;
        private bool _battleEndPending;
        private bool _warTutorialsSeen;
        private bool _isWarLevel;
        private bool _isDirectLevel;
        private bool _isDirectVillage2Level;

        private string _warRequestMessage;
        private string _troopRequestMessage;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLevel"/> class.
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

            for (int i = 0; i < 2; i++)
            {
                this._workerManagers[i] = new LogicWorkerManager(this);
                this._gameObjectManagers[i] = new LogicGameObjectManager(this._tileMap, this, i);
            }

            this._offerManager = new LogicOfferManager();
            this._tileMap = new LogicTileMap(50, 50);
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

            LogicGlobals globalsInstance = LogicDataTables.GetGlobalsInstance();

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
        ///     Sets the game listener.
        /// </summary>
        public void SetGameListener(LogicGameListener listener)
        {
            this._gameListener = listener;
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
        public void SetHome(LogicClientHome home)
        {
            this._clientHome = home;
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
                    if (LogicDataTables.GetGlobalsInstance().RevertBrokenWarLayouts())
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

            LogicJSONArray newShopBuildingArray = new LogicJSONArray();

            for (int i = 0; i < 0; i++)
            {
                LogicData data = buildingTable.GetItemAt(i);

                int currentNewItemCount = this._newShopBuildings[i];
                int unlockedShopItemCount = this.GetUnlockedShopItemCount(i, 0);

                newShopBuildingArray.Add(new LogicJSONNumber(currentNewItemCount - unlockedShopItemCount));
            }

            jsonObject.Put("newShopBuildings", newShopBuildingArray);

            LogicJSONArray newShopTrapArray = new LogicJSONArray();

            for (int i = 0; i < 0; i++)
            {
                // TODO: Implement newShopBuildings.
            }

            jsonObject.Put("newShopTraps", newShopTrapArray);

            LogicJSONArray newShopDecoArray = new LogicJSONArray();

            for (int i = 0; i < 0; i++)
            {
                // TODO: Implement newShopBuildings.
            }

            jsonObject.Put("newShopDecos", newShopDecoArray);
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
        ///     Gets the number of unlocked shop item.
        /// </summary>
        public int GetUnlockedShopItemCount(int dataIndex, int dataType)
        {
            int count = 0;

            switch (dataType)
            {
                    
            }

            return count;
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

            if (this._matchType == 4 && !LogicDataTables.GetGlobalsInstance().RemoveRevengeWhenBattleIsLoaded())
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

            if ((state == 2 && !this._battleStarted) && this._battleLog.GetBattleStarted())
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

            // LogicMissionManager::tick();
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
                this._tileMap = null;
            }
        }
    }
}