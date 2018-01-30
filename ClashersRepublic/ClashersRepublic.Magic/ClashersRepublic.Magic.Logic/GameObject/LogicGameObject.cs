namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicGameObject
    {
        protected int _seed;
        protected int _globalId;
        protected int _villageType;

        protected LogicLevel _level;
        protected LogicVector2 _position;
        protected LogicArrayList<LogicComponent> _components;

        private LogicData _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObject"/> class.
        /// </summary>
        public LogicGameObject(LogicData data, LogicLevel level)
        {
            this._data = data;
            this._level = level;
            this._globalId = -1;

            this._position = new LogicVector2();
            this._components = new LogicArrayList<LogicComponent>();

            for (int i = 0; i < 22; i++)
            {
                this._components.Add(null);
            }
        }

        /// <summary>
        ///     Adds the specified component to gameobject.
        /// </summary>
        public void AddComponent(LogicComponent component)
        {
            if (this._components[component.GetComponentType()] != null)
            {
                Debugger.Error("LogicGameObject::addComponent - Component is already added.");
                return;
            }

            this._level.GetGameObjectManagerAt(this._villageType).GetComponentManager().AddComponent(component);
            this._components[component.GetComponentType()] = component;
        }
        
        /// <summary>
        ///     Gets the hitpoint component instance.
        /// </summary>
        public LogicHitpointComponent GetHitpointComponent()
        {
            return (LogicHitpointComponent) this._components[2];
        }

        /// <summary>
        ///     Gets the combat component instance.
        /// </summary>
        public LogicComponent GetCombatComponent()
        {
            return this._components[1];
        }

        /// <summary>
        ///     Gets the data instance.
        /// </summary>
        public LogicData GetData()
        {
            return this._data;
        }

        /// <summary>
        ///     Gets the global id of this instance.
        /// </summary>
        public int GetGlobalID()
        {
            return this._globalId;
        }

        /// <summary>
        ///     Sets the global id of this instance.
        /// </summary>
        public void SetGlobalID(int id)
        {
            this._globalId = id;
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
        ///     Gets the y position.
        /// </summary>
        public int GetTileY()
        {
            return this._position.Y >> 9;
        }

        /// <summary>
        ///     Gets the width size in tiles.
        /// </summary>
        public virtual int GetWidthInTiles()
        {
            return 1;
        }

        /// <summary>
        ///     Gets the heigh size in tiles.
        /// </summary>
        public virtual int GetHeighInTiles()
        {
            return 1;
        }

        /// <summary>
        ///     Gets a value indicating whether the gameobject can be sell.
        /// </summary>
        public virtual bool CanSell()
        {
            return false;
        }

        /// <summary>
        ///     Sets the position x,y.
        /// </summary>
        public void SetPositionXY(int x, int y)
        {
            this._position.Set(x, y);
        }
        
        /// <summary>
        ///     Gets the component type of this instance.
        /// </summary>
        public virtual int GetGameObjectType()
        {
            return 0;
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
            checksum.WriteValue("x", this._position.X);
            checksum.WriteValue("y", this._position.Y);
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
            LogicJSONNumber xObject = jsonObject.GetJSONNumber("x");
            LogicJSONNumber yObject = jsonObject.GetJSONNumber("y");

            if (xObject != null && yObject != null)
            {
                this.SetPositionXY(xObject.GetIntValue() << 9, yObject.GetIntValue() << 9);
            }
            else
            {
                Debugger.Error("LogicGameObject::load - x or y is NULL!");
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public virtual void Save(LogicJSONObject jsonObject)
        {
            jsonObject.Put("x", new LogicJSONNumber(this._position.X));
            jsonObject.Put("y", new LogicJSONNumber(this._position.Y));
        }

        /// <summary>
        ///     Gets the level instance.
        /// </summary>
        internal LogicLevel GetLevel()
        {
            return this._level;
        }
    }
}