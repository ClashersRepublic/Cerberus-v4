namespace ClashersRepublic.Magic.Logic.Util
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicPathFinder
    {
        protected LogicTileMap _tileMap;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicPathFinder"/> class.
        /// </summary>
        public LogicPathFinder(LogicTileMap tileMap)
        {
            this._tileMap = tileMap;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            this._tileMap = null;
        }

        /// <summary>
        ///     Sets the cost strategy.
        /// </summary>
        public virtual void SetCostStrategy(bool enabled, int quality)
        {
            // SetCostStrategy.
        }

        /// <summary>
        ///     Resets the cost strategy to default.
        /// </summary>
        public virtual void ResetCostStartegyToDefault()
        {
            // ResetCostStartegyToDefault.
        }

        /// <summary>
        ///     Finds the path to the destination position.
        /// </summary>
        public virtual void FindPath(LogicVector2 currentPosition, LogicVector2 destinationPosition, bool floydAlgorithm)
        {
            // FindPath.
        }

        /// <summary>
        ///     Gets the path length.
        /// </summary>
        public virtual int GetPathLength()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the path point.
        /// </summary>
        public virtual LogicVector2 GetPathPoint(LogicVector2 position, int idx)
        {
            return null;
        }

        /// <summary>
        ///     Gets the path point in subtile.
        /// </summary>
        public virtual int GetPathPointSubTile()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the parent.
        /// </summary>
        public virtual int GetParent(int index)
        {
            return 0;
        }
    }
}