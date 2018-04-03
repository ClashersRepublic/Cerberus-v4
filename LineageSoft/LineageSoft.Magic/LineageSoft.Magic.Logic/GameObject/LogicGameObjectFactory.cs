namespace LineageSoft.Magic.Logic.GameObject
{
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Titan.Debug;

    public static class LogicGameObjectFactory
    {
        /// <summary>
        ///     Creates a gameobject with specified data.
        /// </summary>
        public static LogicGameObject CreateGameObject(LogicData data, LogicLevel level, int villageType)
        {
            LogicGameObject gameObject = null;

            switch (data.GetDataType())
            {
                case 0:
                    gameObject = new LogicBuilding(data, level, villageType);
                    break;
                case 3:
                    gameObject = new LogicCharacter(data, level, villageType);
                    break;
                case 7:
                    gameObject = new LogicObstacle(data, level, villageType);
                    break;
                case 38:
                    gameObject = new LogicVillageObject(data, level, villageType);
                    break;

                default:
                {
                    Debugger.Warning("Trying to create game object with data that does not inherit LogicGameObjectData. GlobalId=" + data.GetGlobalID());
                    break;
                }
            }

            return gameObject;
        }
    }
}