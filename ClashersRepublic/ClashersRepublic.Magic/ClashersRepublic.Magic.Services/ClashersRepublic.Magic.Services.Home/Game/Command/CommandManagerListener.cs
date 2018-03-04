namespace ClashersRepublic.Magic.Services.Home.Game.Command
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Command.Listener;
    using ClashersRepublic.Magic.Logic.Command.Server;
    using ClashersRepublic.Magic.Services.Home.Game.Mode;

    internal class CommandManagerListener : LogicCommandManagerListener
    {
        private GameMode _gameMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CommandManagerListener"/> class.
        /// </summary>
        internal CommandManagerListener(GameMode gameMode)
        {
            this._gameMode = gameMode;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._gameMode = null;
        }

        /// <summary>
        ///     Called when the specified <see cref="LogicCommand"/> has been executed.
        /// </summary>
        public override void CommandExecuted(LogicCommand command)
        {
            if (command.IsServerCommand())
            {
                LogicServerCommand serverCommand = (LogicServerCommand) command;

                if (serverCommand.GetId() != -1)
                {

                }
            }
        }

        /// <summary>
        ///     Called when the specified <see cref="LogicCommand"/> has not been executed correctly.
        /// </summary>
        public override void CommandExecuteFailed(LogicCommand command, string log)
        {
            // CommandExecuteFailed.
        }
    }
}