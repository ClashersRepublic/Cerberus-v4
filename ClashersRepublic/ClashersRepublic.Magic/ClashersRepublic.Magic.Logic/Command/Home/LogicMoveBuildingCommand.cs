namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;

    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicMoveBuildingCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private int _gameObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMoveBuildingCommand" /> class.
        /// </summary>
        public LogicMoveBuildingCommand()
        {
            // LogicMoveBuildingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMoveBuildingCommand" /> class.
        /// </summary>
        public LogicMoveBuildingCommand(int gameObjectId, int x, int y)
        {
            this._x = x;
            this._y = y;
            this._gameObjectId = gameObjectId;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._x = stream.ReadInt();
            this._y = stream.ReadInt();
            this._gameObjectId = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._x);
            encoder.WriteInt(this._y);
            encoder.WriteInt(this._gameObjectId);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 501;
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
                int gameObjectType = gameObject.GetGameObjectType();

                if (gameObjectType <= 6 && gameObjectType != 3)
                {
                    if (gameObjectType != 0 || ((LogicBuildingData) gameObject.GetData()).GetVillageType() == level.GetVillageType())
                    {
                        if (gameObjectType == 0)
                        {
                            if (((LogicBuilding) gameObject).GetWallIndex() != 0)
                            {
                                return -21;
                            }
                        }

                        int x = gameObject.GetTileX();
                        int y = gameObject.GetTileY();
                        int width = gameObject.GetWidthInTiles();
                        int height = gameObject.GetHeightInTiles();

                        if (gameObject.GetVillageType() != 0)
                        {
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

                        if (level.IsValidPlaceForBuilding(this._x, this._y, width, height, gameObject))
                        {
                            gameObject.SetPositionXY(this._x << 9, this._y << 9);

                            if (this._x != x || this._y != y)
                            {
                                if (level.GetHomeOwnerAvatar() != null)
                                {
                                    LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

                                    if (homeOwnerAvatar.GetTownHallLevel() >= LogicDataTables.GetGlobals().GetChallengeBaseCooldownTownHall())
                                    {
                                        // TODO: Implement Challenge Cooldown.
                                    }
                                }
                            }

                            return 0;
                        }
                        else
                        {
                            return -3;
                        }
                    }

                    return -32;
                }

                return -1;
            }

            return -2;
        }
    }
}