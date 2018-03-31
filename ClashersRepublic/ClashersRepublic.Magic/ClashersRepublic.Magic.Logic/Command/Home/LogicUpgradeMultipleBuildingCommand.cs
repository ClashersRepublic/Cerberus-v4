namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;

    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Util;

    public sealed class LogicUpgradeMultipleBuildingCommand : LogicCommand
    {
        private bool _useAltResources;
        private LogicArrayList<int> _gameObjectIds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUpgradeMultipleBuildingCommand" /> class.
        /// </summary>
        public LogicUpgradeMultipleBuildingCommand()
        {
            this._gameObjectIds = new LogicArrayList<int>(300);
        }
        
        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._useAltResources = stream.ReadBoolean();

            int count = stream.ReadInt();

            if (count > 300)
            {
                count = 300;
            }

            for (int i = 0; i < count; i++)
            {
                this._gameObjectIds.Add(stream.ReadInt());
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteBoolean(this._useAltResources);

            int count = this._gameObjectIds.Count;

            if (count > 300)
            {
                count = 300;
            }

            for (int i = 0; i < count; i++)
            {
                encoder.WriteInt(this._gameObjectIds[i]);
            }

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 549;
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
            if (this._gameObjectIds.Count > 0)
            {
                LogicResourceData buildResourceData = null;
                int buildCost = 0;

                for (int i = 0; i < this._gameObjectIds.Count; i++)
                {
                    LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectIds[i]);

                    if (gameObject != null && gameObject.GetGameObjectType() == 0)
                    {
                        LogicBuilding building = (LogicBuilding) gameObject;
                        LogicBuildingData buildingData = building.GetBuildingData();

                        int upgradeLevel = building.GetUpgradeLevel();

                        if (buildingData.IsTownHallVillage2())
                        {
                            return -76;
                        }

                        if (building.CanUpgrade(false) && buildingData.GetUpgradeLevelCount() > upgradeLevel + 1 && buildingData.GetAmountCanBeUpgraded(0) == 0)
                        {
                            buildResourceData = buildingData.GetBuildResource(upgradeLevel + 1);

                            if (this._useAltResources)
                            {
                                buildResourceData = buildingData.GetAltBuildResource(upgradeLevel + 1);
                            }

                            buildCost += buildingData.GetBuildCost(upgradeLevel + 1, level);
                        }
                    }
                }

                if (buildResourceData != null)
                {
                    LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                    if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false))
                    {
                        if (level.HasFreeWorkers(this, -1))
                        {
                            bool ignoreState = true;

                            for (int i = 0; i < this._gameObjectIds.Count; i++)
                            {
                                LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectIds[i]);

                                if (gameObject != null && gameObject.GetGameObjectType() == 0)
                                {
                                    LogicBuilding building = (LogicBuilding)gameObject;
                                    LogicBuildingData buildingData = building.GetBuildingData();

                                    int upgradeLevel = building.GetUpgradeLevel();

                                    if (building.CanUpgrade(false) && buildingData.GetUpgradeLevelCount() > upgradeLevel + 1 && buildingData.GetAmountCanBeUpgraded(0) == 0)
                                    {
                                        if (this._gameObjectIds.Count > 6)
                                        {
                                            ignoreState = (building.GetTileX() + building.GetTileY()) % (this._gameObjectIds.Count / 4) == 0;
                                        }

                                        building.StartUpgrading(ignoreState, false);
                                    }
                                }
                            }

                            playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);

                            return 0;
                        }
                    }
                }
            }

            return -2;
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