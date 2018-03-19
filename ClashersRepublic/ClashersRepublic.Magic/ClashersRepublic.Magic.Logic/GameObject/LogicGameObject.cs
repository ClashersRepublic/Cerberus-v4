namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.GameObject.Listener;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicGameObject
    {
        protected LogicData _data;
        protected LogicLevel _level;
        protected LogicVector2 _position;
        protected LogicGameObjectListener _listener;
        protected LogicArrayList<LogicComponent> _components;

        protected int _villageType;
        protected int _globalId;
        protected int _seed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObject" /> class.
        /// </summary>
        public LogicGameObject(LogicData data, LogicLevel level, int villageType)
        {
            Debugger.DoAssert(villageType < 2, "VillageType not set! Game object has not been added to LogicGameObjectManager.");

            this._data = data;
            this._level = level;
            this._villageType = villageType;
            this._globalId = -1;

            this._position = new LogicVector2();
            this._listener = new LogicGameObjectListener();
            this._components = new LogicArrayList<LogicComponent>(21);

            for (int i = 0; i < 17; i++)
            {
                this._components.Add(null);
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            // LogicTileMap::removeGameObject(this);

            for (int i = 0; i < this._components.Count; i++)
            {
                if (this._components[i] != null)
                {
                    this._components[i].Destruct();
                    this._components[i].RemoveGameObjectReferences(this);
                }
            }

            if (this._position != null)
            {
                this._position.Destruct();
                this._position = null;
            }

            if (this._listener != null)
            {
                this._listener.Destruct();
                this._listener = null;
            }

            this._data = null;
            this._level = null;
        }

        /// <summary>
        ///     Adds the specified component.
        /// </summary>
        public void AddComponent(LogicComponent component)
        {
            int componentType = component.GetComponentType();

            if (this._components[componentType] == null)
            {
                this._level.GetComponentManager(this._villageType).AddComponent(component);
                this._components[componentType] = component;
            }
            else
            {
                Debugger.Error("LogicGameObject::addComponent - Component is already added.");
            }
        }

        /// <summary>
        ///     Enable the specified component.
        /// </summary>
        public void EnableComponent(int componentType, bool enable)
        {
            LogicComponent component = this._components[componentType];

            if (component != null)
            {
                component.SetEnabled(enable);
            }
        }

        /// <summary>
        ///     Gets the x position.
        /// </summary>
        public int GetX()
        {
            return this._position.X;
        }

        /// <summary>
        ///     Gets the y position.
        /// </summary>
        public int GetY()
        {
            return this._position.Y;
        }

        /// <summary>
        ///     Gets the tile x position.
        /// </summary>
        public int GetTileX()
        {
            return this._position.X >> 9;
        }

        /// <summary>
        ///     Gets the tile y position.
        /// </summary>
        public int GetTileY()
        {
            return this._position.Y >> 9;
        }

        /// <summary>
        ///     Gets the village type of this gameobject.
        /// </summary>
        public int GetVillageType()
        {
            return this._villageType;
        }

        /// <summary>
        ///     Gets the global id of this gameobject.
        /// </summary>
        public int GetGlobalID()
        {
            return this._globalId;
        }

        /// <summary>
        ///     Gets the <see cref="LogicData"/> instance.
        /// </summary>
        public LogicData GetData()
        {
            return this._data;
        }

        /// <summary>
        ///     Gets the <see cref="LogicLevel" /> instance.
        /// </summary>
        public LogicLevel GetLevel()
        {
            return this._level;
        }

        /// <summary>
        ///     Gets the <see cref="LogicGameObject"/> lister.
        /// </summary>
        public LogicGameObjectListener GetListener()
        {
            return this._listener;
        }

        /// <summary>
        ///     Gets the specified component.
        /// </summary>
        public LogicComponent GetComponent(int componentType)
        {
            LogicComponent component = this._components[componentType];

            if (component != null && component.IsEnabled())
            {
                return component;
            }

            return null;
        }

        /// <summary>
        ///     Gets the hitpoint component.
        /// </summary>
        public LogicCombatComponent GetCombatComponent()
        {
            return (LogicCombatComponent) this._components[1];
        }

        /// <summary>
        ///     Gets the hitpoint component.
        /// </summary>
        public LogicHitpointComponent GetHitpointComponent()
        {
            return (LogicHitpointComponent) this._components[2];
        }

        /// <summary>
        ///     Removes the specified component.
        /// </summary>
        public void RemoveComponent(int componentType)
        {
            if (this._components[componentType] != null)
            {
                this._components[componentType].RemoveGameObjectReferences(this);
                this._components[componentType] = null;
            }
        }

        /// <summary>
        ///     Refreshes passable of tilemap.
        /// </summary>
        public void RefreshPassable()
        {
            this._level.GetTileMap().RefreshPassable(this);
        }

        /// <summary>
        ///     Sets the initial position.
        /// </summary>
        public void SetInitialPosition(int x, int y)
        {
            this._position.Set(x, y);
        }

        /// <summary>
        ///     Sets the gameobject position.
        /// </summary>
        public void SetPosition(LogicVector2 vector2)
        {
            this._position.Set(vector2.X, vector2.Y);
        }

        /// <summary>
        ///     Sets the gameobject position.
        /// </summary>
        public void SetPositionXY(int x, int y)
        {
            this._position.Set(x, y);
        }

        /// <summary>
        ///     Sets the global id.
        /// </summary>
        public void SetGlobalID(int globalId)
        {
            this._globalId = globalId;
        }

        /// <summary>
        ///     Sets the gameobject listener.
        /// </summary>
        public void SetListener(LogicGameObjectListener listener)
        {
            this._listener = listener;
        }

        /// <summary>
        ///     Gets the passable subtiles at edge.
        /// </summary>
        public virtual int PassableSubtilesAtEdge()
        {
            return 1;
        }

        /// <summary>
        ///     Gets the gameobject type.
        /// </summary>
        public virtual int GetGameObjectType()
        {
            return 0;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicGameObject"/> instance is a static object.
        /// </summary>
        public virtual bool IsStaticObject()
        {
            return true;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicGameObject"/> instance is alive.
        /// </summary>
        public virtual bool IsAlive()
        {
            LogicHitpointComponent hitpointComponent = this.GetHitpointComponent();

            if (hitpointComponent != null)
            {
                return hitpointComponent.InternalGetHp() > 0;
            }

            return true;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicGameObject"/> instance is passable.
        /// </summary>
        public virtual bool IsPassable()
        {
            return true;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicGameObject"/> instance is unbuildable.
        /// </summary>
        public virtual bool IsUnbuildable()
        {
            return true;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicGameObject"/> instance is wall.
        /// </summary>
        public virtual bool IsWall()
        {
            return false;
        }

        /// <summary>
        ///     Gets the path finder cost.
        /// </summary>
        public virtual int GetPathFinderCost()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the height of gameobject in tiles.
        /// </summary>
        public virtual int GetHeightInTiles()
        {
            return 1;
        }

        /// <summary>
        ///     Gets the width of gameobject in tiles.
        /// </summary>
        public virtual int GetWidthInTiles()
        {
            return 1;
        }

        /// <summary>
        ///     Gets the remaining boost time.
        /// </summary>
        public virtual int GetRemainingBoostTime()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the max fast forward time.
        /// </summary>
        public virtual int GetMaxFastForwardTime()
        {
            return -1;
        }

        /// <summary>
        ///     Gets if this gameobject should be destroy.
        /// </summary>
        public virtual bool ShouldDestruct()
        {
            return false;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public virtual void FastForwardTime(int time)
        {
            // FastForwardTime.
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public virtual void GetChecksum(ChecksumHelper checksum)
        {
            checksum.StartObject("LogicGameObject");

            checksum.WriteValue("type", this.GetGameObjectType());
            checksum.WriteValue("globalID", this._globalId);
            checksum.WriteValue("dataGlobalID", this._data.GetGlobalID());
            checksum.WriteValue("x", this.GetX());
            checksum.WriteValue("y", this.GetY());
            checksum.WriteValue("seed", this._seed);

            if (this.GetHitpointComponent() != null)
            {
                LogicHitpointComponent hitpointComponent = this.GetHitpointComponent();

                checksum.WriteValue("m_hp", hitpointComponent.InternalGetHp());
                checksum.WriteValue("m_maxHP", hitpointComponent.InternalGetMaxHp());
            }

            checksum.EndObject();
        }

        /// <summary>
        ///     Ticks for update this instance. Called before Tick method.
        /// </summary>
        public virtual void SubTick()
        {
            // SubTick.
        }

        /// <summary>
        ///     Ticks for update this instance.
        /// </summary>
        public virtual void Tick()
        {
            // Tick.
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public virtual void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber xNumber = jsonObject.GetJSONNumber("x");
            LogicJSONNumber yNumber = jsonObject.GetJSONNumber("y");

            for (int i = 0; i < this._components.Count; i++)
            {
                LogicComponent component = this._components[i];

                if (component != null)
                {
                    component.Load(jsonObject);
                }
            }

            if (xNumber == null || yNumber == null)
            {
                Debugger.Error("LogicGameObject::load - x or y is NULL!");
            }

            this.SetPositionXY(xNumber.GetIntValue() << 9, yNumber.GetIntValue() << 9);
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public virtual void Save(LogicJSONObject jsonObject)
        {
            jsonObject.Put("x", new LogicJSONNumber(this.GetTileX()));
            jsonObject.Put("y", new LogicJSONNumber(this.GetTileY()));

            for (int i = 0; i < this._components.Count; i++)
            {
                LogicComponent component = this._components[i];

                if (component != null)
                {
                    component.Save(jsonObject);
                }
            }
        }

        /// <summary>
        ///     Called when the loading of this <see cref="LogicGameObject"/> instance is finished.
        /// </summary>
        public virtual void LoadingFinished()
        {
            // LoadingFinished.
        }
    }
}