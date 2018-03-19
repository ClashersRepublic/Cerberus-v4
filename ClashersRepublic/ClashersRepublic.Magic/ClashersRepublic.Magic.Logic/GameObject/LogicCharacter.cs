namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;

    public sealed class LogicCharacter : LogicGameObject
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuilding" /> class.
        /// </summary>
        public LogicCharacter(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicCharacter.
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
    }
}