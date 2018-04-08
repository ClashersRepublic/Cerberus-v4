namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicBuyTrapCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private LogicTrapData _trapData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyTrapCommand" /> class.
        /// </summary>
        public LogicBuyTrapCommand()
        {
            // LogicBuyTrapCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyTrapCommand" /> class.
        /// </summary>
        public LogicBuyTrapCommand(int x, int y, LogicTrapData trapData)
        {
            this._x = x;
            this._y = y;
            this._trapData = trapData;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._x = stream.ReadInt();
            this._y = stream.ReadInt();
            this._trapData = (LogicTrapData) stream.ReadDataReference(11);

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._x);
            encoder.WriteInt(this._y);
            encoder.WriteDataReference(this._trapData);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 510;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._x = 0;
            this._y = 0;
            this._trapData = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (this._trapData != null)
            {
                if (this._trapData.GetVillageType() == level.GetVillageType())
                {
                    if (level.IsValidPlaceForBuilding(this._x, this._y, this._trapData.GetWidth(), this._trapData.GetHeight(), null))
                    {
                        LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                        LogicResourceData buildResourceData = this._trapData.GetBuildResource();

                        int buildCost = this._trapData.GetBuildCost(0);

                        if (playerAvatar.HasEnoughResources(buildResourceData, buildCost, true, this, false) && !level.IsTrapCapReached(this._trapData, true))
                        {
                            if (buildResourceData.PremiumCurrency)
                            {
                                playerAvatar.UseDiamonds(buildCost);
                            }
                            else
                            {
                                playerAvatar.CommodityCountChangeHelper(0, buildResourceData, -buildCost);
                            }

                            LogicTrap trap = (LogicTrap) LogicGameObjectFactory.CreateGameObject(this._trapData, level, level.GetVillageType());

                            if (this._trapData.GetBuildTime(0) == 0)
                            {
                                trap.FinishConstruction(false);
                            }

                            trap.SetInitialPosition(this._x << 9, this._y << 9);
                            level.GetGameObjectManager().AddGameObject(trap, -1);

                            if (level.IsTrapCapReached(this._trapData, false))
                            {
                                level.GetGameListener().TrapCapReached(this._trapData);
                            }

                            if (trap.GetVillageType() != 0)
                            {
                                int x = trap.GetTileX();
                                int y = trap.GetTileY();
                                int width = trap.GetWidthInTiles();
                                int height = trap.GetHeightInTiles();

                                for (int i = 0; i < width; i++)
                                {
                                    for (int j = 0; j < height; j++)
                                    {
                                        LogicObstacle tallGrass = level.GetTileMap().GetTile(x + i, y + j).GetTallGrass();

                                        if (tallGrass != null)
                                        {
                                            level.GetGameObjectManager().RemoveGameObject(tallGrass);
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

            return -1;
        }
    }
}