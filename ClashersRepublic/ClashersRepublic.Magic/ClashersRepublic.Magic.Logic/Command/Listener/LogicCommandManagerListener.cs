namespace ClashersRepublic.Magic.Logic.Command.Listener
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
        ///     Called when the specified command has been executed.
        /// </summary>
        public virtual void CommandExecuted(LogicCommand command)
        {
            // CommandExecuted.
        }

        /// <summary>
        ///     Called when the specified command has not been executed correctly.
        /// </summary>
        public virtual void CommandExecuteFailed(LogicCommand command, string log)
        {
            // CommandExecuteFailed.
        }
    }
}