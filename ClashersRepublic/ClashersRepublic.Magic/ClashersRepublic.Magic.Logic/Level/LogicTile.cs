namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Titan.Util;

    public sealed class LogicTile
    {
        private byte _passableFlag;
        private int _roomIndex;

        private LogicArrayList<LogicGameObject> _gameObjects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTile"/> class.
        /// </summary>
        public LogicTile()
        {
            this._gameObjects = new LogicArrayList<LogicGameObject>();
        }

        /// <summary>
        ///     Adds the specified gameobject.
        /// </summary>
        public void AddGameObject(LogicGameObject gameObject)
        {
            this._gameObjects.Add(gameObject);
        }

        /// <summary>
        ///     Gets a value indicating whether the specified building is passable.
        /// </summary>
        public bool IsPassable(LogicGameObject gameObject)
        {
            if (this._gameObjects.Count > 0)
            {
                for (int i = 0; i < this._gameObjects.Count; i++)
                {
                    if (this._gameObjects[i] != gameObject)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified gameobject is buildable on this tile.
        /// </summary>
        public bool IsBuildable(LogicGameObject gameObject)
        {
            if (this._gameObjects.Count > 0)
            {
                for (int i = 0; i < this._gameObjects.Count; i++)
                {
                    if (this._gameObjects[i] != gameObject)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified gameobject is buildable on this tile.
        /// </summary>
        public bool IsBuildableWithIgnoreList(LogicGameObject[] gameObjects)
        {
            if (this._gameObjects.Count > 0)
            {
                for (int i = 0, index = -1; i < this._gameObjects.Count; i++, index = -1)
                {
                    LogicGameObject tmp = this._gameObjects[i];
                    
                    for (int j = 0; j < gameObjects.Length; j++)
                    {
                        if (gameObjects[j] == tmp)
                        {
                            index = j;
                            break;
                        }
                    }

                    if (index == -1)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        ///     Removes the specified gameobject of this tile.
        /// </summary>
        public void RemoveGameObject(LogicGameObject gameObject)
        {
            int index = -1;

            for (int i = 0; i < this._gameObjects.Count; i++)
            {
                if (this._gameObjects[i] == gameObject)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this._gameObjects.Remove(index);
            }
        }

        /// <summary>
        ///     Gets the number of gameobjects in instance.
        /// </summary>
        internal int GetGameObjectCount()
        {
            return this._gameObjects.Count;
        }

        /// <summary>
        ///     Gets a value indicating whether this tile is fully not passable.
        /// </summary>
        public bool IsFullyNotPassable()
        {
            return (this._passableFlag & 0xF) == 0xF;
        }

        /// <summary>
        ///     Sets the room index.
        /// </summary>
        public void SetRoomIdx(int index)
        {
            this._roomIndex = index;
        }

        /// <summary>
        ///     Refreshes sub tiles.
        /// </summary>
        public void RefreshSubTiles()
        {
            // TODO Implement LogicTile::refreshSubTiles.
        }

        /// <summary>
        ///     Refreshes the passable flag.
        /// </summary>
        public void RefreshPassableFlag()
        {
            bool isPassable = this.IsPassable(null);
            byte newFlag = (byte) (this._passableFlag & 0xEF);

            if (isPassable)
            {
                newFlag = (byte) (this._passableFlag | 0x10);
            }

            this._passableFlag = newFlag;
            this.RefreshSubTiles();
        }
    }
}