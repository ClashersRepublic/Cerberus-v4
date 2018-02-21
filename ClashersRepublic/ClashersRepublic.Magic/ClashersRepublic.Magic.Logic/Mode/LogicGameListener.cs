namespace ClashersRepublic.Magic.Logic.Mode
{
    public class LogicGameListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicGameListener"/> class.
        /// </summary>
        public LogicGameListener()
        {

        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            // Destruct.
        }
        
        public virtual void ReplayFailed() { }
        public virtual void AllianceCreated() { }
        public virtual void AllianceJoined() { }
    }
}