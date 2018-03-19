namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicBuyBuildingCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private LogicBuildingData _buildingData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicBuyBuildingCommand()
        {
            // LogicBuyBuildingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicBuyBuildingCommand(int x, int y, LogicBuildingData buildingData)
        {
            this._x = x;
            this._y = y;
            this._buildingData = buildingData;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._x = stream.ReadInt();
            this._y = stream.ReadInt();
            this._buildingData = (LogicBuildingData) stream.ReadDataReference(0);
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._x);
            encoder.WriteInt(this._y);
            encoder.WriteDataReference(this._buildingData);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 500;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._x = 0;
            this._y = 0;
            this._buildingData = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            int villageType = level.GetVillageType();

            if (this._buildingData.GetVillageType() == villageType)
            {
                if (this._buildingData.GetBuildingClass().CanBuy)
                {
                    if (level.IsValidPlaceForBuilding(this._x, this._y, this._buildingData.GetWidth(), this._buildingData.GetHeight(), null))
                    {
                        if (!level.IsBuildingCapReached(this._buildingData, true))
                        {
                            LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                            LogicResourceData buildResourceData = this._buildingData.GetBuildResource();
                            int buildResourceCost = this._buildingData.GetBuildCost(0, level);

                            if (playerAvatar.HasEnoughResources(buildResourceData, buildResourceCost, true, this, false))
                            {
                                if (this._buildingData.IsWorkerBuilding() || this._buildingData.GetConstructionTime(0, level, 0) <= 0 && !LogicDataTables.GetGlobals().WorkerForZeroBuilTime() || level.HasFreeWorkers(this, -1))
                                {
                                    if (buildResourceData.PremiumCurrency)
                                    {
                                        playerAvatar.UseDiamonds(buildResourceCost);
                                    }
                                    else
                                    {
                                        playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildResourceCost);
                                    }


                                    LogicBuilding building = (LogicBuilding) LogicGameObjectFactory.CreateGameObject(this._buildingData, level, villageType);
                                    building.SetPositionXY(this._x << 9, this._y << 9);
                                    level.GetGameObjectManager().AddGameObject(building);
                                    building.StartConstructing(false);

                                    if (this._buildingData.IsWall() && level.IsBuildingCapReached(this._buildingData, false))
                                    {
                                        level.GetGameListener().BuildingCapReached(this._buildingData);
                                    }
                                }
                            }

                            return 0;
                        }
                    }
                }

                return -33;
            }

            return -32;
        }
    }
}