namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicGameObjectFilter
    {
        private int _team;
        private bool _enemyOnly;
        private LogicArrayList<LogicGameObject> _ignoreGameObjects;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObjectFilter"/> class.
        /// </summary>
        public LogicGameObjectFilter()
        {
            this._ignoreGameObjects = new LogicArrayList<LogicGameObject>();
        }

        /// <summary>
        ///     Test the specified gameobject.
        /// </summary>
        public virtual void TestGameObject(LogicGameObject gameObject)
        {
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
    }
}