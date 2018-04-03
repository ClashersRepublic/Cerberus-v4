namespace LineageSoft.Magic.Logic.Util
{
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Math;

    public class LogicPathFinderOld : LogicPathFinder
    {
        private int[] _pathBuffer;
        private int[] _heapBuffer;
        private int _heapLength;
        private int _pathLength;
        private int _costStrategy;
        private int _sizeX;
        private int _sizeY;

        private LogicSavedPath[] _savedPaths;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicPathFinderOld"/> class.
        /// </summary>
        public LogicPathFinderOld(LogicTileMap tileMap) : base(tileMap)
        {
            this._sizeX = 3;
            this._sizeY = 4;

            if (tileMap != null)
            {
                this._sizeX = 2 * tileMap.GetSizeX();
                this._sizeY = 2 * tileMap.GetSizeY();
            }

            int arraySize = this._sizeX * this._sizeY;

            this._heapBuffer = new int[arraySize];
            this._pathBuffer = new int[arraySize];
            this._savedPaths = new LogicSavedPath[30];

            for (int i = 0; i < 30; i++)
            {
                this._savedPaths[i] = new LogicSavedPath(4 * this._sizeX);
            }
        }

        /// <summary>
        ///     Adds the specified value to path.
        /// </summary>
        public void Add(int path)
        {
            this._heapLength++;
        }

        /// <summary>
        ///     Adds the specified tile.
        /// </summary>
        public void AStarAddTile(int tile)
        {
            int tmp1 = tile / this._sizeX;
            int tmp2 = tile % this._sizeX;

            this.AStarAddTile(tile, tmp2, tmp1 - 1, tile - this._sizeX, 100);
            this.AStarAddTile(tile, tmp2, tmp1 + 1, this._sizeX + tile, 100);

            this.AStarAddTile(tile, tmp2 - 1, tmp1, tile - 1, 100);
            this.AStarAddTile(tile, tmp2 + 1, tmp1, tile + 1, 100);
            this.AStarAddTile(tile, tmp2 - 1, tmp1 - 1, tile - 1 - this._sizeX, 141);
            this.AStarAddTile(tile, tmp2 - 1, tmp1 + 1, this._sizeX + tile - 1, 141);
            this.AStarAddTile(tile, tmp2 + 1, tmp1 + 1, this._sizeX + tile + 1, 141);
            this.AStarAddTile(tile, tmp2 + 1, tmp1 - 1, tile + 1 - this._sizeX, 141);
        }

        /// <summary>
        ///     Adds the sepcified tile.
        /// </summary>
        public void AStarAddTile(int tile, int uk1, int uk2, int uk3, int uk4)
        {

        }

        /// <summary>
        ///     Gets the path length.
        /// </summary>
        public override int GetPathLength()
        {
            return this._pathLength;
        }

        /// <summary>
        ///     Gets the path point.
        /// </summary>
        public override LogicVector2 GetPathPoint(LogicVector2 position, int idx)
        {
            if (idx < 0 || this._pathLength <= idx)
            {
                Debugger.Error("illegal path index");
            }
            else
            {
                int sizeX = this._sizeX;
                int point = this._pathBuffer[idx];

                return new LogicVector2(point % sizeX, point / sizeX);
            }

            return null;
        }

        /// <summary>
        ///     Gets the path point in subtile.
        /// </summary>
        public override int GetPathPointSubTile()
        {
            Debugger.Warning("getPathPointSubTile() called. Should not be called ever for LogicPathFinderOld!");
            return 0;
        }

        /// <summary>
        ///     Gets if the specified point is in collision with a gameobject.
        /// </summary>
        public bool IsCollision(int x, int y)
        {
            if (this._tileMap != null)
            {
                LogicTile tile = this._tileMap.GetTile(x >> 1, y >> 1);

                if (tile != null)
                {
                    return true; // ?
                }
            }

            return false;
        }

        /// <summary>
        ///     Sets the cost strategy.
        /// </summary>
        public override void SetCostStrategy(bool enabled, int quality)
        {
            // SetCostStrategy.
        }

        /// <summary>
        ///     Resets the cost strategy to default.
        /// </summary>
        public override void ResetCostStartegyToDefault()
        {
            // ResetCostStartegyToDefault.
        }
    }
}