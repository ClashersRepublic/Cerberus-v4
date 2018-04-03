namespace LineageSoft.Magic.Logic.Message.Home
{
    using LineageSoft.Magic.Logic.Command;
    using LineageSoft.Magic.Logic.Command.Server;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Titan.Message;

    public class AvailableServerCommand : PiranhaMessage
    {
        private LogicServerCommand _serverCommand;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvailableServerCommand" /> class.
        /// </summary>
        public AvailableServerCommand() : this(0)
        {
            // AvailableServerCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvailableServerCommand" /> class
        /// </summary>
        public AvailableServerCommand(short messageVersion) : base(messageVersion)
        {
            // AvailableServerCommand.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._serverCommand = LogicCommandManager.DecodeCommand(this.Stream) as LogicServerCommand;
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
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24111;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._serverCommand = null;
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