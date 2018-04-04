namespace RivieraStudio.Magic.Logic.GameObject.Component
{
    using RivieraStudio.Magic.Logic.Time;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicUnitProductionComponent : LogicComponent
    {
        protected LogicTimer _timer;
        protected LogicTimer _boostTimer;
        protected LogicArrayList<LogicUnitProductionSlot> _slots;

        private int _productionType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitProductionComponent" /> class.
        /// </summary>
        public LogicUnitProductionComponent(LogicGameObject gameObject) : base(gameObject)
        {
            this.SetProductionType(gameObject);
        }

        /// <summary>
        ///     Sets the production type.
        /// </summary>
        public void SetProductionType(LogicGameObject gameObject)
        {
            this._productionType = 0;

            if (gameObject.GetGameObjectType() == 0)
            {
                this._productionType = ((LogicBuilding)gameObject).GetBuildingData().ForgesSpells ? 1 : 0;
            }
        }

        /// <summary>
        ///     Gets the production type.
        /// </summary>
        public int GetProductionType()
        {
            return this._productionType;
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 3;
        }
    }
}