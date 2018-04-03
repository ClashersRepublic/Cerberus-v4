namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Level;

    using LineageSoft.Magic.Titan.DataStream;

    public sealed class LogicEditModeShownCommand : LogicCommand
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicEditModeShownCommand" /> class.
        /// </summary>
        public LogicEditModeShownCommand()
        {
            // LogicEditModeShownCommand.
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 544;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            level.SetEditModeShown();
            return 0;
        }
    }
}