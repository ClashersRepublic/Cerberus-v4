namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.GameObject;
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Titan.DataStream;

    public sealed class LogicUnlockBuildingCommand : LogicCommand
    {
        private int _gameObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnlockBuildingCommand" /> class.
        /// </summary>
        public LogicUnlockBuildingCommand()
        {
            // LogicUnlockBuildingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnlockBuildingCommand" /> class.
        /// </summary>
        public LogicUnlockBuildingCommand(int gameObjectId)
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
            return 520;
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
                if (gameObject.GetGameObjectType() == 0)
                {
                    LogicBuilding building = (LogicBuilding) gameObject;

                    if (building.IsLocked())
                    {
                        if (building.GetUpgradeLevel() == 0 && building.CanUnlock(true))
                        {
                            LogicBuildingData buildingData = building.GetBuildingData();

                            if (buildingData.GetConstructionTime(0, level, 0) == 0 || level.HasFreeWorkers(this, -1))
                            {
                                LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                                LogicResourceData buildResource = buildingData.GetBuildResource(0);
                                int buildCost = buildingData.GetBuildCost(0, level);

                                if (playerAvatar.HasEnoughResources(buildResource, buildCost, true, this, false))
                                {
                                    playerAvatar.CommodityCountChangeHelper(0, buildResource, -buildCost);
                                    building.StartConstructing(true);
                                    building.GetListener().RefreshState();

                                    return 0;
                                }
                            }
                        }
                    }
                }
            }

            return -1;
        }
    }
}