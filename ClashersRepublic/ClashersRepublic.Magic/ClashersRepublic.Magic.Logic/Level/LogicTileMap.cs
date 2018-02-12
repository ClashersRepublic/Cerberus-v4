namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.GameObject;

    public sealed class LogicTileMap
    {
        private byte _passableFlag;
        private bool _roomEnabled;

        private int _sizeX;
        private int _sizeY;

        private LogicTile[] _tiles;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTileMap"/> class.
        /// </summary>
        public LogicTileMap(int x, int y)
        {
            this._sizeX = x;
            this._sizeY = y;
            this._tiles = new LogicTile[x * y];

            for (int i = 0; i < this._tiles.Length; i++)
            {
                this._tiles[i] = new LogicTile();
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
                    this._tiles[i].Destruct();
                    this._tiles[i] = null;
                }

                this._tiles = null;
            }
        }

        /// <summary>
        ///     Adds the specified gameobject to tiles.
        /// </summary>
        public void AddGameObject(LogicGameObject gameObject)
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
                            this._tiles[tileX + j + this._sizeX * tileY].AddGameObject(gameObject);
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
                            this._tiles[oldTileX + j + this._sizeX * oldTileY].RemoveGameObject(gameObject);
                            this._tiles[tileX + j + this._sizeX * tileY].AddGameObject(gameObject);
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
        ///     Refreshes passable of this gameobject.
        /// </summary>
        public void RefreshPassable(LogicGameObject gameObject)
        {

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
        public void FillRoom(int tileIndex, int value)
        {
            LogicTile tile = this._tiles[tileIndex];

            if (tile.GetGameObjectCount() == 0)
            {
                // TODO Implement LogicTileMap::fillRoom.
            }
        }
    }
}