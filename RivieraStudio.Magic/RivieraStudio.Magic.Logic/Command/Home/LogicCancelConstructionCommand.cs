namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;

    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicCancelConstructionCommand : LogicCommand
    {
        private int _gameObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCancelConstructionCommand" /> class.
        /// </summary>
        public LogicCancelConstructionCommand()
        {
            // LogicCancelConstructionCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCancelConstructionCommand" /> class.
        /// </summary>
        public LogicCancelConstructionCommand(int gameObjectId)
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
            return 505;
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

                    if (!LogicDataTables.GetGlobals().AllowCancelBuildingConstruction() &&
                        building.GetUpgradeLevel() == 0 &&
                        building.IsConstructing())
                    {
                        if (!building.IsUpgrading())
                        {
                            return -2;
                        }
                    }

                    if (building.IsConstructing())
                    {
                        building.GetListener().CancelNotification();
                        building.CancelConstruction();

                        return 0;
                    }
                }

                if (gameObject.GetGameObjectType() == 3)
                {
                    LogicObstacle obstacle = (LogicObstacle) gameObject;

                    if (obstacle.IsClearingOnGoing())
                    {
                        LogicObstacleData data = obstacle.GetObstacleData();
                        LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                        playerAvatar.CommodityCountChangeHelper(0, data.GetClearResourceData(), data.GetClearCost());
                        obstacle.CancelClearing();

                        return 0;
                    }
                }

                if (gameObject.GetGameObjectType() == 4)
                {
                    LogicTrap trap = (LogicTrap) gameObject;

                    if (trap.IsConstructing())
                    {
                        trap.GetListener().CancelNotification();
                        trap.CancelConstruction();

                        return 0;
                    }
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