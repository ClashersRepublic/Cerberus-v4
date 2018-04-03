namespace RivieraStudio.Magic.Logic.Command.Listener
{
    public class LogicCommandManagerListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCommandManagerListener"/> class.
        /// </summary>
        public LogicCommandManagerListener()
        {
            // LogicCommandManagerListener.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Called when the specified <see cref="LogicCommand"/> has been executed.
        /// </summary>
        public virtual void CommandExecuted(LogicCommand command)
        {
            // CommandExecuted.
        }
    }
}