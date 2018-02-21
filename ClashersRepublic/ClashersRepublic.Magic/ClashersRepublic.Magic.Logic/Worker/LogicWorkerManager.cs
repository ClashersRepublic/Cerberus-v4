namespace ClashersRepublic.Magic.Logic.Worker
{
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicWorkerManager
    {
        private LogicLevel _level;
        private readonly LogicArrayList<LogicGameObject> _constructions;

        private int _workerCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicWorkerManager" /> class.
        /// </summary>
        public LogicWorkerManager(LogicLevel level)
        {
            this._level = level;
            this._workerCount = 0;
            this._constructions = new LogicArrayList<LogicGameObject>();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Gets the number of free workers.
        /// </summary>
        public int GetFreeWorkers()
        {
            return this._workerCount - this._constructions.Count;
        }

        /// <summary>
        ///     Gets the number of workers.
        /// </summary>
        public int GetTotalWorkers()
        {
            return this._workerCount;
        }

        /// <summary>
        ///     Allocates a worker to specified gameobject.
        /// </summary>
        public void AllocateWorker(LogicGameObject gameObject)
        {
            int index = -1;

            for (int i = 0; i < this._constructions.Count; i++)
            {
                if (this._constructions[i] == gameObject)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                Debugger.Warning("LogicWorkerManager::allocateWorker called twice for same target!");
            }
        }

        /// <summary>
        ///     Deallocates the worker associed to specified gameobject.
        /// </summary>
        public void DeallocateWorker(LogicGameObject gameObject)
        {
            int index = this._constructions.IndexOf(gameObject);

            if (index != -1)
            {
                this._constructions.Remove(index);
            }
        }

        /// <summary>
        ///     Removes the reference at specified gameobject.
        /// </summary>
        public void RemoveGameObjectReference(LogicGameObject gameObject)
        {
            this.DeallocateWorker(gameObject);
        }

        /// <summary>
        ///     Increases the number of workers.
        /// </summary>
        public void IncreaseWorkerCount()
        {
            ++this._workerCount;
        }

        /// <summary>
        ///     Decreases the number of workers.
        /// </summary>
        public void DecreaseWorkerCount()
        {
            if (--this._workerCount <= 0)
            {
                Debugger.Error("LogicWorkerManager - Total worker count below 0");
            }
        }
    }
}