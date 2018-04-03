namespace LineageSoft.Magic.Logic.Level
{
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.GameObject;
    using LineageSoft.Magic.Logic.Util;

    public sealed class LogicTileMap
    {
        private bool _roomEnabled;

        private readonly int _sizeX;
        private readonly int _sizeY;

        private LogicTile[] _tiles;
        private LogicPathFinder _pathFinder;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTileMap" /> class.
        /// </summary>
        public LogicTileMap(int x, int y)
        {
            this._sizeX = x;
            this._sizeY = y;
            this._tiles = new LogicTile[x * y];

            for (int i = 0; i < this._tiles.Length; i++)
            {
                this._tiles[i] = new LogicTile((byte) (i % x), (byte) (i / x));
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._tiles != null)
            {
                for (int i = 0; i < this._tiles.Length; i++)
                {
                    if (this._tiles[i] != null)
                    {
                        this._tiles[i].Destruct();
                        this._tiles[i] = null;
                    }
                }

                this._tiles = null;
            }
        }

        /// <summary>
        ///     Gets the size x.
        /// </summary>
        public int GetSizeX()
        {
            return this._sizeX;
        }

        /// <summary>
        ///     Gets the size y.
        /// </summary>
        public int GetSizeY()
        {
            return this._sizeY;
        }

        /// <summary>
        ///     Gets the <see cref="LogicTile"/> instance.
        /// </summary>
        public LogicTile GetTile(int x, int y)
        {
            if (x > -1 && y > -1)
            {
                if (this._sizeX > x && this._sizeY > y)
                {
                    return this._tiles[x + this._sizeX * y];
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the <see cref="LogicPathFinder"/> instance.
        /// </summary>
        public LogicPathFinder GetPathFinder()
        {
            if (this._pathFinder == null)
            {
                if (LogicDataTables.GetGlobals().UseNewPathFinder())
                {
                    this._pathFinder = new LogicPathFinderNew(this);
                }
                else
                {
                    this._pathFinder = new LogicPathFinderOld(this);
                }
            }

            return this._pathFinder;
        }

        /// <summary>
        ///     Adds the specified <see cref="LogicGameObject"/> instance to tiles.
        /// </summary>
        public void AddGameObject(LogicGameObject gameObject)
        {
            if (gameObject.IsStaticObject())
            {
                int tileX = gameObject.GetTileX();
                int tileY = gameObject.GetTileY();

                if (tileX >= 0)
                {
                    if (tileY >= 0)
                    {
                        int sizeX = gameObject.GetWidthInTiles();
                        int sizeY = gameObject.GetHeightInTiles();

                        for (int i = 0; i < sizeY; i++)
                        {
                            for (int j = 0; j < sizeX; j++)
                            {
                                this.GetTile(tileX + j, tileY + i).AddGameObject(gameObject);
                            }
                        }

                        if (!gameObject.IsPassable())
                        {
                            this.UpdateRoomIndices();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the specified gameobject has been moved.
        /// </summary>
        public void GameObjectMoved(LogicGameObject gameObject, int oldTileX, int oldTileY)
        {
            if (gameObject.IsStaticObject())
            {
                int tileX = gameObject.GetTileX();
                int tileY = gameObject.GetTileY();

                if (tileX >= 0)
                {
                    if (tileY >= 0)
                    {
                        int sizeX = gameObject.GetWidthInTiles();
                        int sizeY = gameObject.GetHeightInTiles();

                        for (int i = 0; i < sizeY; i++)
                        {
                            for (int j = 0; j < sizeX; j++)
                            {
                                this.GetTile(oldTileX + j, oldTileY + i).RemoveGameObject(gameObject);
                                this.GetTile(tileX + j, tileY + i).AddGameObject(gameObject);
                            }
                        }

                        if (!gameObject.IsPassable())
                        {
                            this.UpdateRoomIndices();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the specified <see cref="LogicGameObject"/> instance.
        /// </summary>
        public void RemoveGameObject(LogicGameObject gameObject)
        {
            if (gameObject.IsStaticObject())
            {
                int tileX = gameObject.GetTileX();
                int tileY = gameObject.GetTileY();

                if (tileX >= 0)
                {
                    if (tileY >= 0)
                    {
                        int sizeX = gameObject.GetWidthInTiles();
                        int sizeY = gameObject.GetHeightInTiles();

                        for (int i = 0; i < sizeY; i++)
                        {
                            for (int j = 0; j < sizeX; j++)
                            {
                                this.GetTile(tileX + j, tileY + i).RemoveGameObject(gameObject);
                            }
                        }

                        if (!gameObject.IsPassable())
                        {
                            this.UpdateRoomIndices();
                        }
                    }
                }
            }
        }
        
        /// <summary>
        ///     Enables the room indices.
        /// </summary>
        public void EnableRoomIndices(bool state)
        {
            if (state)
            {
                if (!this._roomEnabled)
                {
                    this._roomEnabled = true;
                    this.UpdateRoomIndices();
                }
            }

            this._roomEnabled = state;
        }

        /// <summary>
        ///     Refreshes passable of the specified <see cref="LogicGameObject"/> instance..
        /// </summary>
        public void RefreshPassable(LogicGameObject gameObject)
        {
            if (gameObject.IsStaticObject())
            {
                int tileX = gameObject.GetTileX();
                int tileY = gameObject.GetTileY();

                if (tileX >= 0)
                {
                    if (tileY >= 0)
                    {
                        int sizeX = gameObject.GetWidthInTiles();
                        int sizeY = gameObject.GetHeightInTiles();

                        for (int i = 0; i < sizeY; i++)
                        {
                            for (int j = 0; j < sizeX; j++)
                            {
                                this.GetTile(tileX + j, tileY + i).RefreshPassableFlag();
                            }
                        }

                        if (!gameObject.IsPassable())
                        {
                            this.UpdateRoomIndices();
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Updates all room indices.
        /// </summary>
        public void UpdateRoomIndices()
        {
            if (this._roomEnabled)
            {
                int tileCount = this._sizeX * this._sizeY;

                for (int i = 0; i < tileCount; i++)
                {
                    LogicTile tmp = this._tiles[i];
                    tmp.SetRoomIdx(tmp.IsFullyNotPassable() ? -1 : 0);
                }

                for (int i = 0, roomId = 1; i < tileCount; i++)
                {
                    LogicTile tmp = this._tiles[i];

                    if (tmp.GetGameObjectCount() == 0)
                    {
                        this.FillRoom(i, roomId++);
                    }
                }
            }
        }

        /// <summary>
        ///     Fills the specified room.
        /// </summary>
        public void FillRoom(int tileIndex, int roomIdx)
        {
            LogicTile tile = this._tiles[tileIndex];

            if (tile.GetGameObjectCount() == 0)
            {
                // TODO Implement LogicTileMap::fillRoom.
            }
        }

        /// <summary>
        ///     Gets if the specified position is passable by path finder.
        /// </summary>
        public bool IsPassablePathFinder(int x, int y)
        {
            if (this._sizeX > x / 2 && this._sizeY > y / 2)
            {
                if (x / 2 + y / 2 >= 0)
                {
                    LogicTile tile = this.GetTile(x / 2, y / 2);

                    if (tile != null)
                    {
                        return tile.IsPassablePathFinder(x % 2, y % 2);
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets if the specified position is a valid attack position.
        /// </summary>
        public bool IsValidAttackPos(int x, int y)
        {
            for (int i = 0, posX = x - 1; i < 2; i++, posX++)
            {
                for (int j = 0, posY = y - 1; j < 2; j++, posY++)
                {
                    if (this._sizeX > posX && this._sizeY > posY)
                    {
                        if (posX > -1 && posY > -1)
                        {
                            LogicTile tile = this.GetTile(posX + i, posY + j);

                            if (tile != null)
                            {
                                for (int k = 0; k < tile.GetGameObjectCount(); k++)
                                {
                                    LogicGameObject gameObject = tile.GetGameObject(k);

                                    if (!gameObject.IsPassable())
                                    {
                                        if (gameObject.GetGameObjectType() == 0)
                                        {
                                            LogicBuilding building = (LogicBuilding) gameObject;

                                            if (!building.GetBuildingData().Hidden)
                                            {
                                                return false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }
    }
}