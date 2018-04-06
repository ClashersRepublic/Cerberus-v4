namespace RivieraStudio.Magic.Logic.GameObject
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject.Component;
    using RivieraStudio.Magic.Logic.Level;

    public sealed class LogicSpell : LogicGameObject
    {
        private int _upgradeLevel;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSpell" /> class.
        /// </summary>
        public LogicSpell(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicSpell.
        }

        /// <summary>
        ///     Gets the <see cref="LogicSpellData" /> instance.
        /// </summary>
        public LogicSpellData GetSpellData()
        {
            return (LogicSpellData) this._data;
        }

        /// <summary>
        ///     Gets the gameobject type.
        /// </summary>
        public override int GetGameObjectType()
        {
            return 7;
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