namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;

    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicUpgradeBuildingCommand : LogicCommand
    {
        private int _gameObjectId;
        private bool _useAltResources;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUpgradeBuildingCommand" /> class.
        /// </summary>
        public LogicUpgradeBuildingCommand()
        {
            // LogicUpgradeBuildingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUpgradeBuildingCommand" /> class.
        /// </summary>
        public LogicUpgradeBuildingCommand(int gameObjectId, bool useAltResources)
        {
            this._gameObjectId = gameObjectId;
            this._useAltResources = useAltResources;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObjectId = stream.ReadInt();
            this._useAltResources = stream.ReadBoolean();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectId);
            encoder.WriteBoolean(this._useAltResources);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 502;
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
                    LogicBuildingData buildingData = building.GetBuildingData();

                    if (buildingData.IsTownHallVillage2())
                    {
                        if (!this.UnlockVillage2())
                        {
                            return -76;
                        }
                    }

                    if (buildingData.GetVillageType() == level.GetVillageType())
                    {
                        if (building.GetWallIndex() == 0)
                        {
                            if (building.CanUpgrade(true))
                            {
                                int nextUpgradeLevel = building.GetUpgradeLevel() + 1;
                                int buildCost = buildingData.GetBuildCost(nextUpgradeLevel, level);

                                LogicResourceData buildResourceData = this._useAltResources ? buildingData.GetAltBuildResource(nextUpgradeLevel) : buildingData.GetBuildResource(nextUpgradeLevel);

                                if (buildResourceData != null)
                                {
                                    LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                                    if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false))
                                    {
                                        int constructionTime = buildingData.GetConstructionTime(nextUpgradeLevel, level, 0);

                                        if (constructionTime != 0 || LogicDataTables.GetGlobals().WorkerForZeroBuilTime())
                                        {
                                            if (!level.HasFreeWorkers(this, -1))
                                            {
                                                return -1;
                                            }
                                        }

                                        playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);
                                        building.StartUpgrading(true, false);

                                        return 0;
                                    }
                                }
                            }

                            return -1;
                        }

                        return -35;
                    }

                    return -32;
                }
                else if (gameObject.GetGameObjectType() == 4)
                {
                    LogicTrap trap = (LogicTrap) gameObject;

                    if (trap.CanUpgrade(true))
                    {
                        LogicTrapData data = trap.GetTrapData();
                        LogicResourceData buildResourceData = data.GetBuildResource();
                        LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                        int buildCost = data.GetBuildCost(trap.GetUpgradeLevel() + 1);

                        if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false))
                        {
                            int constructionTime = data.GetBuildTime(trap.GetUpgradeLevel() + 1);

                            if (constructionTime != 0 || LogicDataTables.GetGlobals().WorkerForZeroBuilTime())
                            {
                                if (!level.HasFreeWorkers(this, -1))
                                {
                                    return -1;
                                }

                                playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);
                                trap.StartUpgrading();

                                return 0;
                            }
                        }
                    }
                }
                else if (gameObject.GetGameObjectType() == 8)
                {
                    if (!this._useAltResources)
                    {
                        LogicVillageObject villageObject = (LogicVillageObject) gameObject;
                        // TODO: Implement upgrade vObjs.
                    }

                    return -31;
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