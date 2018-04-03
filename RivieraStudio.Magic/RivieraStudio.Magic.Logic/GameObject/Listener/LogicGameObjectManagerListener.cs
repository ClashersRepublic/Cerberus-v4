namespace RivieraStudio.Magic.Logic.GameObject.Listener
{
    public class LogicGameObjectManagerListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameObjectManagerListener"/> class.
        /// </summary>
        public LogicGameObjectManagerListener()
        {
            // LogicGameObjectManagerListener.
        }

        /// <summary>
        ///     Called when the specified <see cref="LogicGameObject"/> has been added to <see cref="LogicGameObjectManager"/> instance.
        /// </summary>
        public virtual void AddGameObject(LogicGameObject gameObject)
        {
            // AddGameObject.
        }

        /// <summary>
        ///     Called when the specified <see cref="LogicGameObject"/> has been removed to <see cref="LogicGameObjectManager"/> instance.
        /// </summary>
        public virtual void RemoveGameObject(LogicGameObject gameObject)
        {
            // RemoveGameObject.
        }
    }
}