namespace RivieraStudio.Magic.Logic.Worker
{
    using System;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.GameObject.Component;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

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
                return;
            }

            this._constructions.Add(gameObject);
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
            if (this._workerCount-- <= 0)
            {
                Debugger.Error("LogicWorkerManager - Total worker count below 0");
            }
        }

        /// <summary>
        ///     Gets the shortest task.
        /// </summary>
        public LogicGameObject GetShortestTaskGO()
        {
            LogicGameObject gameObject = null;

            for (int i = 0, minRemaining = -1, tmpRemaining = 0; i < this._constructions.Count; i++, tmpRemaining = 0)
            {
                LogicGameObject tmp = this._constructions[i];

                switch (this._constructions[i].GetGameObjectType())
                {
                    case 0:
                        LogicBuilding building = (LogicBuilding) tmp;

                        if (building.IsConstructing())
                        {
                            tmpRemaining = building.GetRemainingConstructionTime();
                        }
                        else
                        {
                            LogicHeroBaseComponent heroBaseComponent = building.GetHeroBaseComponent();

                            if (heroBaseComponent == null)
                            {
                                Debugger.Warning("LogicWorkerManager - Worker allocated to altar/herobase without hero upgrading");
                            }
                            else
                            {
                                if (heroBaseComponent.IsUpgrading())
                                {
                                    tmpRemaining = heroBaseComponent.GetRemainingUpgradeSeconds();
                                }
                                else
                                {
                                    Debugger.Warning("LogicWorkerManager - Worker allocated to building with remaining construction time 0");
                                }
                            }
                        }

                        break;
                    case 3:
                        LogicObstacle obstacle = (LogicObstacle)tmp;

                        if (obstacle.IsClearingOnGoing())
                        {
                            tmpRemaining = obstacle.GetRemainingClearingTime();
                        }
                        else
                        {
                            Debugger.Warning("LogicWorkerManager - Worker allocated to obstacle with remaining clearing time 0");
                        }

                        break;
                    case 4:
                        LogicTrap trap = (LogicTrap) tmp;

                        if (trap.IsConstructing())
                        {
                            tmpRemaining = trap.GetRemainingConstructionTime();
                        }
                        else
                        {
                            Debugger.Warning("LogicWorkerManager - Worker allocated to trap with remaining construction time 0");
                        }
                        break;
                    case 8:

                        break;
                }

                if (gameObject == null || minRemaining > tmpRemaining)
                {
                    gameObject = tmp;
                    minRemaining = tmpRemaining;
                }
            }

            return gameObject;
        }

        /// <summary>
        ///     Finishes the task of one worker.
        /// </summary>
        public bool FinishTaskOfOneWorker()
        {
            LogicGameObject gameObject = this.GetShortestTaskGO();

            if (gameObject != null)
            {
                switch (gameObject.GetGameObjectType())
                {
                    case 0:
                        LogicBuilding building = (LogicBuilding) gameObject;

                        if (building.IsConstructing())
                        {
                            return building.SpeedUpConstruction();
                        }

                        if (building.GetHeroBaseComponent() != null)
                        {
                            // TODO: Implement LogicHeroBaseComponent::speedUpUpgrade.
                        }

                        break;
                    case 3:
                        LogicObstacle obstacle = (LogicObstacle) gameObject;

                        if (obstacle.IsClearingOnGoing())
                        {
                            return obstacle.SpeedUpClearing();
                        }

                        break;
                    case 4:
                        LogicTrap trap = (LogicTrap) gameObject;

                        if (trap.IsConstructing())
                        {
                            return trap.SpeedUpConstruction();
                        }

                        break;
                    case 8:
                        LogicVillageObject villageObject = (LogicVillageObject) gameObject;

                        break;
                }
            }

            return false;
        }
    }
}