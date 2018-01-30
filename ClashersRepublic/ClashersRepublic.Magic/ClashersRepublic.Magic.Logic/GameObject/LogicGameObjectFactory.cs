namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Debug;

    public static class LogicGameObjectFactory
    {
        /// <summary>
        ///     Creates a gameobject with specified data.
        /// </summary>
        public static LogicGameObject CreateGameObject(LogicData data, LogicLevel level)
        {
            LogicGameObject gameObject = null;

            switch (data.GetDataType())
            {
                case 0:
                {
                    gameObject = new LogicBuilding(data, level);
                    break;
                }

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