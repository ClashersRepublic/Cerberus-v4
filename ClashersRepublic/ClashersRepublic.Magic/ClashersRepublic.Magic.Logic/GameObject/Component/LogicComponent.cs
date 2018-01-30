namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicComponent
    {
        protected bool _enabled;
        protected LogicGameObject _parent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicComponent"/> class.
        /// </summary>
        public LogicComponent(LogicGameObject gameObject)
        {
            this._parent = gameObject;
        }

        /// <summary>
        ///     Gets the parent of this component.
        /// </summary>
        public LogicGameObject GetParent()
        {
            return this._parent;
        }

        /// <summary>
        ///     Gets a value indicating whether this component is enabled.
        /// </summary>
        public bool IsEnabled()
        {
            return this._enabled;
        }

        /// <summary>
        ///     Sets the enabled value for disable of enable the component.
        /// </summary>
        public void SetEnabled(bool value)
        {
            this._enabled = value;
        }

        /// <summary>
        ///     Gets the component type of this instance.
        /// </summary>
        public virtual int GetComponentType()
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
            // GetChecksum.
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
            // Load.
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public virtual void Save(LogicJSONObject jsonObject)
        {
            // Save.
        }
    }
}