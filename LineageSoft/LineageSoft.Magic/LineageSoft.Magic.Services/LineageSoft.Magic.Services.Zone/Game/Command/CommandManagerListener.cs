namespace LineageSoft.Magic.Services.Zone.Game.Command
{
    using LineageSoft.Magic.Logic.Command;
    using LineageSoft.Magic.Logic.Command.Listener;
    using LineageSoft.Magic.Logic.Command.Server;

    using LineageSoft.Magic.Services.Zone.Game.Mode;

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
                    this._gameMode.RemoveServerCommand(serverCommand);
                }
            }
        }
    }
}