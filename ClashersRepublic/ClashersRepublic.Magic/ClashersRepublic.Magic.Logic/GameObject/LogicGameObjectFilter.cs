namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicGameObjectFilter
    {
        private int _team;
        private bool _enemyOnly;

        private LogicArrayList<bool> _gameObjectTypes;
        private LogicArrayList<LogicGameObject> _ignoreGameObjects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObjectFilter"/> class.
        /// </summary>
        public LogicGameObjectFilter()
        {
            this._team = -1;
        }

        /// <summary>
        ///     Initializes the game object types array.
        /// </summary>
        public void InitGameObjectTypes()
        {
            if (this._gameObjectTypes == null)
            {
                this._gameObjectTypes = new LogicArrayList<bool>();
            }
        }

        /// <summary>
        ///     Initializes the ignore object array.
        /// </summary>
        public void InitIgnoreObjects()
        {
            if (this._ignoreGameObjects == null)
            {
                this._ignoreGameObjects = new LogicArrayList<LogicGameObject>();
            }
        }

        /// <summary>
        ///     Allowes the specified gameobject type.
        /// </summary>
        public void AddGameObjectType(int type)
        {
            if (this._gameObjectTypes == null)
            {
                this._gameObjectTypes = new LogicArrayList<bool>();
            }

            this._gameObjectTypes[type] = true;
        }
        

        /// <summary>
        ///     Test the specified gameobject.
        /// </summary>
        public virtual bool TestGameObject(LogicGameObject gameObject)
        {
            if (this._gameObjectTypes != null)
            {
                if (!this._gameObjectTypes[gameObject.GetGameObjectType()])
                {
                    return false;
                }
            }

            if (this._ignoreGameObjects != null)
            {
                for (int i = 0; i < this._ignoreGameObjects.Count; i++)
                {
                    if (this._ignoreGameObjects[i].GetData() == gameObject.GetData())
                    {
                        return false;
                    }
                }
            }

            if (this._team != -1)
            {
                LogicHitpointComponent hitpointComponent = gameObject.GetHitpointComponent();

                if (hitpointComponent != null)
                {
                    bool isEnemy = hitpointComponent.IsEnemyForTeam(this._team);

                    if (gameObject.IsAlive())
                    {
                        if (isEnemy || !this._enemyOnly)
                        {
                            return !this._enemyOnly || !isEnemy;
                        }
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     Gets if this filter is a component filter.
        /// </summary>
        public virtual bool IsComponentFilter()
        {
            return false;
        }

        /// <summary>
        ///     Passes enemy only.
        /// </summary>
        public void PassEnemyOnly(LogicGameObject gameObject)
        {
            if (gameObject.GetHitpointComponent() != null)
            {
                this._team = gameObject.GetHitpointComponent().GetTeam();
                this._enemyOnly = true;
            }
            else
            {
                this._team = -1;
            }
        }

        /// <summary>
        ///     Passes friendly only.
        /// </summary>
        public void PassFriendlyOnly(LogicGameObject gameObject)
        {
            if (gameObject.GetHitpointComponent() != null)
            {
                this._team = gameObject.GetHitpointComponent().GetTeam();
                this._enemyOnly = false;
            }
            else
            {
                this._team = -1;
            }
        }

        /// <summary>
        ///     Removes all gameobjects ignored.
        /// </summary>
        public void RemoveAllIgnoreObjects()
        {
            if (this._ignoreGameObjects != null)
            {
                if (this._ignoreGameObjects.Count != 0)
                {
                    do
                    {
                        this._ignoreGameObjects.Remove(0);
                    } while (this._ignoreGameObjects.Count != 0);

                    this._ignoreGameObjects = null;
                }
            }
        }

        /// <summary>
        ///     Adds the specified gameobject to ignore object.
        /// </summary>
        public void AddIgnoreObject(LogicGameObject gameObject)
        {
            if (this._ignoreGameObjects == null)
            {
                this._ignoreGameObjects = new LogicArrayList<LogicGameObject>();
            }

            this._ignoreGameObjects.Add(gameObject);
        }
    }
}