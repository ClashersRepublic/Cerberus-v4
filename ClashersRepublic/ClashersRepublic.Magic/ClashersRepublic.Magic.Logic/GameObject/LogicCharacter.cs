namespace ClashersRepublic.Magic.Logic.GameObject
{
    using System;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Level;

    public sealed class LogicCharacter : LogicGameObject
    {
        private int _upgradeLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuilding" /> class.
        /// </summary>
        public LogicCharacter(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            LogicCharacterData characterData = (LogicCharacterData) data;

            this.AddComponent(new LogicHitpointComponent(this, characterData.GetHitpoints(0), 0));
            this.AddComponent(new LogicCombatComponent(this));
            this.AddComponent(new LogicMovementComponent(this, characterData.GetSpeed(), characterData.IsHealerTrigger(), characterData.IsUnderground()));
            this.SetUpgradeLevel(0);
        }

        /// <summary>
        ///     Gets the <see cref="LogicBuildingData" /> instance.
        /// </summary>
        public LogicCharacterData GetCharacterData()
        {
            return (LogicCharacterData) this._data;
        }

        /// <summary>
        ///     Gets the gameobject type.
        /// </summary>
        public override int GetGameObjectType()
        {
            return 1;
        }

        /// <summary>
        ///     Sets the initial position.
        /// </summary>
        public override void SetInitialPosition(int x, int y)
        {
            base.SetInitialPosition(x, y);

            LogicMovementComponent movementComponent = this.GetMovementComponent();

            if (movementComponent != null)
            {
                movementComponent.GetMovementSystem().Reset(x, y);
            }
        }

        /// <summary>
        ///     Gets the upgrade level.
        /// </summary>
        public int GetUpgradeLevel()
        {
            return this._upgradeLevel;
        }

        /// <summary>
        ///     Sets the upgrade level.
        /// </summary>
        public void SetUpgradeLevel(int upgLevel)
        {
            this._upgradeLevel = upgLevel;
        }
    }
}