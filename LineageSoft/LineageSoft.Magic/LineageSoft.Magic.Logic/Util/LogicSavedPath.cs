namespace LineageSoft.Magic.Logic.Util
{
    public class LogicSavedPath
    {
        private int[] _path;
        private int _size;
        private int _length;
        private int _tile;
        private int _cost;
        private int _strategy;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSavedPath"/> class.
        /// </summary>
        public LogicSavedPath(int size)
        {
            this._path = new int[size];
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._path = null;
            this._size = 0;
            this._length = 0;
            this._tile = 0;
            this._cost = -1;
            this._strategy = 0;
        }

        /// <summary>
        ///     Gets the length of path.
        /// </summary>
        public int GetLength()
        {
            return this._length;
        }

        /// <summary>
        ///     Saves the path.
        /// </summary>
        public void SavePath(int[] path, int length, int tile, int cost, int costStrategy)
        {
            if (this._size >= length)
            {
                if (length > 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        this._path[i] = path[i];
                    }
                }

                this._tile = tile;
                this._cost = cost;
            }
        }

        /// <summary>
        ///     Extracts this path to the specified int array.
        /// </summary>
        public void ExtractPath(int[] path)
        {
            for (int i = 0; i < this._length; i++)
            {
                path[i] = this._path[i];
            }
        }

        /// <summary>
        ///     Gets if the specified path information is equal to this path.
        /// </summary>
        public bool IsEqual(int tile, int cost, int costStrategy)
        {
            return this._tile == tile && this._cost == cost && this._strategy == costStrategy;
        }
    }
}