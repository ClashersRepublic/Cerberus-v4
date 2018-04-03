namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.GameObject;
    using LineageSoft.Magic.Logic.Level;

    using LineageSoft.Magic.Titan.DataStream;
    using LineageSoft.Magic.Titan.Util;

    public sealed class LogicClearObstacleCommand : LogicCommand
    {
        private int _gameObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClearObstacleCommand" /> class.
        /// </summary>
        public LogicClearObstacleCommand()
        {
            // LogicClearObstacleCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClearObstacleCommand" /> class.
        /// </summary>
        public LogicClearObstacleCommand(int gameObjectId)
        {
            this._gameObjectId = gameObjectId;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObjectId = stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectId);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 507;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectId);

            if (gameObject != null)
            {
                if (gameObject.GetGameObjectType() == 3)
                {
                    LogicObstacle obstacle = (LogicObstacle) gameObject;

                    if (obstacle.GetObstacleData().GetVillageType() == level.GetVillageType())
                    {
                        LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                        if (obstacle.CanStartClearing())
                        {
                            LogicObstacleData obstacleData = obstacle.GetObstacleData();

                            if (obstacle.GetVillageType() == 1)
                            {
                                int village2TownHallLevel = playerAvatar.GetVillage2TownHallLevel();

                                if (village2TownHallLevel < LogicDataTables.GetGlobals().GetMinVillage2TownHallLevelForDestructObstacle() &&
                                    obstacleData.GetClearTime() > 0)
                                {
                                    return 0;
                                }
                            }

                            LogicResourceData clearResourceData = obstacleData.GetClearResourceData();
                            int clearCost = obstacleData.GetClearCost();

                            if (playerAvatar.HasEnoughResources(clearResourceData, clearCost, true, this, false))
                            {
                                if (obstacleData.GetClearTime() == 0 || level.HasFreeWorkers(this, -1))
                                {
                                    playerAvatar.CommodityCountChangeHelper(0, clearResourceData, -clearCost);
                                    obstacle.StartClearing();

                                    if (obstacle.IsTombstone())
                                    {
                                        int tombGroup = obstacle.GetTombGroup();

                                        if (tombGroup != 2)
                                        {
                                            LogicArrayList<LogicGameObject> gameObjects = level.GetGameObjectManager().GetGameObjects(3);

                                            for (int i = 0; i < gameObjects.Count; i++)
                                            {
                                                LogicObstacle go = (LogicObstacle) gameObjects[i];

                                                if (go.IsTombstone() && go.GetTombGroup() == tombGroup)
                                                {
                                                    go.StartClearing();
                                                }
                                            }
                                        }
                                    }
                                }

                                return 0;
                            }
                        }

                        return -1;
                    }

                    return -32;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Unlockes the village 2.
        /// </summary>
        public bool UnlockVillage2()
        {
            return false;
        }
    }
}