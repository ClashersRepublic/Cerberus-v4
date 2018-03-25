namespace ClashersRepublic.Magic.Services.Core.Message.Home
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Command.Server;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;

    public class AllowServerCommandMessage : NetMessage
    {
        private LogicServerCommand _serverCommand;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._serverCommand != null)
            {
                this._serverCommand.Destruct();
                this._serverCommand = null;
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            LogicCommandManager.EncodeCommand(this.Stream, this._serverCommand);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._serverCommand = (LogicServerCommand) LogicCommandManager.DecodeCommand(this.Stream);
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10511;
        }

        /// <summary>
        ///     Removes the <see cref="LogicServerCommand"/> instance.
        /// </summary>
        public LogicServerCommand RemoveServerCommand()
        {
            LogicServerCommand tmp = this._serverCommand;
            this._serverCommand = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="LogicServerCommand"/> instance.
        /// </summary>
        public void SetServerCommand(LogicServerCommand command)
        {
            this._serverCommand = command;
        }
    }
}