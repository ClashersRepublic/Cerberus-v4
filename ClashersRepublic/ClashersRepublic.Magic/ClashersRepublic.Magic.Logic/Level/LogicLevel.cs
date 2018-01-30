namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.Achievement;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Logic.Offer;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Worker;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;
    using Supercell.Magic.Logic;

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

        private LogicGameListener _gameListener;

        private LogicArrayList<int> _layoutState;
        private LogicArrayList<int> _layoutCooldown;
        private LogicArrayList<int> _layoutStateVillage2;
        private LogicArrayList<string> _armyNames;
        private LogicArrayList<LogicDataSlot> _unplacedObjects;


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

        private bool _warBase;
        private bool _androidClient;
        private bool _battleEndPending;
        private bool _warTutorialsSeen;
        private bool _isWarLevel;
        private bool _isDirectLevel;
        private bool _isDirectVillage2Level;

        private string _warRequestMessage;
        private string _troopRequestMessage;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCommandManager"/> class.
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
            this._offerManager = new LogicOfferManager();
            this._tileMap = new LogicTileMap(50, 50);
            this._map = new LogicRect(3, 3, 47, 47);
            this._layoutState = new LogicArrayList<int>(8);
            this._layoutCooldown = new LogicArrayList<int>(8);
            this._layoutStateVillage2 = new LogicArrayList<int>(8);
            this._unplacedObjects = new LogicArrayList<LogicDataSlot>();

            for (int i = 0; i < 2; i++)
            {
                this._workerManagers[i] = new LogicWorkerManager(this);
                this._gameObjectManagers[i] = new LogicGameObjectManager(this._tileMap, this, i);
            }

            for (int i = 0; i < 8; i++)
            {
                this._layoutState.Add(0);
            }

            for (int i = 0; i < 8; i++)
            {
                this._layoutStateVillage2.Add(0);
            }

            for (int i = 0; i < 8; i++)
            {
                this._layoutCooldown.Add(0);
            }

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
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(home.GetHomeJSON());

            this._androidClient = LogicJSONHelper.GetJSONBoolean(jsonObject, "android_client");
            this._warBase = LogicJSONHelper.GetJSONBoolean(jsonObject, "war_base");
            this._activeLayout = LogicJSONHelper.GetJSONNumber(jsonObject, "active_layout");
            this._activeLayoutVillage2 = LogicJSONHelper.GetJSONNumber(jsonObject, "act_l2");
            this._warLayout = LogicJSONHelper.GetJSONNumber(jsonObject, "war_layout");

            LogicJSONArray layoutStateArray = jsonObject.GetJSONArray("layout_state");
            LogicJSONArray layoutState2Array = jsonObject.GetJSONArray("layout_state2");
            LogicJSONArray layoutCooldownArray = jsonObject.GetJSONArray("layout_cooldown");
            LogicJSONArray unplacedArray = jsonObject.GetJSONArray("unplaced");

            if (layoutStateArray != null)
            {
                for (int i = 0; i < layoutStateArray.Size(); i++)
                {
                    if (i >= this._layoutState.Count)
                    {
                        break;
                    }

                    if (layoutStateArray[i] != null)
                    {
                        this._layoutState[i] = layoutStateArray.GetJSONNumber(i).GetIntValue();
                    }
                }
            }

            if (layoutState2Array != null)
            {
                for (int i = 0; i < layoutState2Array.Size(); i++)
                {
                    if (i >= this._layoutStateVillage2.Count)
                    {
                        break;
                    }

                    if (layoutState2Array[i] != null)
                    {
                        this._layoutState[i] = layoutState2Array.GetJSONNumber(i).GetIntValue();
                    }
                }
            }

            if (layoutCooldownArray != null)
            {
                for (int i = 0; i < layoutCooldownArray.Size(); i++)
                {
                    if (i >= this._layoutCooldown.Count)
                    {
                        break;
                    }

                    if (layoutCooldownArray[i] != null)
                    {
                        this._layoutState[i] = layoutCooldownArray.GetJSONNumber(i).GetIntValue();
                    }
                }
            }

            if (unplacedArray != null)
            {
                this._unplacedObjects.EnsureCapacity(unplacedArray.Size());

                for (int i = 0; i < unplacedArray.Size(); i++)
                {
                    LogicDataSlot slot = new LogicDataSlot(null, 0);
                    slot.ReadFromJSON(unplacedArray.GetJSONObject(i));
                    this._unplacedObjects.Add(slot);
                }
            }

            // TODO: LogicEventManager::load();

            this._waveNumber = LogicJSONHelper.GetJSONNumber(jsonObject, "wave_num");
            this._isWarLevel = LogicJSONHelper.GetJSONBoolean(jsonObject, "war");
            this._isDirectLevel = LogicJSONHelper.GetJSONBoolean(jsonObject, "direct");
            this._isDirectVillage2Level = LogicJSONHelper.GetJSONBoolean(jsonObject, "direct2");
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
            for (int i = 0; i < 2; i++)
            {
                this._gameObjectManagers[i].Tick();
            }
        }
    }
}