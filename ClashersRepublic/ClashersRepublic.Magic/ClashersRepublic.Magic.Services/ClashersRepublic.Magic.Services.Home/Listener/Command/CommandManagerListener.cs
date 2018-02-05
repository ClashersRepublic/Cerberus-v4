namespace ClashersRepublic.Magic.Services.Home.Listener.Command
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Command.Listener;

    internal sealed class CommandManagerListener : LogicCommandManagerListener
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandManagerListener"/> class.
        /// </summary>
        internal CommandManagerListener()
        {
            // CommandManagerListener.
        }

        /// <summary>
        ///     Called when the specified has been executed.
        /// </summary>
        public override void CommandExecuted(LogicCommand command)
        {
            base.CommandExecuted(command);

            if (command.IsServerCommand())
            {
                // TODO: Remove server command.
            }
        }
    }
}