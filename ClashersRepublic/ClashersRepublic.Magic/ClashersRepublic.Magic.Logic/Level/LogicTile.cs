namespace ClashersRepublic.Magic.Logic.Level
{
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public sealed class LogicTile
    {
        private byte _passableFlag;
        private byte _tileX;
        private byte _tileY;

        private int _roomIndex;
        private int _pathFinderCost;

        private LogicArrayList<LogicGameObject> _gameObjects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicTile" /> class.
        /// </summary>
        public LogicTile(byte tileX, byte tileY)
        {
            this._gameObjects = new LogicArrayList<LogicGameObject>(4);
            this._tileX = tileX;
            this._tileY = tileY;
            this._passableFlag = 16;
            this._roomIndex = -1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._gameObjects = null;
            this._passableFlag = 16;
        }

        /// <summary>
        ///     Adds the specified gameobject.
        /// </summary>
        public void AddGameObject(LogicGameObject gameObject)
        {
            this._gameObjects.Add(gameObject);

            if (!gameObject.IsPassable())
            {
                this._passableFlag &= 0xEF;
            }

            this.RefreshSubTiles();
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
                        if (this._gameObjects[i].IsPassable() && !this._gameObjects[i].IsUnbuildable())
                        {
                            return false;
                        }
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
                        if (!this._gameObjects[i].IsPassable() || this._gameObjects[i].IsUnbuildable())
                        {
                            return false;
                        }
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
                this.RefreshPassableFlag();
            }
        }

        /// <summary>
        ///     Gets the tall grass object.
        /// </summary>
        public LogicObstacle GetTallGrass()
        {
            for (int i = 0; i < this._gameObjects.Count; i++)
            {
                LogicGameObject gameObject = this._gameObjects[i];

                if (gameObject.GetGameObjectType() == 3)
                {
                    LogicObstacle obstacle = (LogicObstacle) this._gameObjects[i];

                    if (obstacle.GetObstacleData().TallGrass)
                    {
                        return obstacle;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the <see cref="LogicGameObject"/> instance at the specified index.
        /// </summary>
        public LogicGameObject GetGameObject(int idx)
        {
            return this._gameObjects[idx];
        }

        /// <summary>
        ///     Gets the number of gameobjects in instance.
        /// </summary>
        public int GetGameObjectCount()
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
            this._passableFlag &= 0xF0;

            for (int i = 0; i < this._gameObjects.Count; i++)
            {
                LogicGameObject gameObject = this._gameObjects[i];

                this._pathFinderCost = LogicMath.Max(this._pathFinderCost, gameObject.GetPathFinderCost());

                if (!gameObject.IsPassable())
                {
                    int width = gameObject.GetWidthInTiles();

                    if (width == 1)
                    {
                        this._passableFlag |= 0xF0;
                    }
                    else
                    {
                        int tileX = gameObject.GetTileX();
                        int tileY = gameObject.GetTileY();
                        int edge = gameObject.PassableSubtilesAtEdge();
                        int startX = 2 * width - edge;
                        int startY = 2 * width - edge;
                        int endX = 2 * (width - edge);
                        int endY = 2 * (width - edge);

                        // RIP

                        /* do
                           {
                               v11 = v20 + v19;
                               v12 = -1;
                               v13 = v20;
                               do
                               {
                                   v14 = 2 * (v28 - v27) + v12 + 1;
                                   v16 = __OFSUB__(v14, v9);
                                   v15 = v14 - v9 < 0;
                                   if ( v14 < v9 )
                                   {
                                       v16 = __OFSUB__(v11, v10);
                                       v15 = v11 - v10 < 0;
                                   }
                                   if ( v15 ^ v16 )
                                   {
                                       v18 = __OFSUB__(v11, v8);
                                       v17 = v11 - v8 < 0;
                                       if ( v11 >= v8 )
                                       {
                                           v18 = __OFSUB__(v14, v8);
                                           v17 = v14 - v8 < 0;
                                       }
                                        if ( !(v17 ^ v18) )
                                        *(_BYTE *)(v1 + 8) |= 1 << v13;
                                    }
                                    ++v12;
                                    v13 += 2;
                               }
                               while ( v12 < 1 );
                               v19 = v29;
                               v16 = __OFSUB__(v20, 1);
                               v15 = v20++ - 1 < 0;
                           }
                           while ( v15 ^ v16 );
                           */
                    }
                }
            }
        }

        /// <summary>
        ///     Refreshes the passable flag.
        /// </summary>
        public void RefreshPassableFlag()
        {
            if (this._gameObjects.Count < 1)
            {
                this._passableFlag = (byte) (this._passableFlag | 0x10);
            }
            else
            {
                for (int i = 0; i < this._gameObjects.Count; i++)
                {
                    if (this._gameObjects[i] != null)
                    {
                        if (!this._gameObjects[i].IsPassable())
                        {
                            break;
                        }
                    }
                }

                this._passableFlag &= 0xEF;
            }

            this.RefreshSubTiles();
        }

        /// <summary>
        ///     Gets the passable flag.
        /// </summary>
        public byte GetPassableFlag()
        {
            return (byte) ((this._passableFlag >> 4) & 1);
        }
    }
}