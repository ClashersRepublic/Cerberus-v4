namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using RivieraStudio.Magic.Titan.Json;

    public sealed class LogicCombatComponent : LogicComponent
    {
        private int _maxAmmo;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCombatComponent" /> class.
        /// </summary>
        public LogicCombatComponent(LogicGameObject gameObject) : base(gameObject)
        {
        }

        /// <summary>
        ///     Gets the max of ammo.
        /// </summary>
        public int GetMaxAmmo()
        {
            return this._maxAmmo;
        }
        
        /// <summary>
        ///     Ticks for update this component. Called before Tick method.
        /// </summary>
        public override void SubTick()
        {
            base.SubTick();
        }

        /// <summary>
        ///     Ticks for update this component.
        /// </summary>
        public override void Tick()
        {
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 1;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
        }
    }
}