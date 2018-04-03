namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.GameObject;
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Titan.DataStream;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Util;

    public sealed class LogicMoveMultipleBuildingCommand : LogicCommand
    {
        private LogicArrayList<int> _xPositions;
        private LogicArrayList<int> _yPositions;
        private LogicArrayList<int> _gameObjectIds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicMoveMultipleBuildingCommand()
        {
            this._xPositions = new LogicArrayList<int>();
            this._yPositions = new LogicArrayList<int>();
            this._gameObjectIds = new LogicArrayList<int>();
        }
        
        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            int size = stream.ReadInt();

            if (size > 0)
            {
                if (size > 500)
                {
                    size = 500;
                }

                int idx = 0;

                do
                {
                    this._xPositions.Add(stream.ReadInt());
                    this._yPositions.Add(stream.ReadInt());
                    this._gameObjectIds.Add(stream.ReadInt());
                } while (++idx < size);
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            int size = this._gameObjectIds.Count;

            if (size > 500)
            {
                size = 500;
            }
            
            encoder.WriteInt(size);

            for (int i = 0; i < size; i++)
            {
                encoder.WriteInt(this._xPositions[i]);
                encoder.WriteInt(this._yPositions[i]);
                encoder.WriteInt(this._gameObjectIds[i]);
            }

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 533;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._xPositions = null;
            this._yPositions = null;
            this._gameObjectIds = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            int count = this._gameObjectIds.Count;

            if (count > 0)
            {
                bool validSize = true;
                bool validGameObjects = true;

                if (this._xPositions.Count != count || this._xPositions.Count != count || count > 500)
                {
                    validSize = false;
                }

                if (validSize)
                {
                    LogicGameObject[] gameObjects = new LogicGameObject[count];

                    for (int i = 0; i < count; i++)
                    {
                        LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectIds[i]);

                        if (gameObject != null)
                        {
                            int gameObjectType = gameObject.GetGameObjectType();

                            if (gameObjectType > 6 || gameObjectType == 3)
                            {
                                validGameObjects = false;
                            }

                            gameObjects[i] = gameObject;
                        }
                        else
                        {
                            validGameObjects = false;
                        }
                    }

                    if (validGameObjects)
                    {
                        bool validWallBlock = true;

                        for (int i = 0; i < count; i++)
                        {
                            LogicGameObject gameObject = gameObjects[i];

                            if (gameObject.GetGameObjectType() == 0 && ((LogicBuilding) gameObject).GetWallIndex() != 0 && validWallBlock)
                            {
                                LogicBuilding baseWallBlock = (LogicBuilding) gameObject;

                                int x = this._xPositions[i];
                                int y = this._yPositions[i];
                                int minX = 0;
                                int minY = 0;
                                int maxX = 0;
                                int maxY = 0;
                                int minWallBlockX = 0;
                                int minWallBlockY = 0;
                                int maxWallBlockX = 0;
                                int maxWallBlockY = 0;

                                bool success = true;

                                int wallBlockCnt = -1;

                                for (int j = 0; j < count; j++)
                                {
                                    LogicGameObject obj = gameObjects[j];

                                    if (obj.GetGameObjectType() == 0 && ((LogicBuilding) obj).GetWallIndex() == baseWallBlock.GetWallIndex())
                                    {
                                        LogicBuilding wallBlock = (LogicBuilding)obj;

                                        int tmp1 = x - this._xPositions[j];
                                        int tmp2 = y - this._yPositions[j];

                                        if ((x & this._xPositions[j]) != -1)
                                        {
                                            success = false;
                                        }

                                        minX = LogicMath.Min(minX, tmp1);
                                        minY = LogicMath.Min(minY, tmp2);
                                        maxX = LogicMath.Max(maxX, tmp1);
                                        maxY = LogicMath.Max(maxY, tmp2);

                                        int wallBlockX = wallBlock.GetBuildingData().GetWallBlockX(j);
                                        int wallBlockY = wallBlock.GetBuildingData().GetWallBlockY(j);

                                        minWallBlockX = LogicMath.Min(minWallBlockX, wallBlockX);
                                        minWallBlockY = LogicMath.Min(minWallBlockY, wallBlockY);
                                        maxWallBlockX = LogicMath.Min(maxWallBlockX, wallBlockX);
                                        maxWallBlockY = LogicMath.Min(maxWallBlockY, wallBlockY);

                                        ++wallBlockCnt;
                                    }
                                }

                                if (baseWallBlock.GetBuildingData().GetWallBlockCount() == wallBlockCnt)
                                {
                                    int wallBlockSizeX = maxWallBlockX - minWallBlockX;
                                    int wallBlockSizeY = maxWallBlockY - minWallBlockY;
                                    int lengthX = maxX - minX;
                                    int lengthY = maxY - minY;

                                    if (wallBlockSizeX != lengthX || wallBlockSizeY != lengthY)
                                    {
                                        if (!success && (wallBlockSizeX != lengthX) != (wallBlockSizeY != lengthY))
                                        {
                                            validGameObjects = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // EditmodeInvalidGameObjectType.
                    }

                    bool objectsOverlap = false;

                    if (validGameObjects)
                    {
                        int idx = 0;

                        while (idx < count)
                        {
                            int x = this._xPositions[idx];
                            int y = this._yPositions[idx];

                            LogicGameObject gameObject = gameObjects[idx];

                            int width = gameObject.GetWidthInTiles();
                            int height = gameObject.GetHeightInTiles();

                            int tmp1 = x + width;
                            int tmp2 = y + height;

                            for (int i = 0; i < count; i++)
                            {
                                LogicGameObject gameObject2 = gameObjects[idx];

                                if (gameObject2 != gameObject)
                                {
                                    int x2 = this._xPositions[i];
                                    int y2 = this._yPositions[i];
                                    int width2 = gameObject2.GetWidthInTiles();
                                    int height2 = gameObject2.GetHeightInTiles();
                                    int tmp3 = x + width2;
                                    int tmp4 = y + height2;

                                    if (tmp1 > x2 && tmp2 > y2 && x2 < tmp3 && y2 < tmp4)
                                    {
                                        objectsOverlap = true;
                                        return 0;
                                    }
                                }
                            }

                            ++idx;
                        }
                    }

                    if (validGameObjects)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int x = this._xPositions[i];
                            int y = this._yPositions[i];

                            LogicGameObject gameObject = gameObjects[i];

                            int width = gameObject.GetWidthInTiles();
                            int height = gameObject.GetHeightInTiles();

                            if (!level.IsValidPlaceForBuildingWithIgnoreList(x, y, width, height, gameObjects, count))
                            {
                                if (validGameObjects)
                                {
                                    // EditmodeInvalidPosition.
                                }

                                validGameObjects = false;
                            }
                        }
                    }

                    if (validGameObjects)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            int x = this._xPositions[i];
                            int y = this._yPositions[i];

                            LogicGameObject gameObject = gameObjects[i];

                            gameObject.SetPositionXY(x << 9, y << 9);
                        }

                        for (int i = 0; i < count; i++)
                        {
                            int x = this._xPositions[i];
                            int y = this._yPositions[i];

                            LogicGameObject gameObject = gameObjects[i];

                            int width = gameObject.GetWidthInTiles();
                            int height = gameObject.GetHeightInTiles();

                            for (int j = 0; j < width; j++)
                            {
                                for (int k = 0; k < height; k++)
                                {
                                    LogicObstacle tallGrass = level.GetTileMap().GetTile(x + j, y + k).GetTallGrass();

                                    if (tallGrass != null)
                                    {
                                        level.GetGameObjectManager().RemoveGameObject(tallGrass);
                                    }
                                }
                            }
                        }

                        if (validGameObjects)
                        {
                            if (level.GetHomeOwnerAvatar() != null)
                            {
                                LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

                                if (homeOwnerAvatar.GetTownHallLevel() >= LogicDataTables.GetGlobals().GetChallengeBaseCooldownTownHall())
                                {
                                    level.SetLayoutCooldownSecs(level.GetActiveLayout(level.GetVillageType()), LogicDataTables.GetGlobals().GetChallengeBaseSaveCooldown());
                                }
                            }
                        }

                        return 0;
                    }
                }
            }

            return -1;
        }
    }
}