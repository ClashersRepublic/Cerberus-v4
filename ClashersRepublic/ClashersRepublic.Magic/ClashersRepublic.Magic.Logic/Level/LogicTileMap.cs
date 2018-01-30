namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.GameObject;

    public sealed class LogicTileMap
    {
        private byte _passableFlag;
        private bool _roomEnabled;
        private int _x;
        private int _y;
        private LogicTile[] _tiles;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTileMap"/> class.
        /// </summary>
        public LogicTileMap(int x, int y)
        {
            this._x = x;
            this._y = y;
            this._tiles = new LogicTile[x * y];

            for (int i = 0; i < this._tiles.Length; i++)
            {
                this._tiles[i] = new LogicTile();
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
                    int sizeY = gameObject.GetHeighInTiles();

                    for (int i = 0; i < sizeY; i++)
                    {
                        for (int j = 0; j < sizeX; j++)
                        {
                            this._tiles[tileX + j + this._x * tileY].AddGameObject(gameObject);
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
                    int sizeY = gameObject.GetHeighInTiles();

                    for (int i = 0; i < sizeY; i++)
                    {
                        for (int j = 0; j < sizeX; j++)
                        {
                            this._tiles[oldTileX + j + this._x * oldTileY].RemoveGameObject(gameObject);
                            this._tiles[tileX + j + this._x * tileY].AddGameObject(gameObject);
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
        ///     Updates all room indices.
        /// </summary>
        public void UpdateRoomIndices()
        {
            if (this._roomEnabled)
            {
                int tileCount = this._x * this._y;

                for (int i = 0; i < tileCount; i++)
                {
                    LogicTile tmp = this._tiles[i];

                    bool isPassable = tmp.IsFullyNotPassable();
                    byte isPassableVal = isPassable ? (byte) 1 : (byte) 0;

                    tmp.SetRoomIdx(isPassableVal << 31 >> 31);
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